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
        private AudioManager _audioManager;
        private LightManager _lightManager;
        private SceneManager _sceneManager;
        private CollisionManager _collisionManager;
        private EnemyManager _enemyManager;
        private UIManager _uiManager;
        private DialogueManager _dialogueManager;
        private CameraShaker _cameraShaker;
        //temp
        

        public GameManager(BoxingViewportAdapter viewportAdapter) {
            _camera = new OrthographicCamera(viewportAdapter);
            _audioManager = new AudioManager();
            _lightManager = new LightManager();
            _player = new Player();

            _collisionManager = new CollisionManager(_player);
            _collisionManager.AddCollider(_player);
            _collisionManager.CallLoadScene += OnCallLoadScene;

            _sceneManager = new SceneManager();
            _sceneManager.SceneLoaded += OnSceneLoaded;
            _sceneManager.LoadScene(Scene.tilemap_electrical_room.ToString(), _camera, _collisionManager, _lightManager);

            _enemyManager = new EnemyManager(_player);
            _enemyManager.EnemySpawned += OnEnemySpawned;
            _enemyManager.SpawnEnemy();

            _uiManager = new UIManager(_player, _camera);

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

            _lightManager.Update(_camera.GetViewMatrix());

            _audioManager.Update(_player);

            _uiManager.Update();

            _cameraShaker.Update(gameTime);

            if (_player.Sanity <= 0) {
                _player.Reset();
                Restart();
            }

            DebugUpdate();
        }

        public void Draw(GameTime gameTime) {
            LightManager.Penumbra.BeginDraw();

            MainGame.GraphicsDevice.Clear(Color.White);

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
            _enemyManager.ClearEnemies();
            _sceneManager.LoadScene(Scene.tilemap_electrical_room.ToString(), _camera, _collisionManager, _lightManager);
        }

        private void OnCallLoadScene(string sceneName) {
            _enemyManager.ClearEnemies();
            _sceneManager.LoadScene(sceneName, _camera, _collisionManager, _lightManager);
        }

        private void OnSceneLoaded(Vector2 spawnPosition) {
            _player.LoadScenePosition(spawnPosition);
            AudioManager.Instance.PlaySFX(Sound.DoorClose);
        }
        
        private void OnEnemySpawned(ICollisionActor enemy) {
            _collisionManager.AddCollider(enemy, "enemies");
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
    }
}