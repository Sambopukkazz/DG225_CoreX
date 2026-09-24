using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private Player _player;
        public UIManager(Player player) {
            _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            _spacebar = MainGame.Content.Load<Texture2D>("UI/ui_spacebar");
        }

        public void Update() {

        }

        public void Draw() {
            MainGame.SpriteBatch.DrawString(_font, $"Test", new Vector2(150, 150), Color.White);
            
            if (_player.CanInteract) {
                MainGame.SpriteBatch.Draw(_spacebar, new Vector2(960 - 300, 540 + 350), Color.White);
            }
        }
    }
}
