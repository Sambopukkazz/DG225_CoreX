using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class Wall : ICollisionActor {
        public int Id { get; }
        public CollisionShape2D Shape { get; }

        public Wall(int id, Vector2 pos, Vector2 size) {
            Id = id;
            BoundingBox2D bounds = BoundingBox2D.CreateFromPositionAndSize(pos, size);
            Shape = new CollisionShape2D(bounds);
        }
    }
}
