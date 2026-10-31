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

        public override void Initialize() {
            BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(MainGame.Instance.Window, MainGame.GraphicsDevice, Layout.ScreenWidth, Layout.ScreenHeight);
            _gameManager = new(viewportAdapter);

            UpdateWhenInactive = true;
            DrawWhenInactive = true;
            _root = new Panel();
            _root.Dock(Dock.Fill);

            _pauseMenu = new ContainerRuntime();
            _pauseMenu.Width = Layout.ScreenWidth;
            _pauseMenu.Height = Layout.ScreenHeight;
            _pauseMenu.Visible = false;
            _root.AddChild(_pauseMenu);

            //AudioManager.Instance.UnmuteAudio();
            GumUIHelper.AddSettingsPanel(_pauseMenu, true);
            _pauseMenu.AddChild(GumUIHelper.CreateTextButton("Return to Menu", Layout.MenuFontScale, Layout.ActionButtonX, Layout.ActionButtonY, Layout.ActionButtonWidth, Layout.ActionButtonHeight, ReturnToMenu));

            _root.AddChild(GumUIHelper.CreatePauseButton(TogglePause));

            _root.AddToRoot();

            MainGame.GameState = GameState.Gameplay;
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
            if (_pauseMenu.Visible == true) {
                _pauseMenu.Visible = false;
                MainGame.GameState = GameState.Gameplay;
                //AudioManager.Instance.ResumeAudio();
                //Time.TimeScale = 1;
            }
            else if (_pauseMenu.Visible == false) {
                _pauseMenu.Visible = true;
                MainGame.GameState = GameState.Pause;
                //AudioManager.Instance.PauseAudio();
                //Time.TimeScale = 0;
            }
        }

        public override void UnloadContent() {
            _gameManager.Dispose();
        }

        private void ReturnToMenu() {
            if (MainGame.GameState == GameState.Pause) {
                MainGame.ScreenManager.ClearScreens(new MainMenuScreen(), 1, 1);
                MainGame.GameState = GameState.Menu;
            }
        }
    }
}
