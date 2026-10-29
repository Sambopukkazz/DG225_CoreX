using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Collisions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class Collectible : ICollisionActor {
        public int Id { get; }
        public CollisionShape2D Shape { get; }
        public string Tag { get; }

        public Collectible(int id, Vector2 pos, Vector2 size, string tag) {
            Id = id;
            Tag = tag;
            BoundingBox2D bounds = BoundingBox2D.CreateFromPositionAndSize(pos, size);
            Shape = new CollisionShape2D(bounds);
        }
    }
}
