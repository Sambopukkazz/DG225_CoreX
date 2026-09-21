using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using System.Drawing;

namespace Imaginophobia {
    public class Scopophobia : GameObject, IMoveable, ICollisionActor{
        public Vector2 Direction { get; private set; }
        public Vector2 Velocity { get; private set; }
        public float MoveSpeed { get; private set; } = 300f;
        public int Id { get; }
        public CollisionShape2D Shape { get; private set; }
        private readonly Vector2 _size;

        private Texture2D texture;

        public Scopophobia(Vector2 pos, GameObject player, Texture2D texture) : base("Scopophobia", "Enemy") {
            this.texture = texture;
            BoundingBox2D bounds = BoundingBox2D.CreateFromPositionAndSize(pos, _size);
            Shape = new CollisionShape2D(bounds);

            Transform.Position = pos;
            if(Transform.Position.X - player.Transform.Position.X < 0) {
                Direction = Vector2.UnitX * -1f;
            }
            else {
                Direction = Vector2.UnitX;
            }
        }

        public override void Update() {
            Velocity = MoveSpeed * Direction;
            Transform.Position = Transform.Position.Translate(Velocity.X * Time.DeltaTime, 0);
        }

        public override void Draw() {
            
        }

        private void UpdateShape() {
            _origin.X = Transform.Position.X + (_size.X / 2f);
            _origin.Y = Transform.Position.Y + (_size.Y / 2f);
            BoundingBox2D bounds = BoundingBox2D.CreateFromPositionAndSize(_origin, _size);
            Shape = new CollisionShape2D(bounds);
        }
    }
}
