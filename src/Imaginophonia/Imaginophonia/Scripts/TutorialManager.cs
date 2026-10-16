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
        private int _index;

        public TutorialManager() {
            _data = MainGame.Content.Load<TutorialData>("Datas/tutorial");

            ObjectiveManager.Completed += OnCompleted;
            Advance();
        }

        public void OnCompleted(string complete) {
            Advance();
        }

        public string GetText() {
            if (_index < _data.Steps.Count && !string.IsNullOrEmpty(_data.Steps[_index].Text)) {
                return _data.Steps[_index].Text;
            }

            return string.Empty;
        }

        private void Advance() {
            while (_index < _data.Steps.Count && ObjectiveManager.IsComplete(_data.Steps[_index].CompleteOn))
                _index++;
        }
    }
}
