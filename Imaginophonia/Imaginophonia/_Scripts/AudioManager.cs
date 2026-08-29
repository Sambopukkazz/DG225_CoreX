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
        private readonly List<SoundEffectInstance> _activeSoundEffectInstances;
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

        public AudioManager() {
            _activeSoundEffectInstances = new List<SoundEffectInstance>();
        }

        ~AudioManager() => Dispose(false);

        public void Update() {
            for (int i = _activeSoundEffectInstances.Count - 1; i >= 0; i--) {
                SoundEffectInstance instance = _activeSoundEffectInstances[i];

                if (instance.State == SoundState.Stopped) {
                    if (!instance.IsDisposed) {
                        instance.Dispose();
                    }
                    _activeSoundEffectInstances.RemoveAt(i);
                }
            }
        }

        public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect) {
            return PlaySoundEffect(soundEffect, 1.0f, 0.0f, 0.0f, false);
        }

        public SoundEffectInstance PlaySoundEffect(SoundEffect soundEffect, float volume, float pitch, float pan, bool isLooped) {
            // Create an instance from the sound effect given.
            SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();

            // Apply the volume, pitch, pan, and loop values specified.
            soundEffectInstance.Volume = volume;
            soundEffectInstance.Pitch = pitch;
            soundEffectInstance.Pan = pan;
            soundEffectInstance.IsLooped = isLooped;

            // Tell the instance to play
            soundEffectInstance.Play();

            // Add it to the active instances for tracking
            _activeSoundEffectInstances.Add(soundEffectInstance);

            return soundEffectInstance;
        }

        public SoundEffectInstance Play3DSoundEffect(SoundEffect soundEffect) {
            // Create an instance from the sound effect given.
            SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();

            // Apply the volume, pitch, pan, and loop values specified.
            soundEffectInstance.Volume = 1f;
            soundEffectInstance.Pitch = 0f;
            soundEffectInstance.Pan = 0f;
            soundEffectInstance.IsLooped = true;

            AudioEmitter audioEmitter = new();
            AudioListener audioListener = new();
            audioListener.Position = new Vector3(0,0,0);
            audioEmitter.Position = Vector3.Right * 100;
            soundEffectInstance.Apply3D(audioListener,audioEmitter);

            // Tell the instance to play
            soundEffectInstance.Play();

            // Add it to the active instances for tracking
            _activeSoundEffectInstances.Add(soundEffectInstance);

            return soundEffectInstance;
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
            foreach (SoundEffectInstance soundEffectInstance in _activeSoundEffectInstances) {
                soundEffectInstance.Pause();
            }
        }

        public void ResumeAudio() {
            // Resume paused music
            MediaPlayer.Resume();

            // Resume any active sound effects.
            foreach (SoundEffectInstance soundEffectInstance in _activeSoundEffectInstances) {
                soundEffectInstance.Resume();
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
                foreach (SoundEffectInstance soundEffectInstance in _activeSoundEffectInstances) {
                    soundEffectInstance.Dispose();
                }
                _activeSoundEffectInstances.Clear();
            }

            IsDisposed = true;
        }
    }
}
