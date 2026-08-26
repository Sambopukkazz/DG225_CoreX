using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;
using MonoGame.Extended.ViewportAdapters;
using Penumbra;
using System.Reflection.Emit;

namespace Imaginophonia {
    public class Game1 : Core {
        GameManager _gameManager;
        PenumbraComponent penumbra;
        SoundEffect steps;
        SoundEffectInstance tset;
        Rectangle player;
        AudioListener soundListener;
        AudioEmitter soundEmitter;

        public static AudioManager Audio {  get; private set; }

        public Game1() : base("Imaginophobia", 1920, 1080, false) {
            _gameManager = new();
            Audio = new AudioManager();
            penumbra = new PenumbraComponent(this);
            Components.Add(penumbra);
            penumbra.AmbientColor = Color.Gray;
            PointLight light = new PointLight();
            light.Position = new Vector2(900, 600);
            light.Color = Color.Orange;
            light.Scale = new Vector2(50*20, 50*20);
            light.Intensity = 0.1f;

            PointLight light2 = new PointLight();
            light2.Position = new Vector2(1500, 600);
            light2.Color = Color.Orange;
            light2.Scale = new Vector2(50 * 20, 50 * 20);
            light2.Intensity = 0.1f;
            light2.Radius = 75f;

            penumbra.Lights.Add(light);
            penumbra.Lights.Add(light2);
            player = new Rectangle(500,500,60,80);
            soundListener = new AudioListener();
            soundListener.Position = new Vector3(900, 500, 0);
            soundEmitter = new AudioEmitter();
            soundEmitter.Position = new Vector3(100, 100,0);
            steps = Content.Load<SoundEffect>("Audio/sfx_step_01");
            tset = Audio.PlaySoundEffect(steps, 1f, 0, 0, true);
            tset.Apply3D(soundListener, soundEmitter);
        }

        protected override void Initialize() {
            // TODO: Add your initialization logic here
            
            base.Initialize();
            
            

        }

        protected override void LoadContent() {

            // TODO: use this.Content to load your game content here

            base.LoadContent();

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

            soundEmitter.Position += new Vector3(InputManager.Direction * 10, 0);

            _gameManager.Update();
            Audio.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            penumbra.BeginDraw();
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            SpriteBatch.Begin();
            SpriteBatch.DrawRectangle(player, Color.Black,3);
            SpriteBatch.DrawCircle(soundListener.Position.X, soundListener.Position.Y, 10f,120,Color.Red,4);
            SpriteBatch.DrawCircle(soundEmitter.Position.X, soundEmitter.Position.Y, 50f, 120, Color.Red, 2);
            SpriteBatch.End();
            _gameManager.Draw();
            
            base.Draw(gameTime);
        }
    }
}
