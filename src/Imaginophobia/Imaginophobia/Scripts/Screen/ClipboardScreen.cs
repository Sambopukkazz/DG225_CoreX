using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Tweening;

namespace Imaginophobia {
    public class ClipboardScreen : Screen {
        public Vector2 ClipboardPos;
        private Texture2D _clipboard;
        private Player _player;
        private Tweener _tweener;

        public ClipboardScreen(Player player) {
            _clipboard = MainGame.Content.Load<Texture2D>("UI/sprite_clipboard_er1_page1");
            _player = player;
            ClipboardPos = new Vector2(960 - 315, 1080);

            _tweener = new Tweener();
            _tweener.TweenTo(target: this, expression: screen => screen.ClipboardPos, toValue: new Vector2(645, 120), duration: 0.75f)
                .Easing(EasingFunctions.ExponentialOut);
        }

        public override void Update(GameTime gameTime) {
            if (KeyboardExtended.GetState().WasKeyPressed(Keys.A)) {
                AudioManager.Instance.PlaySFX(SoundEffectName.PageReverse);
            }
            else if (KeyboardExtended.GetState().WasKeyPressed(Keys.D)) {
                AudioManager.Instance.PlaySFX(SoundEffectName.PageAdvance);
            }

            if (KeyboardExtended.GetState().WasKeyPressed(Keys.Q)) {
                _player.ToggleReading();
                this.ScreenManager.CloseScreen();
            }

            _tweener.Update(Time.DeltaTime);
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch spriteBatch = MainGame.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.Draw(_clipboard, ClipboardPos, Color.White);
            spriteBatch.End();
        }

        public override void Dispose() {
            _tweener?.Dispose();
        }
    }
}
