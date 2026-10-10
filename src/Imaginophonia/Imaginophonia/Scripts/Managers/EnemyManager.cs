using Gum.Forms.Controls.Games;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class EnemyManager {
        public List<GameObject> enemies;
        private float SpawnInterval = 5;
        private int _autophobiaSpawnChance = 0; // 0-100
        private int _scopophobiaSpawnChance = 100;
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
                for (int i = 1; i <= 9; i++) {
                    if (i == 9) builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0));
                    else builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0.13));
                }
            });
            
            _autophobiaSpriteSheet.DefineAnimation("idle", builder => {
                builder.IsLooping(false)
                .AddFrame("sprite_idle", TimeSpan.FromSeconds(0));
            });
        }

        public void Update(Rectangle worldBound) {
            for (int i = enemies.Count - 1; i >= 0; i--) {
                if (enemies[i].Active == false) {
                    enemies[i].OnDestroyed();
                    enemies.RemoveAt(i);
                    continue;
                }
                enemies[i].Update();
            }

            if (SceneManager.GetActiveScene().Name == "tilemap_sewer") {
                if(_continueSpawnEnemy == false) {
                    _continueSpawnEnemy = true;
                    Time.AddTimer(SpawnEnemy, 2f);
                    
                }
            }
            else {
                _continueSpawnEnemy = false;
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
            if (_continueSpawnEnemy) {
                Vector2 pos;
                if (_player.CurrentAction == CharacterAction.Hiding) {
                    pos.Y = _player.PreviousPos.Y;
                }
                else {
                    pos.Y = _player.Transform.Position.Y;
                }
                int posOffset = _rand.Next(100, 500);
                bool spawnLeft;
                if (_rand.Next(0, 10) < 5) {
                    pos.X = _worldBound.Left - posOffset;
                    spawnLeft = true;
                }
                else {
                    pos.X = _worldBound.Right + posOffset;
                    spawnLeft = false;
                }

                GameObject enemy;

                int[] autophobiaNumber = new int[_autophobiaSpawnChance];
                for (int i = 0; i < autophobiaNumber.Length; i++) {
                    autophobiaNumber[i] = _rand.Next(0, 101);
                }

                int[] scopophobiaNumber = new int[_scopophobiaSpawnChance];
                for (int i = 0; i < scopophobiaNumber.Length; i++) {
                    scopophobiaNumber[i] = _rand.Next(0, 101);
                }

                int randomNumber = _rand.Next(0, 101);
                if (autophobiaNumber.Contains<int>(randomNumber)) {
                    Autophobia autophobia = new Autophobia(pos, _player, _autophobiaSpriteSheet);
                    enemies.Add(autophobia);
                    EnemySpawned?.Invoke(autophobia);
                }
                else if (scopophobiaNumber.Contains<int>(randomNumber) && enemies.Exists(enem => enem.GetType().Name == "Scopophobia") == false) {
                    if (spawnLeft) {
                        pos.X -= 1920;
                    }
                    else {
                        pos.X += 1920;
                    }
                    Scopophobia scopophobia = new Scopophobia(pos, _player, _scopophobiaTexture);
                    enemies.Add(scopophobia);
                    EnemySpawned?.Invoke(scopophobia);
                }

                Time.AddTimer(SpawnEnemy, SpawnInterval);
            }
        }

        public void SetSpawnRange() {

        }

        public void ClearEnemies() {
            foreach (GameObject enemy in enemies) {
                enemy.OnDestroyed();
            }
            enemies.Clear();
        }
    }
}
