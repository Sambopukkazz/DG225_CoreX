using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Tilemaps;
using MonoGame.Extended.Tilemaps.Rendering;
using MonoGame.Extended.ViewportAdapters;
using Penumbra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophonia {
    public class GameManager {
        private Tilemap _tilemap;
        private TilemapSpriteBatchRenderer _renderer;
        private OrthographicCamera _camera;
        private Player _player;
        private AudioManager _audioManager;
        private LightManager _lightManager;



        public GameManager(BoxingViewportAdapter viewportAdapter) {
            _camera = new OrthographicCamera(viewportAdapter);
            _audioManager = new AudioManager();
            _lightManager = new LightManager();
            _player = new Player();

            _tilemap = MainGame.Content.Load<Tilemap>("Tilemap/testmapfr");
            _renderer = new TilemapSpriteBatchRenderer();
            _renderer.LoadTilemap(_tilemap);
        }

        public void Update(GameTime gameTime) {
            _player.Update();

            _camera.LookAt(_player.Transform.Position);

            _renderer.Update(gameTime);

            Matrix transformMatrix = _camera.GetViewMatrix();

            _lightManager.Update(transformMatrix);

            _audioManager.Update(_player);

        }

        public void Draw() {
            LightManager.Penumbra.BeginDraw();

            MainGame.GraphicsDevice.Clear(Color.White);

            Matrix transformMatrix = _camera.GetViewMatrix();

            _renderer.Draw(MainGame.SpriteBatch, _camera);

            MainGame.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: transformMatrix);

            _player.Draw();

            MainGame.SpriteBatch.End();

        }
    }
}
