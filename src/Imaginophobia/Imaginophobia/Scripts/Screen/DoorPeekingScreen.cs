using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace Imaginophobia {
    public class DoorPeekingScreen : Screen {
        private Trigger _trigger;
        private Player _player;
        public DoorPeekingScreen(Player player, Trigger trigger) {
            _trigger = trigger;
            _player = player;
        }
        public override void Update(GameTime gameTime) {
            if (KeyboardExtended.GetState().WasKeyPressed(Keys.Q)) {
                _player.TogglePeek();
                this.ScreenManager.CloseScreen();
            }

            //_trigger.Progress = _progressValue;
        }

        public override void Draw(GameTime gameTime) {
            MainGame.SpriteBatch.Begin();
            MainGame.SpriteBatch.FillRectangle(0, 0, 500, 500, Color.Green);
            MainGame.SpriteBatch.End();
        }
    }
}
