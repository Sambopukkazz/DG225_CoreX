using Gum;
using Gum.Forms.Controls;
using Gum.GueDeriving;
using Gum.Wireframe;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class GameplayScreen : Screen {
        private GameManager _gameManager;
        private Panel _root;
        private ContainerRuntime _pauseMenu;

        public GameplayScreen(BoxingViewportAdapter viewportAdapter) {
            _gameManager = new(viewportAdapter);
            UpdateWhenInactive = true;
            DrawWhenInactive = true;

            _root = new Panel();
            _root.Dock(Dock.Fill);

            //RectangleRuntime background = new RectangleRuntime();
            //background.IsFilled = true;
            //background.FillColor = Color.Green;
            //background.StrokeAlpha = 0;
            //background.Width = Layout.ScreenWidth;
            //background.Height = Layout.ScreenHeight;
            //_root.AddChild(background);

            _pauseMenu = new ContainerRuntime();
            _pauseMenu.Width = Layout.ScreenWidth;
            _pauseMenu.Height = Layout.ScreenHeight;
            _pauseMenu.Visible = false;
            _root.AddChild(_pauseMenu);

            GumUIHelper.AddSettingsPanel(_pauseMenu);
            _pauseMenu.AddChild(GumUIHelper.CreateTextButton("Return to Menu", Layout.MenuFontScale, Layout.ActionButtonX, Layout.ActionButtonY, Layout.ActionButtonWidth, Layout.ActionButtonHeight, ReturnToMenu));

            _root.AddChild(GumUIHelper.CreatePauseButton(TogglePause));

            _root.AddToRoot();
        }

        public override void Update(GameTime gameTime) {
            _gameManager.Update(gameTime);

            if (GumService.Default.Keyboard.KeyPushed(Keys.Escape)) {
                TogglePause();
            }
        }

        public override void Draw(GameTime gameTime) {
            _gameManager.Draw(gameTime);
        }

        public override void OnActivated() {
            _root.IsVisible = true;
        }

        public override void OnDeactivated() {
            //Unload game (unsub event etc.)
        }

        public override void Dispose() {
            _root.RemoveFromRoot();
            base.Dispose();
        }

        private void TogglePause() {
            _pauseMenu.Visible = !_pauseMenu.Visible;
        }

        public override void UnloadContent() {
            _gameManager.Dispose();
        }
        private void ReturnToMenu() {
            //FadeTransition fadeTransition = new FadeTransition(MainGame.GraphicsDevice, Color.Black, 1f);
            //fadeTransition.Completed += (_, _) => {
            //    MainGame.ScreenManager.ClearScreens();
            //    MainGame.ScreenManager.ShowScreen(new MainMenuScreen(), fadeTransition);
            //};

            //MainGame.ScreenManager.CloseScreen(fadeTransition);

            MainGame.ScreenManager.ClearScreens();
            MainGame.ScreenManager.ShowScreen(new MainMenuScreen());
        }
    }
}
