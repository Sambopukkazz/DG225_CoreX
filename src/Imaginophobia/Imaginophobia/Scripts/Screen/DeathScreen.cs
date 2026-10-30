using Gum;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Gum.Input;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using RenderingLibrary.Graphics;
using System;

namespace Imaginophobia {
    public class DeathScreen : Screen {
        private Panel _root;
        private TextRuntime _tipsMessage;
        private Random _random = new Random();

        private string[] _tips = {
            "You should look at people when they're talking to you.",
            "Are you really alone in the darkness? Make use of your flashlight.",
            "You don't like being watched, do you? Doesn't it make you want to hide?",
            "Don't forget about your Scopophobia. Watch and listen... They won't find you in the locker.",
            "Your Nyctophobia won't let you stay in here long. Stay out of the darkness.",
            "You can press CTRL to lock the direction the character is facing.",
            "Don't keep \"It\" waiting. Check the doors regularly.",
            "\"It\" doesn't like lights. You won't find \"It\" in the Electrical rooms.",
            "Your sanity will not decrease in the Electrical room."
        };

        public DeathScreen() {
            _root = new Panel();
            _root.Dock(Dock.Fill);

            ContainerRuntime canvas = new ContainerRuntime();
            canvas.Width = Layout.ScreenWidth;
            canvas.Height = Layout.ScreenHeight;
            _root.AddChild(canvas);

            SpriteRuntime background = new SpriteRuntime();
            background.SourceFileName = "UI/background_death.png";
            canvas.AddChild(background);

            canvas.AddChild(GumUIHelper.CreateTextButton("< Return to Menu", Layout.BackButtonFontScale, 0, 0, 500, 120, ReturnToMenu));

            _tipsMessage = GumUIHelper.CreateText("", 1.5f, 20, 0, Layout.ScreenWidth, 120, true, 0);
            _tipsMessage.HorizontalAlignment = HorizontalAlignment.Left;
            _tipsMessage.VerticalAlignment = VerticalAlignment.Center;
            _tipsMessage.XOrigin = HorizontalAlignment.Left;
            _tipsMessage.YOrigin = VerticalAlignment.Bottom;
            _tipsMessage.XUnits = GeneralUnitType.PixelsFromBaseline;
            _tipsMessage.YUnits = GeneralUnitType.PixelsFromBaseline;
            canvas.AddChild(_tipsMessage);

            RandomizeTip();
            _root.AddToRoot();
            AudioManager.Instance.PlayBackgroundSound(BackgroundSound.BgmDeathScreen);
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

        private void RandomizeTip() {
            int index = _random.Next(_tips.Length);
            _tipsMessage.Text = _tips[index];
        }

        private void ReturnToMenu() {
            MainGame.ScreenManager.ReplaceScreen(new MainMenuScreen());
        }
    }
}
