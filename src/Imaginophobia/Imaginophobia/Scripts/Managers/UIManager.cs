
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
    public class UIManager : IDisposable {
        private Texture2D _spacebar;
        private Rectangle _spaceBarBackgroundRect;
        private Player _player;
        private OrthographicCamera _camera;
        private VignetteOverlay _nyctophobiaOverlay;
        private VignetteOverlay _vignetteOverlay;
        private VignetteOverlay _blinkOverlay;
        private BottomFadeOverlay _rectangleFadeOverlay;
        private readonly Tweener _tweener;
        private CharacterAction _previousAction;
        private TutorialManager _tutorialManager;
        private TextManager _textManager;
        private bool _blackScreen;
        private VignettePhase _vignettePhase;

        //temp
        List<Keys> pressedKey;

        public UIManager(Player player, OrthographicCamera camera) {
            _tutorialManager = new TutorialManager();
            _textManager = new TextManager();

            _spacebar = MainGame.Content.Load<Texture2D>("UI/sprite_spacebar");

            _vignetteOverlay = new VignetteOverlay();
            _blinkOverlay = new VignetteOverlay();
            _nyctophobiaOverlay = new VignetteOverlay() {
                Softness = 15f,
            };
            _rectangleFadeOverlay = new BottomFadeOverlay();

            _spaceBarBackgroundRect = new Rectangle(960 - (int)((600 * 0.75f) / 2f), 540 + 400, (int)(600 * 0.75f), (int)(100 * 0.75f));

            _tweener = new Tweener();

            _player = player;
            _player.SanityStateChanged += OnSanityStateChanged;

            _camera = camera;

            //temp
            pressedKey = new List<Keys>();
        }

        public void Update() {
            //temp
            foreach (Keys keyPressed in KeyboardExtended.GetState().GetPressedKeys().ToList()) {
                if (pressedKey.Contains(keyPressed) == false) {
                    pressedKey.Add(keyPressed);
                }
            }

            if(pressedKey.Contains(Keys.A) && pressedKey.Contains(Keys.D) && pressedKey.Contains(Keys.F)) {
                ObjectiveManager.Complete("move");
            }
            //endtemp

            if (MainGame.Saturation < 1) {
                MainGame.Saturation += 0.3f * Time.DeltaTime; // 3++ second to normal color
                if (MainGame.Saturation > 1) {
                    MainGame.Saturation = 1;
                }
            }

            ZoomVignette();

            if (_player.Blink) {
                BlinkClose();
                _player.Blink = false;
            }


            _nyctophobiaOverlay.Center = _camera.WorldToScreen(_player.Transform.Position); 

            if (_player.Nyctophobia && _vignettePhase != VignettePhase.StartNyctophobia) {
                NyctoClose();
                _vignettePhase = VignettePhase.StartNyctophobia;
            }
            else if (_player.Nyctophobia == false && _vignettePhase == VignettePhase.StartNyctophobia) {
                NyctoOpen();
                _vignettePhase = VignettePhase.StopNyctophobia;
            }

            if (_nyctophobiaOverlay.Scale == Vector2.Zero) {
                _nyctophobiaOverlay.Scale = new Vector2(0.9f, 0.5f);
                _player.Sanity = 0;
            }

            _previousAction = _player.CurrentAction;

            _tweener.Update(Time.DeltaTime);

            DebugUpdate();
            _textManager.InputTutorial(_tutorialManager.GetText());
            _textManager.Update();
        }

        public void Draw() {
            _vignetteOverlay.Render();
            _blinkOverlay.Render();
            _nyctophobiaOverlay.Render();
            _rectangleFadeOverlay.Render();

            MainGame.SpriteBatch.Begin();
            DebugDraw();
            //MainGame.SpriteBatch.DrawString(_font, $"PlayerPos {_player.Transform.Position} ", new Vector2(150, 400), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"PlayerPos {_player.Transform.WorldPosition} ", new Vector2(150, 420), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Listener {_player.Listener.Position.X}", new Vector2(150, 440), Color.White);
            Color color;
            if (_player.CanInteract && _player.CanRepair) {
                MainGame.SpriteBatch.Draw(_spacebar, _spaceBarBackgroundRect, ColorHelper.FromHex("#70707041"));
                //MainGame.SpriteBatch.Draw(_spacebar, _spaceBarPos, null, ColorHelper.FromHex("#70707041"), 0,Vector2.Zero,0.75f,SpriteEffects.None,0);

                if (_player.CooldownTimer != null && (_player.CurrentAction == CharacterAction.ReadyToHide || _player.CurrentAction == CharacterAction.Hiding)) {
                    
                    float timeFactor;
                    if (_player.CooldownTimer.Name == "cooldownTime") {
                        color = ColorHelper.FromHex("#a40e0e64");
                        timeFactor = 150;
                    }
                    else {
                        color = ColorHelper.FromHex("#0e61a464");
                        timeFactor = 45;
                    }
                    MainGame.SpriteBatch.FillRectangle(960 - 225, 540 + 400, _player.CooldownTimer.TimeLeft * timeFactor, 75, color);
                }
            }

            color = Color.White;
            if(_player.SanityState == SanityState.Anxious) {
                color = Color.Yellow;
            }
            else if (_player.SanityState == SanityState.Insane) {
                color = Color.Red;
            }
            
            MainGame.SpriteBatch.DrawRectangle(210, 100, 300, 20, Color.White);
            MainGame.SpriteBatch.FillRectangle(210, 100, 3 * _player.Sanity, 20, color);
            MainGame.SpriteBatch.DrawRectangle(210, 150, 300, 20, Color.White);
            MainGame.SpriteBatch.FillRectangle(210, 150, 3 * _player.Battery, 20, Color.White);

            _textManager.Draw();

            if (_blackScreen) {
                MainGame.SpriteBatch.FillRectangle(0, 0, 1920, 1080, Color.Black);
            }

            MainGame.SpriteBatch.End();
            

            //MainGame.SpriteBatch.FillRectangle(new Rectangle(0,0,1920,480),Color.Black);


            //foreach (Timer timer in Time.Timers) {
            //    MainGame.SpriteBatch.DrawString(_font, $"\nTimer:{timer.TimeLeft}", new Vector2(100, 500 + (40 * Time.Timers.IndexOf(timer))), Color.White);
            //}
        }

        private void OnSanityStateChanged() {
            if (_player.SanityState == SanityState.Anxious) {
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.55f, 0.4f), duration: 3)
                .Easing(EasingFunctions.QuadraticOut);
            }
            else if (_player.SanityState == SanityState.Insane) {
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.45f, 0.35f), duration: 2)
                .Easing(EasingFunctions.ExponentialOut);
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Color, toValue: ColorHelper.FromHex("#2e0202"), duration: 2)
                .Easing(EasingFunctions.ExponentialOut);
            }
            else if (_player.SanityState == SanityState.Normal) {
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.9f, 0.5f), duration: 1)
                .Easing(EasingFunctions.Linear);
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Color, toValue: Color.Black, duration: 1)
                .Easing(EasingFunctions.Linear);
            }
        }

        public void BlackScreen() {
            AudioManager.Instance.PlaySFX(SoundEffectName.ScreenOff);
            _blackScreen = true;

            Action turnOffBlackScreen = () => {
                _blackScreen = false;
                AudioManager.Instance.PlaySFX(SoundEffectName.ScreenOn);
            };

            if (MainGame.ScreenManager.ActiveScreen is SpinTheValveScreen minigame) {
                minigame.MinigameVisible = false;

                turnOffBlackScreen = () => {
                    AudioManager.Instance.PlaySFX(SoundEffectName.ScreenOn);
                    minigame.MinigameVisible = true;
                    _blackScreen = false;
                };

                Time.AddTimer(turnOffBlackScreen, 1f);
                return;
            }

            Time.AddTimer(turnOffBlackScreen, 1f);
        }

        private void DebugUpdate() {
            //if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
                
            //}
            //if (KeyboardExtended.GetState().WasKeyPressed(Keys.NumPad7)) {
            //    NyctoClose();
            //}
            //if (KeyboardExtended.GetState().WasKeyPressed(Keys.NumPad8)) {
            //    NyctoOpen();
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.Down)) {
            //    _blinkOverlay.Scale += new Vector2(0, 0.01f);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
            //    _blinkOverlay.Scale += new Vector2(0.01f, 0);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
            //    _blinkOverlay.Scale -= new Vector2(0.01f, 0);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad6)) {
            //    _blinkOverlay.Softness += 0.01f;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad4)) {
            //    _blinkOverlay.Softness -= 0.01f;
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
        public void NyctoClose() {
            //_nyctophobiaOverlay.Center.SetX(_player.Transform.Position.X);
            //_tweener.TweenTo(target: _nyctophobiaOverlay, expression: vignette => vignette.Center, toValue: _camera.WorldToScreen(_player.Transform.Position), duration: 3)
            //    .Easing(EasingFunctions.Linear);
            _tweener.TweenTo(target: _nyctophobiaOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0f, 0f), duration: 25)
                .Easing(EasingFunctions.ExponentialOut);
        }

        public void NyctoOpen() {
            //_nyctophobiaOverlay.Center = _camera.WorldToScreen(_player.Transform.Position);
            //_tweener.TweenTo(target: _nyctophobiaOverlay, expression: vignette => vignette.Center, toValue: _camera.WorldToScreen(_player.Transform.Position), duration: 3)
            //    .Easing(EasingFunctions.Linear);
            _tweener.TweenTo(target: _nyctophobiaOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.9f, 0.5f), duration: 10)
                .Easing(EasingFunctions.SineOut);
        }

        public void ShowMessage(string text, float duration = 4f) {
            _textManager.ShowMessage(text, duration);
        }

        public void BlinkClose() {
            //Close eye -> scale Y to 0
            _tweener.TweenTo(target: _blinkOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.332f, 0), duration: 0.15f)
                .Easing(EasingFunctions.Linear)
                .OnEnd(_ => BlinkOpen());
        }

        public void BlinkOpen() {
            _tweener.TweenTo(target: _blinkOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.9f, 0.5f), duration: 0.2f)
                .Easing(EasingFunctions.Linear);
        }

        public void ZoomVignette() {
            if (_player.CurrentAction != CharacterAction.Repairing && _player.CurrentAction != CharacterAction.Checking) {
                _vignetteOverlay.CameraZoom = _camera.Zoom;
            }

            if (_player.CurrentAction == CharacterAction.Hiding) {
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Center, toValue: _camera.WorldToScreen(_player.Transform.Position), duration: 3)
                .Easing(EasingFunctions.Linear);
            }

            if (_previousAction == CharacterAction.ReadyToHide && _player.CurrentAction == CharacterAction.Hiding) {
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.375f, 0.175f), duration: 4)
                .Easing(EasingFunctions.ExponentialOut);
            }
            else if ((_previousAction == CharacterAction.Hiding && _player.CurrentAction == CharacterAction.Idle) || (_previousAction == CharacterAction.Hiding && _player.CurrentAction == CharacterAction.ReadyToHide)) {
                //_vignetteOverlay.Center = new Vector2(960, 540);
                BackToStageScale();
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Center, toValue: new Vector2(960, 540), duration: 3)
                .Easing(EasingFunctions.Linear);

            }
            
        }

        public void BackToStageScale() {
            if (_player.SanityState == SanityState.Normal) {
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.9f, 0.5f), duration: 4)
            .Easing(EasingFunctions.ExponentialOut);
            }
            else if (_player.SanityState == SanityState.Anxious) {
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.55f, 0.4f), duration: 3)
            .Easing(EasingFunctions.ExponentialOut);
            }
            else if (_player.SanityState == SanityState.Insane) {
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.45f, 0.35f), duration: 1)
            .Easing(EasingFunctions.BackOut);
            }
        }

        private void DebugDraw() {
            //BitmapFont _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            //MainGame.SpriteBatch.DrawString(_font, $"Scale {_blinkOverlay.Scale} ", new Vector2(150, 500), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Softness: {_blinkOverlay.Softness}", new Vector2(150, 50), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Sharpenss {_vignetteOverlay.Sharpness}", new Vector2(150, 150), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Animation {_animatedSprite.CurrentAnimation}", new Vector2(150, 200), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Listener {Listener.Position.X}", new Vector2(150, 520), Color.White);

            //foreach (Timer timer in Time.Timers) {
            //    MainGame.SpriteBatch.DrawString(_font, $"\nTimer:{timer.TimeLeft}", new Vector2(100, 500 + (40 * Time.Timers.IndexOf(timer))), Color.White);
            //}
        }

        public void Dispose() {
            _player.SanityStateChanged -= OnSanityStateChanged;
            _tutorialManager.Dispose();
            _vignetteOverlay.Dispose();
            _blinkOverlay.Dispose();
            _nyctophobiaOverlay.Dispose();
            _rectangleFadeOverlay.Dispose();
        }
    }
}
