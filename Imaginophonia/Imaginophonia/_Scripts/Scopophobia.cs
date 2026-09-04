using Microsoft.Xna.Framework;

namespace Imaginophonia {
    public class Scopophobia : GameObject, IMoveable{
        public Vector2 Direction { get; private set; }
        public Vector2 Velocity { get; private set; }
        public float MoveSpeed { get; private set; } = 100f;

        public Scopophobia() : base("Scopophobia", "Enemy") {
            
        }
    }
}
