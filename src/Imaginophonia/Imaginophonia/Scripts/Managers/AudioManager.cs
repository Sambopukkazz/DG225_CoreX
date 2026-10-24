using FlatRedBall.Glue.StateInterpolation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Content;
using MonoSound;
using MonoSound.Default;
using MonoSound.Filters;
using MonoSound.Streaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Imaginophobia {
    public class AudioManager : IDisposable {
        private static AudioManager s_instance;
        public static AudioManager Instance => s_instance;
        private readonly List<AudioSource> _activeAudioSources;
        private float _previousAmbientVolume;
        private float _previousSoundEffectVolume;
        public bool IsMuted { get; private set; }
        private bool _debugging = true;

        public float BgsVolume {
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
        private Dictionary<SoundEffectName, SoundEffect> SfxByName;
        private Dictionary<BackgroundSound, Song> BgsByName;
        private Random _rand;

        public AudioManager() {
            if (s_instance != null) {
                throw new InvalidOperationException($"Only a single AudioManager instance can be created");
            }
            s_instance = this;

            ContentManager content = MainGame.Content;

            MonoSoundLibrary.Init(MainGame.Instance);

            _activeAudioSources = new List<AudioSource>();

            _rand = new Random();

            //Load Sound Effect
            SfxByName = new Dictionary<SoundEffectName, SoundEffect> {
                { SoundEffectName.DoorClose, content.Load<SoundEffect>("Audio/sfx_door_close") },
                { SoundEffectName.DoorOpen, content.Load<SoundEffect>("Audio/sfx_door_open") },
                { SoundEffectName.DoorStuck, content.Load<SoundEffect>("Audio/sfx_door_stuck") },
                { SoundEffectName.LockerClose, content.Load<SoundEffect>("Audio/sfx_locker_close") },
                { SoundEffectName.LockerOpen, content.Load<SoundEffect>("Audio/sfx_locker_open") },
                { SoundEffectName.PipeComplete, content.Load<SoundEffect>("Audio/sfx_pipe_complete") },
                { SoundEffectName.PipeFail, content.Load<SoundEffect>("Audio/sfx_pipe_failed") },
                { SoundEffectName.PipeRepairing, content.Load<SoundEffect>("Audio/sfx_pipe_repairing") },
                { SoundEffectName.PipeSuccess, content.Load<SoundEffect>("Audio/sfx_pipe_success") },
                { SoundEffectName.ElecComplete, content.Load<SoundEffect>("Audio/sfx_electric_complete") },
                { SoundEffectName.ElecFail, content.Load<SoundEffect>("Audio/sfx_electric_failed") },
                { SoundEffectName.ElecRepairing, content.Load<SoundEffect>("Audio/sfx_electric_repairing") },
                { SoundEffectName.ElecSuccess, content.Load<SoundEffect>("Audio/sfx_electric_success") },
                { SoundEffectName.ScopoIndicator, content.Load<SoundEffect>("Audio/sfx_scopophobia_indicator") },
                { SoundEffectName.PhobiaHit, content.Load<SoundEffect>("Audio/sfx_phobias_hit") },
                { SoundEffectName.HeartBeat, content.Load<SoundEffect>("Audio/sfx_heartbeat") },
                { SoundEffectName.Painting, content.Load<SoundEffect>("Audio/sfx_panting") },
                { SoundEffectName.LightBreak, content.Load<SoundEffect>("Audio/sfx_light_break") },
                { SoundEffectName.Darkness, content.Load<SoundEffect>("Audio/sfx_darkness") },
                { SoundEffectName.ScreenOff, content.Load<SoundEffect>("Audio/sfx_screen_off") },
                { SoundEffectName.ScreenOn, content.Load<SoundEffect>("Audio/sfx_screen_on") },
                { SoundEffectName.FlashlightToggle, content.Load<SoundEffect>("Audio/sfx_flashlight_toggle") },
                { SoundEffectName.LightFlickerLong, content.Load<SoundEffect>("Audio/sfx_light_flicker_long") },
                { SoundEffectName.LightFlickerShort1, content.Load<SoundEffect>("Audio/sfx_light_flicker_short_01") },
                { SoundEffectName.LightFlickerShort2, content.Load<SoundEffect>("Audio/sfx_light_flicker_short_02") },
                { SoundEffectName.LightFlickerShort3, content.Load<SoundEffect>("Audio/sfx_light_flicker_short_03") }
            };

            //int MyBQR = FilterLoader.RegisterBiquadResonantFilter(SoundFilterType.LowPass, strength: 1f, frequency: 2000, resonance: 15);
            //int MyBQR = FilterLoader.RegisterBiquadResonantFilter(SoundFilterType.BandPass, strength: 1f, frequency: 2000, resonance: 15);
            //int MyBQR = FilterLoader.RegisterBiquadResonantFilter(SoundFilterType.HighPass, strength: 1f, frequency: 2000, resonance: 15);

            //int MyEcho = FilterLoader.RegisterEchoFilter(strength: 1f, delay: 0.3f, decay: 70, bias: 0);
            //int MyReverb = FilterLoader.RegisterReverbFilter(strength: 1f, feedback: 0.5, dampness: 0.5 ,stereoWidth: 0);

            //Multiple effect
            //{ Sound.DoorClose, EffectLoader.GetMultiFilteredEffect("Content/Audio/sfx_door_close.ogg", MyBQR, /*MyEffect2*/, /*MyEffect3*/) }

            //NameSFX = new Dictionary<Sound, SoundEffect> {
            //    { Sound.DoorClose, EffectLoader.GetMultiFilteredEffect("Content/Audio/sfx_door_close.ogg", MyBQR) },
            //    { Sound.DoorOpen, EffectLoader.GetMultiFilteredEffect("Content/Audio/sfx_door_open.ogg", MyBQR) },
            //    { Sound.DoorStuck, EffectLoader.GetMultiFilteredEffect("Content/Audio/sfx_door_stuck.ogg", MyBQR) },
            //    { Sound.LockerClose, EffectLoader.GetMultiFilteredEffect("Content/Audio/sfx_locker_close.ogg", MyBQR) },
            //    { Sound.LockerOpen, EffectLoader.GetMultiFilteredEffect("Content/Audio/sfx_locker_open.ogg", MyBQR) },
            //    { Sound.PipeComplete, EffectLoader.GetMultiFilteredEffect("Content/Audio/sfx_pipe_complete.ogg", MyBQR) },
            //    { Sound.PipeFail, EffectLoader.GetMultiFilteredEffect("Content/Audio/sfx_pipe_failed.ogg", MyBQR) },
            //    { Sound.PipeRepairing, EffectLoader.GetMultiFilteredEffect("Content/Audio/sfx_pipe_repairing.ogg", MyBQR) },
            //    { Sound.PipeSuccess, EffectLoader.GetMultiFilteredEffect("Content/Audio/sfx_pipe_success.ogg", MyBQR) },
            //    { Sound.ElecComplete, EffectLoader.GetMultiFilteredEffect("Content/Audio/sfx_electric_complete.ogg", MyBQR) },
            //    { Sound.ElecFail, EffectLoader.GetMultiFilteredEffect("Content/Audio/sfx_electric_failed.ogg", MyBQR) },
            //    { Sound.ElecRepairing, EffectLoader.GetMultiFilteredEffect("Content/Audio/sfx_electric_repairing.ogg",MyBQR) },
            //    { Sound.ElecSuccess, EffectLoader.GetMultiFilteredEffect("Content/Audio/sfx_electric_success.ogg",MyBQR) },
            //    { Sound.ScopoIndicator, EffectLoader.GetMultiFilteredEffect("Content/Audio/sfx_scopophobia_indicator.ogg",MyBQR) },
            //    { Sound.AutoIndicator, EffectLoader.GetMultiFilteredEffect("Content/Audio/sfx_autophobia_indicator.ogg",MyBQR) }
            //};

            _steps = new SoundEffect[8];

            for(int i = 1; i < 9; i++) {
                _steps[i-1] = content.Load<SoundEffect>($"Audio/sfx_step_0{i}");
                //_steps[i - 1] = EffectLoader.GetMultiFilteredEffect($"Content/Audio/sfx_step_0{i}.xnb", MyBQR);
            }

            BgsByName = new Dictionary<BackgroundSound, Song> {
                { BackgroundSound.AmbElectric, content.Load<Song>("Audio/amb_electricroom") },
                { BackgroundSound.AmbSewer, content.Load<Song>("Audio/amb_sewer") },
                { BackgroundSound.BgmMainMenu, content.Load<Song>("Audio/bgm_mainmenu_screen") },
                { BackgroundSound.BgmDeathScreen, content.Load<Song>("Audio/bgm_death_screen") },
                { BackgroundSound.BgmWinScreen, content.Load<Song>("Audio/bgm_win_screen") }
            };
            
            BgsVolume = 0.2f;
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
            if (_debugging == false) {
                return;
            }
            //BitmapFont font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            foreach (AudioSource audioSource in _activeAudioSources) {
                audioSource.Draw();
                //MainGame.SpriteBatch.DrawString(font, $"\nSource Pos:{audioSource.Transform.LocalMatrix}", new Vector2(100, 500 + (40 * _activeAudioSources.IndexOf(audioSource))), Color.White);
            }
        }

        public void PlaySFX(SoundEffectName name, bool repeat = false, bool playAfterLastCompleted = false, float volume = 1) {
            if (playAfterLastCompleted == true && _activeAudioSources.Exists(audio => audio.Name == name.ToString()) == true) {
                return;
            }

            AudioSource audioSource = new AudioSource(name);
            _activeAudioSources.Add(audioSource);

            SoundEffect soundEffect = SfxByName[name];
            
            if (repeat) {
                audioSource.PlayLoop(soundEffect, volume);
            }
            else {
                audioSource.PlayOneShot(soundEffect, volume);
            }
        }

        public AudioSource Play3DSFX(Vector2 pos, SoundEffectName name, float innerRadius = 10, float outerRadius = 80, float volume = 1) {
            AudioSource audioSource = new(pos, innerRadius, outerRadius);
            _activeAudioSources.Add(audioSource);

            SoundEffect soundEffect = SfxByName[name];

            if (soundEffect != null) {
                audioSource.PlayOneShot(soundEffect, volume);
            }

            return audioSource;
        }

        public void PlayBackgroundSound(BackgroundSound bgsName) {
            if (MediaPlayer.State == MediaState.Playing) {
                MediaPlayer.Stop();
            }

            Song bgs = BgsByName[bgsName];
            MediaPlayer.Play(bgs);
            MediaPlayer.IsRepeating = true;
        }

        public void PlayStepsSFX(Vector2 pos, float innerRadius = 10, float outerRadius = 80) {
            AudioSource audioSource = new(pos, innerRadius, outerRadius);
            _activeAudioSources.Add(audioSource);

            if (_stepOrder >= _steps.Length) {
                _stepOrder = 0;
            }

            audioSource.PlayOneShot(_steps[_stepOrder++]);
        }

        public void PlayLightFlickerSFX(Vector2 pos, float innerRadius = 10, float outerRadius = 80) {
            AudioSource audioSource = new(pos, innerRadius, outerRadius);
            _activeAudioSources.Add(audioSource);
            
            SoundEffect soundEffect = null;
            switch(_rand.Next(0, 3)) {
                case 0:
                    soundEffect = SfxByName[SoundEffectName.LightFlickerShort1];
                    break;
                case 1:
                    soundEffect = SfxByName[SoundEffectName.LightFlickerShort2];
                    break;
                case 2:
                    soundEffect = SfxByName[SoundEffectName.LightFlickerShort3];
                    break;
            }

            audioSource.PlayOneShot(soundEffect, 0.15f);
        }

        public void StopSFX(SoundEffectName name) {
            if (_activeAudioSources.Exists(audio => audio.Name == name.ToString())) {
                foreach (var audioSource in _activeAudioSources.Where(audio => audio.Name == name.ToString())) {
                    audioSource.SetActive(false);
                }
            }
        }

        public void ClearSFXs() {
            foreach (var audioSource in _activeAudioSources) {
                audioSource.AudioPlayer.Stop();
                audioSource.SetActive(false);
            }
        }

        //public void ClearSFXs(params string[] soundEffectNames) {
        //    var test = _activeAudioSources.ExceptBy(soundEffectNames, p => p.Name);
        //    foreach (AudioSource audioSource in test)  {
        //        audioSource.AudioPlayer.Stop();
        //        audioSource.SetActive(false);
        //    }
        //}

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
