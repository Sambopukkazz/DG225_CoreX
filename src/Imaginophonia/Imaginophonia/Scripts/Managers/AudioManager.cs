using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Imaginophobia {
    public class AudioManager : IDisposable {
        private static AudioManager s_instance;
        public static AudioManager Instance => s_instance;
        private readonly List<AudioSource> _activeAudioSources;
        private float _previousAmbientVolume;
        private float _previousSoundEffectVolume;
        public bool IsMuted { get; private set; }
        private bool _debugging = true;

        public float AmbVolume {
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
        private Dictionary<string, SoundEffect> NameSFX;
        private Song[] _amb;

        public AudioManager() {
            if (s_instance != null) {
                throw new InvalidOperationException($"Only a single AudioManager instance can be created");
            }
            s_instance = this;

            _activeAudioSources = new List<AudioSource>();

            //Load Sound Effect
            NameSFX = new Dictionary<string, SoundEffect> {
                { "sfx_door_close", MainGame.Content.Load<SoundEffect>("Audio/sfx_door_close") },
                { "sfx_door_open", MainGame.Content.Load<SoundEffect>("Audio/sfx_door_open") },
                { "sfx_locker_close", MainGame.Content.Load<SoundEffect>("Audio/sfx_locker_close") },
                { "sfx_locker_open", MainGame.Content.Load<SoundEffect>("Audio/sfx_locker_open") },
                { "sfx_pipe_complete", MainGame.Content.Load<SoundEffect>("Audio/sfx_pipe_complete") },
                { "sfx_pipe_failed", MainGame.Content.Load<SoundEffect>("Audio/sfx_pipe_failed") },
                { "sfx_pipe_repairing", MainGame.Content.Load<SoundEffect>("Audio/sfx_pipe_repairing") },
                { "sfx_pipe_success", MainGame.Content.Load<SoundEffect>("Audio/sfx_pipe_success") },
                { "sfx_door_close", MainGame.Content.Load<SoundEffect>("Audio/sfx_door_close") }
            };


            _steps = new SoundEffect[8];

            for(int i = 1; i < 9; i++) {
                _steps[i-1] = MainGame.Content.Load<SoundEffect>($"Audio/sfx_step_0{i}");
            }

            _amb = new Song[2];
            _amb[0] = MainGame.Content.Load<Song>($"Audio/amb_electricroom");
            _amb[1] = MainGame.Content.Load<Song>($"Audio/amb_sewer");

            tempFx[0] = 
            tempFx[1] = MainGame.Content.Load<SoundEffect>($);
            tempFx[2] = MainGame.Content.Load<SoundEffect>($"Audio/sfx_locker_close");
            tempFx[3] = MainGame.Content.Load<SoundEffect>($"Audio/sfx_locker_open");
            tempFx[4] = MainGame.Content.Load<SoundEffect>($"Audio/sfx_pipe_complete");
            tempFx[5] = MainGame.Content.Load<SoundEffect>($"Audio/sfx_pipe_failed");
            tempFx[6] = MainGame.Content.Load<SoundEffect>($"Audio/sfx_pipe_repairing");
            tempFx[7] = MainGame.Content.Load<SoundEffect>($"Audio/sfx_pipe_success");
            tempFx[8] = MainGame.Content.Load<SoundEffect>($"Audio/sfx_scopophobia_indicator");
        }

        ~AudioManager() => Dispose(false);

        public void Update(Player player) {
            for (int i = _activeAudioSources.Count - 1; i >= 0; i--) {
                AudioSource audioSource = _activeAudioSources[i];
                audioSource.Update();

                if (audioSource.Sound.State == SoundState.Stopped || audioSource.Active == false) {
                    if (!audioSource.IsDisposed) {
                        audioSource.Dispose();
                    }
                    _activeAudioSources.RemoveAt(i);
                }

                if (audioSource.Tag == "AudioSource3D") {
                    audioSource.UpdateSpatialAudio(player);
                }
            }
        }

        public void Draw() {
            if (_debugging) {
                //BitmapFont font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
                foreach (AudioSource audioSource in _activeAudioSources) {
                    audioSource.Draw();
                    //MainGame.SpriteBatch.DrawString(font, $"\nSource Pos:{audioSource.Transform.LocalMatrix}", new Vector2(100, 500 + (40 * _activeAudioSources.IndexOf(audioSource))), Color.White);
                }
            }
        }

        public void PlaySFX(Vector2 pos, string name, bool repeat = false, string source = null) {
            AudioSource audioSource = new(pos);
            _activeAudioSources.Add(audioSource);

            SoundEffect soundEffect = tempFx[0];
            if (name == "locker-open") {
                soundEffect = tempFx[3];
            }
            else if (name == "locker-close") {
                soundEffect = tempFx[2];
            }
            else if (name == "door-close") {
                soundEffect = tempFx[1];
            }
            else if (name == "door-close") {
                soundEffect = tempFx[0];
            }

            if (repeat) {
                audioSource.PlayLoop(soundEffect);
            }
            else {
                audioSource.PlayOneShot(soundEffect);
            }
        }

        public AudioSource Play3DSFX(Vector2 pos, string name, float innerRadius = 10, float outerRadius = 80) {
            AudioSource audioSource = new(pos, innerRadius, outerRadius);
            _activeAudioSources.Add(audioSource);

            SoundEffect soundEffect = null;
            if (name == "scopophobia") {
                soundEffect = tempFx[8];
            }

            if (soundEffect != null) {
                audioSource.PlayOneShot(soundEffect);
            }

            return audioSource;
        }

        public void PlayStepsSFX(Vector2 pos, float innerRadius  = 10, float outerRadius = 80) {
            AudioSource instance = new(pos, innerRadius, outerRadius);
            _activeAudioSources.Add(instance);

            if (_stepOrder >= _steps.Length) {
                _stepOrder = 0;
            }

            instance.PlayOneShot(_steps[_stepOrder++]);
        }

        public void PlayAmbiance(string ambName) {
            if (MediaPlayer.State == MediaState.Playing) {
                MediaPlayer.Stop();
            }

            Song amb;
            if (ambName == "sewer") {
                amb = _amb[1];
            }
            else {
                amb = _amb[0];
            }
            AmbVolume = 0.2f;
            MediaPlayer.Play(amb);
            MediaPlayer.IsRepeating = true;
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
