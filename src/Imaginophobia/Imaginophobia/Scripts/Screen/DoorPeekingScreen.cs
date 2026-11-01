using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using Penumbra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class DoorPeekingScreen : Screen {
        private Trigger _door;
        private Player _player;
        private PointLight _light;
        private Texture2D _sewerBackground;
        private Texture2D _doorForegound;
        private Texture2DAtlas _textureAtlas;

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

            _textureAtlas = MainGame.Content.Load<Texture2DAtlas>("Phobias/spritesheet_entamaphobia");

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

            AudioManager.Instance.ChangeMediaVolume(false, 0.5f);

            MainGame.Loading = false;
        }

        public override void Update(GameTime gameTime) {
            
            _light.Enabled = _player.FlashLight.Enabled;

            if (InputManager.PressedKey(Keys.Q) && _player.CurrentAction == CharacterAction.Peeking) {
                Action quit = () => {
                    LightManager.Penumbra.Lights.Remove(_light);
                    _light = null;

                    LightManager.Penumbra.AmbientColor = ColorHelper.FromHex("#242323");

                    foreach (Light light in LightManager.Penumbra.Lights.Where(light => light != _player.FlashLight && light != _player.ScotopicLight)) {
                        light.Enabled = true;
                    }

                    AudioManager.Instance.UnmuteAudio();

                    AudioManager.Instance.ChangeMediaVolume(true, 0.5f);
                    MainGame.GameState = GameState.Gameplay;
                    MainGame.ScreenManager.FadeIn(0.5f);
                    MainGame.Loading = false;
                };

                _player.TogglePeek();
                ObjectiveManager.Complete("peek");
                MainGame.ScreenManager.CloseScreen(0.5f);
                Time.AddTimer(quit, 0.5f);
            }
        }

        public override void Draw(GameTime gameTime) {
            LightManager.Penumbra.BeginDraw();

            SpriteBatch spriteBatch = MainGame.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.Draw(_sewerBackground, Vector2.Zero, Color.White);

            if (_door.Progress != 0) {
                spriteBatch.Draw(_textureAtlas[$"entama_stage_{_door.Progress}"], Vector2.Zero, Color.White);
            }

            spriteBatch.Draw(_doorForegound, Vector2.Zero, Color.White);

            if (_door.Progress > 2) {
                spriteBatch.Draw(_textureAtlas[$"entama_hand"], Vector2.Zero, Color.White);
            }

            spriteBatch.End();

            LightManager.Penumbra.Draw(gameTime);
        }
    }
}
