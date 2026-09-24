using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens.Transitions;
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
        public enum SceneName { electricalroom, electricalroom2, sewer }
        private SceneName _sceneName;
        private static Scene _activeScene;
        public static Scene GetActiveScene() => _activeScene;
        private string _lastSceneName;
        private TilemapSpriteBatchRenderer _renderer;
        //private FadeTransition _fadeTransition;
        public event Action<Vector2> SceneLoaded;

        public SceneManager() {
            _renderer = new TilemapSpriteBatchRenderer();
            //_fadeTransition = new FadeTransition(MainGame.GraphicsDevice, Color.Black);
        }

        public void Update(GameTime gameTime) {
            //_renderer.Update(gameTime); only if we have animated sprite
        }

        public void Draw(OrthographicCamera camera) {
            _renderer.Draw(MainGame.SpriteBatch, camera); 
        }

        private void Debug() {
            BitmapFont _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            //MainGame.SpriteBatch.Begin();
            //MainGame.SpriteBatch.DrawString(_font, $"SpawnPoint Count {_activeScene?.PlayerSpawnPoints.Count}", new Vector2(150, 200), Color.White);
            //foreach (SpawnPoint spawnPoint in _activeScene.PlayerSpawnPoints) {
            //    MainGame.SpriteBatch.DrawString(_font, $"SpawnPoint Pos {spawnPoint.Position}", new Vector2(150, 250 + 50 * _activeScene.PlayerSpawnPoints.IndexOf(spawnPoint)), Color.White);
            //}
            //MainGame.SpriteBatch.End();
        }

        public void LoadScene(string sceneName, OrthographicCamera camera, CollisionManager collisionManager, LightManager lightManager) {
            UnloadScene();
            _activeScene = new Scene(sceneName, collisionManager, lightManager);
            _renderer.LoadTilemap(_activeScene.TileMap);
            camera.EnableWorldBounds(_activeScene.TileMap.WorldBounds);

            if (_activeScene.Name == "sewer") {
                AudioManager.Instance.PlayAmbiance("sewer");
            }
            else {
                AudioManager.Instance.PlayAmbiance("test");
            }

            foreach (SpawnPoint spawnPoint in _activeScene.PlayerSpawnPoints) {
                if (spawnPoint.Name == _lastSceneName || spawnPoint.Name == "") {
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
