
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Tweening;
using Penumbra;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Imaginophobia {
    public class Player : GameObject, ISpatialSoundApplicable, ICollisionActor, IDisposable {
        public int Id { get; }
        public CollisionShape2D Shape { get; private set; }
        private readonly Vector2 _eyeLevel;
        private LineSegment2D _eyeSight;
        public LineSegment2D EyeSight => _eyeSight;
        private float _eyeSightLength;
        private readonly Vector2 _size;
        private AnimatedSprite _animatedSprite;
        public AudioListener Listener { get; private set; }
        private PointLight _scotopicLight;
        public PointLight ScotopicLight => _scotopicLight;
        private Spotlight _flashLight;
        public Spotlight FlashLight => _flashLight;
        public Vector2 Direction { get; private set; }
        public Vector2 Velocity { get; private set; }
        private Vector2 _flashlightOffset;
        public float MoveSpeed { get; private set; } = 250f;
        private int _previousFrame;
        public SanityState SanityState { get; set; }
        public CharacterAction CurrentAction { get; set; }
        private bool _lockFacingDirection;
        public float Sanity { get; set; } = 100;
        public float Battery { get; set; } = 100;
        private bool _allowMovement { get; set; }
        //private bool _readyToHide;
        public readonly float HideTime = 10f;
        public readonly float HideCooldown = 3f;
        public float FocusLevel { get; private set; }
        public Vector2 CameraTarget;
        public bool Blink { get; set; }
        public bool Nyctophobia { get; set; }
        public event Action SanityStateChanged;
        public Vector2 PreviousPos { get; set; }
        private float _sewerSanityRedectionSpeed = 0.5f;
        private (float, float, float) _availableSanityRedectionSpeed = (0.5f, 0.2f, 0f); //per second Normal,Anxious,Insane
        public Trigger CollidingTrigger { get; set; }
        private Tweener _tweener;


        public Player() : base("Player", "Player"){
            SetUpAnimation();

            //SetUp Light
            _scotopicLight = new PointLight() {
                Color = Color.FromHSV(150f, 0.5f, 0.5f),
                Scale = new Vector2(500, 500),
                Intensity = 0.5f,
            };

            _flashLight = new Spotlight() {
                Color = Color.FromHSV(150f, 0.5f, 0.5f),
                Scale = new Vector2(600, 1250),
                Intensity = 1,
                Enabled = false
            };
            _flashlightOffset = new Vector2(85,-12);

            LightManager.Penumbra.Lights.Add(_scotopicLight);
            LightManager.Penumbra.Lights.Add(_flashLight);

            //Listener
            Listener = new AudioListener();
            _size = new Vector2(100, 240);
            Transform.Scale = Vector2.One * 3f;

            //Eyesight
            _eyeLevel = new Vector2(20,-85);
            _eyeSight = new LineSegment2D();

            _tweener = new Tweener();

            _allowMovement = true;

            UpdateShape();
        }

        public override void Update() {
            if (_allowMovement) {
                Direction = InputManager.Direction;
                Velocity = MoveSpeed * InputManager.Direction;
                _lockFacingDirection = false;

                if (KeyboardExtended.GetState().IsKeyDown(Keys.LeftControl)) {
                    Velocity /= 1.7f;
                    _lockFacingDirection = true;
                }
                

                Transform.Position = Transform.Position.Translate(Velocity.X * Time.DeltaTime, 0);
                Listener.Position = new Vector3(Transform.Position, 0);

                _scotopicLight.Position = Transform.Position;
                
                Animate();
                _animatedSprite.Update(Time.ElapsedTime);

                UpdateShape();
            }

            if (KeyboardExtended.GetState().WasKeyPressed(Keys.F) && CurrentAction != CharacterAction.Interacting && CurrentAction != CharacterAction.Hiding) {
                ToggleFlashLight();
            }

            if (CurrentAction == CharacterAction.Interacting || CurrentAction == CharacterAction.Checking) {
                //if (KeyboardExtended.GetState().IsKeyDown(Keys.A)) {
                //    CameraTarget.X = MathHelper.Lerp(CameraTarget.X, Transform.Position.X - 400, 0.1f);
                //    _animatedSprite.Effect = SpriteEffects.FlipHorizontally;
                //    _animatedSprite.SetAnimation("check");
                //    CurrentAction = CharacterAction.Checking;
                //}
                //else if (KeyboardExtended.GetState().IsKeyDown(Keys.D)) {
                //    CameraTarget.X = MathHelper.Lerp(CameraTarget.X, Transform.Position.X + 400, 0.1f);
                //    _animatedSprite.Effect = SpriteEffects.None;
                //    _animatedSprite.SetAnimation("check");
                //    CurrentAction = CharacterAction.Checking;
                //}
                //else {
                //    _animatedSprite.SetAnimation("back");
                //    CurrentAction = CharacterAction.Interacting;
                //    CameraTarget.X = MathHelper.Lerp(CameraTarget.X, Transform.Position.X, 0.01f);
                //    if (_flashLight.Enabled) {
                //        ToggleFlashLight();
                //    }
                //    _eyeSightLength = 0;
                //}

                Vector2 pos = Transform.Position;
                if (KeyboardExtended.GetState().WasKeyPressed(Keys.A)) {
                    pos.X -= 400;
                    _tweener.TweenTo(target: this, expression: player => player.CameraTarget, toValue: pos, duration: 0.5f)
                            .Easing(EasingFunctions.QuadraticOut);
                    _animatedSprite.Effect = SpriteEffects.FlipHorizontally;
                    _animatedSprite.SetAnimation("check");
                    CurrentAction = CharacterAction.Checking;
                }
                else if (KeyboardExtended.GetState().WasKeyPressed(Keys.D)) {
                    pos.X += 400;
                    _tweener.TweenTo(target: this, expression: player => player.CameraTarget, toValue: pos, duration: 0.5f)
                            .Easing(EasingFunctions.QuadraticOut);
                    _animatedSprite.Effect = SpriteEffects.None;
                    _animatedSprite.SetAnimation("check");
                    CurrentAction = CharacterAction.Checking;
                }
                else if (KeyboardExtended.GetState().IsKeyUp(Keys.A) && KeyboardExtended.GetState().IsKeyUp(Keys.D)) {
                    if (KeyboardExtended.GetState().WasKeyReleased(Keys.A) || KeyboardExtended.GetState().WasKeyReleased(Keys.D)) {
                        _tweener.TweenTo(target: this, expression: player => player.CameraTarget, toValue: Transform.Position, duration: 2.5f)
                            .Easing(EasingFunctions.QuadraticOut);
                    }
                    
                    _animatedSprite.SetAnimation("back");
                    if (_flashLight.Enabled) {
                        ToggleFlashLight();
                    }
                    CurrentAction = CharacterAction.Interacting;
                    _eyeSightLength = 0;
                }

                _tweener.Update(Time.DeltaTime);
            }
            else {
                CameraTarget = Transform.Position;
            }

            if (_animatedSprite.Effect == SpriteEffects.FlipHorizontally) {
                if (_flashLight.Rotation != MathHelper.ToRadians(180)) {
                    _flashlightOffset.X *= -1;
                    _flashLight.Rotation = MathHelper.ToRadians(180);
                }
                _eyeSight.Start.X = Transform.Position.X - _eyeLevel.X;
                _eyeSight.Start.Y = Transform.Position.Y + _eyeLevel.Y;
                _eyeSight.End.X = Transform.Position.X - _eyeLevel.X - _eyeSightLength;
                _eyeSight.End.Y = Transform.Position.Y + _eyeLevel.Y;
            }
            else if (_animatedSprite.Effect == SpriteEffects.None) {
                if (_flashLight.Rotation != 0) {
                    _flashlightOffset.X *= -1;
                    _flashLight.Rotation = 0;
                }
                _eyeSight.Start = Transform.Position + _eyeLevel;
                _eyeSight.End.X = Transform.Position.X + _eyeLevel.X + _eyeSightLength;
                _eyeSight.End.Y = Transform.Position.Y + _eyeLevel.Y;
            }

            _flashLight.Position = Transform.Position + _flashlightOffset;

            if (_flashLight.Enabled) {
                if (Battery <= 0) {
                    _flashLight.Enabled = false;
                }
                Battery -= 3.5f * Time.DeltaTime;
                _eyeSightLength = 400;
            }
            else {
                _eyeSightLength = 0;
            }

            if (SceneManager.GetActiveScene().Name == Scene.tilemap_sewer.ToString()) {
                Sanity -= _sewerSanityRedectionSpeed * Time.DeltaTime;
            }

            CheckSanityStage();
        }

        public override void Draw() {
            //BoundingBox2D bounds = BoundingBox2D.CreateFromPositionAndSize(Transform.Position, _size);
            //MainGame.SpriteBatch.FillRectangle(_origin, _size, Color.Red);
            if (Visible) {
                MainGame.SpriteBatch.Draw(_animatedSprite, Transform.Position, 0, Transform.Scale);
                //MainGame.SpriteBatch.DrawLine(_eyeSight.Start, _eyeSight.End,Color.Yellow,10);
                //MainGame.SpriteBatch.DrawPoint(new Vector2(Listener.Position.X, Listener.Position.Y), Color.Red, 100);
            }
        }

        private void SetUpAnimation() {
            //Set up animation
            Texture2DAtlas atlas = MainGame.Content.Load<Texture2DAtlas>("Character/spritesheet_player");
            SpriteSheet spriteSheet = new("player", atlas);
            int walkFrameCount = 9;
            int idleFrameCount = 12;

            spriteSheet.DefineAnimation("walk-forward", builder => {
                builder.IsLooping(true);
                for (int i = 1; i <= walkFrameCount; i++) {
                    if (i == walkFrameCount) builder.AddFrame($"sprite_walk_{i:D2}", TimeSpan.FromSeconds(0));
                    else builder.AddFrame($"sprite_walk_{i:D2}", TimeSpan.FromSeconds(0.15));
                    //builder.AddFrame($"sprite_walk_{i:D2}", TimeSpan.FromSeconds(0.15));
                }
            });

            spriteSheet.DefineAnimation("sneak-forward", builder => {
                builder.IsLooping(true);
                for (int i = 1; i <= walkFrameCount; i++) {
                    if (i == walkFrameCount) builder.AddFrame($"sprite_walk_{i:D2}", TimeSpan.FromSeconds(0));
                    else builder.AddFrame($"sprite_walk_{i:D2}", TimeSpan.FromSeconds(0.2));
                }
            });

            spriteSheet.DefineAnimation("sneak-backward", builder => {
                builder.IsLooping(true);
                for (int i = walkFrameCount; i > 0; i--) {
                    if (i == 1) builder.AddFrame($"sprite_walk_{i:D2}", TimeSpan.FromSeconds(0));
                    else builder.AddFrame($"sprite_walk_{i:D2}", TimeSpan.FromSeconds(0.2));
                }
            });

            spriteSheet.DefineAnimation("idle", builder => {
                builder.IsLooping(true);
                for (int i = 1; i <= idleFrameCount; i++) {
                    if (i == idleFrameCount) builder.AddFrame($"sprite_idle_{i:D2}", TimeSpan.FromSeconds(0));
                    else builder.AddFrame($"sprite_idle_{i:D2}", TimeSpan.FromSeconds(0.4));
                }
            });

            spriteSheet.DefineAnimation("back", builder => {
                builder.IsLooping(false)
                .AddFrame("sprite_back", TimeSpan.FromSeconds(0));
            });

            spriteSheet.DefineAnimation("check", builder => {
                builder.IsLooping(false)
                .AddFrame("sprite_idle_01", TimeSpan.FromSeconds(0));
            });

            _animatedSprite = new AnimatedSprite(spriteSheet, "idle");
        }

        private void Animate() {
            if (InputManager.Direction.X < 0 && !_lockFacingDirection) {
                _animatedSprite.Effect = SpriteEffects.FlipHorizontally;
                //CurrentAction = CharacterAction.Walking;
            }
            else if (InputManager.Direction.X > 0 && !_lockFacingDirection) {
                _animatedSprite.Effect = SpriteEffects.None;
                //CurrentAction = CharacterAction.Walking;
            }

            if (!_lockFacingDirection && InputManager.Direction != Vector2.Zero && _animatedSprite.CurrentAnimation != "walk-forward") {
                _animatedSprite.SetAnimation("walk-forward");
                CurrentAction = CharacterAction.Walking;
            }
            else if (_lockFacingDirection && InputManager.Direction != Vector2.Zero) {
                if(_animatedSprite.CurrentAnimation != "sneak-backward" && ((_animatedSprite.Effect == SpriteEffects.None && InputManager.Direction.X < 0) || (_animatedSprite.Effect == SpriteEffects.FlipHorizontally && InputManager.Direction.X > 0))) {
                    _animatedSprite.SetAnimation("sneak-backward");
                }
                else if (_animatedSprite.CurrentAnimation != "sneak-forward" && ((_animatedSprite.Effect == SpriteEffects.None && InputManager.Direction.X > 0) || (_animatedSprite.Effect == SpriteEffects.FlipHorizontally && InputManager.Direction.X < 0))) {
                    _animatedSprite.SetAnimation("sneak-forward");
                }
                CurrentAction = CharacterAction.Walking;

            }
            else if (InputManager.Direction == Vector2.Zero && _animatedSprite.CurrentAnimation != "idle") {
                _animatedSprite.SetAnimation("idle");
                CurrentAction = CharacterAction.Idle;
                _previousFrame = 0;
            }

            if ((_animatedSprite.CurrentAnimation == "walk-forward" || _animatedSprite.CurrentAnimation == "sneak-backward" || _animatedSprite.CurrentAnimation == "sneak-forward") && _animatedSprite.Controller.CurrentFrame != _previousFrame) {
                switch (_animatedSprite.Controller.CurrentFrame) {
                    case 15:
                    case 19:
                        AudioManager.Instance.PlayStepsSFX(Transform.WorldPosition,10,150);
                        _previousFrame = _animatedSprite.Controller.CurrentFrame;
                        break;
                }                
            }

            int offsetX = 0;
            int offsetY = 0;
            switch (_animatedSprite.Controller.CurrentFrame) {
                case 1:
                    offsetX = 80;
                    offsetY = -13;
                    break;
                case 13:
                    offsetX = 70;
                    offsetY = -11;
                    break;
                case 15:
                    offsetX = 75;
                    offsetY = -13;
                    break;
                case 17:
                    offsetX = 78;
                    offsetY = -15;
                    break;
            }
            if (offsetX != 0 && _animatedSprite.Effect == SpriteEffects.None) {
                _flashlightOffset.X = offsetX;
                _flashlightOffset.Y = offsetY;
            }
            else if (offsetX != 0 && _animatedSprite.Effect == SpriteEffects.FlipHorizontally) {
                _flashlightOffset.X = -offsetX;
                _flashlightOffset.Y = offsetY;
            }
            //if (!_animatedSprite.Controller.IsAnimating) {
            //    switch (_characterState) {
            //        case CharacterState.Walking:
            //            _characterState = CharacterState.Idle;
            //            _animatedSprite.SetAnimation("idle");
            //            break;
            //            // Handle other state transitions...
            //    }
            //}
        }

        public void CollideWithWallMove(Vector2 delta) {
            Transform.Position += delta;
            UpdateShape();
        }

        private void UpdateShape() {
            _origin.X = Transform.Position.X - (_size.X / 2f);
            _origin.Y = Transform.Position.Y - (_size.Y /2f);
            BoundingBox2D bounds = BoundingBox2D.CreateFromPositionAndSize(_origin, _size);
            Shape = new CollisionShape2D(bounds);
        }

        public void ToggleHide() {
            if (Visible && CurrentAction != CharacterAction.Hiding && Time.TimerExist("hideCooldown") == false) {
                SetActive(false);
                _allowMovement = false;
                FocusLevel = 1.25f;
                CurrentAction = CharacterAction.Hiding;
                Time.AddTimer(ToggleHide, HideTime, "hideTime");
                AudioManager.Instance.PlaySFX(SoundEffectName.LockerClose);
            }
            else if (!Visible && CurrentAction == CharacterAction.Hiding) {
                ObjectiveManager.Complete("hide");
                SetActive(true);
                _allowMovement = true;
                FocusLevel = 1;
                CurrentAction = CharacterAction.Idle;
                Transform.Position = PreviousPos;
                Time.RemoveTimer("hideTime");
                Time.AddTimer(HideCooldown, "hideCooldown");
                AudioManager.Instance.PlaySFX(SoundEffectName.LockerOpen);
            }

            Visible = Active;
            _scotopicLight.Enabled = Active;
        }

        public void ToggleFlashLight() {
            AudioManager.Instance.PlaySFX(SoundEffectName.FlashlightToggle, interrupt:true);
            if (_flashLight.Enabled) {
                _flashLight.Enabled = false;
            }
            else if (!_flashLight.Enabled && Battery > 0) {
                _flashLight.Enabled = true;
            }
        }

        public void ToggleInteracting() {
            if (CurrentAction != CharacterAction.Interacting && CurrentAction != CharacterAction.Checking) {
                _allowMovement = false;
                _animatedSprite.SetAnimation("back");
                CurrentAction = CharacterAction.Interacting;
                _flashLight.Enabled = false;
                FocusLevel = 1.25f;
            }
            else if (CurrentAction == CharacterAction.Interacting || CurrentAction == CharacterAction.Checking) {
                _allowMovement = true;
                _animatedSprite.SetAnimation("idle");
                CurrentAction = CharacterAction.Idle;
                FocusLevel = 1;
            }
        }

        public void TogglePeek() {
            if (CurrentAction != CharacterAction.Peeking) {
                _allowMovement = false;
                _animatedSprite.SetAnimation("idle");
                CurrentAction = CharacterAction.Peeking;
                _flashLight.Enabled = false;
                FocusLevel = 1.75f;
            }
            else if (CurrentAction == CharacterAction.Peeking) {
                _allowMovement = true;
                _animatedSprite.SetAnimation("idle");
                CurrentAction = CharacterAction.Idle;
                FocusLevel = 1;
            }
        }

        public void ToggleReading() {
            if (CurrentAction != CharacterAction.Reading) {
                _allowMovement = false;
                _animatedSprite.SetAnimation("back");
                CurrentAction = CharacterAction.Reading;
                _flashLight.Enabled = false;
                FocusLevel = 1.25f;
            }
            else if (CurrentAction == CharacterAction.Reading) {
                _allowMovement = true;
                _animatedSprite.SetAnimation("idle");
                CurrentAction = CharacterAction.Idle;
                FocusLevel = 1;
            }
        }

        public void CheckSanityStage() {
            if (Sanity <= ((float)SanityState.Anxious) && SanityState == SanityState.Anxious) {
                SanityState = SanityState.Insane;
                _sewerSanityRedectionSpeed = _availableSanityRedectionSpeed.Item3;
                SanityStateChanged?.Invoke();

                AudioManager.Instance.StopSFX(SoundEffectName.Painting);
                Time.RemoveTimer("anxietySound");

                AudioManager.Instance.PlaySFX(SoundEffectName.HeartBeat);
                Time.AddTimer(() => AudioManager.Instance.PlaySFX(SoundEffectName.Painting, volume: 0.065f), 3);

                Action anxietySound = () => {
                    AudioManager.Instance.PlaySFX(SoundEffectName.HeartBeat);
                    Time.AddTimer(() => AudioManager.Instance.PlaySFX(SoundEffectName.Painting, volume: 0.065f), 3);
                };

                Time.AddTimer(anxietySound, 10, 16, "anxietySound");
            }
            else if (Sanity <= ((float)SanityState.Normal) && SanityState == SanityState.Normal) {
                SanityState = SanityState.Anxious;
                _sewerSanityRedectionSpeed = _availableSanityRedectionSpeed.Item2;
                SanityStateChanged?.Invoke();

                AudioManager.Instance.PlaySFX(SoundEffectName.Painting, volume: 0.025f);
                Time.AddTimer(() => AudioManager.Instance.PlaySFX(SoundEffectName.Painting, volume: 0.025f), 20, 26, "anxietySound");
            }
            else if (Sanity > (float)SanityState.Normal && SanityState != SanityState.Normal) {
                SanityState = SanityState.Normal;
                _sewerSanityRedectionSpeed = _availableSanityRedectionSpeed.Item1;
                SanityStateChanged?.Invoke();
            }
        }

        public void Dead() {
            Sanity = 0;
        }

        public void LoadScenePosition(Vector2 pos) {
            Transform.Position = pos;
        }

        public void Reset() {
            Sanity = 100;
            Battery = 100;
            FocusLevel = 1;
            SetActive(true);
            _allowMovement = true;
            Visible = true;
            CurrentAction = CharacterAction.Idle;
        }

        public void Dispose() {
            Children.Clear();
        }
    }
}
