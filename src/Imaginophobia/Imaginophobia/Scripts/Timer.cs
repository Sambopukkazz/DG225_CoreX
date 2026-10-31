using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class Timer {
        public readonly string Name;
        private readonly float _timeLength;
        public float TimeLeft { get; private set; }
        private bool _active;
        public bool Active => _active;
        private bool _paused;
        public bool Repeat { get; set; }
        public bool Random { get; set; }
        public (int, int) MinMax { get; set; }
        public event EventHandler OnTimerCompleted;
        private Action _method;
        private Random _rand;

        public Timer(float timeLength) { 
            _timeLength = timeLength;
            TimeLeft = timeLength;
            _active = true;
        }

        public Timer(float timeLength, string name) {
            Name = name;
            _timeLength = timeLength;
            TimeLeft = timeLength;
            _active = true;
            _rand = new Random();
        }

        public Timer(Action method, float timeLength, string name) {
            Name = name;
            _method = method;
            _timeLength = timeLength;
            TimeLeft = timeLength;
            _active = true;
            _rand = new Random();
        }

        public void Update(float deltaTime) {
            if (!_active || _paused) {
                return;
            }

            TimeLeft -= deltaTime;

            if (TimeLeft <= 0) {
                if(_method != null) {
                    _method();
                }

                OnTimerCompleted?.Invoke(this, EventArgs.Empty);

                if (Repeat && Random) {
                    TimeLeft = _rand.Next(MinMax.Item1,MinMax.Item2);
                }
                else if (Repeat) {
                    Reset();
                }
                else {
                    ToggleActive();
                    TimeLeft = 0;
                }
            }
        }

        public void ToggleActive() {
            if (_active) {
                _active = false;
            }
            else {
                _active = true;
            }
        }

        public void SetInActive() {
            _active = false;
        }

        public void Reset() {
            TimeLeft = _timeLength;
        }

        public void Pause() {
            if (_paused == false) {
                _paused = true;
            }
        }

        public void Resume() {
            if (_paused == true) {
                _paused = false;
            }
        }
    }
}
