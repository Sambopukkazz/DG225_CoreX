using Gum.GueDeriving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class Entamaphobia : GameObject {
        private int _stage = 1;
        private float _increaseValue;
        private float _decreaseValue;
        private float _increaseStageValue;
        private float _decreaseStageValue;
        private float _increaseSpeed = 60;
        private float _decreaseSpeed = 1;
        private Player _player;
        private  Trigger _door;
        private Random _rand;
        public Entamaphobia(Player player, Trigger door) : base("Entamaphobia", "Enemy") {
            _player = player;
            _door = door;
            _rand = new Random();

            _decreaseValue = 4;
            _increaseStageValue = _rand.Next(343, 492);

            AudioManager.Instance.Play3DSFX(_door.Shape.BoundingBox.Center, SoundEffectName.EntamaAdvance, 60, 1920, volume: 0.8f);

            if (_player.SanityState == SanityState.Insane) {
                _increaseSpeed = _increaseSpeed * 1.2f;
            }
            //switch (_player.SanityState) {
            //    case SanityState.Anxious:
            //        _increaseSpeed = _increaseSpeed * 1.1f;
            //        break;
            //    case SanityState.Insane:
                    
            //        break;
            //}
        }

        public override void Update() {
            //Not peeking, enta stage increasing
            if (_player.CurrentAction != CharacterAction.Peeking) {
                //if (_value < _nextStageValue) {
                //    _value += _increaseSpeed * Time.DeltaTime;
                //}

                _increaseValue += _increaseSpeed * Time.DeltaTime;

                if (_increaseValue >= _increaseStageValue && _stage < 4) {
                    _stage ++;
                    _increaseValue = 0;

                    switch (_stage) {
                        case 2:
                            AudioManager.Instance.Play3DSFX(_door.Shape.BoundingBox.Center, SoundEffectName.EntamaAdvance, 60, 1920, volume: 0.85f);
                            _increaseStageValue = _rand.Next(521, 678);
                            break;
                        case 3:
                            AudioManager.Instance.Play3DSFX(_door.Shape.BoundingBox.Center, SoundEffectName.EntamaAdvance, 60, 1920, volume: 0.9f);
                            AudioManager.Instance.PlaySFX(SoundEffectName.EntamaPresence, repeat: true, volume: 0.5f);
                            _increaseStageValue = _rand.Next(60, 1500);
                            break;
                        case 4:
                            AudioManager.Instance.Play3DSFX(_door.Shape.BoundingBox.Center, SoundEffectName.EntamaAdvance, 60, 1920, volume: 1f);
                            AudioManager.Instance.ChangeSFXVolume(SoundEffectName.EntamaPresence, 0.75f);
                            break;
                    }
                    //_nextStageValue = _rand.Next(3, 6);
                }
            }

            //Peeking, enta stage stable/decreasing
            else if (_player.CurrentAction == CharacterAction.Peeking) {
                if (_player.FlashLight.Enabled) {
                    _decreaseValue -= _decreaseSpeed * Time.DeltaTime;
                }

                if (_decreaseValue <= 0 && _stage > 0) {
                    _stage--;
                    switch (_stage) {
                        case 0:
                            AudioManager.Instance.Play3DSFX(_door.Shape.BoundingBox.Center, SoundEffectName.EntamaDecrease, 60, 1920, volume: 0.2f);
                            AudioManager.Instance.StopSFX(SoundEffectName.EntamaPresence);
                            MainGame.ScreenManager.PulseFade(0,0.1f);
                            SetActive(false);
                            break;
                        case 1:
                            AudioManager.Instance.Play3DSFX(_door.Shape.BoundingBox.Center, SoundEffectName.EntamaDecrease, 60, 1920, volume: 0.4f);
                            AudioManager.Instance.StopSFX(SoundEffectName.EntamaPresence);
                            _decreaseStageValue = 3;
                            MainGame.ScreenManager.PulseFade(0, 0.1f);
                            break;
                        case 2:
                            AudioManager.Instance.Play3DSFX(_door.Shape.BoundingBox.Center, SoundEffectName.EntamaDecrease, 60, 1920, volume: 0.6f);
                            AudioManager.Instance.FadeSFXVolumeOut(SoundEffectName.EntamaPresence, 1.5f);
                            _decreaseStageValue = 3;
                            MainGame.ScreenManager.PulseFade(0, 0.1f);
                            break;
                        case 3:
                            AudioManager.Instance.Play3DSFX(_door.Shape.BoundingBox.Center, SoundEffectName.EntamaDecrease, 20, 1920, volume: 0.8f);
                            _decreaseStageValue = 4;
                            MainGame.ScreenManager.PulseFade(0, 0.1f);
                            break;
                    }
                    //_nextStageValue = _rand.Next(3, 6);
                    _decreaseValue = _decreaseStageValue;
                }
            }

            _door.Progress = _stage;
        }

        public override void OnDestroyed() {
            switch (_stage) {
                case 1:
                    _player.Sanity -= 25;
                    break;
                case 2:
                    _player.Sanity -= 30;
                    break;
                case 3:
                    _player.Sanity -= 35;
                    break;
                case 4:
                    _player.Sanity -= 40;
                    break;
            }

            Time.AddTimer(10, "EntamaCooldown");
        }
    }
}
