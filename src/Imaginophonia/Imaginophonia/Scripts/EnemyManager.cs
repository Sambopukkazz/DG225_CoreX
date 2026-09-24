using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Collisions;

namespace Imaginophobia {
    public class EnemyManager {
        public List<GameObject> enemies;
        private float SpawnInterval = 5;
        private int _autophobiaSpawnChance = 30; // 0-100
        private int _scopophobiaSpawnChance = 5;
        private Texture2D _scopophobiaTexture;
        private SpriteSheet _autophobiaSpriteSheet;
        public event Action<ICollisionActor> EnemySpawned;
        private Random _rand;
        private bool _continueSpawnEnemy;
        private Player _player;
        Rectangle _worldBound;

        public EnemyManager(Player player) {
            enemies = new List<GameObject>();
            _player = player;
            _rand = new Random();

            //Load enemies texture
            _scopophobiaTexture = MainGame.Content.Load<Texture2D>("Phobias/Scopophobia-Entity");

            Texture2DAtlas atlas = MainGame.Content.Load<Texture2DAtlas>("Character/spitesheet_player");
            _autophobiaSpriteSheet = new("autophobia", atlas);

            _autophobiaSpriteSheet.DefineAnimation("walk-forward", builder => {
                builder.IsLooping(true);
                for (int i = 1; i < 8; i++) {
                    if (i == 7) builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0));
                    else builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0.3));
                }
            });
            
            _autophobiaSpriteSheet.DefineAnimation("idle", builder => {
                builder.IsLooping(false)
                .AddFrame("sprite_idle", TimeSpan.FromSeconds(0));
            });

            _continueSpawnEnemy = true;
        }

        public void Update(Rectangle worldBound) {
            for (int i = enemies.Count - 1; i >= 0; i--) {
                if (enemies[i].Active == false) {
                    enemies.RemoveAt(i);
                    continue;
                }
                enemies[i].Update();
            }

            if(_worldBound != worldBound) {
                _worldBound = worldBound;
            }
        }

        public void Draw() {
            for (int i = enemies.Count - 1; i >= 0; i--) {
                enemies[i].Draw();
            }
        }

        public void SpawnEnemy() {
            Vector2 pos = _player.Transform.Position;
            int posOffset = _rand.Next(100, 500);
            if (_rand.Next(0,10) < 5) {
                pos.X = _worldBound.Left - posOffset;
            }
            else {
                pos.X = _worldBound.Right + posOffset;
            }

            GameObject enemy;
            int[] spawnNumber = new int[_autophobiaSpawnChance];
            for (int i = 0; i< spawnNumber.Length; i++) {
                spawnNumber[i] = _rand.Next(0, 101);
            }

            int randomNumber = _rand.Next(0, 101);
            if (spawnNumber.Contains<int>(randomNumber)) {
                //Autophobia autophobia = new Autophobia(pos, _player, _autophobiaSpriteSheet);
                Scopophobia autophobia = new Scopophobia(pos,_player,_scopophobiaTexture);
                enemies.Add(autophobia);
                EnemySpawned?.Invoke(autophobia);
            }

            if (_continueSpawnEnemy) {
                Time.AddTimer(SpawnEnemy, SpawnInterval);
            }
        }

        public void SetSpawnRange() {

        }

        public void ClearEnemies() {
            enemies.Clear();
        }
    }
}
