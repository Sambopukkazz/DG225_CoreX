using Gum;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Gum.Input;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using RenderingLibrary.Graphics;
using System;

namespace Imaginophobia {
    public class WinScreen : Screen {
        private Panel _root;
        private float _timeLeft;

        public WinScreen(float timeLeft) {
            _root = new Panel();
            _timeLeft = timeLeft;
        }

        public override void Initialize() {
            _root.Dock(Dock.Fill);

            ContainerRuntime canvas = new ContainerRuntime();
            canvas.Width = Layout.ScreenWidth;
            canvas.Height = Layout.ScreenHeight;
            _root.AddChild(canvas);

            SpriteRuntime background = new SpriteRuntime();
            background.SourceFileName = "UI/background_win.png";
            canvas.AddChild(background);

            canvas.AddChild(GumUIHelper.CreateText("You survived.", Layout.TitleFontScale, 600, 220, 720, 120));
            string message = TimeSpan.FromSeconds(_timeLeft).ToString(@"mm\:ss");
            TextRuntime time = GumUIHelper.CreateText($"Time left: {message}", 2, 960, 480, 600, 120);
            time.XOrigin = HorizontalAlignment.Center;
            time.YOrigin = VerticalAlignment.Center;
            canvas.AddChild(time);

            TextRuntime thanks = GumUIHelper.CreateText("Thanks for playing.", 1.75f, 960, 680, 600, 120);
            thanks.XOrigin = HorizontalAlignment.Center;
            thanks.YOrigin = VerticalAlignment.Center;
            canvas.AddChild(thanks);

            canvas.AddChild(GumUIHelper.CreateTextButton("Return to Menu", Layout.MenuFontScale, 760, 880, 400, 80, ReturnToMenu));

            _root.AddToRoot();

            AudioManager.Instance.PlayBackgroundSound(BackgroundSound.BgmWinScreen);

            MainGame.GameState = GameState.Win;
        }

        public override void Update(GameTime gameTime) {
            if (GumService.Default.Keyboard.KeyPushed(Keys.Escape)) {
                ReturnToMenu();
            }
        }

        public override void Draw(GameTime gameTime) {
        }

        public override void OnActivated() {
            _root.IsVisible = true;
        }

        public override void OnDeactivated() {
            _root.IsVisible = DrawWhenInactive;
        }

        public override void Dispose() {
            _root.RemoveFromRoot();
            base.Dispose();
        }

        private void ReturnToMenu() {
            if (MainGame.GameState == GameState.Win) {
                MainGame.ScreenManager.ClearScreens(new MainMenuScreen(), 1, 1, 1);
                MainGame.GameState = GameState.Menu;
            }
        }
    }
}
