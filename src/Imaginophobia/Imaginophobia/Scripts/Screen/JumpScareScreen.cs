using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;

namespace Imaginophobia {
    public class JumpScareScreen : Screen {
        Texture2DAtlas _textureAtlas;
        public JumpScareScreen() {
            _textureAtlas = MainGame.Content.Load<Texture2DAtlas>("Phobias/spritesheet_entamaphobia");

            AudioManager.Instance.StopSFX(SoundEffectName.EntamaPresence);

            AudioManager.Instance.PlaySFX(SoundEffectName.EntamaJumpscare, volume: 0.2f);

            MainGame.GameState = GameState.Jumpscare;

            Action action = () => {
                MainGame.GameState = GameState.Gameplay;
                ScreenManager.CloseScreen();
            };

            Time.AddTimer(action, 1.6f);
        }

        public override void Update(GameTime gameTime) { }

        public override void Draw(GameTime gameTime) { 
            SpriteBatch spriteBatch = MainGame.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.Draw(_textureAtlas["entama_jumpscare"], Vector2.Zero, Color.White);
            spriteBatch.End();
        }


    }
}
