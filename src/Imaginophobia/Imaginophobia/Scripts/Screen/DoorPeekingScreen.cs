using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Penumbra;
using Microsoft.Xna.Framework.Graphics;

namespace Imaginophobia {
    public class DoorPeekingScreen : Screen {
        private Trigger _door;
        private Player _player;
        private PointLight _light;
        private Texture2D _sewerBackground;
        private Texture2D _doorForegound;

        public DoorPeekingScreen(Player player, Trigger trigger) {
            _door = trigger;
            _player = player;

            if (SceneManager.GetActiveScene().Name == Scene.tilemap_electrical_room.ToString()) {
                _sewerBackground = MainGame.Content.Load<Texture2D>("UI/background_peek_er1");
            }
            else {
                _sewerBackground = MainGame.Content.Load<Texture2D>("UI/background_peek_er2");
            }
            
            _doorForegound = MainGame.Content.Load<Texture2D>("UI/foreground_peek_door");

            _light = new PointLight() {
                Position = new Vector2(540, 810),
                Color = Color.FromHSV(150f, 0.5f, 0.5f),
                Scale = new Vector2(800, 800),
                Intensity = 1f,
                Enabled = false
            };
        }

        public override void Initialize() {
            foreach (Light light in LightManager.Penumbra.Lights.Where(light => light != _player.FlashLight && light != _player.ScotopicLight)) {
                light.Enabled = false;
            }

            LightManager.Penumbra.AmbientColor = ColorHelper.FromHex("#615D5D");

            LightManager.Penumbra.Lights.Add(_light);
        }

        public override void Update(GameTime gameTime) {
            
            _light.Enabled = _player.FlashLight.Enabled;

            if (KeyboardExtended.GetState().WasKeyPressed(Keys.Q) && _player.CurrentAction == CharacterAction.Peeking) {
                Action quit = () => {
                    LightManager.Penumbra.Lights.Remove(_light);
                    _light = null;

                    LightManager.Penumbra.AmbientColor = ColorHelper.FromHex("#242323");

                    foreach (Light light in LightManager.Penumbra.Lights.Where(light => light != _player.FlashLight && light != _player.ScotopicLight)) {
                        light.Enabled = true;
                    }

                    MainGame.ScreenManager.FadeIn(0.5f);
                };

                _player.TogglePeek();
                MainGame.ScreenManager.CloseScreen(0.5f);
                Time.AddTimer(quit, 0.5f);
            }
        }

        public override void Draw(GameTime gameTime) {
            LightManager.Penumbra.BeginDraw();

            SpriteBatch spriteBatch = MainGame.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.Draw(_sewerBackground, Vector2.Zero, Color.White);

            Color color = Color.Black;
            switch (_door.Progress) {
                case 50:
                    color = Color.Green;
                    break;
                case 100:
                    color = Color.Yellow;
                    break;
                case 150:
                    color = Color.Orange;
                    break;
                case 200:
                    color = Color.Red;
                    break;
            }

            if (_light.Enabled == false) {
                spriteBatch.FillRectangle(260, 90, 1400, 900, Color.Black * 0.7f);
            }

            spriteBatch.Draw(_doorForegound, Vector2.Zero, Color.White);
            spriteBatch.End();

            LightManager.Penumbra.Draw(gameTime);
        }
    }
}
