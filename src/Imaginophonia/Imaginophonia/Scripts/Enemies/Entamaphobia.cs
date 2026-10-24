using Gum.GueDeriving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class Entamaphobia : GameObject {
        private int _stage;
        private float _value;
        private float _nextStageValue;
        private float _speed;
        private Player _player;
        private Random _rand;
        public Entamaphobia(Player player) : base("Entamaphobia", "Enemy") {
            _player = player;
            _rand = new Random();
        }

        public override void Update() {
            if (_value == 0) {
                _nextStageValue = _rand.Next(1,10);
            }

            if (_player.FlashLight.Enabled) {
                _value = -1 * Time.DeltaTime;
            }
        }
    }
}
