using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                if(timer.Active == false) {
                    Timers.Remove(timer);
                }
            }
        }

        public static Timer AddTimer(float seconds, string name) {
            Timer timer = new(seconds, name);
            Timers.Add(timer);
            return timer;
        }

        public static Timer AddTimer(Action method, float seconds, string name = "") {
            Timer timer = new(method, seconds, name);
            Timers.Add(timer);
            return timer;
        }
        public static Timer AddTimer(Action method, int min, int max, string name) {
            Timer timer = new(method, min, name);
            timer.Repeat = true;
            timer.Random = true;
            timer.MinMax = (min, max);
            Timers.Add(timer);
            return timer;
        }

        public static void RemoveTimer(Timer timer) {
            Timers.Remove(timer);
            timer = null;
        }

        public static void RemoveTimer(string timerName) {
            Timer timer = Timers.Find(timer => timer.Name == timerName);
            Timers.Remove(timer);
        }

        public static bool TimerExist(string timerName) {
            return Timers.Exists(timer => timer.Name == timerName);
        }

        public static Timer GetTimer(string timerName) {
            return Timers.Find(timer => timer.Name == timerName);
        }

        public static void Purge() {
            Timers.Clear();
        }
    }
}
