using Autofac.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class AudioSource : GameObject,IDisposable {
        public SoundEffectInstance AudioPlayer { get; private set; }
        private AudioEmitter _emitter;

        private float _duration;
        private float _innerRadius = 100;
        private float _outerRadius = 200;
        const float SPEED_OF_SOUND = 343.5f;

        public bool IsDisposed { get; private set; }

        public AudioSource(Sound name) : base (name.ToString(), "AudioSource") {
            Transform.Position = Vector2.Zero;
        }
        public AudioSource(Vector2 pos, float innerRadius, float outerRadius) : base("AudioSource", "AudioSource3D") {
            Transform.Position = pos;
            _innerRadius = innerRadius;
            _outerRadius = outerRadius;

            _emitter = new();
            _emitter.Position = new Vector3(pos, 0);
        }

        public override void Update() {
            base.Update();
            if (_emitter != null) {
                _emitter.Position = new Vector3(Transform.Position.X, Transform.Position.Y, 0);
            }
        }

        public override void Draw() {
            MainGame.SpriteBatch.DrawCircle(Transform.Position, _innerRadius, 120, Color.White);
            MainGame.SpriteBatch.DrawCircle(Transform.Position, _outerRadius, 120, Color.White);

            base.Draw();
        }

        public void PlayLoop(SoundEffect soundEffect, float volume = 1) {
            AudioPlayer = soundEffect.CreateInstance();
            _duration = (float)soundEffect.Duration.TotalSeconds;
            AudioPlayer.IsLooped = true;
            AudioPlayer.Play();
        }

        public void PlayOneShot(SoundEffect soundEffect, float volume = 1) {
            AudioPlayer = soundEffect.CreateInstance();
            _duration = (float)soundEffect.Duration.TotalSeconds;
            AudioPlayer.Play();
        }

        public void UpdateSpatialAudio(Player player) {
            IAudioApplicable emitter = (IAudioApplicable)Parent;

            float distance = Vector3.Distance(player.Listener.Position, _emitter.Position);

            float volume = 1f - MathHelper.Clamp((distance - _innerRadius) / (_outerRadius - _innerRadius), 0f, 1f);
            AudioPlayer.Volume = volume;

            Vector2 directionFromListener = new Vector2(_emitter.Position.X - player.Listener.Position.X, _emitter.Position.Y - player.Listener.Position.Y);
            
            //CircleF.Contains(new CircleF(Transform.Position, _innerRadius), new Vector2(Player.Listener.Position.X, Player.Listener.Position.Y)
            if (directionFromListener == Vector2.Zero || ((player.Listener.Position.X >= (_emitter.Position.X - _innerRadius)) && (player.Listener.Position.X <= (_emitter.Position.X + _innerRadius)))) {
                AudioPlayer.Pan = 0;
            }
            else if (directionFromListener != Vector2.Zero) {
                directionFromListener.Normalize();
                AudioPlayer.Pan = MathHelper.Clamp(directionFromListener.X, -1f, 1f);
            }

            float vEmitter = Vector2.Dot(Vector2.Zero, directionFromListener);
            if (emitter != null) {
                vEmitter = Vector2.Dot(emitter.Velocity, directionFromListener);
            }
            float vListener = Vector2.Dot(player.Velocity, directionFromListener) / 10f;
            float pitchFactor = (SPEED_OF_SOUND + vListener) / (SPEED_OF_SOUND + vEmitter);
            AudioPlayer.Pitch = MathHelper.Clamp(pitchFactor - 1f, -1f, 1f);
        }

        ~AudioSource() => Dispose(false);

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }

            if (disposing) {
                AudioPlayer?.Dispose();
            }

            IsDisposed = true;
        }
    }
}
