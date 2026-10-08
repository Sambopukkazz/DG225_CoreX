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
        private Dictionary<Sound, SoundEffect> NameSFX;
        private Song[] _amb;

        public AudioManager() {
            if (s_instance != null) {
                throw new InvalidOperationException($"Only a single AudioManager instance can be created");
            }
            s_instance = this;

            _activeAudioSources = new List<AudioSource>();

            //Load Sound Effect
            NameSFX = new Dictionary<Sound, SoundEffect> {
                { Sound.DoorClose, MainGame.Content.Load<SoundEffect>("Audio/sfx_door_close") },
                { Sound.DoorOpen, MainGame.Content.Load<SoundEffect>("Audio/sfx_door_open") },
                { Sound.DoorStuck, MainGame.Content.Load<SoundEffect>("Audio/sfx_door_stuck") },
                { Sound.LockerClose, MainGame.Content.Load<SoundEffect>("Audio/sfx_locker_close") },
                { Sound.LockerOpen, MainGame.Content.Load<SoundEffect>("Audio/sfx_locker_open") },
                { Sound.PipeComplete, MainGame.Content.Load<SoundEffect>("Audio/sfx_pipe_complete") },
                { Sound.PipeFail, MainGame.Content.Load<SoundEffect>("Audio/sfx_pipe_failed") },
                { Sound.PipeRepairing, MainGame.Content.Load<SoundEffect>("Audio/sfx_pipe_repairing") },
                { Sound.PipeSuccess, MainGame.Content.Load<SoundEffect>("Audio/sfx_pipe_success") },
                { Sound.ElecComplete, MainGame.Content.Load<SoundEffect>("Audio/sfx_electric_complete") },
                { Sound.ElecFail, MainGame.Content.Load<SoundEffect>("Audio/sfx_electric_failed") },
                { Sound.ElecRepairing, MainGame.Content.Load<SoundEffect>("Audio/sfx_electric_repairing") },
                { Sound.ElecSuccess, MainGame.Content.Load<SoundEffect>("Audio/sfx_electric_success") },
                { Sound.ScopoIndicator, MainGame.Content.Load<SoundEffect>("Audio/sfx_scopophobia_indicator") },
                { Sound.AutoIndicator, MainGame.Content.Load<SoundEffect>("Audio/sfx_autophobia_indicator") }
            };

            _steps = new SoundEffect[8];

            for(int i = 1; i < 9; i++) {
                _steps[i-1] = MainGame.Content.Load<SoundEffect>($"Audio/sfx_step_0{i}");
            }

            _amb = new Song[2];
            _amb[0] = MainGame.Content.Load<Song>($"Audio/amb_electricroom");
            _amb[1] = MainGame.Content.Load<Song>($"Audio/amb_sewer");
        }

        ~AudioManager() => Dispose(false);

        public void Update(Player player) {
            for (int i = _activeAudioSources.Count - 1; i >= 0; i--) {
                AudioSource audioSource = _activeAudioSources[i];
                audioSource.Update();

                if (audioSource.AudioPlayer.State == SoundState.Stopped || audioSource.Active == false) {
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
            if (!_debugging) {
                return;
            }
            //BitmapFont font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            foreach (AudioSource audioSource in _activeAudioSources) {
                audioSource.Draw();
                //MainGame.SpriteBatch.DrawString(font, $"\nSource Pos:{audioSource.Transform.LocalMatrix}", new Vector2(100, 500 + (40 * _activeAudioSources.IndexOf(audioSource))), Color.White);
            }
        }

        public void PlaySFX(Sound name, bool repeat = false, bool playAfterLastCompleted = false) {
            if (playAfterLastCompleted == true && _activeAudioSources.Exists(audio => audio.Name == name.ToString()) == true) {
                return;
            }

            AudioSource audioSource = new AudioSource(name);
            _activeAudioSources.Add(audioSource);

            SoundEffect soundEffect = NameSFX[name];
            
            if (repeat) {
                audioSource.PlayLoop(soundEffect);
            }
            else {
                audioSource.PlayOneShot(soundEffect);
            }
        }

        public AudioSource Play3DSFX(Vector2 pos, Sound name, float innerRadius = 10, float outerRadius = 80) {
            AudioSource audioSource = new(pos, innerRadius, outerRadius);
            _activeAudioSources.Add(audioSource);

            SoundEffect soundEffect = NameSFX[name];

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

        public void StopSFX(Sound name) {
            if (_activeAudioSources.Exists(audio => audio.Name == name.ToString())) {
                foreach (var audioSource in _activeAudioSources.Where(audio => audio.Name == name.ToString())) {
                    audioSource.SetActive(false);
                }
            }
        }

        public void PauseAudio() {
            // Pause any active songs playing.
            MediaPlayer.Pause();

            // Pause any active sound effects.
            foreach (AudioSource audioSource in _activeAudioSources) {
                audioSource.AudioPlayer.Pause();
            }
        }

        public void ResumeAudio() {
            // Resume paused music
            MediaPlayer.Resume();

            // Resume any active sound effects.
            foreach (AudioSource audioSource in _activeAudioSources) {
                audioSource.AudioPlayer.Resume();
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
