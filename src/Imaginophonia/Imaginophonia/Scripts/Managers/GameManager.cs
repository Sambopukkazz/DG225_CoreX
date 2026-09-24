using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
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
        private AudioManager _audioManager;
        private LightManager _lightManager;
        private SceneManager _sceneManager;
        private CollisionManager _collisionManager;

        //temp UI
        private Texture2D _spacebar;

        public GameManager(BoxingViewportAdapter viewportAdapter) {
            _camera = new OrthographicCamera(viewportAdapter);
            _audioManager = new AudioManager();
            _lightManager = new LightManager();
            _player = new Player();

            _collisionManager = new CollisionManager();
            _collisionManager.AddCollider(_player);
            _collisionManager.CallLoadScene += OnCallLoadScene;

            _sceneManager = new SceneManager();
            _sceneManager.SceneLoaded += OnSceneLoaded;
            _sceneManager.LoadScene(SceneName.tilemap_electrical_room.ToString(), _camera, _collisionManager, _lightManager);

            //temp
            _spacebar = MainGame.Content.Load<Texture2D>("UI/ui_spacebar");
        }

        public void Update(GameTime gameTime) {
            InputManager.Update();

            _player.Update();

            _camera.LookAt(new Vector2(_player.Transform.Position.X, _player.Transform.Position.Y -260));

            _sceneManager.Update(gameTime);

            _collisionManager.Update(_player);

            _lightManager.Update(_camera.GetViewMatrix());

            _audioManager.Update(_player);

        }

        public void Draw(GameTime gameTime) {
            LightManager.Penumbra.BeginDraw();

            MainGame.GraphicsDevice.Clear(Color.White);

            _sceneManager.Draw(_camera);

            MainGame.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: _camera.GetViewMatrix());
            _player.Draw();

            if (_player.CanInteract) {
                MainGame.SpriteBatch.Draw(_spacebar, new Vector2(960 - 300, 540 + 350), Color.White);
            }

            AudioManager.Instance.Draw();

            MainGame.SpriteBatch.End();

        }

        private void OnCallLoadScene(string sceneName) {
            _sceneManager.LoadScene(sceneName, _camera, _collisionManager, _lightManager);
        }

        private void OnSceneLoaded(Vector2 spawnPosition) {
            _player.LoadScenePosition(spawnPosition);
        }
        
    }
}