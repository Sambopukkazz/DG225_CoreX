using Microsoft.Xna.Framework;
using System;

namespace Imaginophobia {
    // Where an eye is looking, plus the twitch. No shake: the pupil only moves because it
    // is gazing or jerking.
    public class EyeMotion {
        private readonly EyeShape _eye;
        private readonly float _pupilRadius;
        private readonly Random _random;

        private Vector2 _gazeOffset = Vector2.Zero;
        private Vector2 _gazeTarget = Vector2.Zero;
        private float _gazeSpeed = 0f;
        private Vector2 _twitchDirection = Vector2.UnitX;
        private float _twitchTimer = 0f;
        private float _twitchDuration = 1f;
        private float _twitchMagnitude = 0f;

        public Vector2 PupilPosition { get; private set; }
        public bool IsTwitching => _twitchTimer > 0f;

        public EyeMotion(EyeShape eye, float pupilRadius, Random random) {
            _eye = eye;
            _pupilRadius = pupilRadius;
            _random = random;
            PupilPosition = eye.Center;
        }

        // Offset from the eye centre the pupil is trying to sit at, chased at a speed in
        // pixels per second. Zero is straight ahead.
        public void SetGazeOffset(Vector2 offset, float speed) {
            _gazeTarget = offset;
            _gazeSpeed = speed;
        }

        // A jerk in a set direction, so a whole group of eyes can flinch together.
        public void Twitch(Vector2 direction, float magnitude, float duration) {
            _twitchDirection = direction;
            _twitchMagnitude = magnitude;
            _twitchDuration = duration;
            _twitchTimer = duration;
        }

        public void Twitch(float magnitude, float duration) {
            float angle = (float)(_random.NextDouble() * Math.PI * 2.0);
            Twitch(new Vector2(MathF.Cos(angle), MathF.Sin(angle)), magnitude, duration);
        }

        public void CancelTwitch() {
            _twitchTimer = 0f;
        }

        public void Update() {
            Vector2 twitch = Vector2.Zero;

            if (_twitchTimer > 0f) {
                _twitchTimer -= Time.DeltaTime;
                float remaining = MathHelper.Clamp(_twitchTimer / _twitchDuration, 0f, 1f);
                twitch = _twitchDirection * _twitchMagnitude * (remaining * remaining);
            }

            // The pupil creeps toward the look instead of snapping onto it.
            float step = _gazeSpeed * Time.DeltaTime;
            Vector2 toTarget = _gazeTarget - _gazeOffset;
            float distance = toTarget.Length();

            if (distance <= step) {
                _gazeOffset = _gazeTarget;
            }
            else {
                _gazeOffset += toTarget / distance * step;
            }

            // Where the pupil wants to be, and where it is allowed to be.
            PupilPosition = _eye.ClampPupil(_eye.Center + _gazeOffset + twitch, _pupilRadius);
        }
    }
}
