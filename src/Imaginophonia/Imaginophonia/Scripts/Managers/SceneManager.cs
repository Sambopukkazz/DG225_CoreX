using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Imaginophobia {
    public class SceneManager {
        private static SceneBuilder _activeScene;
        public static SceneBuilder GetActiveScene() => _activeScene;
        private string _lastSceneName;
        private TilemapSpriteBatchRenderer _renderer;

        private TileHighlightRenderer _tileHighlight;
        private SceneBuilder _highlightScene;
        private Player _player;
        private HighlightSettings _valveSettings = new HighlightSettings { ScanSpeed = 0.5f, ScanColor = ColorHelper.FromHex("#bfd8ff"), OutlineRadius = 4f, OutlineColor = ColorHelper.FromHex("#ffd83f") };
        private HighlightSettings _lockerSettings = new HighlightSettings { ScanSpeed = 0.25f, ScanColor = ColorHelper.FromHex("#bfd8ff"), OutlineRadius = 4f, OutlineColor = ColorHelper.FromHex("#ffd83f") };
        private HighlightSettings _panelSettings = new HighlightSettings { ScanSpeed = 0.15f, ScanColor = ColorHelper.FromHex("#bfd8ff"), OutlineRadius = 4f, OutlineColor = ColorHelper.FromHex("#ffd83f") };
        private HighlightSettings _doorSettings = new HighlightSettings { ScanSpeed = 0.4f, ScanColor = ColorHelper.FromHex("#bfd8ff"), OutlineRadius = 4f, OutlineColor = ColorHelper.FromHex("#ffd83f") };
        //private FadeTransition _fadeTransition;
        public event Action<Vector2> SceneLoaded;

        public SceneManager(Player player) {
            _renderer = new TilemapSpriteBatchRenderer();
            //_fadeTransition = new FadeTransition(MainGame.GraphicsDevice, Color.Black);
            _tileHighlight = new TileHighlightRenderer();
            _tileHighlight.ValveGroup.Settings = _valveSettings;
            _tileHighlight.LockerGroup.Settings = _lockerSettings;
            _tileHighlight.PanelGroup.Settings = _panelSettings;
            _tileHighlight.DoorGroup.Settings = _doorSettings;

            _player = player;
        }

        public void Update(CollisionManager collisionManager) {
            _tileHighlight.Update();
            UpdateHighlight(collisionManager);
            //_renderer.Update(gameTime); only if we have animated sprite
        }

        public void Draw(OrthographicCamera camera) {
            _renderer.Draw(MainGame.SpriteBatch, camera);

            _tileHighlight.Draw();
        }

        public void Render(OrthographicCamera camera) => _tileHighlight.Render(camera);

        private void Debug() {
            BitmapFont _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            //MainGame.SpriteBatch.Begin();
            //MainGame.SpriteBatch.DrawString(_font, $"SpawnPoint Count {_activeScene?.PlayerSpawnPoints.Count}", new Vector2(150, 200), Color.White);
            //foreach (SpawnPoint spawnPoint in _activeScene.PlayerSpawnPoints) {
            //    MainGame.SpriteBatch.DrawString(_font, $"SpawnPoint Pos {spawnPoint.Position}", new Vector2(150, 250 + 50 * _activeScene.PlayerSpawnPoints.IndexOf(spawnPoint)), Color.White);
            //}
            //MainGame.SpriteBatch.End();
        }
        private void UpdateHighlight(CollisionManager collisionManager) {
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
                if (trigger.CompareTag("dots") && ObjectiveManager.IsPanelRepaired(trigger.Id)) {
                    continue;
                }
                if ((trigger.CompareTag("skillcheck") || trigger.CompareTag("dots")) && _player.CurrentAction == CharacterAction.Repairing) {
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

                if (trigger == collisionManager.FocusedTrigger) {
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

        public void LoadScene(string sceneName, OrthographicCamera camera, CollisionManager collisionManager, LightManager lightManager, bool saveOutgoing = true) {
            if (_activeScene != null) {
                if (saveOutgoing) {
                    SaveManager.SaveScene(_activeScene.Name, _activeScene.Triggers, lightManager.LightAreas);
                }
                UnloadScene();
            }

            _activeScene = new SceneBuilder(sceneName, collisionManager, lightManager);
            SaveManager.ApplyToScene(sceneName, _activeScene.Triggers, lightManager.LightAreas);
            _renderer.LoadTilemap(_activeScene.TileMap);
            camera.EnableWorldBounds(_activeScene.TileMap.WorldBounds);
            camera.IsZoomClampedToWorldBounds = true;

            if (_activeScene.Name == "tilemap_sewer") {
                AudioManager.Instance.PlayAmbiance("sewer");
            }
            else {
                AudioManager.Instance.PlayAmbiance("test");
            }

            foreach (SpawnPoint spawnPoint in _activeScene.PlayerSpawnPoints) {
                if (spawnPoint.Name == _lastSceneName || spawnPoint.Name == "Start") {
                    SceneLoaded?.Invoke(spawnPoint.Position);
                    _activeScene.PlayerSpawnPoints.Clear();
                    return;
                }
            }
        }

        private void UnloadScene() {
            _lastSceneName = _activeScene?.Name;
            _renderer.UnloadTilemap();
            _activeScene?.PlayerSpawnPoints.Clear();
            _activeScene = null;
            
        }
    }
}
