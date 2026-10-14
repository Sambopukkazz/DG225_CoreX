using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Tilemaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class Trigger : ICollisionActor {
        public int Id { get; }
        public CollisionShape2D Shape { get; }
        public string Name { get; }
        public string Tag { get; }
        public bool Interactable { get; set; }
        public float Progress { get; set; }
        public TilemapTileLayer TileLayer { get; set; }
        public List<TilemapTileEntry> Tiles { get; }

        public Trigger(int id, Vector2 pos, Vector2 size, string tag, string name = null) {
            Id = id;
            Name = name;
            Tag = tag;
            BoundingBox2D bounds = BoundingBox2D.CreateFromPositionAndSize(pos, size);
            Shape = new CollisionShape2D(bounds);
            Interactable = true;
            Tiles = new List<TilemapTileEntry>();
        }

        public bool CompareTag(string tag) {
            if(Tag == tag) return true;
            return false;
        }
    }
}
