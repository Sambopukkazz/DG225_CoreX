using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    
    public class BottomFadeOverlay {
        private readonly Texture2D _blankTexture;
        private readonly Effect _shader;
        private Rectangle _rect;

        public BottomFadeOverlay() {
            _shader = MainGame.Content.Load<Effect>("Shaders/BottomFade");

            _blankTexture = new Texture2D(MainGame.GraphicsDevice, 1, 1);
            _blankTexture.SetData(new[] { Color.White });

            _rect = new Rectangle(0,0,1920,480);
            _shader.Parameters["FadePercentage"]?.SetValue(0.5f);
        }

        public void Render() {
            MainGame.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, effect: _shader);
            MainGame.SpriteBatch.Draw(_blankTexture, _rect, Color.Black);
            MainGame.SpriteBatch.End();
        }
    }
}
