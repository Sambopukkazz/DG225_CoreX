using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class AudioManager : IDisposable {
        private static AudioManager s_instance;
        public static AudioManager Instance => s_instance;
        private readonly List<AudioSource> _activeAudioSources;
        private float _previousAmbientVolume;
        private float _previousSoundEffectVolume;
        public bool IsMuted { get; private set; }
        private bool _debugging = true;

        public float BgmVolume {
            get {
                if (IsMuted) {
                    return 0.0f;
                }

                return MediaPlayer.Volume;
            }
            set {
                if (IsMuted) {
                    return;
                }

                MediaPlayer.Volume = Math.Clamp(value, 0.0f, 1.0f);
            }
        }

        public float SoundEffectVolume {
            get {
                if (IsMuted) {
                    return 0.0f;
                }

                return SoundEffect.MasterVolume;
            }
            set {
                if (IsMuted) {
                    return;
                }

                SoundEffect.MasterVolume = Math.Clamp(value, 0.0f, 1.0f);
            }
        }

        public bool IsDisposed { get; private set; }

        private SoundEffect[] _steps;
        private int _stepOrder;

        public AudioManager() {
            if (s_instance != null) {
                throw new InvalidOperationException($"Only a single AudioManager instance can be created");
            }
            s_instance = this;

            _activeAudioSources = new List<AudioSource>();

            //Load Sound Effect
            _steps = new SoundEffect[8];
            for(int i = 1; i < 9; i++) {
                _steps[i-1] = MainGame.Content.Load<SoundEffect>($"Audio/sfx_step_0{i}");
            }
        }

        ~AudioManager() => Dispose(false);

        public void Update(Player player) {
            for (int i = _activeAudioSources.Count - 1; i >= 0; i--) {
                AudioSource instance = _activeAudioSources[i];
                instance.Update();

                if (instance.Sound.State == SoundState.Stopped) {
                    if (!instance.IsDisposed) {
                        instance.Dispose();
                    }
                    _activeAudioSources.RemoveAt(i);
                }

                if (instance.Tag == "AudioSource3D") {
                    instance.UpdateSpatialAudio(player);
                }
            }
        }

        public void Draw() {
            if (_debugging) {
                foreach(AudioSource instance in _activeAudioSources) {
                    instance.Draw();
                }
            }
        }

        public void PlayStepsSFX(Vector2 pos) {
            AudioSource instance = new(pos,10,80);
            _activeAudioSources.Add(instance);

            if (_stepOrder >= _steps.Length) {
                _stepOrder = 0;
            }

            instance.PlayOneShot(_steps[_stepOrder++]);
        }

        public void PlayBgm(Song bgm, bool isRepeating = true) {
            if (MediaPlayer.State == MediaState.Playing) {
                MediaPlayer.Stop();
            }

            MediaPlayer.Play(bgm);
            MediaPlayer.IsRepeating = isRepeating;
        }

        public void PauseAudio() {
            // Pause any active songs playing.
            MediaPlayer.Pause();

            // Pause any active sound effects.
            foreach (AudioSource audioSource in _activeAudioSources) {
                audioSource.Sound.Pause();
            }
        }

        public void ResumeAudio() {
            // Resume paused music
            MediaPlayer.Resume();

            // Resume any active sound effects.
            foreach (AudioSource audioSource in _activeAudioSources) {
                audioSource.Sound.Resume();
            }
        }

        public void MuteAudio() {
            // Store the volume so they can be restored during UnmuteAudio
            _previousAmbientVolume = MediaPlayer.Volume;
            _previousSoundEffectVolume = SoundEffect.MasterVolume;

            // Set all volumes to 0
            MediaPlayer.Volume = 0.0f;
            SoundEffect.MasterVolume = 0.0f;

            IsMuted = true;
        }

        public void UnmuteAudio() {
            // Restore the previous volume values.
            MediaPlayer.Volume = _previousAmbientVolume;
            SoundEffect.MasterVolume = _previousSoundEffectVolume;

            IsMuted = false;
        }

        public void ToggleMute() {
            if (IsMuted) {
                UnmuteAudio();
            }
            else {
                MuteAudio();
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }

            if (disposing) {
                foreach (AudioSource audioSource in _activeAudioSources) {
                    audioSource.Dispose();
                }
                _activeAudioSources.Clear();
            }

            IsDisposed = true;
        }
    }
}
