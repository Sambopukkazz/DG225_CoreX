using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class WirePath {
        public Color Color { get; set; }
        public List<Point> Points { get; set; } = new List<Point>();
        public bool IsComplete { get; set; }
    }
}
