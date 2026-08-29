using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;
using MonoGame.Extended.ViewportAdapters;
using Penumbra;
using System.Net.Sockets;
using System.Reflection.Emit;

namespace Imaginophonia {
    public class Game1 : Core {
        GameManager _gameManager;
        PenumbraComponent penumbra;
        SoundEffect soundEffect;
        SoundEffectInstance soundEffectInstance;
        Rectangle player;
        AudioListener soundListener;
        AudioEmitter soundEmitter;
        PointLight light2;
        BitmapFont _font;

        private Tilemap _tilemap;
        private TilemapSpriteBatchRenderer _renderer;
        private OrthographicCamera _camera;

        public static AudioManager Audio {  get; private set; }

        public Game1() : base("Imaginophobia", 1920, 1080, false) {
            _gameManager = new();
            Audio = new AudioManager();
            penumbra = new PenumbraComponent(this);
            Components.Add(penumbra);
            penumbra.AmbientColor = Color.FromHSL(10,10,10);
            PointLight light = new PointLight();
            light.Position = new Vector2(960, 540);
            light.Color = Color.Blue;
            light.Scale = new Vector2(50*20, 50*20);
            light.Intensity = 0.1f;

            light2 = new PointLight();
            light2.Position = new Vector2(1500, 600);
            light2.Color = Color.Red;
            light2.Scale = new Vector2(50 * 20, 50 * 20);
            light2.Intensity = 0.1f;
            light2.Radius = 75f;

            penumbra.Lights.Add(light);
            penumbra.Lights.Add(light2);
            player = new Rectangle(500,500,60,80);
            soundListener = new AudioListener();
            soundEmitter = new AudioEmitter();
            soundListener.Position = new Vector3(960,540,0);
            soundEmitter.Position = new Vector3(960, 540, 0);
            soundEmitter.Forward = new Vector3(0,0,1000);
            soundEmitter.Up = new Vector3(0,0,1000);
            soundEffect = Content.Load<SoundEffect>("Audio/sfx_step_01");
            soundEffectInstance = soundEffect.CreateInstance();
            soundEffectInstance.IsLooped = true;
            soundEffectInstance.Apply3D(soundListener,soundEmitter);
            soundEffectInstance.Play();
            SoundEffect.DistanceScale = 100f;
            _font = Content.Load<BitmapFont>("Font/GenerationFonting");

        }

        public void UpdateSpatialAudio(SoundEffectInstance instance, Vector3 listenerPos, Vector3 emitterPos, float minDistance, float maxDistance) {
            float distance = Vector3.Distance(listenerPos, emitterPos);

            float volume = 1f - MathHelper.Clamp((distance - minDistance) / (maxDistance - minDistance), 0f, 1f);
            instance.Volume = volume;

            Vector3 direction = emitterPos - listenerPos;
            if (direction != Vector3.Zero) {
                direction.Normalize();
                instance.Pan = MathHelper.Clamp(direction.X, -1f, 1f);
            }
        }


        protected override void Initialize() {
            // TODO: Add your initialization logic here
            
            base.Initialize();

            BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 800, 480);
            _camera = new OrthographicCamera(viewportAdapter);
        }

        protected override void LoadContent() {

            // TODO: use this.Content to load your game content here

            base.LoadContent();
            Transform2 test = new();

            _tilemap = Content.Load<Tilemap>("Tilemap/testmapfr");

            _renderer = new TilemapSpriteBatchRenderer();
            _renderer.LoadTilemap(_tilemap);
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            InputManager.Update();
            // TODO: Add your update logic here
            //player.X += (int)(Vector2.UnitX.X * InputManager.Direction.X * 10);
            //player.Y += (int)(Vector2.UnitY.Y * InputManager.Direction.Y * 10);
            //soundListener.Position = new Vector3(player.X, player.Y,0);
            //soundListener.Velocity = new Vector3(InputManager.Direction,0);
            //if (Keyboard.GetState().IsKeyDown(Keys.E)) ;
            //float distanceFactor = 10f;

            //Vector3 relativePosition = soundEmitter.Position - soundListener.Position;
            //MathHelper.Clamp(relativePosition.X,0f,200f);
            //soundEmitter.Position = soundListener.Position + (relativePosition / distanceFactor);
            _renderer.Update(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.Right)){
                light2.Scale += new Vector2(10,10);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left)){
                light2.Scale -= new Vector2(10, 10);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad6)) {
                light2.Radius += 100;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad4)) {
                light2.Radius -= 100;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
                light2.Intensity += 0.01f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) {
                light2.Intensity -= 0.01f;
            }

            
            soundListener.Position += new Vector3(InputManager.Direction * 2, 0);
            light2.Position = new Vector2(soundListener.Position.X, soundListener.Position.Y);
            soundListener.Velocity = new Vector3(InputManager.Direction*20,0);
            if (Keyboard.GetState().IsKeyDown(Keys.E)) {
                UpdateSpatialAudio(soundEffectInstance, soundListener.Position, soundEmitter.Position, 150f, 500f);
            }
            else soundEffectInstance.Apply3D(soundListener, soundEmitter);
            _camera.LookAt(new Vector2(soundListener.Position.X, soundListener.Position.Y));

            _gameManager.Update();
            Audio.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            penumbra.BeginDraw();
            GraphicsDevice.Clear(Color.White);

            _renderer.Draw(SpriteBatch, _camera);

            // TODO: Add your drawing code here
            SpriteBatch.Begin();
            SpriteBatch.DrawRectangle(player, Color.Black, 3);
            SpriteBatch.DrawCircle(soundListener.Position.X, soundListener.Position.Y, 1f, 120, Color.Red, 4);
            SpriteBatch.DrawCircle(soundEmitter.Position.X, soundEmitter.Position.Y, 1f, 10, Color.Red, 2);
            SpriteBatch.DrawString(_font, $"Light radius: {light2.Radius}\nLight scale: {light2.Scale.X}.{light2.Scale.Y}\nLight intensity: {light2.Intensity}", new Vector2(100, 100), Color.Black);

            //SpriteBatch.DrawEllipse(soundEmitter.Position.X, soundEmitter.Position.Y, 1f, 120, Color.Red, 2);
            SpriteBatch.End();
            _gameManager.Draw();

            base.Draw(gameTime);
        }
    }
}
