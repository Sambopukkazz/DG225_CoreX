using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophonia {
    public class AudioManager : IDisposable {
        private static AudioManager s_instance;
        public static AudioManager Instance => s_instance;
        private readonly List<AudioSource> _activeAudioSources;
        private float _previousAmbientVolume;
        private float _previousSoundEffectVolume;
        public bool IsMuted { get; private set; }

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

        public void Update(IMoveable listener) {
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
                    instance.UpdateSpatialAudio(listener);
                }
            }
        }

        public void PlayStepsSFX(Vector2 pos) {
            AudioSource instance = new(pos);
            _activeAudioSources.Add(instance);

            if (_stepOrder >= _steps.Length) {
                _stepOrder = 0;
            }
            instance.PlayOneShot(_steps[_stepOrder++]);
        }

        //public void 

        //public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect) {
        //    return PlaySoundEffect(soundEffect, 1.0f, 0.0f, 0.0f, false);
        //}

        //public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect, float volume, float pitch, float pan, bool isLooped) {
        //    // Create an instance from the sound effect given.
        //    SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();

        //    // Apply the volume, pitch, pan, and loop values specified.
        //    soundEffectInstance.Volume = volume;
        //    soundEffectInstance.Pitch = pitch;
        //    soundEffectInstance.Pan = pan;
        //    soundEffectInstance.IsLooped = isLooped;

        //    // Tell the instance to play
        //    soundEffectInstance.Play();

        //    // Add it to the active instances for tracking
        //    _activeAudioSources.Add(soundEffectInstance);

        //    return soundEffectInstance;
        //}

        //public SoundEffectInstance Play3DSoundEffect(SoundEffect soundEffect) {
        //    //Create an instance from the sound effect given.
        //    SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();

        //    //Apply the volume, pitch, pan, and loop values specified.
        //    soundEffectInstance.Volume = 1f;
        //    soundEffectInstance.Pitch = 0f;
        //    soundEffectInstance.Pan = 0f;
        //    soundEffectInstance.IsLooped = true;

        //    AudioEmitter audioEmitter = new();
        //    AudioListener audioListener = new();
        //    audioListener.Position = new Vector3(0, 0, 0);
        //    audioEmitter.Position = Vector3.Right * 100;
        //    soundEffectInstance.Apply3D(audioListener, audioEmitter);

        //    //// Tell the instance to play
        //    soundEffectInstance.Play();

        //    //// Add it to the active instances for tracking
        //    _activeAudioSources.Add(soundEffectInstance);

        //    return soundEffectInstance;
        //}

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
