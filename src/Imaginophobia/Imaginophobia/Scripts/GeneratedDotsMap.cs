using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class GeneratedDotsMap {
        public int Columns { get; set; }
        public int Rows { get; set; }
        public Dictionary<int, (Point start, Point end)> Dots { get; set; } = new();
        public Dictionary<int, int> PathLengths { get; set; } = new();
    }
}
