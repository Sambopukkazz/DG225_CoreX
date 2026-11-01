using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class TutorialData {
        public List<TutorialStep> Steps { get; set; } = new();

        private static readonly string _path = Path.Combine(AppContext.BaseDirectory, "Content", "Datas", "tutorial.json");

        private static readonly JsonSerializerOptions _opts = new() {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

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
    }
}
