using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Content.ContentReaders;
using Penumbra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class LightArea : ICollisionActor {
        public CollisionShape2D Shape { get; private set; }
        public int Id { get; }
        public string Tag { get; }
        public Light Light { get; }
        private Random _rand = new Random();
        public bool IsFlickering { get; private set; } = false;
        private float _flickerDurationTimer = 0f;
        private float _nextFlickerTimer = 0f;
        private float _flickerSpeed = 1;
        private float _baseIntensity;
        private float _areaSize = 600; // 5 tiles 120x120 per tile
        private bool _powered = true;
        public bool On => _powered;
        private bool _scopoAlarm = false;
        private float _scopoTimer = 0f;
        public bool ScopoAlarm => _scopoAlarm;

        public LightArea(Light light, Vector2 pos, int id, string tag) {
            Light = light;
            Id = id;
            Tag = tag;
            _baseIntensity = light.Intensity;

            Vector2 collisionPos = pos;
            collisionPos.X -= _areaSize/2f;

            Vector2 size = new Vector2(_areaSize, _areaSize);

            BoundingBox2D bounds = BoundingBox2D.CreateFromPositionAndSize(collisionPos, size);
            Shape = new CollisionShape2D(bounds);
        }

        public void Update() {
            if (_scopoAlarm) { 
                UpdateScopoAlarm(); 
                return; 
            }

            if (!IsFlickering) {
                return;
            }

            _flickerDurationTimer -= Time.DeltaTime;
            _nextFlickerTimer -= Time.DeltaTime;

            if (_flickerDurationTimer <= 0) {
                IsFlickering = false;
                Light.Intensity = _baseIntensity;
                Light.Enabled = true;
                return;
            }

            if (_nextFlickerTimer <= 0) {
                float randomFactor = (float)(_rand.NextDouble() * 0.9 + 0.1);
                Light.Intensity = _baseIntensity * randomFactor;
                //Light.Enabled = _rand.NextDouble() > 0.4;

                float baseInterval = (float)(_rand.NextDouble() * 0.09 + 0.03);
                _nextFlickerTimer = baseInterval / Math.Max(0.1f, _flickerSpeed);
            }
        }

        private void UpdateScopoAlarm() {
            _scopoTimer -= Time.DeltaTime;
            if (_scopoTimer <= 0f) {                 // done: on, red, permanent
                _scopoAlarm = false;
                Light.Color = Color.Red;            // no changing back
                Light.Intensity = _baseIntensity;
                Light.Enabled = true;
                return;
            }
            float elapsed = 8 - _scopoTimer;
            if (elapsed < 6) {
                _nextFlickerTimer -= Time.DeltaTime;
                if (_nextFlickerTimer <= 0f) {
                    Light.Enabled = _rand.NextDouble() > 0.4;               // <- your line 63
                    _nextFlickerTimer = (float)(_rand.NextDouble() * 0.09 + 0.03);
                }
            }
            else {
                Light.Enabled = false;               // 2s blackout
            }
        }

        public void StartScopoAlarm() {
            if (_powered == false) {
                return;
            }
            _scopoAlarm = true;
            _scopoTimer = 8f;
            IsFlickering = false;
            _powered = true;
            Light.Enabled = true;
            Light.Intensity = _baseIntensity;
            _nextFlickerTimer = 0f;
        }

        public void StartFlicker(float durationSeconds = 1.5f) {
            if (_scopoAlarm || !_powered || IsFlickering) {
                return;
            }
            IsFlickering = true;
            _flickerDurationTimer = durationSeconds;
            _nextFlickerTimer = 0f;
        }

        public void TurnOff() {
            _scopoAlarm = false;
            IsFlickering = false;
            Light.Intensity = _baseIntensity;
            _powered = false;
            Light.Enabled = false;
        }

        public void TurnOn() {
            _powered = true;
            Light.Enabled = true;
        }
    }
}
