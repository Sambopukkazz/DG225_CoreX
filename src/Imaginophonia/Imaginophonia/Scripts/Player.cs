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
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Imaginophobia {
    public class Player : GameObject, IMoveable, ICollisionActor{
        public int Id { get; }
        public CollisionShape2D Shape { get; private set; }
        private readonly Vector2 _size;
        private AnimatedSprite _animatedSprite;
        public AudioListener Listener { get; private set; }
        private PointLight _light;
        public Vector2 Direction { get; private set; }
        public Vector2 Velocity { get; private set; }
        public float MoveSpeed { get; private set; } = 100f;
        private enum CharacterState { Idle, Walking }
        private CharacterState _characterState;
        private int _previousFrame;
        public SpriteEffects Effect { get; set; }

        

        
        public Player() : base("Player", "Player"){
            //Set up animation
            Texture2DAtlas atlas = MainGame.Content.Load<Texture2DAtlas>("Character/spitesheet_player");
            SpriteSheet spriteSheet = new("player", atlas);

            spriteSheet.DefineAnimation("walk", builder => {
                builder.IsLooping(true);
                for(int i = 1; i < 8; i++) {
                    if(i == 7) builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0));
                    else builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0.3));
                }
            });

            spriteSheet.DefineAnimation("idle", builder => {
                builder.IsLooping(false)
                .AddFrame("sprite_idle", TimeSpan.FromSeconds(0));
            });

            _animatedSprite = new AnimatedSprite(spriteSheet,"idle");

            _size = new Vector2(60,120);

            //SetUpLight
            _light = new PointLight();
            _light.Color = Color.FromHSV(150f,0.5f,0.5f);
            LightManager.Penumbra.Lights.Add(_light);

            //Listener
            Listener = new AudioListener();

            Transform.Position = new Vector2(0,800);

            UpdateShape();
        }

        public override void Update() {
            //else _characterState = CharacterState.Idle;
            Direction = InputManager.Direction;
            Velocity = MoveSpeed * InputManager.Direction;
            Transform.Position = Transform.Position.Translate(Velocity.X * Time.DeltaTime, 0);
            //Transform.Position += new Vector2(Velocity.X * Time.DeltaTime, Velocity.Y * Time.DeltaTime);

            _light.Position = Transform.Position;
            Testing();

            Listener.Position = new Vector3(Transform.Position, 0);
            Animate();
            _animatedSprite.Update(Time.ElapsedTime);

            if (_animatedSprite.CurrentAnimation == "walk" && _animatedSprite.Controller.CurrentFrame != _previousFrame) {
                switch (_animatedSprite.Controller.CurrentFrame) {
                    case 2:
                    case 4:
                    case 6:
                        AudioManager.Instance.PlayStepsSFX(Transform.Position);
                        _previousFrame = _animatedSprite.Controller.CurrentFrame;
                        break;

                }
            }

            UpdateShape();
            //base.Update();
        }

        public override void Draw() {
            if (Visible) {
                MainGame.SpriteBatch.Draw(_animatedSprite, Transform.Position,0,Transform.Scale * 4);
            }
            DebugTest();

            base.Draw();
        }

        private void Animate() {
            if (InputManager.Direction.X < 0) _animatedSprite.Effect = SpriteEffects.FlipHorizontally;
            else if (InputManager.Direction.X > 0) _animatedSprite.Effect = SpriteEffects.None;

            if (InputManager.Direction != Vector2.Zero && _characterState != CharacterState.Walking) {
                _characterState = CharacterState.Walking;
                _animatedSprite.SetAnimation("walk");
            }
            else if (InputManager.Direction == Vector2.Zero && _characterState != CharacterState.Idle) {
                _characterState = CharacterState.Idle;
                _animatedSprite.SetAnimation("idle");
                _previousFrame = 0;
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

        public void Move(Vector2 delta) {
            Transform.Position += delta;
            UpdateShape();
        }

        private void UpdateShape() {
            BoundingBox2D bounds = BoundingBox2D.CreateFromPositionAndSize(Transform.Position, _size);
            Shape = new CollisionShape2D(bounds);
        }

        private void Testing() {
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
                _light.Scale += new Vector2(10, 10);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
                _light.Scale -= new Vector2(10, 10);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad6)) {
                _light.Radius += 100;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad4)) {
                _light.Radius -= 100;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad1)) {
                _light.Enabled = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad3)) {
                _light.Enabled = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
                _light.Intensity += 0.01f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) {
                _light.Intensity -= 0.01f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad5)) {
                Visible = true;
            }
            if (KeyboardExtended.GetState().WasKeyPressed(Keys.NumPad7)) {
                Time.AddTimer(5);
            }
        }

        private void DebugTest() {
            

            BitmapFont _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            MainGame.SpriteBatch.DrawString(_font, $"Light radius: {_light.Radius}\nLight scale: {_light.Scale.X}.{_light.Scale.Y}\nLight intensity: {_light.Intensity}", new Vector2(150, 100), Color.White);
            MainGame.SpriteBatch.DrawString(_font, $"Frame {_animatedSprite.Controller.CurrentFrame}", new Vector2(150, 200), Color.White);
            MainGame.SpriteBatch.DrawString(_font, $"Listener {Listener.Position.X}", new Vector2(150, 300), Color.White);

            foreach (Timer timer in Time.Timers) {
                MainGame.SpriteBatch.DrawString(_font, $"\nTimer:{timer.TimeLeft}", new Vector2(100, 500 + (40 * Time.Timers.IndexOf(timer))), Color.White);
            }
        }
    }
}
