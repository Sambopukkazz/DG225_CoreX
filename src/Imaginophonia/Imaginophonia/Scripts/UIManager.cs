using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class UIManager {
        private BitmapFont _font;
        private Texture2D _spacebar;
        private Vector2 _spaceBarPos;
        private Player _player;
        public UIManager(Player player) {
            _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            _spacebar = MainGame.Content.Load<Texture2D>("UI/ui_spacebar");

            _player = player;
            _spaceBarPos = new Vector2(960 - 300, 540 + 350);
        }

        public void Update() {

        }

        public void Draw() {
            MainGame.SpriteBatch.DrawString(_font, $"Objective : Fix everything and leave\nSanity : {_player.Sanity}\nBattery : {_player.Battery}", new Vector2(100, 100), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"PlayerPos {_player.Transform.Position} ", new Vector2(150, 400), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"PlayerPos {_player.Transform.WorldPosition} ", new Vector2(150, 420), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Listener {_player.Listener.Position.X}", new Vector2(150, 440), Color.White);

            if (_player.CanInteract) {
                MainGame.SpriteBatch.Draw(_spacebar, _spaceBarPos, Color.White);
            }

            //MainGame.SpriteBatch.FillRectangle(Transform.Position.X + 50, Transform.Position.Y, 15, 120, Color.DarkGray);
            //MainGame.SpriteBatch.FillRectangle(Transform.Position.X + 50, Transform.Position.Y, 15, _timer.TimeLeft * 40, Color.LightBlue);


            //foreach (Timer timer in Time.Timers) {
            //    MainGame.SpriteBatch.DrawString(_font, $"\nTimer:{timer.TimeLeft}", new Vector2(100, 500 + (40 * Time.Timers.IndexOf(timer))), Color.White);
            //}
        }
    }
}
