using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Imaginophobia {
    // Reads every shift out of Content/Datas/shifts.json and hands one out on demand.
    // Static like SaveManager, because the menu only ever needs the count and one shift at a
    // time and the data never changes while the game runs.
    //
    // The file writes every stage block by name - {"Normal":…,"Anxious":…,"Insane":…} - but
    // the game wants (Normal, Anxious, Insane) tuples. System.Text.Json can't fill a
    // ValueTuple, so the flattening happens here rather than in the data class: this is the
    // only place that knows the JSON's shape.
    public static class ShiftLoader {
        private static readonly string _path = Path.Combine(AppContext.BaseDirectory, "Content", "Datas", "shifts.json");
        private static readonly JsonDocumentOptions _opts = new() {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        // Declared after _opts on purpose: static initializers run top to bottom, so Load()
        // must not run before the options it reads.
        private static readonly List<ShiftData> _shifts = Load();

        // The shift the player last started, so dying doesn't kick the menu back to Shift 1.
        public static int Selected { get; set; } = 1;

        public static int Count => _shifts.Count;

        public static ShiftData Get(int shift) {
            foreach (ShiftData data in _shifts) {
                if (data.Shift == shift) {
                    return data;
                }
            }

            return _shifts[0];
        }

        // The arrows wrap rather than stop, so there is no dead end at either end.
        // A single-shift file has nothing to wrap to, so both arrows resolve to it.
        public static int Next(int shift) => (Count <= 1 || shift >= Count) ? 1 : shift + 1;
        public static int Previous(int shift) => (Count <= 1 || shift <= 1) ? Count : shift - 1;

        private static List<ShiftData> Load() {
            try {
                if (File.Exists(_path)) {
                    using JsonDocument document = JsonDocument.Parse(File.ReadAllText(_path), _opts);

                    List<ShiftData> shifts = new();
                    foreach (JsonElement entry in document.RootElement.EnumerateArray()) {
                        shifts.Add(ReadShift(entry));
                    }

                    if (shifts.Count > 0) {
                        return shifts;
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine($"Shift load failed, falling back to the current tuning: {e.Message}");
            }

            return new List<ShiftData> { DefaultShift() };
        }

        private static ShiftData ReadShift(JsonElement entry) {
            ShiftData shift = new() {
                Shift = entry.GetProperty("Shift").GetInt32(),
                SanityReduction = ReadStages(entry.GetProperty("SanityReduction")),
            };

            // Optional, so a file that omits it keeps ShiftData's default clock.
            if (entry.TryGetProperty("TimeLimit", out JsonElement timeLimit)) {
                shift.TimeLimit = timeLimit.GetSingle();
            }

            foreach (JsonProperty enemy in entry.GetProperty("EnemyChance").EnumerateObject()) {
                shift.EnemyChance[enemy.Name] = ReadStages(enemy.Value);
            }

            return shift;
        }

        // {"Normal":…,"Anxious":…,"Insane":…} -> (Normal, Anxious, Insane).
        // A missing stage throws and the whole file is rejected, rather than silently
        // becoming a zero drain or a zero spawn chance.
        private static (float, float, float) ReadStages(JsonElement stages) {
            return (stages.GetProperty("Normal").GetSingle(),
                    stages.GetProperty("Anxious").GetSingle(),
                    stages.GetProperty("Insane").GetSingle());
        }

        // The numbers the game already plays with. Used when shifts.json is missing or
        // unreadable, so a bad file degrades to today's difficulty instead of zero enemies.
        private static ShiftData DefaultShift() {
            return new ShiftData {
                Shift = 1,
                TimeLimit = 601f,
                SanityReduction = (0.5f, 0.2f, 0f),
                EnemyChance = new Dictionary<string, (float, float, float)> {
                    { "Autophobia", (20, 40, 55) },
                    { "Scopophobia", (3, 12, 20) },
                    { "Nyctophobia", (35, 50, 65) },
                    { "Entamaphobia", (1, 20, 25) },
                },
            };
        }
    }
}
