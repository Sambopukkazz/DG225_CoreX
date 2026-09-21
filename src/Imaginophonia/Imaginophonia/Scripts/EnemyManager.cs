using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class EnemyManager {
        public List<GameObject> enemies;
        public EnemyManager() {
            enemies = new List<GameObject>();

            //Load enemies texture
            MainGame.Content.Load<Texture2D>("Phobias/Scopophobia-Entity");
        }

        private void Update() {
            for(int i = enemies.Count - 1; i >= 0; i--) {
                enemies[i].Update();
            }
        }

        private void Draw() {
            for (int i = enemies.Count - 1; i >= 0; i--) {
                enemies[i].Draw();
            }
        }
    }
}
