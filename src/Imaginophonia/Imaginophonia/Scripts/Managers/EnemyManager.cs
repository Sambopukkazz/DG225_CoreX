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
    public class EnemyManager : IDisposable {
        public List<GameObject> Enemies;
        
        private (int, int, int) _autophobiaAvailableChance = (20, 40, 55); // Normal, Anxious, Insane
        private int _autophobiaSpawnChance = 20; // 0-100
        private float _autoSpawnInterval = 4.96f;

        private (int, int, int) _scopophobiaAvailableChance = (5, 12, 20); // Normal, Anxious, Insane
        private int _scopophobiaSpawnChance = 5; // 0-100
        private float _scopoSpawnInterval = 4.98f;

        private (int, int, int) _nyctophobiaAvailableChance = (35, 50, 65); // Normal, Anxious, Insane
        private int _nyctophobiaSpawnChance = 30; // 0-100
        private float _nyctophobiaSpawnInterval = 30;

        private (int, int, int) _entamaphobiaAvailableChance = (10, 50, 65); // Normal, Anxious, Insane
        private int _entamaphobiaSpawnChance = 10; // 0-100
        private float _entamaphobiaSpawnInterval = 5;
        
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
        public bool HasActiveEntamaphobia() => Enemies.Exists(enemy => enemy is Entamaphobia);

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

            Timer scopoAutoTimer = Time.AddTimer(SpawnScopophobia, _scopoSpawnInterval, "ScopoSpawnTimer");
            scopoAutoTimer.Pause();
            scopoAutoTimer.Repeat = true;
            Timer AutoTimer = Time.AddTimer(SpawnAutophobia, _autoSpawnInterval, "AutoSpawnTimer");
            AutoTimer.Pause();
            AutoTimer.Repeat = true;
            Timer nyctoTimer = Time.AddTimer(SpawnNyctophobia, _nyctophobiaSpawnInterval, "NyctoSpawnTimer");
            nyctoTimer.Pause();
            nyctoTimer.Repeat = true;
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
                    Time.GetTimer("ScopoSpawnTimer").Resume();
                    Time.GetTimer("AutoSpawnTimer").Resume();
                    Time.GetTimer("NyctoSpawnTimer").Resume();
                }
            }
            else {
                _continueSpawnEnemy = false;
                Time.GetTimer("ScopoSpawnTimer").Pause();
                Time.GetTimer("AutoSpawnTimer").Pause();
                Time.GetTimer("NyctoSpawnTimer").Pause();
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

        public void SpawnScopophobia() {
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

            if (randomNumber < _scopophobiaSpawnChance && HasActiveScopophobia() == false) {
                if (spawnLeft) {
                    pos.X -= 1920;
                }
                else {
                    pos.X += 1920;
                }

                if (randomNumber < _autophobiaSpawnChance) {
                    Autophobia autophobia = new Autophobia(pos, _player, _autophobiaSpriteSheet);
                    Enemies.Add(autophobia);
                    EnemySpawned?.Invoke(autophobia);
                }

                Scopophobia scopophobia = new Scopophobia(pos, _player, _scopophobiaTexture);
                Enemies.Add(scopophobia);
                EnemySpawned?.Invoke(scopophobia);
            }
        }

        public void SpawnAutophobia() {
            Vector2 pos;
            if (_player.CurrentAction == CharacterAction.Hiding) {
                pos.Y = _player.PreviousPos.Y;
            }
            else {
                pos.Y = _player.Transform.Position.Y;
            }
            int posOffset = _rand.Next(100, 500);

            if (_rand.Next(0, 10) < 5) {
                pos.X = _worldBound.Left - posOffset;
            }
            else {
                pos.X = _worldBound.Right + posOffset;
            }

            int randomNumber = _rand.Next(0, 101);

            if (randomNumber < _autophobiaSpawnChance && HasActiveScopophobia() == false) {
                Autophobia autophobia = new Autophobia(pos, _player, _autophobiaSpriteSheet);
                Enemies.Add(autophobia);
                EnemySpawned?.Invoke(autophobia);
            }
        }

        public void SpawnNyctophobia() {
            int randomNumber = _rand.Next(0, 101);
            if (randomNumber < _nyctophobiaSpawnChance && HasActiveNyctophobia() == false) {
                Nyctophobia nyctophobia = new Nyctophobia();
                Enemies.Add(nyctophobia);
                EnemySpawned?.Invoke(nyctophobia);
            }
        }

        public void SpawnEntamaphobia() {
            int randomNumber = _rand.Next(0, 101);

            if (randomNumber < _entamaphobiaSpawnChance && HasActiveEntamaphobia() == false) {
                Entamaphobia entamaphobia = new Entamaphobia(_player);
                Enemies.Add(entamaphobia);
                //EnemySpawned?.Invoke(entamaphobia);
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

        public void Dispose() {
            _player.SanityStateChanged -= OnStageChanged;
            ClearEnemies();
        }
    }
}
