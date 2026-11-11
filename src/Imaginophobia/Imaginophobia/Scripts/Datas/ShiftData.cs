using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Imaginophobia {
    // One shift's difficulty numbers. Filled in by ShiftLoader, which flattens each
    // {"Normal":…,"Anxious":…,"Insane":…} block into the tuple the game actually uses.
    public class ShiftData {
        public int Shift { get; set; }

        // Seconds on the "MainTimer" clock. Defaults to the 1.0.0 value so a shift file that
        // forgets the key doesn't hand the player a zero-second run.
        public float TimeLimit { get; set; } = 601f;

        // Sanity lost per second while in the sewer.
        public (float, float, float) SanityReduction { get; set; }

        // Enemy name -> spawn chance 0-100.
        public Dictionary<string, (float, float, float)> EnemyChance { get; set; } = new();

        // An enemy with no entry in this shift keeps the chance table the manager already
        // held, so adding a new phobia later doesn't invalidate the older shift files.
        public (float, float, float) EnemyChanceTuple(string enemy, (float, float, float) fallback) {
            if (EnemyChance.TryGetValue(enemy, out (float, float, float) chance)) {
                return chance;
            }

            return fallback;
        }
    }
}
