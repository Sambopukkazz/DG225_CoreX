using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class Dot {
        public Color Color { get; set; }
        public Rectangle Bound { get; set; }
        public bool IsWiring { get; set; }
        public bool IsComplete { get; set; }

        public Dot(Rectangle rect,Color color) {
            Bound = rect;
            Color = color;
        }

        public void Update(bool isAddingWire) {

        }

        public void Draw() {
            MainGame.SpriteBatch.DrawCircle(Bound.X + Bound.Width / 2f, Bound.Y + Bound.Height / 2f, 10, 10, Color,20);
        }
    }
}
