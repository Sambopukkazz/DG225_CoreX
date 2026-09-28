using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

namespace Imaginophobia {
    public class Scopophobia : GameObject, IAudioApplicable, ICollisionActor{
        public Vector2 Direction { get; private set; }
        public Vector2 Velocity { get; private set; }
        public float MoveSpeed { get; private set; } = 2600f;
        public int Id { get; }
        public CollisionShape2D Shape { get; private set; }
        private readonly Vector2 _size;
        private bool _startMovement;

        private Texture2D _texture;

        public Scopophobia(Vector2 pos, GameObject player, Texture2D texture) : base("Scopophobia", "Enemy") {
            this._texture = texture;
            BoundingBox2D bounds = BoundingBox2D.CreateFromPositionAndSize(pos, _size);
            Shape = new CollisionShape2D(bounds);

            Transform.Position = pos;
            if(Transform.Position.X - player.Transform.Position.X > 0) {
                Direction = Vector2.UnitX * -1f;
            }
            else {
                Direction = Vector2.UnitX;
            }

            Transform.Position = new Vector2(pos.X, pos.Y -220);
            Transform.Scale = Vector2.One * 4;
            _size = new Vector2(480, 110) * Transform.Scale;

            AudioManager.Instance.Play3DSFX(_origin,"scopophobia");
            UpdateShape();
            

            Time.AddTimer(StartMovement, 8);
            Time.AddTimer(SetActive, 15f);
        }

        public override void Update() {
            if (_startMovement) {
                Velocity = MoveSpeed * Direction;
                Transform.Position = Transform.Position.Translate(Velocity.X * Time.DeltaTime, 0);

                UpdateShape();
            }
        }

        public override void Draw() {
            //MainGame.SpriteBatch.FillRectangle(Transform.Position, _size, Color.Red);
            SpriteEffects effect;
            if (Direction.X < 0) {
                effect = SpriteEffects.FlipHorizontally;
            }
            else {
                effect = SpriteEffects.None;
            }
            MainGame.SpriteBatch.Draw(_texture, Transform.Position, null, Color.White, 0, Vector2.Zero, Transform.Scale, effect, 0);
        }

        private void UpdateShape() {
            _origin.X = Transform.Position.X + (_size.X / 2f);
            _origin.Y = Transform.Position.Y + (_size.Y / 2f);
            BoundingBox2D bounds = BoundingBox2D.CreateFromPositionAndSize(Transform.Position, _size);
            Shape = new CollisionShape2D(bounds);
        }

        private void StartMovement() {
            _startMovement = true;
        }
    }
}
