using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Timers;
using MonoGame.Extended.Tweening;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class UIManager {
        private BitmapFont _font;
        private Texture2D _spacebar;
        private Rectangle _spaceBarBackgroundRect;
        private Player _player;
        private OrthographicCamera _camera;
        private FadeOverlay _vignetteOverlay;
        public FadeOverlay RectangleFadeOverlay;
        private readonly Tweener _tweener;
        private CharacterAction _previousAction;
        private float _blinkSpeed;

        public UIManager(Player player, OrthographicCamera camera) {
            _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            _spacebar = MainGame.Content.Load<Texture2D>("UI/ui_spacebar");

            _vignetteOverlay = new FadeOverlay();
            RectangleFadeOverlay = new FadeOverlay() {
                Center = new Vector2(500, 800),
                Shape = "Rectangle",
                Scale = new Vector2(100,100)
            };

            _spaceBarBackgroundRect = new Rectangle(960 - (int)((600 * 0.75f) / 2f), 540 + 400, (int)(600 * 0.75f), (int)(100 * 0.75f));

            _tweener = new Tweener();

            _player = player;
            _player.StateChanged += OnSanityStateChanged;

            _camera = camera;
        }

        public void Update() {
            //VignetteOverlay.Scale = new Vector2(VignetteOverlay.Scale.X, MathHelper.Lerp(VignetteOverlay.Scale.Y, 0, 0.01f));
            ZoomVignette();
            _vignetteOverlay.CameraZoom = _camera.Zoom;
            //if (_player.CurrentAction == CharacterAction.Hiding) {
            //    _vignetteOverlay.Center = _camera.WorldToScreen(_player.Transform.Position);
            //    _vignetteOverlay.Scale = new Vector2(0.1f, 0.2f);
            //    _vignetteOverlay.Softness = 1;
            //}

            if (_player.Blink) {
                BlinkFx();
                _player.Blink = false;
            }

            _previousAction = _player.CurrentAction;

            _tweener.Update(Time.DeltaTime);

            DebugUpdate();
        }

        public void Draw() {
            _vignetteOverlay.Render();
            RectangleFadeOverlay.Render();

            MainGame.SpriteBatch.Begin();
            DebugDraw();

            MainGame.SpriteBatch.DrawString(_font, $"Objective : Fix everything and leave\nSanity : {_player.Sanity}\nBattery : {_player.Battery}", new Vector2(100, 100), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"PlayerPos {_player.Transform.Position} ", new Vector2(150, 400), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"PlayerPos {_player.Transform.WorldPosition} ", new Vector2(150, 420), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Listener {_player.Listener.Position.X}", new Vector2(150, 440), Color.White);

            if (_player.CanInteract && _player.CanRepair) {
                MainGame.SpriteBatch.Draw(_spacebar, _spaceBarBackgroundRect, ColorHelper.FromHex("#70707041"));
                //MainGame.SpriteBatch.Draw(_spacebar, _spaceBarPos, null, ColorHelper.FromHex("#70707041"), 0,Vector2.Zero,0.75f,SpriteEffects.None,0);

                if (_player.CooldownTimer != null && (_player.CurrentAction == CharacterAction.ReadyToHide || _player.CurrentAction == CharacterAction.Hiding)) {
                    Color color;
                    if (_player.CooldownTimer.Name == "cooldownTime") {
                        color = ColorHelper.FromHex("#a40e0e64");
                    }
                    else {
                        color = ColorHelper.FromHex("#0e61a464");
                    }
                    MainGame.SpriteBatch.FillRectangle(960 - 225, 540 + 400, _player.CooldownTimer.TimeLeft * 150, 75, color);
                }
            }

            

            MainGame.SpriteBatch.End();
            

            //MainGame.SpriteBatch.FillRectangle(new Rectangle(0,0,1920,480),Color.Black);

            


            //foreach (Timer timer in Time.Timers) {
            //    MainGame.SpriteBatch.DrawString(_font, $"\nTimer:{timer.TimeLeft}", new Vector2(100, 500 + (40 * Time.Timers.IndexOf(timer))), Color.White);
            //}
        }

        private void OnSanityStateChanged() {
            if (_player.SanityState == SanityState.Anxious) {
                _blinkSpeed = 0.5f;
                _vignetteOverlay.Scale = new Vector2(0.5f, 0.5f);
            }
            else if (_player.SanityState == SanityState.Normal) {
                _blinkSpeed = 0.4f;
                _vignetteOverlay.Scale = new Vector2(0.9f, 0.5f);
            }
        }

        private void DebugUpdate() {
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
                _vignetteOverlay.Scale -= new Vector2(0, 0.01f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) {
                _vignetteOverlay.Scale += new Vector2(0, 0.01f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
                _vignetteOverlay.Scale += new Vector2(0.01f, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
                _vignetteOverlay.Scale -= new Vector2(0.01f, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad6)) {
                _vignetteOverlay.Softness += 0.01f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad4)) {
                _vignetteOverlay.Softness -= 0.01f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.I)) {
                RectangleFadeOverlay.Scale -= new Vector2(0, 0.01f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.K)) {
                RectangleFadeOverlay.Scale += new Vector2(0, 0.01f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.L)) {
                RectangleFadeOverlay.Scale += new Vector2(0.01f, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.J)) {
                RectangleFadeOverlay.Scale -= new Vector2(0.01f, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.T)) {
                RectangleFadeOverlay.Softness += 0.01f;
            }
            if (KeyboardExtended.GetState().WasKeyPressed(Keys.G)) {
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.9f, 0.5f), duration: 3)
                .Easing(EasingFunctions.Linear);
            }
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad8)) {
            //    _vignetteOverlay.Sharpness += 0.01f;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad2)) {
            //    _vignetteOverlay.Sharpness -= 0.01f;
            //}
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad1)) {
                _vignetteOverlay.Color = Color.Red;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad0)) {
                _vignetteOverlay.Color = Color.Black;
            }
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad3)) {
            //    _scotopicLight.Enabled = false;
            //}

        }

        public void BlinkFx() {
            _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(_vignetteOverlay.Scale.X, 0), duration: _blinkSpeed)
                .Repeat(1)
                .AutoReverse()
                .Easing(EasingFunctions.Linear);
        }

        public void ZoomVignette() {
            if (_previousAction == CharacterAction.ReadyToHide && _player.CurrentAction == CharacterAction.Hiding) {
                _vignetteOverlay.Center = _camera.WorldToScreen(_player.Transform.Position);
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.1f, 0.2f), duration: 3)
                .Easing(EasingFunctions.Linear);
            }
            else if ((_previousAction == CharacterAction.Hiding && _player.CurrentAction == CharacterAction.Idle) || (_previousAction == CharacterAction.Hiding && _player.CurrentAction == CharacterAction.ReadyToHide)) {
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.9f, 0.5f), duration: 3)
                .Easing(EasingFunctions.Linear);
            }
            
        }

        private void DebugDraw() {
            BitmapFont _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            MainGame.SpriteBatch.DrawString(_font, $"Scale {_vignetteOverlay.Scale} ", new Vector2(150, 500), Color.White);
            MainGame.SpriteBatch.DrawString(_font, $"Brightness: {_vignetteOverlay.Softness}", new Vector2(150, 50), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Sharpenss {_vignetteOverlay.Sharpness}", new Vector2(150, 150), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Animation {_animatedSprite.CurrentAnimation}", new Vector2(150, 200), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Listener {Listener.Position.X}", new Vector2(150, 520), Color.White);

            //foreach (Timer timer in Time.Timers) {
            //    MainGame.SpriteBatch.DrawString(_font, $"\nTimer:{timer.TimeLeft}", new Vector2(100, 500 + (40 * Time.Timers.IndexOf(timer))), Color.White);
            //}
        }
    }
}
