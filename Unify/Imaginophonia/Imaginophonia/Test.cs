using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.Input;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;
using MonoGame.Extended.ViewportAdapters;
using Penumbra;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection.Emit;

namespace Imaginophonia {
    public class Test : Core {
        GameManager _gameManager;
        PenumbraComponent penumbra;
        SoundEffect soundEffect;
        SoundEffectInstance soundEffectInstance;
        Rectangle player;
        AudioListener soundListener;
        AudioEmitter soundEmitter;
        PointLight light2;
        BitmapFont _font;
        Player _player;

        List<Timer> _timers = new();

        private Tilemap _tilemap;
        private TilemapSpriteBatchRenderer _renderer;
        private OrthographicCamera _camera;

        public static AudioManager Audio {  get; private set; }

        public Test() : base("Imaginophobia", 1920, 1080, false) {
            Audio = new AudioManager();
            penumbra = new PenumbraComponent(this);
            Components.Add(penumbra);
            penumbra.AmbientColor = Color.FromHSL(10,0,40);
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
            soundEffect = Content.Load<SoundEffect>("Audio/sfx_step_01");
            soundEffectInstance = soundEffect.CreateInstance();
            soundEffectInstance.IsLooped = true;
            soundEffectInstance.Apply3D(soundListener,soundEmitter);
            soundEffectInstance.Play();
            SoundEffect.DistanceScale = 100f;
            _font = Content.Load<BitmapFont>("Font/GenerationFonting");

            _player = new();
            _player.Transform.Position = new Vector2(0,500);

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
            else if (direction == Vector3.Zero) {
                instance.Pan = 0;
            }
        }


        protected override void Initialize() {
            // TODO: Add your initialization logic here
            
            base.Initialize();

            BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 1920, 1080);
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
            //_renderer.Update(gameTime);
            if(KeyboardExtended.GetState().WasKeyPressed(Keys.NumPad1)) {
                Time.AddTimer(5);
            }

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

            _player.Update();
            soundListener.Position = new Vector3(_player.Transform.Position, 0);
            light2.Position = _player.Transform.Position;
            soundListener.Velocity = new Vector3(InputManager.Direction*50,0);
            if (Keyboard.GetState().IsKeyDown(Keys.E)) {
                UpdateSpatialAudio(soundEffectInstance, soundListener.Position, soundEmitter.Position, 150f, 500f);
                _camera.LookAt(new Vector2(soundEmitter.Position.X, soundEmitter.Position.Y));
            }
            else {
                soundEffectInstance.Apply3D(soundListener, soundEmitter);
                _camera.LookAt(_player.Transform.Position);
            } 

            Time.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            penumbra.BeginDraw();
            
            GraphicsDevice.Clear(Color.White);

            Matrix transformMatrix = _camera.GetViewMatrix();
            penumbra.Transform = transformMatrix;

            _renderer.Draw(SpriteBatch, _camera);
            // TODO: Add your drawing code here
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp,transformMatrix: transformMatrix);
            SpriteBatch.DrawRectangle(player, Color.Black, 3);
            SpriteBatch.DrawCircle(soundListener.Position.X, soundListener.Position.Y, 1f, 120, Color.Red, 4);
            SpriteBatch.DrawCircle(soundEmitter.Position.X, soundEmitter.Position.Y, 1f, 10, Color.White, 2);
            SpriteBatch.DrawString(_font, $"Light radius: {light2.Radius}\nLight scale: {light2.Scale.X}.{light2.Scale.Y}\nLight intensity: {light2.Intensity}", new Vector2(100, 100), Color.White);
            SpriteBatch.DrawString(_font, $"ListenerPos:{soundListener.Position.X}\nEmitterPosL:{soundEmitter.Position.X}\nCameraPos:{_camera.Position}", new Vector2(100, 300), Color.White);
            SpriteBatch.DrawString(_font, $"\nElapseTime:{Time.DeltaTime}", new Vector2(100, 400), Color.White);
            foreach (Timer timer in Time.Timers) {
                SpriteBatch.DrawString(_font, $"\nTimer:{timer.TimeLeft}", new Vector2(100, 500 + (40 * Time.Timers.IndexOf(timer))), Color.White);
            }
            //SpriteBatch.DrawString(_font, $"Velocity: {_player.Velocity}\nVelocityCal: {_player.VelocityCal}", new Vector2(200, 500), Color.White);
            _player.Draw();
            //SpriteBatch.DrawEllipse(soundEmitter.Position.X, soundEmitter.Position.Y, 1f, 120, Color.Red, 2);
            SpriteBatch.End();
            

            base.Draw(gameTime);
        }
    }
}
