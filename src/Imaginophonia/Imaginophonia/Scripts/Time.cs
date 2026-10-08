using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Imaginophobia {
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
                timer.Update();
                if(timer.TimeLeft <= 0 || timer.Active == false) {
                    Timers.Remove(timer);
                }
            }
        }

        public static Timer AddTimer(float seconds) {
            Timer timer = new(seconds);
            Timers.Add(timer);
            return timer;
        }
        public static Timer AddTimer(float seconds, string name) {
            Timer timer = new(seconds, name);
            Timers.Add(timer);
            return timer;
        }
        public static Timer AddTimer(Action method, float seconds) {
            Timer timer = new(method,seconds);
            Timers.Add(timer);
            return timer;
        }

        public static Timer AddTimer(Action method, float seconds, string name) {
            Timer timer = new(method, seconds, name);
            Timers.Add(timer);
            return timer;
        }

        public static void RemoveTimer(Timer timer) {
            Timers.Remove(timer);
            timer = null;
        }
    }
}
