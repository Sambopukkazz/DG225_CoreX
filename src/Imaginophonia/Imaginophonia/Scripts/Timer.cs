using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class Timer {
        private readonly string Name;
        private readonly float _timeLength;
        public float TimeLeft { get; private set; }
        private bool _active;
        public bool Repeat { get; set; }
        public event EventHandler OnTimerCompleted;
        private Action _method;

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
        }

        public Timer(Action method,float timeLength) {
            _method = method;
            _timeLength = timeLength;
            TimeLeft = timeLength;
            _active = true;
        }

        public void Update() {
            if (!_active) return;
            TimeLeft -= Time.DeltaTime;

            if (TimeLeft <= 0) {
                if(_method != null) {
                    _method();
                }

                OnTimerCompleted?.Invoke(this, EventArgs.Empty);

                if (Repeat) {
                    Reset();
                }
                else {
                    Toggle();
                    TimeLeft = 0;
                }
            }
        }

        public void Toggle() {
            if (_active) {
                _active = false;
            }
            else {
                _active = true;
            }
        }

        public void Reset() {
            TimeLeft = _timeLength;
        }
    }
}
