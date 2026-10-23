using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;
using MonoGame.Extended.Timers;
using MonoGame.Extended.ViewportAdapters;
using Penumbra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static Imaginophobia.SceneManager;

namespace Imaginophobia {
    public class GameManager : IDisposable {
        private OrthographicCamera _camera;
        private Player _player;
        private LightManager _lightManager;
        private SceneManager _sceneManager;
        private CollisionManager _collisionManager;
        private EnemyManager _enemyManager;
        private UIManager _uiManager;
        private CameraShaker _cameraShaker;
        

        public GameManager(BoxingViewportAdapter viewportAdapter) {
            _camera = new OrthographicCamera(viewportAdapter);
            _lightManager = new LightManager();
            _player = new Player();

            _enemyManager = new EnemyManager(_player);
            _enemyManager.EnemySpawned += OnEnemySpawned;
            _enemyManager.EnemyRemoved += OnEnemyRemoved;

            _collisionManager = new CollisionManager(_player, _enemyManager.Enemies);
            _collisionManager.AddCollider(_player);
            _collisionManager.DoorInteracted += OnDoorInteracted;

            SaveManager.Delete();
            _sceneManager = new SceneManager(_player);
            _sceneManager.SceneLoaded += OnSceneLoaded;
            _sceneManager.LoadScene(Scene.tilemap_electrical_room.ToString(), _camera, _collisionManager, _lightManager);

            Time.AddTimer(() => MainGame.ScreenManager.ReplaceScreen(new WinScreen()), 481, "MainTimer");
            _uiManager = new UIManager(_player, _camera);

            //temp
            _cameraShaker = new CameraShaker();

        }

        public void Update(GameTime gameTime) {
            InputManager.Update();

            _player.Update();

            _enemyManager.Update(_camera.WorldBounds);

            _camera.LookAt(_player.CameraTarget + _cameraShaker.ShakeOffset);

            if (_camera.Zoom != _player.FocusLevel && _player.FocusLevel > _camera.Zoom) {
                _camera.Zoom = MathHelper.Lerp(_camera.Zoom, _player.FocusLevel, 0.05f);
            }
            else if (_camera.Zoom != _player.FocusLevel && _player.FocusLevel < _camera.Zoom) {
                _camera.Zoom = MathHelper.Lerp(_camera.Zoom, _player.FocusLevel, 0.1f);
            }

            _collisionManager.Update();

            _sceneManager.Update(_collisionManager);

            _lightManager.Update(_camera.GetViewMatrix());

            AudioManager.Instance.Update(_player);

            _uiManager.Update();

            _cameraShaker.Update(gameTime);
            
            if (_player.Sanity <= 0) {
                MainGame.ScreenManager.ReplaceScreen(new DeathScreen());
                //Restart();
            }

            DebugUpdate();
        }

        public void Draw(GameTime gameTime) {
            // Build the highlight first, into its own target - binding render targets must not happen
            // inside the lighting capture
            _sceneManager.Render(_camera);

            LightManager.Penumbra.BeginDraw();

            MainGame.GraphicsDevice.Clear(Color.White);

            // Behind the player, so it sits inside the lighting capture
            _sceneManager.Draw(_camera);
            Matrix traformMatrix = _camera.GetViewMatrix();

            MainGame.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: traformMatrix);
            
            _player.Draw();
            _enemyManager.Draw();
            AudioManager.Instance.Draw();

            MainGame.SpriteBatch.End();
            //End lightning
            LightManager.Penumbra.Draw(gameTime);


            //Draw static object that doesn't move with camera
            _uiManager.Draw();
        }

        private void Restart() {
            _player.Reset();
            SaveManager.Delete();
            ObjectiveManager.ResetValves();
            _enemyManager.ClearEnemies();
            _sceneManager.LoadScene(Scene.tilemap_electrical_room.ToString(), _camera, _collisionManager, _lightManager, false);
            Time.GetTimer("MainTimer").Reset();
        }

        private void OnDoorInteracted(string sceneName) {
            if (sceneName == "end") {
                if (ObjectiveManager.AllValvesRepaired) {
                    ObjectiveManager.Complete("exit-ladder");
                    MainGame.ScreenManager.ReplaceScreen(new WinScreen());
                }
                else {
                    _uiManager.ShowMessage("I still have work to do.", 2f);
                }
                return;
            }

            if (_enemyManager.HasActiveScopophobia()) {
                AudioManager.Instance.PlaySFX(SoundEffectName.DoorStuck);
            }
            else {
                AudioManager.Instance.ClearSFXs("anxietySound");
                _enemyManager.ClearEnemies();
                _sceneManager.LoadScene(sceneName, _camera, _collisionManager, _lightManager);
            }
        }

        private void OnSceneLoaded(Vector2 spawnPosition) {
            _player.LoadScenePosition(spawnPosition);
            AudioManager.Instance.PlaySFX(SoundEffectName.DoorClose);
            if (SceneManager.GetActiveScene()?.Name == "tilemap_sewer") {
                ObjectiveManager.Complete("entered-sewer");
            } 
        }
        
        private void OnEnemySpawned(GameObject enemy) {
            if (enemy is Nyctophobia) {
                AudioManager.Instance.PlaySFX(SoundEffectName.LightFlickerLong);
                SaveManager.ResetScene(Scene.tilemap_electrical_room.ToString());
                SaveManager.ResetScene(Scene.tilemap_electrical_room_2.ToString());
                ObjectiveManager.ResetPanels();
                _lightManager.NyctoAlarmAll();
                return;
            }

            _collisionManager.AddCollider(enemy as ICollisionActor, "enemies");

            if (enemy is Scopophobia) {
                SaveManager.ResetScene(Scene.tilemap_electrical_room.ToString());
                SaveManager.ResetScene(Scene.tilemap_electrical_room_2.ToString());
                _lightManager.CleanUpScopo();
                ObjectiveManager.ResetPanels();
                _uiManager.BlackScreen();
                _lightManager.ScopoAlarmAll();
                //_lightManager.ScopoAlarmAll();
            }
        }

        private void OnEnemyRemoved(GameObject enemy) {
            if (enemy is Scopophobia && ObjectiveManager.IsComplete("fixBlackout") == false) {
                _uiManager.ShowMessage("Repairing electric system in the electric room should fix the lights.", 8f);
            }
            else if (enemy is Autophobia autiophobia && autiophobia.AllowMovement != false && ObjectiveManager.IsComplete("collidedAutophobia") == false) {
                ObjectiveManager.Complete("collidedAutophobia");
                _uiManager.ShowMessage("Flashlight might be useful.", 4f);
            }

            _collisionManager.RemoveCollider(enemy as ICollisionActor);
        }

        private void DebugUpdate() {
            if (KeyboardExtended.GetState().WasKeyPressed(Keys.K)) {
                _cameraShaker.Shake(5f, 1f);
            }
        }

        public void Dispose() {
            _enemyManager.EnemySpawned -= OnEnemySpawned;
            _enemyManager.EnemyRemoved -= OnEnemyRemoved;
            _collisionManager.DoorInteracted -= OnDoorInteracted;
            _sceneManager.SceneLoaded -= OnSceneLoaded;

            _uiManager.Dispose();
            _sceneManager.Dispose();
            _collisionManager.Dispose();
            _enemyManager.Dispose();
            _player.Dispose();
            _lightManager.Dispose();

            MainGame.Saturation = 1f;
            ObjectiveManager.Reset();
            Time.SessionActive = false;
            Time.Purge();
        }
    }
}