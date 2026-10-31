using Gum;
using Gum.Wireframe;
using KernSmith.Gum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
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
        public static ModifiedScreenManager ScreenManager;
        public static GameState GameState { get; set; }
        
        private GumService Gum => GumService.Default;

        public MainGame() {
            if (s_instance != null) {
                throw new InvalidOperationException($"Only a single MainGame instance can be created");
            }

            s_instance = this;

            Graphics = new GraphicsDeviceManager(this);

            Graphics.PreferredBackBufferWidth = Layout.ScreenWidth;
            Graphics.PreferredBackBufferHeight = Layout.ScreenHeight;
            Graphics.IsFullScreen = false;

            Graphics.ApplyChanges();

            Window.Title = "Imaginophobia";

            Content = base.Content;
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
        }

        protected override void Initialize() {
            GraphicsDevice = base.GraphicsDevice;
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            Gum.Initialize(this);
            CustomSetPropertyOnRenderable.InMemoryFontCreator = new KernSmithFontCreator(GraphicsDevice);
            KernSmithFontCreator.RegisterFont("generationfonting", "Fonts/TrueTypeFont/GenerationFonting.ttf");

            new AudioManager();

            ScreenManager = new ModifiedScreenManager();
            ScreenManager.Initialize();
            ScreenManager.ShowScreen(new MainMenuScreen(), 1);

            GameState = GameState.Menu;

            base.Initialize();
        }

        protected override void LoadContent() {
            
        }

        protected override void Update(GameTime gameTime) {
            Time.Update(gameTime);

            ScreenManager.Update(gameTime);

            Gum.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            ScreenManager.Draw(gameTime);
            Gum.Draw();

            //Saturation Effect
            GraphicsDevice.SetRenderTarget(null);
            ScreenManager.DrawGraded();

            ScreenManager.DrawFade();

            base.Draw(gameTime);
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
