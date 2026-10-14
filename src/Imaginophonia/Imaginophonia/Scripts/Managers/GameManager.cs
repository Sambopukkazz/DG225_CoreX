using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
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
    public class GameManager {
        private OrthographicCamera _camera;
        private Player _player;
        //private AudioManager _audioManager;
        private LightManager _lightManager;
        private SceneManager _sceneManager;
        private CollisionManager _collisionManager;
        private EnemyManager _enemyManager;
        private UIManager _uiManager;
        private TutorialManager _tutorialManager;
        private DialogueManager _dialogueManager;
        private CameraShaker _cameraShaker;
        private TileHighlightRenderer _tileHighlight;
        private SceneBuilder _highlightScene;
        private HighlightSettings _valveSettings = new HighlightSettings { ScanSpeed = 0.5f, ScanColor = ColorHelper.FromHex("#bfd8ff"), OutlineRadius = 4f, OutlineColor = ColorHelper.FromHex("#ffd83f") };
        private HighlightSettings _lockerSettings = new HighlightSettings { ScanSpeed = 0.25f, ScanColor = ColorHelper.FromHex("#bfd8ff"), OutlineRadius = 4f, OutlineColor = ColorHelper.FromHex("#ffd83f") };
        private HighlightSettings _panelSettings = new HighlightSettings { ScanSpeed = 0.15f, ScanColor = ColorHelper.FromHex("#bfd8ff"), OutlineRadius = 4f, OutlineColor = ColorHelper.FromHex("#ffd83f") };
        private HighlightSettings _doorSettings = new HighlightSettings { ScanSpeed = 0.4f, ScanColor = ColorHelper.FromHex("#bfd8ff"), OutlineRadius = 4f, OutlineColor = ColorHelper.FromHex("#ffd83f") };

        public GameManager(BoxingViewportAdapter viewportAdapter) {
            _camera = new OrthographicCamera(viewportAdapter);
            AudioManager audioManager = new AudioManager();
            _lightManager = new LightManager();
            _player = new Player();

            _enemyManager = new EnemyManager(_player);
            _enemyManager.EnemySpawned += OnEnemySpawned;
            _enemyManager.EnemyRemoved += OnEnemyRemoved;

            _collisionManager = new CollisionManager(_player, _enemyManager.Enemies);
            _collisionManager.AddCollider(_player);
            _collisionManager.DoorInteracted += OnDoorInteracted;

            SaveManager.Delete();
            _sceneManager = new SceneManager();
            _sceneManager.SceneLoaded += OnSceneLoaded;
            _sceneManager.LoadScene(Scene.tilemap_electrical_room.ToString(), _camera, _collisionManager, _lightManager);

            _tileHighlight = new TileHighlightRenderer();
            _tileHighlight.ValveGroup.Settings = _valveSettings;
            _tileHighlight.LockerGroup.Settings = _lockerSettings;
            _tileHighlight.PanelGroup.Settings = _panelSettings;
            _tileHighlight.DoorGroup.Settings = _doorSettings;

            _uiManager = new UIManager(_player, _camera);

            _tutorialManager = new TutorialManager();

            _dialogueManager = new DialogueManager();
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


            _sceneManager.Update(gameTime);

            _collisionManager.Update();

            _tileHighlight.Update();
            UpdateHighlight();

            _lightManager.Update(_camera.GetViewMatrix());

            AudioManager.Instance.Update(_player);

            _uiManager.Update();

            _tutorialManager.Update();

            _cameraShaker.Update(gameTime);

            if (_player.Sanity <= 0) {
                _player.Reset();
                Restart();
            }

            DebugUpdate();
        }

        public void Draw(GameTime gameTime) {
            // Build the highlight first, into its own target - binding render targets must not happen
            // inside the lighting capture
            _tileHighlight.Render(_camera);

            LightManager.Penumbra.BeginDraw();

            MainGame.GraphicsDevice.Clear(Color.White);

            _sceneManager.Draw(_camera);

            // Behind the player, so it sits inside the lighting capture
            _tileHighlight.Draw();

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
            _tutorialManager.Draw();
        }

        private void Restart() {
            SaveManager.Delete();
            ObjectiveManager.ResetValves();
            _enemyManager.ClearEnemies();
            _sceneManager.LoadScene(Scene.tilemap_electrical_room.ToString(), _camera, _collisionManager, _lightManager, false);
        }

        private void OnDoorInteracted(string sceneName) {
            if (sceneName == "end") {
                if (ObjectiveManager.AllValvesRepaired) {
                    ObjectiveManager.Complete("exit-ladder");
                }
                else {
                    _tutorialManager.ShowMessage("Still have work to do", 2f);
                }
                return;
            }

            if (_enemyManager.HasActiveScopophobia()) {
                AudioManager.Instance.PlaySFX(Sound.DoorStuck);
            }
            else {
                _enemyManager.ClearEnemies();
                _sceneManager.LoadScene(sceneName, _camera, _collisionManager, _lightManager);
            }
        }

        private void OnSceneLoaded(Vector2 spawnPosition) {
            _player.LoadScenePosition(spawnPosition);
            AudioManager.Instance.PlaySFX(Sound.DoorClose);
            if (SceneManager.GetActiveScene()?.Name == "tilemap_sewer") {
                ObjectiveManager.Complete("entered-sewer");
            } 
        }
        
        private void OnEnemySpawned(ICollisionActor enemy) {
            _collisionManager.AddCollider(enemy, "enemies");
            if (enemy is Scopophobia) {
                SaveManager.ResetScene(Scene.tilemap_electrical_room.ToString());
                SaveManager.ResetScene(Scene.tilemap_electrical_room_2.ToString());
                _lightManager.ScopoAlarmAll();
            }
        }

        private void OnEnemyRemoved(ICollisionActor enemy) {
            if (enemy is Scopophobia) {
                _tutorialManager.ShowMessage("Don't stay in the dark. Go to electric room and repair electric system", 8f);
            }
            else if (enemy is Autophobia autiophobia && autiophobia.AllowMovement != false) {
                _tutorialManager.ShowMessage("Try using flashlight to fight them", 4f);
            }

            _collisionManager.RemoveCollider(enemy);
        }

        private void DebugUpdate() {
            if (KeyboardExtended.GetState().WasKeyPressed(Keys.K)) {
                _cameraShaker.Shake(5f, 1f);
            }
            //if (KeyboardExtended.GetState().WasKeyPressed(Keys.Right)) {
            //    _camera.Zoom += 0.05f;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
            //    VignetteOverlay.Scale += new Vector2(0.01f, 0);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
            //    VignetteOverlay.Scale -= new Vector2(0.01f, 0);
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad6)) {
            //    VignetteOverlay.Softness += 0.01f;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad4)) {
            //    VignetteOverlay.Softness -= 0.01f;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad8)) {
            //    _vignetteOverlay.Sharpness += 0.01f;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad2)) {
            //    _vignetteOverlay.Sharpness -= 0.01f;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad1)) {
            //    VignetteOverlay.Color = Color.Red;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad0)) {
            //    VignetteOverlay.Color = Color.Black;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad3)) {
            //    _scotopicLight.Enabled = false;
            //}

        }

        private void UpdateHighlight() {
            SceneBuilder scene = SceneManager.GetActiveScene();

            if (scene == null) {
                return;
            }

            // The active scene changes on every door transition, so resync the renderer to the new map
            if (!ReferenceEquals(scene, _highlightScene)) {
                _highlightScene = scene;
                _tileHighlight.LoadScene(scene.TileMap);
            }

            _tileHighlight.Clear();

            foreach (Trigger trigger in scene.Triggers) {
                if (trigger.CompareTag("skillcheck") && ObjectiveManager.IsValveRepaired(trigger.Id)) {
                    continue;
                }
                if (trigger.CompareTag("dots") && ObjectiveManager.IsComplete("repair")) {
                    continue;
                }
                if (trigger.CompareTag("hideout") && _player.CurrentAction == CharacterAction.Hiding) {
                    continue;
                }

                if (trigger.Tiles.Count == 0) {
                    continue;
                }

                HighlightGroup group;

                if (trigger.TileLayer.Name == "Valves") {
                    group = _tileHighlight.ValveGroup;
                }
                else if (trigger.TileLayer.Name == "Lockers") {
                    group = _tileHighlight.LockerGroup;
                }
                else if (trigger.TileLayer.Name == "ElectricalPanel") {
                    group = _tileHighlight.PanelGroup;
                }
                else if (trigger.TileLayer.Name == "Doors") {
                    group = _tileHighlight.DoorGroup;
                }
                else {
                    continue;
                }

                group.Layer = trigger.TileLayer;

                if (trigger == _collisionManager.FocusedTrigger) {
                    foreach (TilemapTileEntry tile in trigger.Tiles) {
                        group.FocusTiles.Add(tile);
                    }
                }
                else {
                    foreach (TilemapTileEntry tile in trigger.Tiles) {
                        group.ScanTiles.Add(tile);
                    }
                }
            }
        }
    }
}