using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophonia {
    public static class Time {
        public static float DeltaTime { get; private set; }
        public static float TimeScale { get; set; }
        public static float TimeDilation { get; set; }
        public static TimeSpan ElapsedTime { get; private set; }
        public static List<Timer> Timers { get; private set; } = new();

        public static void Update(GameTime gameTime) {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            ElapsedTime = gameTime.ElapsedGameTime;

            foreach (Timer timer in Timers.ToList()) {
                timer?.Update();
                if(timer.TimeLeft <= 0) {
                    Timers.Remove(timer);
                }
            }
        }

        public static void AddTimer(float second) {
            Timer timer = new(second);
            Timers.Add(timer);
        }
    }
}
