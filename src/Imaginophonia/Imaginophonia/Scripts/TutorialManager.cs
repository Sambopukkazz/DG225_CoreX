using MonoGame.Extended.BitmapFonts;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class TutorialManager {
        private readonly TutorialData _data;
        private readonly BitmapFont _font;
        private int _index;
        private string _message;
        private float _messageTimer;
        public TutorialManager() {
            _data = MainGame.Content.Load<TutorialData>("Datas/tutorial");
            _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            ObjectiveManager.Completed += _ => Advance();
            Advance();
        }
        private void OnObjectiveCompleted(string id) {
            if (_index < _data.Steps.Count && _data.Steps[_index].CompleteOn == id)
                _index++;
        }
        // duration <= 0 → message stays until replaced
        public void ShowMessage(string text, float duration = 4f) {
            _message = text;
            _messageTimer = duration;
        }
        public void Update() {
            Advance();
            if (_message == null || _messageTimer <= 0f) return;   // <=0 means "sticky"
            _messageTimer -= Time.DeltaTime;
            if (_messageTimer <= 0f) _message = null;
        }
        public void Draw() {
            MainGame.SpriteBatch.Begin();
            if (_index < _data.Steps.Count && !string.IsNullOrEmpty(_data.Steps[_index].Text))
                MainGame.SpriteBatch.DrawString(_font, _data.Steps[_index].Text, new Vector2(700, 100), Color.White);
            if (!string.IsNullOrEmpty(_message))
                MainGame.SpriteBatch.DrawString(_font, _message, new Vector2(700, 200), Color.Yellow);
            MainGame.SpriteBatch.End();
        }

        private void Advance() {
            while (_index < _data.Steps.Count && ObjectiveManager.IsComplete(_data.Steps[_index].CompleteOn))
                _index++;
        }
    }
}
