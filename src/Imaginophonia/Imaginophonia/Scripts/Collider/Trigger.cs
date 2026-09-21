using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;

namespace Imaginophobia {
    public class Trigger : ICollisionActor {
        public int Id { get; }
        public CollisionShape2D Shape { get; }
        public string Name { get; }
        public string Tag { get; }
        public bool Interactable { get; set; }
        public float Progress { get; set; }

        public Trigger(int id, Vector2 pos, Vector2 size, string tag) {
            Id = id;
            Tag = tag;
            BoundingBox2D bounds = BoundingBox2D.CreateFromPositionAndSize(pos, size);
            Shape = new CollisionShape2D(bounds);
            Interactable = true;
        }
        public Trigger(int id, Vector2 pos, Vector2 size, string tag, string name) {
            Id = id;
            Name = name;
            Tag = tag;
            BoundingBox2D bounds = BoundingBox2D.CreateFromPositionAndSize(pos, size);
            Shape = new CollisionShape2D(bounds);
            Interactable = true;
        }

        public bool CompareTag(string tag) {
            if(Tag == tag) return true;
            return false;
        }
    }
}
