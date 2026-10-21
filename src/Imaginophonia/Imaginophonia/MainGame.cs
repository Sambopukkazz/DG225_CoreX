using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using MonoSound;
using System;

namespace Imaginophobia {
    public class MainGame : Game {
        private static MainGame s_instance;
        public static MainGame Instance => s_instance;
        public static GraphicsDeviceManager Graphics { get; private set; }
        public static new GraphicsDevice GraphicsDevice { get; private set; }
        public static SpriteBatch SpriteBatch { get; private set; }
        public static new ContentManager Content { get; private set; }
        public static ScreenManager ScreenManager;
        private RenderTarget2D _frameRT;
        private Effect _grade;

        // 0 = grayscale, 1 = normal. Tween this for a sanity / mood effect.
        public static float Saturation { get; set; } = 1f;

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

            Window.Title = "Imaginophobia";

            Content = base.Content;
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        protected override void Initialize() {
            // TODO: Add your initialization logic here
            GraphicsDevice = base.GraphicsDevice;

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            ScreenManager = new ScreenManager();
            BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 1920, 1080);
            //_gameManager = new(viewportAdapter);
            var screen = new GameplayScreen(viewportAdapter);
            

            ScreenManager.ShowScreen(screen);
            ScreenManager.Initialize();
            //Components.Add(ScreenManager);
            //Components.Add(LightManager.Penumbra);

            base.Initialize();
        }

        protected override void LoadContent() {
            // TODO: use this.Content to load your game content here
            _grade = Content.Load<Effect>("Shaders/Saturation");
            _grade.CurrentTechnique = _grade.Techniques["ApplySaturation"];
        }

        protected override void Update(GameTime gameTime) {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();

            // TODO: Add your update logic here
            
            Time.Update(gameTime);

            ScreenManager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            EnsureFrameTarget();
            GraphicsDevice.SetRenderTarget(_frameRT);
            //LightManager.Penumbra.BeginDraw();

            //GraphicsDevice.Clear(Color.White);
            //GraphicsDevice.Clear(Color.Black);
            //// TODO: Add your drawing code here
            //SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            ScreenManager.Draw(gameTime);
            GraphicsDevice.SetRenderTarget(null);
            DrawGraded();
            //SpriteBatch.End();

            base.Draw(gameTime);
        }

        private void EnsureFrameTarget() {
            Viewport viewport = GraphicsDevice.Viewport;

            if (_frameRT != null && _frameRT.Width == viewport.Width && _frameRT.Height == viewport.Height) {
                return;
            }

            _frameRT?.Dispose();
            _frameRT = new RenderTarget2D(GraphicsDevice, viewport.Width, viewport.Height, false, SurfaceFormat.Color, DepthFormat.None);
        }

        // Whole frame -> one saturation pass -> screen
        private void DrawGraded() {
            _grade.Parameters["Saturation"]?.SetValue(Saturation);

            SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null, _grade);
            SpriteBatch.Draw(_frameRT, GraphicsDevice.Viewport.Bounds, Color.White);
            SpriteBatch.End();
        }

        protected override void UnloadContent() {
            // Dispose of the audio controller.
            AudioManager.Instance?.Dispose();

            base.UnloadContent();
        }

        protected override void OnExiting(object sender, ExitingEventArgs args) {
            MonoSoundLibrary.DeInit();
            base.OnExiting(sender, args);
        }
    }
}
