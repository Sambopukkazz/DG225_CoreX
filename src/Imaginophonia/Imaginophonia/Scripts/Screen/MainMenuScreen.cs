using Gum.Forms;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using ToolsUtilities;

namespace Imaginophobia {
    public class MainMenuScreen : Screen {
        private Panel _root;

        public MainMenuScreen() {
            _root = new Panel();
            _root.Dock(Dock.Fill);

            ContainerRuntime canvas = new ContainerRuntime();
            canvas.Width = Layout.ScreenWidth;
            canvas.Height = Layout.ScreenHeight;
            _root.AddChild(canvas);

            SpriteRuntime background = new SpriteRuntime();
            background.SourceFileName = "UI/Imaginophobia_bg_placerholder.png";
            canvas.AddChild(background);

            canvas.AddChild(GumUIHelper.CreateTitle("Imaginophobia"));
            canvas.AddChild(GumUIHelper.CreateTextButton("<", Layout.ArrowFontScale, Layout.LeftArrowX, Layout.ArrowY, Layout.ArrowSize, Layout.ArrowSize, null, false));
            canvas.AddChild(GumUIHelper.CreateTextButton(">", Layout.ArrowFontScale, Layout.RightArrowX, Layout.ArrowY, Layout.ArrowSize, Layout.ArrowSize, null, false));
            canvas.AddChild(GumUIHelper.CreateTextButton("Shift 1", Layout.StartButtonFontScale, Layout.StartButtonX, Layout.StartButtonY, Layout.StartButtonWidth, Layout.StartButtonHeight, StartGame));
            canvas.AddChild(GumUIHelper.CreateTextButton("Index", Layout.MenuFontScale, Layout.MenuButtonX, Layout.MenuIndexY, Layout.MenuButtonWidth, Layout.MenuButtonHeight, null, false));
            canvas.AddChild(GumUIHelper.CreateTextButton("Settings", Layout.MenuFontScale, Layout.MenuButtonX, Layout.MenuSettingsY, Layout.MenuButtonWidth, Layout.MenuButtonHeight, OpenSettings));
            canvas.AddChild(GumUIHelper.CreateTextButton("Credits", Layout.MenuFontScale, Layout.MenuButtonX, Layout.MenuCreditsY, Layout.MenuButtonWidth, Layout.MenuButtonHeight, OpenCredits));
            canvas.AddChild(GumUIHelper.CreateTextButton("Exit", Layout.MenuFontScale, Layout.MenuButtonX, Layout.MenuExitY, Layout.MenuButtonWidth, Layout.MenuButtonHeight, ExitGame));

            _root.AddToRoot();
        }

        public override void Update(GameTime gameTime) {
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

        private void StartGame() {
            MainGame.StartGame();
        }

        private void OpenSettings() {
            MainGame.ScreenManager.ShowScreen(new SettingsScreen());
        }

        private void OpenCredits() {
            MainGame.ScreenManager.ShowScreen(new CreditsScreen());
        }

        private void ExitGame() {
            MainGame.Instance.Exit();
        }
    }
}
