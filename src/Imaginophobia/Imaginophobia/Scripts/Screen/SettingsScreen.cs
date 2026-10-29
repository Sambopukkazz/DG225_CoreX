using Gum;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Gum.Input;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;

namespace Imaginophobia {
    public class SettingsScreen : Screen {
        private Panel _root;

        public SettingsScreen() {
            _root = new Panel();
            _root.Dock(Dock.Fill);

            ContainerRuntime canvas = new ContainerRuntime();
            canvas.Width = Layout.ScreenWidth;
            canvas.Height = Layout.ScreenHeight;
            _root.AddChild(canvas);

            SpriteRuntime background = new SpriteRuntime();
            background.SourceFileName = "UI/Imaginophobia_bg_placerholder.png";
            canvas.AddChild(background);

            GumUIHelper.AddSettingsPanel(canvas);
            canvas.AddChild(GumUIHelper.CreateTextButton("Back", Layout.BackButtonFontScale, Layout.BackButtonX, Layout.BackButtonY, Layout.BackButtonSize, Layout.BackButtonSize, Back));

            _root.AddToRoot();
        }

        public override void Update(GameTime gameTime) {
            if (GumService.Default.Keyboard.KeyPushed(Keys.Escape)) {
                Back();
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

        private void Back() {
            MainGame.ScreenManager.CloseScreen();
        }
    }
}
