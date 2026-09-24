using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Penumbra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class LightManager {
        public static PenumbraComponent Penumbra { get; private set; }
        private List<Light> _lights;
        public LightManager() {
            if(Penumbra == null) {
                Penumbra = new PenumbraComponent(MainGame.Instance);
                Penumbra.Initialize();
            }
            Penumbra.AmbientColor = Color.FromHSL(10, 0, 20);

            _lights = new List<Light>();
        }

        public void Update(Matrix transformMatrix) {
            Penumbra.Transform = transformMatrix;
        }

        public void AddLight(Light light) {
            Penumbra.Lights.Add(light);
            _lights.Add(light);
        }

        public void ClearLights() {
            foreach(Light light in _lights) {
                Penumbra.Lights.Remove(light);
            }
            _lights.Clear();
        }

        public void StartBlinking(Light light) {
            if (light.Enabled) {
                light.Enabled = false;
            }
            else {
                light.Enabled = true;
            }
            Time.AddTimer(5f);
        }
    }
}
