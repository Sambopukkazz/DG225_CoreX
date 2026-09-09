using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace Imaginophonia {
    public class MainGame : Game {
        private static MainGame s_instance;
        public static MainGame Instance => s_instance;
        public static GraphicsDeviceManager Graphics { get; private set; }
        public static new GraphicsDevice GraphicsDevice { get; private set; }
        public static SpriteBatch SpriteBatch { get; private set; }
        public static new ContentManager Content { get; private set; }

        GameManager _gameManager;

        public MainGame() {
            if (s_instance != null) {
                throw new InvalidOperationException($"Only a single MainGame instance can be created");
            }

            s_instance = this;

            Graphics = new GraphicsDeviceManager(this);

            Graphics.PreferredBackBufferWidth = 1920;
            Graphics.PreferredBackBufferHeight = 1080;
            Graphics.IsFullScreen = false;

            Graphics.ApplyChanges();

            Window.Title = "Imaginophobias";

            Content = base.Content;
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        protected override void Initialize() {
            // TODO: Add your initialization logic here
            GraphicsDevice = base.GraphicsDevice;

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 1920, 1080);
            _gameManager = new(viewportAdapter);

            Components.Add(LightManager.Penumbra);

            base.Initialize();
        }

        protected override void LoadContent() {
            // TODO: use this.Content to load your game content here

        }

        protected override void Update(GameTime gameTime) {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            // TODO: Add your update logic here
            InputManager.Update();

            _gameManager.Update(gameTime);

            Time.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            //LightManager.Penumbra.BeginDraw();

            //GraphicsDevice.Clear(Color.White);

            //// TODO: Add your drawing code here
            //SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            //SpriteBatch.End();
            _gameManager.Draw();

            base.Draw(gameTime);
        }

        protected override void UnloadContent() {
            // Dispose of the audio controller.
            AudioManager.Instance.Dispose();

            base.UnloadContent();
        }
    }
}
