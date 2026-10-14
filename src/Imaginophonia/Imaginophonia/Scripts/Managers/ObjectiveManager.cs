using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public static class ObjectiveManager {
        // ---- discrete objectives (drives the tutorial) ----
        public static event Action<string> Completed;   // fires once per id

        private static readonly HashSet<string> _completed = new();

        public static bool IsComplete(string id) => _completed.Contains(id);

        public static void Complete(string id) {
            if (_completed.Add(id))
                Completed?.Invoke(id);
        }

        // ---- valve progress (the sewer main goal) ----
        public const int TargetValves = 5;
        private static readonly HashSet<int> _repairedValves = new();
        public static int RepairedValveCount => _repairedValves.Count;
        public static bool AllValvesRepaired => RepairedValveCount >= TargetValves;

        public static bool IsValveRepaired(int valveId) => _repairedValves.Contains(valveId);

        // call on a *successful* valve completion — keyed by trigger id, so no double-count
        public static void RepairValve(int valveId) {
            if (!_repairedValves.Add(valveId)) return;   // same valve twice = no-op
            if (AllValvesRepaired) Complete("valves-all"); // ties the goal into the objective/tutorial system
        }

        // run reset (death): the sewer reloads fresh, so the tally must follow
        public static void ResetValves() => _repairedValves.Clear();
    }
}
