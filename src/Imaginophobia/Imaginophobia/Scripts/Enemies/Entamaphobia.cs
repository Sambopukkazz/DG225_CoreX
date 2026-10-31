using Gum.GueDeriving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class Entamaphobia : GameObject {
        private int _stage = 1;
        private float _value;
        private float _nextStageValue;
        private float _speed = 1;
        private Player _player;
        public  Trigger Door;
        private Random _rand;
        public Entamaphobia(Player player) : base("Entamaphobia", "Enemy") {
            _player = player;
            _rand = new Random();
        }

        public override void Update() {
            if (_player.CurrentAction != CharacterAction.Peeking) {
                _value += _speed * Time.DeltaTime;

                if (_value >= _nextStageValue) {
                    _stage++;
                    _value = 0;
                    switch (_stage) {
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                    }
                    _nextStageValue = _rand.Next(1, 10);
                }
            }
            else if (_player.CurrentAction == CharacterAction.Peeking) {
                if (_player.FlashLight.Enabled) {
                    _value -= 1 * Time.DeltaTime;
                }

                if (_value <= 0) {
                    _stage--;
                    _value = 0;
                    switch (_stage) {
                        case 0:
                            break;
                    }
                    _nextStageValue = _rand.Next(1, 10);
                }
            }

            Door.Progress = _stage * 50;
        }
    }
}
