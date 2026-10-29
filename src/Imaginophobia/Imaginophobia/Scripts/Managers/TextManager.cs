using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended;
using Microsoft.Xna.Framework;
using RenderingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;


namespace Imaginophobia {
    public class TextManager {
        private string _message;
        private string _tutorialMessage;
        private float _messageTimer;
        private BitmapFont _font;
        private Timer _timer;

        public TextManager() {
            _font = MainGame.Content.Load<BitmapFont>("Fonts/GenerationFonting");

            _timer = Time.Timers.Find(timer => timer.Name == "MainTimer");
        }

        public void Update() {
            if (_message == null || _messageTimer <= 0f) return;   // <=0 means "sticky"
            _messageTimer -= Time.DeltaTime;
            if (_messageTimer <= 0f) _message = null;
        }

        public void Draw() {
            SpriteBatch spriteBatch = MainGame.SpriteBatch;

            if (!string.IsNullOrEmpty(_tutorialMessage)) {
                spriteBatch.DrawString(_font, _tutorialMessage, new Vector2(700, 100), Color.White);
            }
            if (!string.IsNullOrEmpty(_message)) {

                spriteBatch.DrawString(_font, _message, new Vector2(700, 200), Color.Yellow);
            }
            spriteBatch.DrawString(_font, $"Sanity: ", new Vector2(100, 100), Color.White);
            spriteBatch.DrawString(_font, $"Battery: ", new Vector2(100, 150), Color.White);
            if (ObjectiveManager.IsComplete("entered-sewer")) {
                spriteBatch.DrawString(_font, $"Valves repaired: {ObjectiveManager.RepairedValveCount}/{ObjectiveManager.TargetValves}", new Vector2(120, 240), Color.White);
            }

            spriteBatch.DrawString(_font,TimeSpan.FromSeconds(_timer.TimeLeft).ToString(@"mm\:ss"), new Vector2(960-32, 0), Color.White);
        }

        public void InputTutorial(string text) {
            _tutorialMessage = text;
        }

        // duration <= 0 → message stays until replaced
        public void ShowMessage(string text, float duration = 4f) {
            _message = text;
            _messageTimer = duration;
        }
    }
}
