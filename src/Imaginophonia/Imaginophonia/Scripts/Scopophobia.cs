using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Imaginophobia {
    public class Scopophobia : GameObject, IMoveable{
        public Vector2 Direction { get; private set; }
        public Vector2 Velocity { get; private set; }
        public float MoveSpeed { get; private set; } = 300f;

        public Scopophobia(Vector2 pos, GameObject player) : base("Scopophobia", "Enemy") {
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
    }
}
