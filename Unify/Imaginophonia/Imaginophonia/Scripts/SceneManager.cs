using MonoGame.Extended.Tilemaps.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophonia {
    public class SceneManager {
        enum SceneName { TestScene, ERScene }
        private SceneName _sceneName;
        private static Scene s_activeScene;
        public static Scene GetActiveScene() => s_activeScene;
        private static Scene _lastScene;
        private List<Scene> _loadedScene;
        private TilemapSpriteBatchRenderer _renderer;

        public SceneManager(TilemapSpriteBatchRenderer renderer) {
            _renderer = renderer;
            _loadedScene = new List<Scene>();

        }

        public void Update() {
            LoadScene(SceneName.TestScene.ToString());
        }

        public void LoadScene(string sceneName) {
            _renderer.LoadTilemap(s_activeScene.TileMap);
        }

        private void UnloadScene() {
            _renderer.UnloadTilemap();
        }
    }
}
