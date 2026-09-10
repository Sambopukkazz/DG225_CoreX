using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Tilemaps.Rendering;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class SceneManager {
        public enum SceneName { electricalroom, testmapfr }
        private SceneName _sceneName;
        private static Scene s_activeScene;
        public static Scene GetActiveScene() => s_activeScene;
        private static Scene _lastScene;
        private List<Scene> _loadedScene;
        private TilemapSpriteBatchRenderer _renderer;

        public SceneManager() {
            _renderer = new TilemapSpriteBatchRenderer();
            _loadedScene = new List<Scene>();
        }

        public void Update(GameTime gameTime) {
            _renderer.Update(gameTime);
        }

        public void Draw(OrthographicCamera camera) {
            _renderer.Draw(MainGame.SpriteBatch, camera);
        }

        public void LoadScene(string sceneName, OrthographicCamera camera, CollisionManager collisionManager, LightManager lightManager) {
            s_activeScene = new Scene(sceneName, collisionManager, lightManager);
            _renderer.LoadTilemap(s_activeScene.TileMap);
            camera.EnableWorldBounds(s_activeScene.TileMap.WorldBounds);
        }

        private void UnloadScene() {
            _loadedScene.Add(s_activeScene);
            s_activeScene = null;
            _renderer.UnloadTilemap();
        }
    }
}
