using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophonia {
    public static class Globals {
        public static GraphicsDeviceManager Graphics;
        public static ContentManager Content { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
    }
}