using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace Imaginophobia {
    public class CameraShaker {
        private Random _random = new Random();

        private float _shakeTimer;
        private float _shakeDuration;
        private float _shakeMagnitude;
        public Vector2 ShakeOffset { get; private set; }

        public void Shake(float duration, float magnitude) {
            _shakeDuration = duration;
            _shakeTimer = duration;
            _shakeMagnitude = magnitude;
        }

        public void Update(GameTime gameTime) {
            if (_shakeTimer > 0) {
                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _shakeTimer -= deltaTime;

                float intensity = MathHelper.Clamp(_shakeTimer / _shakeDuration, 0f, 1f);

                float offsetX = ((float)_random.NextDouble() * 2f - 1f);
                float offsetY = ((float)_random.NextDouble() * 2f - 1f);

                ShakeOffset = new Vector2(offsetX, offsetY) * _shakeMagnitude * intensity;
            }
            else {
                ShakeOffset = Vector2.Zero;
            }
        }
    }
}
