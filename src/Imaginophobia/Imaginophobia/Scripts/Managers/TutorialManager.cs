using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class TutorialManager : IDisposable {
        private static readonly string _path = Path.Combine(AppContext.BaseDirectory, "Content", "Datas", "tutorial.json");
        private readonly TutorialData _data;
        private int _index;
        private static readonly JsonSerializerOptions _opts = new() {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        public TutorialManager() {
            _data = Load();

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

        public static TutorialData Load() {
            try {
                if (File.Exists(_path)) {
                    return JsonSerializer.Deserialize<TutorialData>(File.ReadAllText(_path), _opts) ?? new TutorialData();
                }
            }
            catch (Exception e) {
                Console.WriteLine($"Tutorial load failed, running without tutorial: {e.Message}");
            }
            return new TutorialData();
        }

        public void Dispose() {
            ObjectiveManager.Completed -= OnCompleted;
        }
    }
}
