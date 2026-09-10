using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
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
        private AudioManager _audioManager;
        private LightManager _lightManager;
        private SceneManager _sceneManager;
        private CollisionManager _collisionManager;

        

        public GameManager(BoxingViewportAdapter viewportAdapter) {
            _camera = new OrthographicCamera(viewportAdapter);
            _audioManager = new AudioManager();
            _lightManager = new LightManager();
            _player = new Player();

            _collisionManager = new CollisionManager();
            _collisionManager.AddCollision(_player);

            _sceneManager = new SceneManager();
            _sceneManager.LoadScene(SceneName.electricalroom.ToString(), _camera, _collisionManager, _lightManager);
        }
        public void Update(GameTime gameTime) {

            _player.Update();

            _camera.LookAt(new Vector2(_player.Transform.Position.X, _player.Transform.Position.Y -260));

            _sceneManager.Update(gameTime);

            _collisionManager.Update(_player);

            Matrix transformMatrix = _camera.GetViewMatrix();

            _lightManager.Update(transformMatrix);

            _audioManager.Update(_player);

        }

        public void Draw() {
            LightManager.Penumbra.BeginDraw();

            MainGame.GraphicsDevice.Clear(Color.White);

            Matrix transformMatrix = _camera.GetViewMatrix();

            _sceneManager.Draw(_camera);

            MainGame.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);

            _player.Draw();
            AudioManager.Instance.Draw();

            MainGame.SpriteBatch.End();

        }
    }
}
