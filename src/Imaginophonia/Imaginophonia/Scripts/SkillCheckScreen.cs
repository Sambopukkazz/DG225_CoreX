using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using MonoGame.Extended;

namespace Imaginophobia {
    public class SkillCheckScreen : Screen {
        private Rectangle _needle;
        private Vector2 _needlePos;
        private Vector2 _needleOrigin;
        private Texture2D _fillZone;
        private Slider _progressBar;
        private float _rotationSpeed = 200f;
        private float _currentAngle = 0f;
        private float _fillZoneStartAngle = 0f;
        private float _fillZoneEndAngle = 0f;

        public bool skillCheckIsActive = false;
        public SkillCheckScreen() {

        }

        public override void Update(GameTime gameTime) {
            if(skillCheckIsActive) {
                _currentAngle += _rotationSpeed * Time.DeltaTime;
                _needlePos.RotateAround(_needleOrigin, -_currentAngle);
                _needle.Location = new Point(0, 0);
                _progressBar.Value += 0.05f * Time.DeltaTime;

                if (KeyboardExtended.GetState().WasKeyPressed(Keys.Space)) {
                    EvaluateInput();
                }

                if (_progressBar.Value == 1f) {
                    //playerActions.PlayRepairCompleteSound(playerCollision.obj);
                    skillCheckIsActive = false;
                    //skillCheckUI.SetActive(false);
                    //playerActions.allowMovement = true;
                    //playerCollision.readyToRepair = false;
                    //ObjectiveManager.repairedObjectCount++;
                }
            }
        }

        public override void Draw(GameTime gameTime) {
            var center = new Vector2((MainGame.GraphicsDevice.Viewport.Width/2f) - _fillZone.Width/2f, (MainGame.GraphicsDevice.Viewport.Height/2f) - _fillZone.Height / 2f);
            MainGame.SpriteBatch.Draw(_fillZone, center, null, Color.White, _fillZoneStartAngle, center, 1, SpriteEffects.None, 0);
        }

        public void StartSkillCheck() {
            _progressBar.Value = 0f;
            skillCheckIsActive = true;
            _currentAngle = 0f;
            RandomZone();
        }

        private void RandomZone() {
            Random rand = new Random();
            _fillZoneStartAngle = rand.Next(0,315);
            _fillZoneEndAngle = _fillZoneStartAngle + 45f;
            //_fillZone.rectTransform.rotation = Quaternion.Euler(0f, 0f, -_fillZoneStartAngle);
            //_fillZone.fillAmount = 45f / 360f;
        }

        private void EvaluateInput() {
            float needleAngle = _currentAngle % 360f;
            if (_currentAngle < 0f) needleAngle += 360f;

            if (needleAngle >= _fillZoneStartAngle && needleAngle <= _fillZoneEndAngle) {
                _progressBar.Value += 0.2f;
                //if (playerCollision.obj == "Pipe") {
                //    playerActions.PlayPipeSkillCheckSound(true);
                //}
                //else {
                //    playerActions.PlayElecSkillCheckSound(true);
                //}
            }
            else {
                _progressBar.Value -= 0.25f;
                //if (playerCollision.obj == "Pipe") {
                //    playerActions.PlayPipeSkillCheckSound(false);
                //}
                //else {
                //    playerActions.PlayElecSkillCheckSound(false);
                //}
            }

            _rotationSpeed *= -1f;
            RandomZone();
        }
    }
}
