using Autofac.Core;
using Microsoft.Xna.Framework.Audio;
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
        private float _innerBound;
        private float _outerBound;

        public bool IsDisposed { get; private set; }

        public AudioSource(SoundEffect soundEffect) {
            Sound = soundEffect.CreateInstance();
            _duration = (float)soundEffect.Duration.TotalSeconds;
        }
        public AudioSource(SoundEffect soundEffect, float volume, float pitch, float pan, bool isLooped) {
            Sound = soundEffect.CreateInstance();
            _duration = (float)soundEffect.Duration.TotalSeconds;
        }

        public override void Update() {
            
            base.Update();
        }

        public override void Draw() {
            
            base.Draw();
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
