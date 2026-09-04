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

namespace Imaginophonia {
    public class AudioSource : GameObject,IDisposable {
        public SoundEffectInstance Sound { get; private set; }
        private AudioEmitter _emitter;

        private float _duration;
        private float _innerRadius;
        private float _outerRadius;
        const float SPEED_OF_SOUND = 343.5f;

        public bool IsDisposed { get; private set; }

        public AudioSource(Vector2 pos) : base ("AudioSource", "AudioSource") {
            _emitter = new();
            _emitter.Position = new Vector3(pos, 0);
        }
        public AudioSource(Vector2 pos, float minDist, float maxDist) : base("AudioSource", "AudioSource3D") {
            _innerRadius = minDist;
            _outerRadius = maxDist;

            _emitter = new();
            _emitter.Position = new Vector3(pos, 0);
        }

        public override void Update() {
            base.Update();

            _emitter.Position = new Vector3(Transform.Position.X, Transform.Position.Y, 0);
        }

        public override void Draw() {
            MainGame.SpriteBatch.DrawCircle(Transform.Position, _innerRadius, 120,Color.LightYellow);
            MainGame.SpriteBatch.DrawCircle(Transform.Position, _outerRadius, 120, Color.LightYellow);


            base.Draw();
        }

        public void PlayLoopSFX(SoundEffect soundEffect, float volume = 1) {
            Sound = soundEffect.CreateInstance();
            _duration = (float)soundEffect.Duration.TotalSeconds;
            Sound.IsLooped = true;
            Sound.Play();
        }

        public void PlayOneShot(SoundEffect soundEffect, float volume = 1) {
            Sound = soundEffect.CreateInstance();
            _duration = (float)soundEffect.Duration.TotalSeconds;
            Sound.Play();
        }

        public void UpdateSpatialAudio(IMoveable listener) {
            IMoveable emitter = (IMoveable)Parent;

            Vector2 directionFromListener = new Vector2(_emitter.Position.X - Player.Listener.Position.X, _emitter.Position.Y - Player.Listener.Position.Y);
            if (directionFromListener != Vector2.Zero && true) {
                directionFromListener.Normalize();
                Sound.Pan = MathHelper.Clamp(directionFromListener.X, -1f, 1f);
            }
            //CircleF.Contains(new CircleF(Transform.Position, _innerRadius), new Vector2(Player.Listener.Position.X, Player.Listener.Position.Y)
            else if (directionFromListener == Vector2.Zero || ((Player.Listener.Position.X >= (_emitter.Position.X - _innerRadius)) && (Player.Listener.Position.X <= (_emitter.Position.X + _innerRadius)))) {
                Sound.Pan = 0;
            }
            float vEmitter = Vector2.Dot(emitter.Velocity, directionFromListener);
            float vListener = Vector2.Dot(listener.Velocity, directionFromListener);
            float pitchFactor = (SPEED_OF_SOUND + vListener) / (SPEED_OF_SOUND + vEmitter);
            Sound.Pitch = MathHelper.Clamp(pitchFactor - 1f, -1f, 1f);
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
                Sound?.Dispose();
            }

            IsDisposed = true;
        }
    }
}
