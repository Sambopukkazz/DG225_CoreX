using MonoGame.Extended.Tilemaps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophonia {
    public abstract class Scene {
        public string Name { get; private set; }
        public Tilemap TileMap { get; }

        public Scene() {
            BuildCollision();
            BuildLight();
        }

        private void BuildCollision() {
            
        }

        private void BuildLight() {

        }
    }
}
