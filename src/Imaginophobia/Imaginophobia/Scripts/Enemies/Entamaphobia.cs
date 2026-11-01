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
        private float _increaseSpeed = 1;
        private float _decreaseSpeed = 2;
        private Player _player;
        private  Trigger _door;
        private Random _rand;
        public Entamaphobia(Player player, Trigger door) : base("Entamaphobia", "Enemy") {
            _player = player;
            _door = door;
            _rand = new Random();
            AudioManager.Instance.Play3DSFX(_door.Shape.BoundingBox.Center, SoundEffectName.EntamaAdvance, 60, 1920, volume: 0.25f);
        }

        public override void Update() {
            //Not peeking, enta stage increasing
            if (_player.CurrentAction != CharacterAction.Peeking) {
                _value += _increaseSpeed * Time.DeltaTime;

                if (_value >= _nextStageValue && _stage < 4) {
                    _stage += 1;
                    _value = 0;

                    switch (_stage) {
                        case 2:
                            AudioManager.Instance.Play3DSFX(_door.Shape.BoundingBox.Center, SoundEffectName.EntamaAdvance, 60, 1920, volume: 0.5f);
                            break;
                        case 3:
                            AudioManager.Instance.Play3DSFX(_door.Shape.BoundingBox.Center, SoundEffectName.EntamaAdvance, 60, 1920, volume: 0.75f);
                            AudioManager.Instance.PlaySFX(SoundEffectName.EntamaPresence, repeat: true, volume: 0.5f);
                            break;
                        case 4:
                            AudioManager.Instance.Play3DSFX(_door.Shape.BoundingBox.Center, SoundEffectName.EntamaAdvance, 60, 1920, volume: 1f);
                            AudioManager.Instance.ChangeVolume(SoundEffectName.EntamaPresence, 0.75f);
                            break;
                    }
                    _nextStageValue = _rand.Next(3, 6);
                }
            }

            //Peeking, enta stage stable/decreasing
            else if (_player.CurrentAction == CharacterAction.Peeking) {
                if (_player.FlashLight.Enabled) {
                    _value -= _decreaseSpeed * Time.DeltaTime;
                }

                if (_value <= 0 && _stage > 0) {
                    _stage--;
                    switch (_stage) {
                        case 0:
                            AudioManager.Instance.Play3DSFX(_door.Shape.BoundingBox.Center, SoundEffectName.EntamaDecrease, 20, 1920, volume: 1f);
                            AudioManager.Instance.StopSFX(SoundEffectName.EntamaPresence);
                            SetActive(false);
                            break;
                        case 1:
                            AudioManager.Instance.Play3DSFX(_door.Shape.BoundingBox.Center, SoundEffectName.EntamaDecrease, 20, 1920, volume: 1f);
                            AudioManager.Instance.StopSFX(SoundEffectName.EntamaPresence);
                            break;
                        case 2:
                            AudioManager.Instance.Play3DSFX(_door.Shape.BoundingBox.Center, SoundEffectName.EntamaDecrease, 20, 1920, volume: 1f);
                            AudioManager.Instance.StopSFX(SoundEffectName.EntamaPresence);
                            break;
                        case 3:
                            AudioManager.Instance.Play3DSFX(_door.Shape.BoundingBox.Center, SoundEffectName.EntamaDecrease, 20, 1920, volume: 1f);
                            break;
                    }
                    _nextStageValue = _rand.Next(3, 6);
                    _value = _nextStageValue;
                }
            }

            _door.Progress = _stage * 50;
        }
    }
}
