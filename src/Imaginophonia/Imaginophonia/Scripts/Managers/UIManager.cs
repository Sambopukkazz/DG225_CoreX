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
        private VignetteOverlay _vignetteOverlay;
        private VignetteOverlay _blinkOverlay;
        private BottomFadeOverlay _rectangleFadeOverlay;
        private readonly Tweener _tweener;
        private CharacterAction _previousAction;
        private float _blinkSpeed = 0.4f;
        //temp
        public static float stage;
        List<Keys> ggKey;

        public UIManager(Player player, OrthographicCamera camera) {
            _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            _spacebar = MainGame.Content.Load<Texture2D>("UI/ui_spacebar");

            _vignetteOverlay = new VignetteOverlay();
            _blinkOverlay = new VignetteOverlay();
            _rectangleFadeOverlay = new BottomFadeOverlay();

            _spaceBarBackgroundRect = new Rectangle(960 - (int)((600 * 0.75f) / 2f), 540 + 400, (int)(600 * 0.75f), (int)(100 * 0.75f));

            _tweener = new Tweener();

            _player = player;
            _player.StateChanged += OnSanityStateChanged;

            _camera = camera;

            //temp
            ggKey = new List<Keys>();
        }

        public void Update() {
            //VignetteOverlay.Scale = new Vector2(VignetteOverlay.Scale.X, MathHelper.Lerp(VignetteOverlay.Scale.Y, 0, 0.01f));
            ZoomVignette();
            
            //temp
            foreach (Keys keyPressed in KeyboardExtended.GetState().GetPressedKeys().ToList()) {
                if (ggKey.Contains(keyPressed) == false) {
                    ggKey.Add(keyPressed);
                }
            }

            if(ggKey.Contains(Keys.A) && ggKey.Contains(Keys.D) && ggKey.Contains(Keys.F) && stage == 0) {
                stage++;
            }
            else if (stage == 1 && _player.CurrentAction == CharacterAction.Repairing) {
                stage++;
            }
            //end temp
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
            _blinkOverlay.Render();
            _rectangleFadeOverlay.Render();

            MainGame.SpriteBatch.Begin();
            DebugDraw();

            //temp
            string dialogue = "";
            if (stage == 0) {
                dialogue = "Press A/D to walk around. F to open a flashlight";
            }
            else if (stage == 1) {
                dialogue = "Head to the electrical panel to do a task";
            }
            else if (stage == 2) {
                dialogue = "Connect all the dots. \nYou can press A/D to look left and right while you are doing the task, \nand Q to leave the task";
            }
            else if (stage == 3) {
                dialogue = "Spin the valve and stay on the green gauge";
            }
            else if (stage == 4) {
                dialogue = "Connect all the dots";
            }
            else if (stage == 5) {
                dialogue = "Enter the locker to hide";
            }
            else if (stage == 6) {
                dialogue = "";
            }
            else if (stage == 7) {
                dialogue = "Leave the room on the left side to go fix pipe. Look for valve";
            }
            else if (stage == 8) {
                dialogue = "test";
            }
            MainGame.SpriteBatch.DrawString(_font, dialogue, new Vector2(700, 100), Color.White);
            //endtemp
            //MainGame.SpriteBatch.DrawString(_font, $"PlayerPos {_player.Transform.Position} ", new Vector2(150, 400), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"PlayerPos {_player.Transform.WorldPosition} ", new Vector2(150, 420), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Listener {_player.Listener.Position.X}", new Vector2(150, 440), Color.White);

            if (_player.CanInteract && _player.CanRepair) {
                MainGame.SpriteBatch.Draw(_spacebar, _spaceBarBackgroundRect, ColorHelper.FromHex("#70707041"));
                //MainGame.SpriteBatch.Draw(_spacebar, _spaceBarPos, null, ColorHelper.FromHex("#70707041"), 0,Vector2.Zero,0.75f,SpriteEffects.None,0);

                if (_player.CooldownTimer != null && (_player.CurrentAction == CharacterAction.ReadyToHide || _player.CurrentAction == CharacterAction.Hiding)) {
                    Color color;
                    float timeFactor;
                    if (_player.CooldownTimer.Name == "cooldownTime") {
                        color = ColorHelper.FromHex("#a40e0e64");
                        timeFactor = 150;
                    }
                    else {
                        color = ColorHelper.FromHex("#0e61a464");
                        timeFactor = 56.25f;
                    }
                    MainGame.SpriteBatch.FillRectangle(960 - 225, 540 + 400, _player.CooldownTimer.TimeLeft * timeFactor, 75, color);
                }
            }

            Color colorasd = Color.White;
            if(_player.SanityState == SanityState.Anxious) {
                colorasd = Color.Yellow;
            }
            else if (_player.SanityState == SanityState.Insane) {
                colorasd = Color.Red;
            }
            MainGame.SpriteBatch.DrawString(_font, $"Sanity: ", new Vector2(100, 100), Color.White);
            MainGame.SpriteBatch.DrawString(_font, $"Battery: ", new Vector2(100, 150), Color.White);
            MainGame.SpriteBatch.DrawRectangle(210, 100, 300, 20, Color.White);
            MainGame.SpriteBatch.FillRectangle(210, 100, 3 * _player.Sanity, 20, colorasd);
            MainGame.SpriteBatch.DrawRectangle(210, 150, 300, 20, Color.White);
            MainGame.SpriteBatch.FillRectangle(210, 150, 3 * _player.Battery, 20, Color.White);

            MainGame.SpriteBatch.End();
            

            //MainGame.SpriteBatch.FillRectangle(new Rectangle(0,0,1920,480),Color.Black);


            //foreach (Timer timer in Time.Timers) {
            //    MainGame.SpriteBatch.DrawString(_font, $"\nTimer:{timer.TimeLeft}", new Vector2(100, 500 + (40 * Time.Timers.IndexOf(timer))), Color.White);
            //}
        }

        private void OnSanityStateChanged() {
            if (_player.SanityState == SanityState.Anxious) {
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.5f, 0.5f), duration: 2)
                .Easing(EasingFunctions.Linear);
            }
            else if (_player.SanityState == SanityState.Insane) {
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.45f, 0.45f), duration: 4)
                .Easing(EasingFunctions.Linear);
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Color, toValue: ColorHelper.FromHex("#2c0000"), duration: 4)
                .Easing(EasingFunctions.Linear);
            }
            else if (_player.SanityState == SanityState.Normal) {
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.9f, 0.5f), duration: 1)
                .Easing(EasingFunctions.Linear);
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Color, toValue: Color.Black, duration: 1)
                .Easing(EasingFunctions.Linear);
            }
        }

        private void DebugUpdate() {
            //if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
            //    _vignetteOverlay.Scale -= new Vector2(0, 0.01f);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.Down)) {
            //    _vignetteOverlay.Scale += new Vector2(0, 0.01f);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
            //    _vignetteOverlay.Scale += new Vector2(0.01f, 0);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
            //    _vignetteOverlay.Scale -= new Vector2(0.01f, 0);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad6)) {
            //    _vignetteOverlay.Softness += 0.01f;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad4)) {
            //    _vignetteOverlay.Softness -= 0.01f;
            //}

            //if (Keyboard.GetState().IsKeyDown(Keys.I)) {
            //    RectangleFadeOverlay.Scale -= new Vector2(0, 0.01f);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.K)) {
            //    RectangleFadeOverlay.Scale += new Vector2(0, 0.01f);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.L)) {
            //    RectangleFadeOverlay.Scale += new Vector2(0.01f, 0);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.J)) {
            //    RectangleFadeOverlay.Scale -= new Vector2(0.01f, 0);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.T)) {
            //    RectangleFadeOverlay.Softness += 0.01f;
            //}
            //if (KeyboardExtended.GetState().WasKeyPressed(Keys.G)) {
            //    _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.9f, 0.5f), duration: 3)
            //    .Easing(EasingFunctions.Linear);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad8)) {
            //    _vignetteOverlay.Sharpness += 0.01f;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad2)) {
            //    _vignetteOverlay.Sharpness -= 0.01f;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad1)) {
            //    _vignetteOverlay.Color = ColorHelper.FromHex("#2c0000");
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad0)) {
            //    _vignetteOverlay.Color = Color.Black;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad3)) {
            //    _scotopicLight.Enabled = false;
            //}

        }

        public void BlinkFx() {
            _tweener.TweenTo(target: _blinkOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(_vignetteOverlay.Scale.X, 0), duration: _blinkSpeed)
                .Repeat(1)
                .AutoReverse()
                .Easing(EasingFunctions.Linear);
        }

        public void ZoomVignette() {
            if (_player.CurrentAction != CharacterAction.Repairing && _player.CurrentAction != CharacterAction.Checking) {
                _vignetteOverlay.CameraZoom = _camera.Zoom;
            }
            if (_player.CurrentAction == CharacterAction.Hiding) {
                //_vignetteOverlay.Center = _camera.WorldToScreen(_player.Transform.Position);
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Center, toValue: _camera.WorldToScreen(_player.Transform.Position), duration: 3)
                .Easing(EasingFunctions.Linear);
            }
            if (_previousAction == CharacterAction.ReadyToHide && _player.CurrentAction == CharacterAction.Hiding) {
                //_vignetteOverlay.Center = _camera.WorldToScreen(_player.Transform.Position);
                //_tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Center, toValue: _camera.WorldToScreen(_player.Transform.Position), duration: 3)
                //.Easing(EasingFunctions.Linear);
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.1f, 0.2f), duration: 8)
                .Easing(EasingFunctions.Linear);
            }
            else if ((_previousAction == CharacterAction.Hiding && _player.CurrentAction == CharacterAction.Idle) || (_previousAction == CharacterAction.Hiding && _player.CurrentAction == CharacterAction.ReadyToHide)) {
                //_vignetteOverlay.Center = new Vector2(960, 540);
                if (_player.SanityState == SanityState.Normal) {
                    _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.9f, 0.5f), duration: 3)
                .   Easing(EasingFunctions.Linear);
                }
                else if (_player.SanityState == SanityState.Anxious) {
                    _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.5f, 0.5f), duration: 3)
                .   Easing(EasingFunctions.Linear);
                }
                else if (_player.SanityState == SanityState.Insane) {
                    _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.4f, 0.45f), duration: 3)
                .Easing(EasingFunctions.Linear);
                }
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Center, toValue: new Vector2(960, 540), duration: 3)
                .Easing(EasingFunctions.Linear);

            }
            
        }

        private void DebugDraw() {
            //BitmapFont _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            //MainGame.SpriteBatch.DrawString(_font, $"Scale {_vignetteOverlay.Scale} ", new Vector2(150, 500), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Brightness: {_vignetteOverlay.Softness}", new Vector2(150, 50), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Sharpenss {_vignetteOverlay.Sharpness}", new Vector2(150, 150), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Animation {_animatedSprite.CurrentAnimation}", new Vector2(150, 200), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Listener {Listener.Position.X}", new Vector2(150, 520), Color.White);

            //foreach (Timer timer in Time.Timers) {
            //    MainGame.SpriteBatch.DrawString(_font, $"\nTimer:{timer.TimeLeft}", new Vector2(100, 500 + (40 * Time.Timers.IndexOf(timer))), Color.White);
            //}
        }
    }
}
