using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophonia {
    public class GameManager {

        public GameManager() {
            
        }

        public void Update() {
            
        }

        public void Draw() {
            Globals.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            Globals.SpriteBatch.End();
        }
    }
}
