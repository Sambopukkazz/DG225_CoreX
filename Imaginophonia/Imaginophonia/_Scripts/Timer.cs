using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophonia {
    public class Timer {
        private readonly float _timeLength;
        private float _timeLeft;
        private bool _active;
        public bool Repeat { get; set; }

        public Timer(float timeLength) { 
            _timeLength = timeLength;
            _timeLeft = timeLength;
        }
        public void Update() {
            if (!_active) return;
            _timeLeft -= Time.DeltaTime;
        }
    }
}
