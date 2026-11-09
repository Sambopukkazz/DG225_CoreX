
using Gum.GueDeriving;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.Particles;
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
        private Texture2DAtlas _buttonAtlas;
        private Texture2D _sanityBar;
        private RectangleF _sanityBarRect;
        private RectangleF _spaceBarBackgroundRect;
        private RectangleF _spaceBarBackgroundMiddleRect;
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

            _buttonAtlas = MainGame.Content.Load<Texture2DAtlas>("UI/spritesheet_buttons");
            _sanityBar = MainGame.Content.Load<Texture2D>("UI/sprite_bar_sanity");

            _vignetteOverlay = new VignetteOverlay();
            _blinkOverlay = new VignetteOverlay();
            _nyctophobiaOverlay = new VignetteOverlay() {
                Softness = 15f,
            };
            _rectangleFadeOverlay = new BottomFadeOverlay();

            _sanityBarRect = new Rectangle(440, 0, 440, 60);
            _spaceBarBackgroundRect = new Rectangle(960 - (int)((600 * 0.75f) / 2f), 540 + 400, 300, 50);
            _spaceBarBackgroundMiddleRect = new Rectangle(960 - (int)((600 * 0.75f) / 2f), 540 + 400, 450, 75);

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

            if (MainGame.ScreenManager.Saturation < 1) {
                MainGame.ScreenManager.Saturation += 0.3f * Time.DeltaTime; // 3++ second to normal color
                if (MainGame.ScreenManager.Saturation > 1) {
                    MainGame.ScreenManager.Saturation = 1;
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
            else if (_player.Nyctophobia == false && _vignettePhase == VignettePhase.StartNyctophobia && _player.TimeOut == false) {
                NyctoOpen();
                _vignettePhase = VignettePhase.StopNyctophobia;
            }

            if (_nyctophobiaOverlay.Scale == Vector2.Zero) {
                _nyctophobiaOverlay.Scale = new Vector2(0.9f, 0.5f);
                _player.Sanity = 0;
            }

            _sanityBarRect.Width = ((100 - _player.Sanity) * 4.4f);
            _sanityBarRect.X = 440 - _sanityBarRect.Width;

            _previousAction = _player.CurrentAction;

            _tweener.Update(Time.DeltaTime);

            _textManager.InputTutorial(_tutorialManager.GetText());
            _textManager.Update();
        }

        public void Draw() {
            _vignetteOverlay.Render();
            _blinkOverlay.Render();
            _nyctophobiaOverlay.Render();
            _rectangleFadeOverlay.Render();

            SpriteBatch spriteBatch = MainGame.SpriteBatch;

            spriteBatch.Begin();
            Color color;
            if (_player.CurrentAction != CharacterAction.Repairing && _player.CurrentAction != CharacterAction.Checking && _player.CurrentAction != CharacterAction.Reading) {
                if (_player.CollidingTrigger != null && _player.CollidingTrigger.Interactable) {
                    if (_player.CollidingTrigger.Tag == "door") {
                        if (_player.CollidingTrigger.Name == Scene.tilemap_sewer.ToString()) {
                            //_spaceBarBackgroundRect.X = _player.CollidingTrigger.Shape.BoundingBox.Center.X - (_spaceBarBackgroundRect.Width / 2f);
                            //_spaceBarBackgroundRect.Y = _player.CollidingTrigger.Shape.BoundingBox.Max.Y - (_spaceBarBackgroundRect.Height * 2);
                            _spaceBarBackgroundMiddleRect.X = 600;
                            _spaceBarBackgroundMiddleRect.Y = 540 + 400;

                            Vector2 buttonPos = _spaceBarBackgroundMiddleRect.Position;
                            buttonPos.X += 500;
                            spriteBatch.Draw(_buttonAtlas["sprite_button_peek"], buttonPos, Color.White);
                        }
                        else {
                            //_spaceBarBackgroundRect.X = _player.CollidingTrigger.Shape.BoundingBox.Max.X;
                            //_spaceBarBackgroundRect.Y = _player.CollidingTrigger.Shape.BoundingBox.Min.Y - _spaceBarBackgroundRect.Height;
                            _spaceBarBackgroundMiddleRect.X = 960 - (_spaceBarBackgroundRect.Width / 2f);
                            _spaceBarBackgroundMiddleRect.Y = 540 + 400 ;
                        }

                        spriteBatch.Draw(_buttonAtlas["sprite_spacebar"], _spaceBarBackgroundMiddleRect.ToRectangle(), Color.White);
                    }
                    else {
                        _spaceBarBackgroundRect.X = _camera.WorldToScreen(_player.CollidingTrigger.Shape.BoundingBox.Center).X - (_spaceBarBackgroundRect.Width / 2f);
                        _spaceBarBackgroundRect.Y = (_camera.WorldToScreen(_player.CollidingTrigger.Shape.BoundingBox.Min).Y - _spaceBarBackgroundRect.Height);
                        //MainGame.SpriteBatch.Draw(_spacebar, _spaceBarBackgroundRect.ToRectangle(), ColorHelper.FromHex("#70707041"));
                        spriteBatch.Draw(_buttonAtlas["sprite_spacebar"], _spaceBarBackgroundRect.ToRectangle(), Color.White);
                    }
                    //_spaceBarBackgroundRect.Position = _camera.WorldToScreen(_spaceBarBackgroundRect.Position);
                    //_spaceBarBackgroundRect.Size = _size * _camera.Zoom;
                }              

                if ((_player.CollidingTrigger?.Tag == "hideout" || _player.CurrentAction == CharacterAction.Hiding)) {

                    Timer timer = Time.GetTimer("hideCooldown");
                    float timeFactor;
                    if (timer != null) {
                        color = ColorHelper.FromHex("#61070786");
                        timeFactor = _spaceBarBackgroundRect.Width /  _player.HideCooldown;
                    }
                    else {
                        timer = Time.GetTimer("hideTime");
                        color = ColorHelper.FromHex("#cccccc64");
                        timeFactor = _spaceBarBackgroundRect.Width / _player.HideTime;
                    }

                    if(timer != null) {
                        spriteBatch.FillRectangle(_spaceBarBackgroundRect.X, _spaceBarBackgroundRect.Y, timer.TimeLeft * timeFactor, _spaceBarBackgroundRect.Height, color);
                    }
                    
                }
            }

            if (MainGame.ScreenManager.ActiveScreen is SpinTheValveScreen) {
                spriteBatch.Draw(_buttonAtlas["sprite_button_exit"], new Vector2(865,905), Color.White * 0.4f);
            }
            else if (MainGame.ScreenManager.ActiveScreen is ConnectTheDotsScreen) {
                spriteBatch.Draw(_buttonAtlas["sprite_button_exit"], new Vector2(1344, 560), Color.White * 0.4f);
            }
            else if (MainGame.ScreenManager.ActiveScreen is ClipboardScreen clipboard && _player.CurrentAction == CharacterAction.Reading) {
                if (clipboard.CurrentPage != 1) {
                    spriteBatch.Draw(_buttonAtlas["sprite_button_left"], new Vector2(385, 506), Color.White * 0.4f);
                }

                if (clipboard.CurrentPage != clipboard.MaxPage) {
                    spriteBatch.Draw(_buttonAtlas["sprite_button_right"], new Vector2(1312, 506), Color.White * 0.4f);
                }

                spriteBatch.Draw(_buttonAtlas["sprite_button_exit"], new Vector2(1312, 605), Color.White * 0.4f);
            }

            color = Color.White;
            if(_player.SanityState == SanityState.Anxious) {
                color = ColorHelper.FromHex("#a89e6c");
            }
            else if (_player.SanityState == SanityState.Insane) {
                color = ColorHelper.FromHex("#752d22");
            }

            //Sanity Bar
            //spriteBatch.DrawRectangle(210, 90, 400, 40, Color.White);
            
            spriteBatch.FillRectangle(210, 90, 4.4f * _player.Sanity, 40, color);
            spriteBatch.Draw(_sanityBar, new Vector2(650 - _sanityBarRect.Width, 80), _sanityBarRect.ToRectangle(), Color.White);

            //Battery Bar
            spriteBatch.DrawRectangle(210, 150, 250, 20, Color.White);
            spriteBatch.FillRectangle(210, 150, 2.5f * _player.Battery, 20, Color.White);
            

            _textManager.Draw();

            if (_blackScreen) {
                spriteBatch.FillRectangle(0, 0, 1920, 1080, Color.Black);
            }

            spriteBatch.End();
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

            if ((_previousAction == CharacterAction.Idle || _previousAction == CharacterAction.Walking) && _player.CurrentAction == CharacterAction.Hiding) {
                _tweener.TweenTo(target: _vignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(0.375f, 0.175f), duration: 4)
                .Easing(EasingFunctions.ExponentialOut);
            }
            else if (_previousAction == CharacterAction.Hiding && _player.CurrentAction == CharacterAction.Idle) {
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

        public void Dispose() {
            _player.SanityStateChanged -= OnSanityStateChanged;
            _tutorialManager.Dispose();
            _vignetteOverlay.Dispose();
            _blinkOverlay.Dispose();
            _nyctophobiaOverlay.Dispose();
            _rectangleFadeOverlay.Dispose();
            _tweener.Dispose();
        }
    }
}
