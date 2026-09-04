using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Animations;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.ECS;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Particles;
using Penumbra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Imaginophonia {
    public class Player : GameObject, IMoveable{

        private SpriteSheet _spriteSheet;
        private AnimatedSprite _animatedSprite;
        public static AudioListener Listener { get; }
        private BoundingCapsule2D _bounds;
        private Spotlight _light;
        public Vector2 Direction { get; private set; }
        public Vector2 Velocity { get; private set; }
        public float MoveSpeed { get; private set; } = 100f;

        public float Alpha { get; set; }

        private enum CharacterState {
            Idle,
            Walking
        }
        private CharacterState _characterState;

        public SpriteEffects Effect { get; set; }

        
        public Player() : base("Player", "Player"){
            //Set up animation
            Texture2DAtlas atlas = MainGame.Content.Load<Texture2DAtlas>("Character/spitesheet_player");
            _spriteSheet = new("player", atlas);

            _spriteSheet.DefineAnimation("walk", builder => {
                builder.IsLooping(true);
                for(int i = 1; i < 8; i++) {
                    builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0.3));
                }
            });

            _spriteSheet.DefineAnimation("idle", builder => {
                builder.IsLooping(false)
                .AddFrame("sprite_idle", TimeSpan.FromSeconds(0));
            });

            _animatedSprite = new AnimatedSprite(_spriteSheet,"idle");

            _light = new Spotlight();
            LightManager.Penumbra.Lights.Add(_light);
            
        }

        public override void Update() {
            //else _characterState = CharacterState.Idle;
            Direction = InputManager.Direction;
            Velocity = MoveSpeed * InputManager.Direction;
            Transform.Position = Transform.Position.Translate(Velocity.X * Time.DeltaTime, 0);
            //Transform.Position += new Vector2(Velocity.X * Time.DeltaTime, Velocity.Y * Time.DeltaTime);

            _light.Position = Transform.Position;
            Testing();

            Animate();
            _animatedSprite.Update(Time.ElapsedTime);
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
            }

            if (_animatedSprite.CurrentAnimation == "walk") {
                switch (_animatedSprite.Controller.CurrentFrame) {
                    case 0:
                        //Play Steps
                        break;
                    case 1:
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
                _light.ConeDecay -= 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad3)) {
                _light.ConeDecay += 1;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
                _light.Intensity += 0.01f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) {
                _light.Intensity -= 0.01f;
            }
        }

        private void DebugTest() {
            BitmapFont _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            MainGame.SpriteBatch.DrawString(_font, $"Light radius: {_light.Radius}\nLight scale: {_light.Scale.X}.{_light.Scale.Y}\nLight intensity: {_light.Intensity}", new Vector2(100, 100), Color.White);
        }
    }
}
