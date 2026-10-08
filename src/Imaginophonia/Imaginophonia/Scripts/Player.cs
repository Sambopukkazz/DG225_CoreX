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
    public class Player : GameObject, IAudioApplicable, ICollisionActor{
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
        private Spotlight _flashLight;
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
        public bool AllowMovement { get; set; }
        private bool _readyToHide;
        private float _hideCoolDown = 3f;
        public bool ReadyToOpenDoor { get; set; }
        private bool _readyToRepair;
        public bool CanRepair => _readyToRepair;
        public bool CanInteract { get; set; }
        public float FocusLevel { get; private set; }
        public Vector2 CameraTarget;
        public Timer CooldownTimer;
        public bool Blink { get; set; }
        public event Action StateChanged;
        public Vector2 PreviousPos { get; set; }


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
                Scale = new Vector2(1000, 1750),
                Intensity = 1,
                Enabled = false
            };
            _flashlightOffset = new Vector2(-10,10);

            LightManager.Penumbra.Lights.Add(_scotopicLight);
            LightManager.Penumbra.Lights.Add(_flashLight);

            //Listener
            Listener = new AudioListener();
            _size = new Vector2(100, 240);
            Transform.Scale = Vector2.One * 4;

            //Eyesight
            _eyeLevel = new Vector2(20,-70);
            _eyeSight = new LineSegment2D();
            _eyeSightLength = 100;

            _readyToHide = true;
            _readyToRepair = true;
            ReadyToOpenDoor = true;
            AllowMovement = true;

            UpdateShape();
        }

        public override void Update() {
            if (AllowMovement) {
                Direction = InputManager.Direction;
                Velocity = MoveSpeed * InputManager.Direction;
                _lockFacingDirection = false;

                if (KeyboardExtended.GetState().IsKeyDown(Keys.LeftControl)) {
                    Velocity /= 1.7f;
                    _lockFacingDirection = true;
                    CurrentAction = CharacterAction.Sneaking;
                }
                

                Transform.Position = Transform.Position.Translate(Velocity.X * Time.DeltaTime, 0);
                Listener.Position = new Vector3(Transform.Position, 0);
                //Transform.Position += new Vector2(Velocity.X * Time.DeltaTime, Velocity.Y * Time.DeltaTime);

                _scotopicLight.Position = Transform.Position;
                _flashLight.Position = Transform.Position + _flashlightOffset;

                UpdateDebug();
                
                Animate();
                _animatedSprite.Update(Time.ElapsedTime);

                UpdateShape();
            }

            if (KeyboardExtended.GetState().WasKeyPressed(Keys.F) && _animatedSprite.CurrentAnimation != "back" && CurrentAction != CharacterAction.Hiding) {
                ToggleFlashLight();
            }

            if (_readyToRepair == false) {
                if (KeyboardExtended.GetState().IsKeyDown(Keys.A)) {
                    CameraTarget.X = MathHelper.Lerp(CameraTarget.X, Transform.Position.X - 400, 0.1f);
                    _animatedSprite.Effect = SpriteEffects.FlipHorizontally;
                    _animatedSprite.SetAnimation("check");
                    CurrentAction = CharacterAction.Checking;
                }
                else if (KeyboardExtended.GetState().IsKeyDown(Keys.D)) {
                    CameraTarget.X = MathHelper.Lerp(CameraTarget.X, Transform.Position.X + 400, 0.1f);
                    _animatedSprite.Effect = SpriteEffects.None;
                    _animatedSprite.SetAnimation("check");
                    CurrentAction = CharacterAction.Checking;
                }
                else {
                    _animatedSprite.SetAnimation("back");
                    if (_flashLight.Enabled) {
                        _flashLight.Enabled = false;
                    }
                    CurrentAction = CharacterAction.Repairing;
                    CameraTarget.X = MathHelper.Lerp(CameraTarget.X, Transform.Position.X, 0.01f);
                    _eyeSightLength = 0;
                }
            }
            else {
                CameraTarget = Transform.Position;
            }


            if (_animatedSprite.Effect == SpriteEffects.FlipHorizontally) {
                _flashLight.Rotation = MathHelper.ToRadians(180);
                _eyeSight.Start.X = Transform.Position.X - _eyeLevel.X;
                _eyeSight.Start.Y = Transform.Position.Y + _eyeLevel.Y;
                _eyeSight.End.X = Transform.Position.X - _eyeLevel.X - _eyeSightLength;
                _eyeSight.End.Y = Transform.Position.Y + _eyeLevel.Y;
            }
            else if (_animatedSprite.Effect == SpriteEffects.None) {
                _flashLight.Rotation = 0;
                _eyeSight.Start = Transform.Position + _eyeLevel;
                _eyeSight.End.X = Transform.Position.X + _eyeLevel.X + _eyeSightLength;
                _eyeSight.End.Y = Transform.Position.Y + _eyeLevel.Y;
            }

            if (_flashLight.Enabled) {
                Battery -= 3f * Time.DeltaTime;
                _eyeSightLength = 800;
            }
            else {
                _eyeSightLength = 100;
            }

            CheckSanityStage();
        }

        public override void Draw() {
            //BoundingBox2D bounds = BoundingBox2D.CreateFromPositionAndSize(Transform.Position, _size);
            //MainGame.SpriteBatch.FillRectangle(_origin,_size, Color.Red);
            if (Visible) {
                MainGame.SpriteBatch.Draw(_animatedSprite, Transform.Position, 0, Transform.Scale);
                MainGame.SpriteBatch.DrawLine(_eyeSight.Start, _eyeSight.End,Color.Yellow,10);
                //MainGame.SpriteBatch.DrawPoint(new Vector2(Listener.Position.X, Listener.Position.Y), Color.Red, 100);
            }
            
            DebugTest();
        }

        private void SetUpAnimation() {
            //Set up animation
            Texture2DAtlas atlas = MainGame.Content.Load<Texture2DAtlas>("Character/spitesheet_player");
            SpriteSheet spriteSheet = new("player", atlas);


            spriteSheet.DefineAnimation("walk-forward", builder => {
                builder.IsLooping(true);
                for (int i = 1; i < 8; i++) {
                    if (i == 7) builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0));
                    else builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0.3));
                }
            });

            spriteSheet.DefineAnimation("sneak-forward", builder => {
                builder.IsLooping(true);
                for (int i = 1; i < 8; i++) {
                    if (i == 7) builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0));
                    else builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0.5));
                }
            });

            spriteSheet.DefineAnimation("sneak-backward", builder => {
                builder.IsLooping(true);
                for (int i = 7; i > 0; i--) {
                    if (i == 1) builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0));
                    else builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0.5));
                }
            });

            spriteSheet.DefineAnimation("idle", builder => {
                builder.IsLooping(false)
                .AddFrame("sprite_idle", TimeSpan.FromSeconds(0));
            });

            spriteSheet.DefineAnimation("back", builder => {
                builder.IsLooping(false)
                .AddFrame("sprite_back", TimeSpan.FromSeconds(0));
            });

            spriteSheet.DefineAnimation("check", builder => {
                builder.IsLooping(false)
                .AddFrame("sprite_idle", TimeSpan.FromSeconds(0));
            });

            _animatedSprite = new AnimatedSprite(spriteSheet, "idle");
        }

        //private void SetUpAnimation() {
        //    //Set up animation
        //    Texture2DAtlas atlas = MainGame.Content.Load<Texture2DAtlas>("Character/spitesheet_player");
        //    SpriteSheet spriteSheet = new("player", atlas);
        //    int walkFrameCount = 13;
        //    int idleFrameCount = 12;

        //    spriteSheet.DefineAnimation("walk-forward", builder => {
        //        builder.IsLooping(true);
        //        for (int i = 1; i <= walkFrameCount; i++) {
        //            if (i == walkFrameCount) builder.AddFrame($"sprite_walk_{i:D2}", TimeSpan.FromSeconds(0));
        //            else builder.AddFrame($"sprite_walk_{i:D2}", TimeSpan.FromSeconds(0.1));
        //        }
        //    });

        //    spriteSheet.DefineAnimation("sneak-forward", builder => {
        //        builder.IsLooping(true);
        //        for (int i = 1; i <= 5; i++) {
        //            if (i == walkFrameCount) builder.AddFrame($"sprite_walk_{i:D2}", TimeSpan.FromSeconds(0));
        //            else builder.AddFrame($"sprite_walk_{i:D2}", TimeSpan.FromSeconds(0.15));
        //        }
        //    });

        //    spriteSheet.DefineAnimation("sneak-backward", builder => {
        //        builder.IsLooping(true);
        //        for (int i = 5; i > 0; i--) {
        //            if (i == 1) builder.AddFrame($"sprite_walk_{i:D2}", TimeSpan.FromSeconds(0));
        //            else builder.AddFrame($"sprite_walk_{i:D2}", TimeSpan.FromSeconds(0.15));
        //        }
        //    });

        //    spriteSheet.DefineAnimation("idle", builder => {
        //        builder.IsLooping(true);
        //        for (int i = 1; i <= idleFrameCount; i++) {
        //            if (i == idleFrameCount) builder.AddFrame($"sprite_idle_{i:D2}", TimeSpan.FromSeconds(0));
        //            else builder.AddFrame($"sprite_idle_{i:D2}", TimeSpan.FromSeconds(0.4));
        //        }
        //    });

        //    //spriteSheet.DefineAnimation("back", builder => {
        //    //    builder.IsLooping(false)
        //    //    .AddFrame("sprite_back", TimeSpan.FromSeconds(0));
        //    //});

        //    _animatedSprite = new AnimatedSprite(spriteSheet, "idle");
        //}

        private void Animate() {
            if (InputManager.Direction.X < 0 && !_lockFacingDirection) {
                _animatedSprite.Effect = SpriteEffects.FlipHorizontally;
                CurrentAction = CharacterAction.Walking;
            }
            else if (InputManager.Direction.X > 0 && !_lockFacingDirection) {
                _animatedSprite.Effect = SpriteEffects.None;
                CurrentAction = CharacterAction.Walking;
            }

            if (!_lockFacingDirection && InputManager.Direction != Vector2.Zero && _animatedSprite.CurrentAnimation != "walk-forward") {
                _animatedSprite.SetAnimation("walk-forward");
            }
            else if (_lockFacingDirection && InputManager.Direction != Vector2.Zero) {
                if(_animatedSprite.CurrentAnimation != "sneak-backward" && ((_animatedSprite.Effect == SpriteEffects.None && InputManager.Direction.X < 0) || (_animatedSprite.Effect == SpriteEffects.FlipHorizontally && InputManager.Direction.X > 0))) {
                    _animatedSprite.SetAnimation("sneak-backward");
                }
                else if (_animatedSprite.CurrentAnimation != "sneak-forward" && ((_animatedSprite.Effect == SpriteEffects.None && InputManager.Direction.X > 0) || (_animatedSprite.Effect == SpriteEffects.FlipHorizontally && InputManager.Direction.X < 0))) {
                    _animatedSprite.SetAnimation("sneak-forward");
                }
                
            }
            else if (InputManager.Direction == Vector2.Zero && _animatedSprite.CurrentAnimation != "idle") {
                _animatedSprite.SetAnimation("idle");
                CurrentAction = CharacterAction.Idle;
                _previousFrame = 0;
            }

            if ((_animatedSprite.CurrentAnimation == "walk-forward" || _animatedSprite.CurrentAnimation == "sneak-backward" || _animatedSprite.CurrentAnimation == "sneak-forward") && _animatedSprite.Controller.CurrentFrame != _previousFrame) {
                switch (_animatedSprite.Controller.CurrentFrame) {
                    case 3:
                    case 5:
                    case 7:
                        AudioManager.Instance.PlayStepsSFX(Transform.WorldPosition,10,150);
                        _previousFrame = _animatedSprite.Controller.CurrentFrame;
                        break;
                }
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
            if (Visible && _readyToHide) {
                SetActive(false);
                AllowMovement = false;
                _readyToHide = false;
                FocusLevel = 1.25f;
                CurrentAction = CharacterAction.Hiding;
                CooldownTimer = Time.AddTimer(ToggleHide, 3, "hideTime");
                AudioManager.Instance.PlaySFX(Sound.CloseLocker);
            }
            else if (CurrentAction == CharacterAction.Hiding) {
                SetActive(true);
                AllowMovement = true;
                FocusLevel = 1;
                CurrentAction = CharacterAction.Idle;
                Transform.Position = PreviousPos;
                CooldownTimer = Time.AddTimer(SetReadyToHide, _hideCoolDown, "cooldownTime");
                AudioManager.Instance.PlaySFX(Sound.OpenLocker);
            }
            Visible = Active;
            _scotopicLight.Enabled = Active;
        }

        public void SetReadyToHide() {
            _readyToHide = true;
            CooldownTimer = null;
        }

        public void ToggleFlashLight() {
            if (_flashLight.Enabled) {
                _flashLight.Enabled = false;
            }
            else if (!_flashLight.Enabled) {
                _flashLight.Enabled = true;
            }
        }

        public void ToggleRepair() {
            if (_readyToRepair) {
                AllowMovement = false;
                _readyToRepair = false;
                _animatedSprite.SetAnimation("back");
                CurrentAction = CharacterAction.Repairing;
                _flashLight.Enabled = false;
                CanInteract = false;
                FocusLevel = 1.25f;
            }
            else if (!_readyToRepair) {
                AllowMovement = true;
                _readyToRepair = true;
                _animatedSprite.SetAnimation("idle");
                CurrentAction = CharacterAction.Idle;
                CanInteract = true;
                FocusLevel = 1;
            }
        }

        public void CheckSanityStage() {
            if (Sanity <= ((float)SanityState.Anxious) && SanityState == SanityState.Anxious) {
                SanityState = SanityState.Insane;
                StateChanged?.Invoke();
            }
            else if (Sanity <= ((float)SanityState.Normal) && SanityState == SanityState.Normal) {
                SanityState = SanityState.Anxious;
                StateChanged?.Invoke();
            }
            else if (Sanity > (float)SanityState.Normal && SanityState != SanityState.Normal) {
                SanityState = SanityState.Normal;
                StateChanged?.Invoke();
            }
        }

        public void LoadScenePosition(Vector2 pos) {
            Transform.Position = pos;
        }

        private void UpdateDebug() {
            //if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
            //    _flashLight.Scale += new Vector2(0, 10);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.Down)) {
            //    _flashLight.Scale -= new Vector2(0, 10);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
            //    _flashLight.Scale += new Vector2(10, 0);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
            //    _flashLight.Scale -= new Vector2(10, 0);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad6)) {
            //    _scotopicLight.Intensity += 0.01f;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad4)) {
            //    _scotopicLight.Intensity -= 0.01f;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad1)) {
            //    _scotopicLight.Enabled = true;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad3)) {
            //    _scotopicLight.Enabled = false;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
            //    _light.Intensity += 0.01f;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.Down)) {
            //    _light.Intensity -= 0.01f;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad5)) {
            //    Visible = true;
            //}
            //if (KeyboardExtended.GetState().WasKeyPressed(Keys.NumPad7)) {
            //    Time.AddTimer(5);
            //}
        }

        private void DebugTest() {
            //BitmapFont _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            //MainGame.SpriteBatch.DrawString(_font, $"PlayerState {CurrentAction} ", new Vector2(150, 400), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Light scale: {_scotopicLight.Scale.X}.{_scotopicLight.Scale.Y}\nLight intensity: {_scotopicLight.Intensity}", new Vector2(150, 50), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Frame {_animatedSprite.Controller.CurrentFrame}", new Vector2(150, 150), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Animation {_animatedSprite.CurrentAnimation}", new Vector2(150, 200), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Listener {Listener.Position.X}", new Vector2(150, 520), Color.White);

            //foreach (Timer timer in Time.Timers) {
            //    MainGame.SpriteBatch.DrawString(_font, $"\nTimer:{timer.TimeLeft}", new Vector2(100, 500 + (40 * Time.Timers.IndexOf(timer))), Color.White);
            //}
        }
    }
}
