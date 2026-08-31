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
        public readonly SoundEffectInstance Sound;
        private AudioEmitter _emitter;

        private float _duration;
        private float _innerRadius;
        private float _outerRadius;
        const float SPEED_OF_SOUND = 343.5f;

        public bool IsDisposed { get; private set; }

        public AudioSource(SoundEffect soundEffect) {
            Sound = soundEffect.CreateInstance();
            _duration = (float)soundEffect.Duration.TotalSeconds;
        }
        public AudioSource(SoundEffect soundEffect, float volume, float pitch, float pan, bool isLooped) {
            Sound = soundEffect.CreateInstance();
            _duration = (float)soundEffect.Duration.TotalSeconds;
            Sound.Volume = volume;
            Sound.Pitch = pitch;
            Sound.Pan = pan;
            Sound.IsLooped = isLooped;
            
        }

        public override void Update() {
            
            base.Update();
        }

        public override void Draw() {
            Game1.SpriteBatch.DrawCircle(Transform.Position, _innerRadius, 120,Color.LightYellow);
            Game1.SpriteBatch.DrawCircle(Transform.Position, _outerRadius, 120, Color.LightYellow);


            base.Draw();
        }

        public void UpdateSpatialAudio(IMoveable listener, IMoveable emitter) {
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
