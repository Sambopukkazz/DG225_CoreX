using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class SpawnPoint {
        public string Name;
        public Vector2 Position;
        public SpawnPoint(string name, Vector2 pos) {
            Name = name;
            Position = pos;
        }
    }
}
