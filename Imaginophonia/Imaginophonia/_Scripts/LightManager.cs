using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Penumbra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophonia {
    public class LightManager {
        public static PenumbraComponent Penumbra { get; private set; }
        public LightManager() {
            Penumbra = new PenumbraComponent(MainGame.Instance);

            Penumbra.AmbientColor = Color.FromHSL(10, 0, 40);
        }

        public void Update(Matrix transformMatrix) {
            Penumbra.Transform = transformMatrix;
        }
    }
}
