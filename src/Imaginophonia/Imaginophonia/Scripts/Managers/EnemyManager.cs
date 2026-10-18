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
        public List<GameObject> Enemies;
        private float _spawnInterval = 5;
        private float _nyctophobiaSpawnInterval = 15;
        private (int, int, int) _autophobiaAvailableChance = (20, 35, 55); // Normal, Anxious, Insane
        private int _autophobiaSpawnChance = 20; // 0-100
        private (int, int, int) _scopophobiaAvailableChance = (5, 12, 17); // Normal, Anxious, Insane
        private int _scopophobiaSpawnChance = 5;
        private (int, int, int) _nyctophobiaAvailableChance = (5, 12, 17); // Normal, Anxious, Insane
        private int _nyctophobiaSpawnChance = 5;
        private Texture2D _scopophobiaTexture;
        private SpriteSheet _autophobiaSpriteSheet;
        public event Action<GameObject> EnemySpawned;
        public event Action<GameObject> EnemyRemoved;
        private Random _rand;
        private bool _continueSpawnEnemy;
        private Player _player;
        private Rectangle _worldBound;

        public bool HasActiveScopophobia() => Enemies.Exists(enemy => enemy is Scopophobia scopo && scopo.LightBrokeCount < 12);
        public bool HasActiveNyctophobia() => Enemies.Exists(enemy => enemy is Nyctophobia);

        public EnemyManager(Player player) {
            Enemies = new List<GameObject>();
            _player = player;
            _rand = new Random();

            _player.SanityStateChanged += OnStageChanged;

            //Load enemies texture
            _scopophobiaTexture = MainGame.Content.Load<Texture2D>("Phobias/sprite_scopophobia");

            Texture2DAtlas atlas = MainGame.Content.Load<Texture2DAtlas>("Character/spritesheet_player");
            _autophobiaSpriteSheet = new("autophobia", atlas);

            _autophobiaSpriteSheet.DefineAnimation("walk-forward", builder => {
                builder.IsLooping(true);
                for (int i = 1; i <= 9; i++) {
                    if (i == 9) builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0));
                    else builder.AddFrame($"sprite_walk_0{i}", TimeSpan.FromSeconds(0.12));
                }
            });
            
            _autophobiaSpriteSheet.DefineAnimation("idle", builder => {
                builder.IsLooping(false)
                .AddFrame("sprite_idle", TimeSpan.FromSeconds(0));
            });
        }

        public void Update(Rectangle worldBound) {
            for (int i = Enemies.Count - 1; i >= 0; i--) {
                if (Enemies[i].Active == false) {
                    EnemyRemoved?.Invoke(Enemies[i]);
                    Enemies[i].OnDestroyed();
                    Enemies.RemoveAt(i);
                    continue;
                }
                Enemies[i].Update();
            }

            if (SceneManager.GetActiveScene().Name == "tilemap_sewer") {
                if(_continueSpawnEnemy == false) {
                    _continueSpawnEnemy = true;
                    Time.AddTimer(SpawnEnemy, 2f);
                    Time.AddTimer(SpawnNyctophobia, 15f);
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
            for (int i = Enemies.Count - 1; i >= 0; i--) {
                Enemies[i].Draw();
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

                int randomNumber = _rand.Next(0, 101);

                if (randomNumber < _autophobiaSpawnChance && HasActiveScopophobia() == false) {
                    Autophobia autophobia = new Autophobia(pos, _player, _autophobiaSpriteSheet);
                    Enemies.Add(autophobia);
                    EnemySpawned?.Invoke(autophobia);
                }

                if (randomNumber < _scopophobiaSpawnChance && HasActiveScopophobia() == false) {
                    if (spawnLeft) {
                        pos.X -= 1920;
                    }
                    else {
                        pos.X += 1920;
                    }

                    Scopophobia scopophobia = new Scopophobia(pos, _player, _scopophobiaTexture);
                    Enemies.Add(scopophobia);
                    EnemySpawned?.Invoke(scopophobia);
                }

                //if (enemy != null) {
                //    Enemies.Add(enemy);
                //    EnemySpawned?.Invoke(enemy as ICollisionActor);
                //}

                Time.AddTimer(SpawnEnemy, _spawnInterval);

                //int[] autophobiaNumber = new int[_autophobiaSpawnChance];
                //for (int i = 0; i < autophobiaNumber.Length; i++) {
                //    autophobiaNumber[i] = _rand.Next(0, 101);
                //}

                //int[] scopophobiaNumber = new int[_scopophobiaSpawnChance];
                //for (int i = 0; i < scopophobiaNumber.Length; i++) {
                //    scopophobiaNumber[i] = _rand.Next(0, 101);
                //}


                //if (autophobiaNumber.Contains<int>(randomNumber)) {
                //    Autophobia autophobia = new Autophobia(pos, _player, _autophobiaSpriteSheet);
                //    Enemies.Add(autophobia);
                //    EnemySpawned?.Invoke(autophobia);
                //}
                //else if (scopophobiaNumber.Contains<int>(randomNumber) && HasActiveScopophobia() == false) {
                //    if (spawnLeft) {
                //        pos.X -= 1920;
                //    }
                //    else {
                //        pos.X += 1920;
                //    }
                //    Scopophobia scopophobia = new Scopophobia(pos, _player, _scopophobiaTexture);
                //    Enemies.Add(scopophobia);
                //    EnemySpawned?.Invoke(scopophobia);
                //}


            }
        }

        //Different interval
        public void SpawnNyctophobia() {
            if (_continueSpawnEnemy) {
                int randomNumber = _rand.Next(0, 101);
                if (randomNumber < _nyctophobiaSpawnChance && HasActiveNyctophobia() == false) {
                    Nyctophobia nyctophobia = new Nyctophobia();
                    Enemies.Add(nyctophobia);
                    EnemySpawned?.Invoke(nyctophobia);
                }

                Time.AddTimer(SpawnNyctophobia, _nyctophobiaSpawnInterval);
            }
        }

        public void ClearEnemies() {
            foreach (GameObject enemy in Enemies) {
                enemy.OnDestroyed();
            }
            Enemies.Clear();
        }

        private void OnStageChanged() {
            if(_player.SanityState == SanityState.Anxious) {
                _autophobiaSpawnChance = _autophobiaAvailableChance.Item2;
                _scopophobiaSpawnChance = _scopophobiaAvailableChance.Item2;
                _nyctophobiaSpawnChance = _nyctophobiaAvailableChance.Item2;
            }
            else if (_player.SanityState == SanityState.Insane) {
                _autophobiaSpawnChance = _autophobiaAvailableChance.Item3;
                _scopophobiaSpawnChance = _scopophobiaAvailableChance.Item3;
                _nyctophobiaSpawnChance = _nyctophobiaAvailableChance.Item3;
            }
            else if (_player.SanityState == SanityState.Normal) {
                _autophobiaSpawnChance = _autophobiaAvailableChance.Item1;
                _scopophobiaSpawnChance = _scopophobiaAvailableChance.Item1;
                _nyctophobiaSpawnChance = _nyctophobiaAvailableChance.Item1;
            }
        }
    }
}
