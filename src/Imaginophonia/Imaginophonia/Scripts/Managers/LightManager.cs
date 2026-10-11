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
        public List<LightArea> LightAreas;
        public LightManager() {
            if(Penumbra == null) {
                Penumbra = new PenumbraComponent(MainGame.Instance);
                Penumbra.Initialize();
            }
            Penumbra.AmbientColor = ColorHelper.FromHex("#242323");

            _lights = new List<Light>();
            LightAreas = new List<LightArea>();
        }

        public void Update(Matrix transformMatrix) {
            Penumbra.Transform = transformMatrix;

            foreach (LightArea lightArea in LightAreas) {
                lightArea.Update();
            }
        }

        public void AddLight(Light light) {
            Penumbra.Lights.Add(light);
            _lights.Add(light);
        }

        public void AddLight(LightArea lightArea) {
            Penumbra.Lights.Add(lightArea.Light);
            LightAreas.Add(lightArea);
        }

        public void ClearLights() {
            foreach(Light light in _lights) {
                Penumbra.Lights.Remove(light);
            }
            _lights.Clear();

            foreach (LightArea lightArea in LightAreas) {
                Penumbra.Lights.Remove(lightArea.Light);
            }
            LightAreas.Clear();
        }
    }
}
