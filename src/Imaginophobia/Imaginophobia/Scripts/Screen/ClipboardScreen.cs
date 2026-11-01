using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class ClipboardScreen : Screen {
        public Vector2 ClipboardPos;
        private Player _player;
        private Texture2DAtlas _textureAtlas;
        private Tweener _tweener;
        private string _currentRoom;
        private int _page = 1;

        public ClipboardScreen(Player player) {
            _textureAtlas = MainGame.Content.Load<Texture2DAtlas>("UI/spritesheet_clipboard");

            _player = player;

            if (_player.CollidingTrigger.Name == "Story1") {
                _currentRoom = "er1";
            }
            else {
                _currentRoom = "er2";
            }

            ClipboardPos = new Vector2(960 - 315, 1080);

            AudioManager.Instance.PlaySFX(SoundEffectName.ClipboardCheck);

            _tweener = new Tweener();
            _tweener.TweenTo(target: this, expression: screen => screen.ClipboardPos, toValue: new Vector2(645, 120), duration: 0.75f)
                .Easing(EasingFunctions.ExponentialOut);
        }

        public override void Update(GameTime gameTime) {
            if (InputManager.PressedKey(Keys.A) && _player.CurrentAction == CharacterAction.Reading) {
                if (_page > 1) {
                    _page--;
                    AudioManager.Instance.PlaySFX(SoundEffectName.ClipboardPageReverse, interrupt: true);
                }
            }
            else if (InputManager.PressedKey(Keys.D) && _player.CurrentAction == CharacterAction.Reading) {
                if (_page < 4) {
                    _page++;
                    AudioManager.Instance.PlaySFX(SoundEffectName.ClipboardPageAdvance);
                }
            }

            if (InputManager.PressedKey(Keys.Q) && _player.CurrentAction == CharacterAction.Reading) {
                _player.ToggleReading();
                AudioManager.Instance.PlaySFX(SoundEffectName.ClipboardCheck);

                _tweener = new Tweener();
                _tweener.TweenTo(target: this, expression: screen => screen.ClipboardPos, toValue: new Vector2(645, 1080), duration: 0.75f)
                    .Easing(EasingFunctions.ExponentialOut);

                Time.AddTimer(this.ScreenManager.CloseScreen, 0.75f);
            }

            _tweener.Update(Time.DeltaTime);
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch spriteBatch = MainGame.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.Draw(_textureAtlas[$"sprite_clipboard_{_currentRoom}_page{_page}"], ClipboardPos, Color.White);
            spriteBatch.End();
        }

        public override void Dispose() {
            _tweener?.Dispose();
        }
    }
}
