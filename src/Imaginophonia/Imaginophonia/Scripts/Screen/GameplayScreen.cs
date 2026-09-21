using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class GameplayScreen : Screen {
        private GameManager _gameManager;

        public GameplayScreen(BoxingViewportAdapter viewportAdapter) {
            _gameManager = new(viewportAdapter);
            UpdateWhenInactive = true;
            DrawWhenInactive = true;
        }

        public override void Update(GameTime gameTime) {
            _gameManager.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            _gameManager.Draw(gameTime);
        }

        public override void OnDeactivated() {
            //Unload game (unsub event etc.)
        }
    }
}
