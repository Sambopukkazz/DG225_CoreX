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
        private List<GameObject> _enemies;
        public IReadOnlyList<GameObject> Enemies => _enemies;
        
        private (float, float, float) _autophobiaAvailableChance = (20, 40, 55); // Normal, Anxious, Insane
        private float _autophobiaSpawnChance = 20; // 0-100
        private float _autoSpawnInterval = 4.96f;

        private (float, float, float) _scopophobiaAvailableChance = (3, 12, 20); // Normal, Anxious, Insane
        private float _scopophobiaSpawnChance = 3; // 0-100
        private float _scopoSpawnInterval = 4.98f;

        private (float, float, float) _nyctophobiaAvailableChance = (35, 50, 65); // Normal, Anxious, Insane
        private float _nyctophobiaSpawnChance = 35; // 0-100
        private float _nyctophobiaSpawnInterval = 29.95f;

        private (float, float, float) _entamaphobiaAvailableChance = (1, 20, 25); // Normal, Anxious, Insane
        private float _entamaphobiaSpawnChance = 1; // 0-100
        private float _entamaphobiaSpawnInterval = 3.97f;
        
        private Texture2D _scopophobiaTexture;
        private SpriteSheet _autophobiaSpriteSheet;
        public event Action<GameObject> EnemySpawned;
        public event Action<GameObject> EnemyRemoved;
        private Random _rand;
        private bool _continueSpawnEnemy;
        private bool _hasEverSpawnScopophobia;
        private Player _player;
        private Rectangle _worldBound;

        public bool HasActiveScopophobia() => _enemies.Exists(enemy => enemy is Scopophobia scopo && scopo.LightPassedCount < 12);
        public bool HasActiveNyctophobia() => _enemies.Exists(enemy => enemy is Nyctophobia);
        public bool HasActiveEntamaphobia() => _enemies.Exists(enemy => enemy is Entamaphobia);

        public EnemyManager(Player player) {
            _enemies = new List<GameObject>();
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
            Timer entamaTimer = Time.AddTimer(SpawnEntamaphobia, _entamaphobiaSpawnInterval, "EntamaSpawnTimer");
            entamaTimer.Repeat = true;
            Time.AddTimer(60, "EntamaCooldown");
        }

        // The shift owns every enemy's chance table. Applied before the first spawn, and it
        // re-reads the current stage so the session starts on the right numbers.
        public void ApplyShift(ShiftData shift) {
            if (shift == null) {
                return;
            }

            _autophobiaAvailableChance = shift.EnemyChanceTuple("Autophobia", _autophobiaAvailableChance);
            _scopophobiaAvailableChance = shift.EnemyChanceTuple("Scopophobia", _scopophobiaAvailableChance);
            _nyctophobiaAvailableChance = shift.EnemyChanceTuple("Nyctophobia", _nyctophobiaAvailableChance);
            _entamaphobiaAvailableChance = shift.EnemyChanceTuple("Entamaphobia", _entamaphobiaAvailableChance);

            OnStageChanged();
        }

        public void Update(Rectangle worldBound) {
            for (int i = _enemies.Count - 1; i >= 0; i--) {
                if (_enemies[i].Active == false) {
                    EnemyRemoved?.Invoke(_enemies[i]);
                    _enemies[i].OnDestroyed();
                    _enemies.RemoveAt(i);
                    continue;
                }
                _enemies[i].Update();
            }

            if (SceneManager.GetActiveScene().Name == "tilemap_sewer" && _continueSpawnEnemy == false) {
                _continueSpawnEnemy = true;
                Time.GetTimer("EntamaSpawnTimer").Pause();
                Time.GetTimer("AutoSpawnTimer").Resume();
                Time.AddTimer(PlayerEnteringSewer,10, "BlackoutEnemyCooldown");
            }
            else if (SceneManager.GetActiveScene().Name != "tilemap_sewer" && _continueSpawnEnemy == true) {
                _continueSpawnEnemy = false;
                Time.RemoveTimer("BlackoutEnemyCooldown");
                Time.GetTimer("AutoSpawnTimer").Pause();
                Time.GetTimer("EntamaSpawnTimer").Resume();
                PlayerExitingSewer();
            }

            if(_worldBound != worldBound) {
                _worldBound = worldBound;
            }

            if (_hasEverSpawnScopophobia == false && ObjectiveManager.AllValvesRepaired && ShiftLoader.Selected >= 3) {
                ForceSpawnScopophobia();
            }
        }

        public void Draw() {
            for (int i = _enemies.Count - 1; i >= 0; i--) {
                if (_enemies[i] is not Scopophobia) {
                    _enemies[i].Draw();
                }
            }

            for (int i = _enemies.Count - 1; i >= 0; i--) {
                if (_enemies[i] is Scopophobia) {
                    _enemies[i].Draw();
                }
            }
        }

        public void ForceSpawnScopophobia() {
            if (HasActiveScopophobia()) {
                return;
            }

            Vector2 pos;
            pos.Y = 700;

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

            if (spawnLeft) {
                pos.X -= 10000;
            }
            else {
                pos.X += 1920;
            }

            Scopophobia scopophobia = new Scopophobia(pos, _player, _scopophobiaTexture);
            _enemies.Add(scopophobia);
            Time.GetTimer("ScopoSpawnTimer").Pause();
            EnemySpawned?.Invoke(scopophobia);
            _hasEverSpawnScopophobia = true;
        }

        public void SpawnScopophobia() {
            if (HasActiveScopophobia()) {
                return;
            }

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

            if (randomNumber < _scopophobiaSpawnChance) {
                if (spawnLeft) {
                    pos.X -= 10000;
                }
                else {
                    pos.X += 1920;
                }

                //if (randomNumber < _autophobiaSpawnChance) {
                //    Autophobia autophobia = new Autophobia(pos, _player, _autophobiaSpriteSheet);
                //    _enemies.Add(autophobia);
                //    EnemySpawned?.Invoke(autophobia);
                //}

                pos.Y = 700;
                Scopophobia scopophobia = new Scopophobia(pos, _player, _scopophobiaTexture);
                _enemies.Add(scopophobia);
                Time.GetTimer("ScopoSpawnTimer").Pause();
                EnemySpawned?.Invoke(scopophobia);
                _hasEverSpawnScopophobia = true;
            }
        }

        public void SpawnAutophobia() {
            if (HasActiveScopophobia()) {
                return;
            }

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

            if (randomNumber < _autophobiaSpawnChance) {
                Autophobia autophobia = new Autophobia(pos, _player, _autophobiaSpriteSheet);
                _enemies.Add(autophobia);
                EnemySpawned?.Invoke(autophobia);
            }
        }

        public void SpawnNyctophobia() {
            if (HasActiveNyctophobia()) {
                return;
            }

            int randomNumber = _rand.Next(0, 101);

            if (randomNumber < _nyctophobiaSpawnChance) {
                Nyctophobia nyctophobia = new Nyctophobia();
                _enemies.Add(nyctophobia);
                EnemySpawned?.Invoke(nyctophobia);
            }
        }

        public void SpawnEntamaphobia() {
            if (Time.TimerExist("EntamaCooldown") || HasActiveEntamaphobia() || _player.CurrentAction == CharacterAction.Peeking || _player.CurrentAction == CharacterAction.Reading) {
                return;
            }

            int randomNumber = _rand.Next(0, 101);

            if (randomNumber < _entamaphobiaSpawnChance) {
                Trigger door = SceneManager.GetActiveScene().Triggers.Find(item => item.Name == "tilemap_sewer");

                Entamaphobia entamaphobia = new Entamaphobia(_player, door);
                _enemies.Add(entamaphobia);
            }
        }

        public void ClearEnemies() {
            foreach (GameObject enemy in _enemies) {
                enemy.OnDestroyed();
            }
            _enemies.Clear();
        }

        private void OnStageChanged() {
            _autophobiaSpawnChance = ChanceAtStage(_autophobiaAvailableChance);
            _scopophobiaSpawnChance = ChanceAtStage(_scopophobiaAvailableChance);
            _nyctophobiaSpawnChance = ChanceAtStage(_nyctophobiaAvailableChance);
            _entamaphobiaSpawnChance = ChanceAtStage(_entamaphobiaAvailableChance);
        }

        // The current sanity stage's element of a (Normal, Anxious, Insane) chance table.
        private float ChanceAtStage((float, float, float) chance) {
            return _player.SanityState switch {
                SanityState.Anxious => chance.Item2,
                SanityState.Insane => chance.Item3,
                _ => chance.Item1,
            };
        }

        public void PlayerEnteringSewer() {
            Time.GetTimer("ScopoSpawnTimer").Resume();
            Time.GetTimer("NyctoSpawnTimer").Resume();
        }

        public void PlayerExitingSewer() {
            Time.GetTimer("ScopoSpawnTimer").Pause();
            Time.GetTimer("NyctoSpawnTimer").Pause();
        }

        public void Dispose() {
            _player.SanityStateChanged -= OnStageChanged;
            ClearEnemies();
        }
    }
}
