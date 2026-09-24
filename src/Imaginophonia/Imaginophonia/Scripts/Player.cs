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
        private enum CharacterState { Normal, Anxious, Insane }
        private CharacterState _characterState;
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
        private Timer _timer;


        public Player() : base("Player", "Player"){
            //Set up animation
            Texture2DAtlas atlas = MainGame.Content.Load<Texture2DAtlas>("Character/spitesheet_player");
            SpriteSheet spriteSheet = new("player", atlas);

            spriteSheet.DefineAnimation("walk-forward", builder => {
                builder.IsLooping(true);
                for(int i = 1; i < 8; i++) {
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

            _animatedSprite = new AnimatedSprite(spriteSheet,"idle");

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
                }
                if (KeyboardExtended.GetState().WasKeyPressed(Keys.F)) {
                    ToggleFlashLight();
                }

                Transform.Position = Transform.Position.Translate(Velocity.X * Time.DeltaTime, 0);
                //Transform.Position += new Vector2(Velocity.X * Time.DeltaTime, Velocity.Y * Time.DeltaTime);

                _scotopicLight.Position = Transform.Position;
                _flashLight.Position = Transform.Position;
                UpdateDebug();

                Listener.Position = new Vector3(Transform.Position, 0);
                Animate();
                _animatedSprite.Update(Time.ElapsedTime);

                UpdateShape();
            }

            _eyeSight.Start = Transform.Position + _eyeLevel;
            float direction;
            if (_animatedSprite.Effect == SpriteEffects.FlipHorizontally) {
                direction = -1;
                _flashLight.Rotation = MathHelper.ToRadians(180);
            }
            else {
                direction = 1;
                _flashLight.Rotation = 0;
            }
            _eyeSight.End = new Vector2(Transform.Position.X + _eyeLevel.X + _eyeSightLength * direction, Transform.Position.Y + _eyeLevel.Y);

            if (_flashLight.Enabled) {
                Battery -= 0.5f * Time.DeltaTime;
                _eyeSightLength = 800;
            }
            else {
                _eyeSightLength = 100;
            }
        }

        public override void Draw() {
            //BoundingBox2D bounds = BoundingBox2D.CreateFromPositionAndSize(Transform.Position, _size);
            //MainGame.SpriteBatch.FillRectangle(_origin,_size, Color.Red);
            if (Visible) {
                MainGame.SpriteBatch.Draw(_animatedSprite, Transform.Position, 0, Transform.Scale);
                MainGame.SpriteBatch.DrawLine(_eyeSight.Start, _eyeSight.End,Color.Yellow,10);
            }
            else {
                MainGame.SpriteBatch.FillRectangle(Transform.Position.X + 50, Transform.Position.Y, 15, 120, Color.DarkGray);
                MainGame.SpriteBatch.FillRectangle(Transform.Position.X + 50, Transform.Position.Y, 15, _timer.TimeLeft * 40, Color.LightBlue);
            }
            
            DebugTest();
        }

        private void Animate() {
            if (InputManager.Direction.X < 0 && !_lockFacingDirection) _animatedSprite.Effect = SpriteEffects.FlipHorizontally;
            else if (InputManager.Direction.X > 0 && !_lockFacingDirection) _animatedSprite.Effect = SpriteEffects.None;

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
                _previousFrame = 0;
            }

            if ((_animatedSprite.CurrentAnimation == "walk-forward" || _animatedSprite.CurrentAnimation == "sneak-backward" || _animatedSprite.CurrentAnimation == "sneak-forward") && _animatedSprite.Controller.CurrentFrame != _previousFrame) {
                switch (_animatedSprite.Controller.CurrentFrame) {
                    case 3:
                    case 5:
                    case 7:
                        AudioManager.Instance.PlayStepsSFX(Transform.WorldPosition);
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
                _timer = Time.AddTimer(ToggleHide, 3);
            }
            else if (!Active) {
                SetActive(true);
                AllowMovement = true;
                Time.AddTimer(SetReadyToHide, _hideCoolDown);
            }
            Visible = Active;
            _scotopicLight.Enabled = Active;
        }

        public void SetReadyToHide() {
            _readyToHide = true;
        }

        public void ToggleFlashLight() {
            if (_flashLight.Enabled) {
                _flashLight.Enabled = false;
            }
            else {
                _flashLight.Enabled = true;
            }
        }

        public void ToggleRepair() {
            //unfiniseh
            if (_readyToRepair) {
                AllowMovement = false;
                _readyToRepair = false;
                _animatedSprite.SetAnimation("back");
                _flashLight.Enabled = false;
            }
            else if (!_readyToRepair) {
                AllowMovement = true;
                _readyToRepair = true;
                _animatedSprite.SetAnimation("idle");
            }
        }

        public void LoadScenePosition(Vector2 pos) {
            Transform.Position = pos;
        }

        private void UpdateDebug() {
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
                _scotopicLight.Scale -= new Vector2(0, 10);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) {
                _scotopicLight.Scale += new Vector2(0, 10);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
                _scotopicLight.Scale += new Vector2(10, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
                _scotopicLight.Scale -= new Vector2(10, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad6)) {
                _scotopicLight.Intensity += 0.01f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad4)) {
                _scotopicLight.Intensity -= 0.01f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad1)) {
                _scotopicLight.Enabled = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad3)) {
                _scotopicLight.Enabled = false;
            }
            //if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
            //    _light.Intensity += 0.01f;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.Down)) {
            //    _light.Intensity -= 0.01f;
            //}
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad5)) {
                Visible = true;
            }
            if (KeyboardExtended.GetState().WasKeyPressed(Keys.NumPad7)) {
                Time.AddTimer(5);
            }
        }

        private void DebugTest() {
            
            BitmapFont _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            //MainGame.SpriteBatch.DrawString(_font, $"PlayerPos {Transform.Position} ", new Vector2(150, 150), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Light scale: {_scotopicLight.Scale.X}.{_scotopicLight.Scale.Y}\nLight intensity: {_scotopicLight.Intensity}", new Vector2(150, 50), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Frame {_animatedSprite.Controller.CurrentFrame}", new Vector2(150, 150), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Animation {_animatedSprite.CurrentAnimation}", new Vector2(150, 200), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Listener {Listener.Position.X}", new Vector2(150, 300), Color.White);

            //foreach (Timer timer in Time.Timers) {
            //    MainGame.SpriteBatch.DrawString(_font, $"\nTimer:{timer.TimeLeft}", new Vector2(100, 500 + (40 * Time.Timers.IndexOf(timer))), Color.White);
            //}
        }
    }
}
