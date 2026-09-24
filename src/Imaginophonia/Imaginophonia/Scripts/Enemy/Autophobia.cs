using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class Autophobia : GameObject, IAudioApplicable, ICollisionActor {
        public Vector2 Direction { get; private set; }
        public Vector2 Velocity { get; private set; }
        public float MoveSpeed { get; private set; } = 300f;
        public int Id { get; }
        public CollisionShape2D Shape { get; private set; }
        private readonly Vector2 _size;

        private AnimatedSprite _animatedSprite;
        private int _previousFrame;

        public Autophobia(Vector2 pos, GameObject player, SpriteSheet spriteSheet) : base("Autophobia", "Enemy") {
            _animatedSprite = new AnimatedSprite(spriteSheet,"walk-forward");
            _animatedSprite.Color = Color.Black;
            Transform.Position = pos;

            if (Transform.Position.X - player.Transform.Position.X > 0) {
                Direction = Vector2.UnitX * -1f;
            }
            else {
                Direction = Vector2.UnitX;
            }

            if (Direction.X < 0) {
                _animatedSprite.Effect = SpriteEffects.FlipHorizontally;
            }
            else {
                _animatedSprite.Effect = SpriteEffects.None;
            }

            _size = new Vector2(100, 240);
        }

        public override void Update() {
            Velocity = MoveSpeed * Direction;
            Transform.Position = Transform.Position.Translate(Velocity.X * Time.DeltaTime, 0);

            _animatedSprite.Update(Time.ElapsedTime);
            if(_animatedSprite.Controller.CurrentFrame != _previousFrame) {
                switch (_animatedSprite.Controller.CurrentFrame) {
                    case 3:
                    case 5:
                    case 7:
                        AudioManager.Instance.PlayStepsSFX(Transform.WorldPosition, 10, 1200);
                        _previousFrame = _animatedSprite.Controller.CurrentFrame;
                        break;
                }
            }
            UpdateShape();
        }

        public override void Draw() {
            MainGame.SpriteBatch.Draw(_animatedSprite, Transform.Position, 0, Transform.Scale * 4);
        }

        private void UpdateShape() {
            _origin.X = Transform.Position.X - (_size.X / 2f);
            _origin.Y = Transform.Position.Y - (_size.Y / 2f);
            BoundingBox2D bounds = BoundingBox2D.CreateFromPositionAndSize(_origin, _size);
            Shape = new CollisionShape2D(bounds);
        }
    }
}
