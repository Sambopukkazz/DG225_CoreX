using Gum;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Gum.Input;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;

namespace Imaginophobia {
    // Pushed over GameplayScreen while paused: the dim backdrop panel, the two volume sliders
    // and a Return to Menu button. ESC closes it and drops back to the gameplay screen.
    public class PauseScreen : Screen {
        private Panel _root;

        public PauseScreen() {
            _root = new Panel();
            _root.Dock(Dock.Fill);

            ContainerRuntime canvas = new ContainerRuntime();
            canvas.Width = Layout.ScreenWidth;
            canvas.Height = Layout.ScreenHeight;
            _root.AddChild(canvas);

            GumUIHelper.AddSettingsPanel(canvas);
            canvas.AddChild(GumUIHelper.CreateTextButton("Return to Menu", Layout.MenuFontScale, Layout.ActionButtonX, Layout.ActionButtonY, Layout.ActionButtonWidth, Layout.ActionButtonHeight, ReturnToMenu));

            _root.AddToRoot();
        }

        public override void Update(GameTime gameTime) {
            if (GumService.Default.Keyboard.KeyPushed(Keys.Escape)) {
                Resume();
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

        private void Resume() {
            MainGame.ScreenManager.CloseScreen();
        }

        private void ReturnToMenu() {
            MainGame.ScreenManager.ReplaceScreen(new MainMenuScreen());
        }
    }
}
