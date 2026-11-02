using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
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
        public int CurrentPage = 1;
        public int MaxPage = 4;
        private Random _rand;
        private float _lensAlpha = 0f;
        private float _zoom = 4f;
        private int _lastScroll = 0;

        // Magnifier lens. Glass = LensSize - 2*Bezel = 240, which every integer zoom
        // from 2 to 8 divides exactly (except 7), so the zoom never resamples the texels.
        private const int LensSize = 256;
        private const int Bezel = 8;
        private bool _secret;
        private const int InnerLine = 3;

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

            AudioManager.Instance.PlaySFX(SoundEffectName.ClipboardCheck, volume: 0.3f);

            Time.GetTimer("MainTimer").Pause();
            Time.GetTimer("EntamaSpawnTimer").Pause();

            _tweener = new Tweener();
            _tweener.TweenTo(target: this, expression: screen => screen.ClipboardPos, toValue: new Vector2(645, 120), duration: 0.75f)
                .Easing(EasingFunctions.ExponentialOut);

            _rand = new Random();
        }

        public override void Update(GameTime gameTime) {
            if (InputManager.PressedKey(Keys.A) && _player.CurrentAction == CharacterAction.Reading) {
                if (CurrentPage > 1) {
                    CurrentPage--;
                    AudioManager.Instance.PlaySFX(SoundEffectName.ClipboardPageReverse, volume: 0.15f);
                }
            }
            else if (InputManager.PressedKey(Keys.D) && _player.CurrentAction == CharacterAction.Reading) {
                if (CurrentPage < MaxPage) {
                    CurrentPage++;
                    if (CurrentPage == 4 && _rand.Next(1, 101) < 3) {
                        _secret = true;
                    }
                    AudioManager.Instance.PlaySFX(SoundEffectName.ClipboardPageAdvance, volume: 0.15f);
                }
            }

            if (InputManager.PressedKey(Keys.Q) && _player.CurrentAction == CharacterAction.Reading) {
                _player.ToggleReading();
                AudioManager.Instance.PlaySFX(SoundEffectName.ClipboardCheck, volume: 0.3f);

                _tweener = new Tweener();
                _tweener.TweenTo(target: this, expression: screen => screen.ClipboardPos, toValue: new Vector2(645, 1080), duration: 0.75f)
                    .Easing(EasingFunctions.ExponentialOut);

                Time.AddTimer(this.ScreenManager.CloseScreen, 0.75f);
                Time.GetTimer("MainTimer").Resume();
                Time.GetTimer("EntamaSpawnTimer").Resume();
            }

            _tweener.Update(Time.DeltaTime);

            UpdateLens();
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch spriteBatch = MainGame.SpriteBatch;

            Texture2DRegion page = PageRegion;

            spriteBatch.Begin();

            spriteBatch.Draw(page, ClipboardPos, Color.White);

            spriteBatch.End();

            if (_lensAlpha > 0.01f) {
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);

                DrawLens(spriteBatch, page);

                spriteBatch.End();
            }
        }

        public override void Dispose() {
            _tweener?.Dispose();
        }

        private Texture2DRegion PageRegion {
            get {
                if (_secret == true && CurrentPage == 4) {
                    return _textureAtlas["sprite_clipboard_secret"];
                }
                else {
                    return _textureAtlas[$"sprite_clipboard_{_currentRoom}_page{CurrentPage}"];
                }
            }
        }

        private Rectangle PageRect {
            get {
                Texture2DRegion page = PageRegion;

                return new Rectangle((int)ClipboardPos.X, (int)ClipboardPos.Y, page.Width, page.Height);
            }
        }

        // The lens exists only while the page is settled on screen and the player is reading it
        private void UpdateLens() {
            MouseStateExtended mouse = MouseExtended.GetState();

            if (mouse.ScrollWheelValue != _lastScroll) {
                float steps = (mouse.ScrollWheelValue - _lastScroll) / 120f;
                _zoom = MathHelper.Clamp(_zoom + steps, 1.5f, 8f);
                _lastScroll = mouse.ScrollWheelValue;
            }

            bool settled = ClipboardPos.Y <= 121f;
            bool reading = _player.CurrentAction == CharacterAction.Reading;
            bool hovering = PageRect.Contains(mouse.Position);

            float target = 0f;
            if (settled && reading && hovering) {
                target = 1f;
            }

            _lensAlpha = MathHelper.Clamp(_lensAlpha + (target - _lensAlpha) * Time.DeltaTime * 12f, 0f, 1f);
        }

        private void DrawLens(SpriteBatch spriteBatch, Texture2DRegion page) {
            Rectangle pageRect = PageRect;
            MouseStateExtended mouseState = MouseExtended.GetState();
            Vector2 mouse = new Vector2(mouseState.X, mouseState.Y);

            Rectangle lens = new Rectangle((int)(mouse.X - LensSize / 2f), (int)(mouse.Y - LensSize / 2f), LensSize, LensSize);

            Rectangle inner = new Rectangle(lens.X + Bezel, lens.Y + Bezel, LensSize - Bezel * 2, LensSize - Bezel * 2);

            // World-space square the glass shows
            float sourceEdge = inner.Width / _zoom;
            Rectangle source = new Rectangle((int)MathF.Round(mouse.X - sourceEdge / 2f), (int)MathF.Round(mouse.Y - sourceEdge / 2f),
                                             (int)MathF.Round(sourceEdge), (int)MathF.Round(sourceEdge));

            // Shadow first, then a black fill so the room never shows through the glass
            spriteBatch.FillRectangle(new Rectangle(lens.X + 10, lens.Y + 10, lens.Width, lens.Height), Color.Black * (_lensAlpha * 0.35f));
            spriteBatch.FillRectangle(inner, Color.Black * _lensAlpha);

            // Only the part of the source square that lies on the page can be drawn, mapped back onto the glass
            Rectangle valid = Rectangle.Intersect(source, pageRect);
            if (valid.Width > 0 && valid.Height > 0) {
                float scale = (float)inner.Width / source.Width;

                Rectangle destination = new Rectangle(inner.X + (int)MathF.Round((valid.X - source.X) * scale),
                                                      inner.Y + (int)MathF.Round((valid.Y - source.Y) * scale),
                                                      Math.Max(1, (int)MathF.Round(valid.Width * scale)),
                                                      Math.Max(1, (int)MathF.Round(valid.Height * scale)));

                Rectangle atlasSource = new Rectangle(page.X + (valid.X - pageRect.X), page.Y + (valid.Y - pageRect.Y),
                                                      valid.Width, valid.Height);

                spriteBatch.Draw(page.Texture, destination, atlasSource, Color.White * _lensAlpha);
                
            }
            spriteBatch.DrawRectangle(inner, Color.White * _lensAlpha, 3);
            //spriteBatch.DrawRectangle(lens, Color.Black * _lensAlpha, 2);
        }
    }
}
