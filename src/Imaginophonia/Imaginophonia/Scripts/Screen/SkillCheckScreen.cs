using Gum.Forms.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using Penumbra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class SkillCheckScreen : Screen {
        private Texture2D _needle;
        private Vector2 _needlePos = new Vector2(960, 540-60);
        private Vector2 _needleOrigin;
        private Vector2 _centerOrigin = new Vector2(960,540);
        private Texture2D _fillZone;
        private Vector2 _fillZoneOrigin;
        private float _rotationSpeed = 200f;
        private float _needleAngle;
        private float _innerFillZoneAngle;
        private int _outerStartAngle = 5;
        private int _outerSweepAngle = 80;
        private float _outer1FillZoneAngle;
        private float _outer2FillZoneAngle;
        private float _outer3FillZoneAngle;
        private float _outer4FillZoneAngle;
        private float _outerNeedleAngle;
        private float _outerNeedleStartAngle;
        private int _fillZoneSweepAngle = 20;
        private int _needleSweepAngle = 3;
        private float _progressValue;
        private bool _innerActive;

        public bool SkillCheckIsActive;
        public SkillCheckScreen() {
            _fillZone = MainGame.Content.Load<Texture2D>("UI/Radial Circle");
            _needle = MainGame.Content.Load<Texture2D>("UI/Needle");
            _fillZoneOrigin = new Vector2(_fillZone.Width / 2f, _fillZone.Height / 2f);
            _needleOrigin = new Vector2(_needle.Width / 2f, _needle.Height / 2f);
            StartSkillCheck();
        }

        public override void Update(GameTime gameTime) {
            if (SkillCheckIsActive) {
                if (_innerActive) {
                    _needleAngle += _rotationSpeed * Time.DeltaTime;
                    float degreePerFrame = _rotationSpeed * Time.DeltaTime;
                    _needlePos.RotateAround(_centerOrigin, MathHelper.ToRadians(degreePerFrame));
                }
                else {
                    _outerNeedleAngle += _rotationSpeed * Time.DeltaTime;
                    if(_outerNeedleAngle <= _outerNeedleStartAngle || _outerNeedleAngle >= _outerNeedleStartAngle + _outerSweepAngle - _needleSweepAngle) {
                        if (_outerNeedleAngle < _outerNeedleStartAngle) {
                            _outerNeedleAngle = _outerNeedleStartAngle;
                        }
                        else if (_outerNeedleAngle > _outerNeedleStartAngle + _outerSweepAngle - _needleSweepAngle) {
                            _outerNeedleAngle = _outerNeedleStartAngle + _outerSweepAngle - _needleSweepAngle;
                        }
                        _rotationSpeed *= -1f;
                    }
                }
                //_progressBar.Value += 0.05f * Time.DeltaTime;

                if (KeyboardExtended.GetState().WasKeyPressed(Keys.Space)) {
                    EvaluateInput();
                }

                //if (_progressBar.Value == 1f) {
                    //playerActions.PlayRepairCompleteSound(playerCollision.obj);
                   // skillCheckIsActive = false;
                    //skillCheckUI.SetActive(false);
                    //playerActions.allowMovement = true;
                    //playerCollision.readyToRepair = false;
                    //ObjectiveManager.repairedObjectCount++;
                //}
                if(_progressValue == 200 || KeyboardExtended.GetState().WasKeyPressed(Keys.Q)) {
                    Time.AddTimer(this.ScreenManager.CloseScreen, 0.5f);
                    SkillCheckIsActive = false;
                }
            }
        }

        public override void Draw(GameTime gameTime) {
            var center = new Vector2((MainGame.GraphicsDevice.Viewport.Width/2f) - _fillZone.Width/2f, (MainGame.GraphicsDevice.Viewport.Height/2f) - _fillZone.Height / 2f);
            MainGame.SpriteBatch.Begin();
            BitmapFont _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            MainGame.SpriteBatch.DrawString(_font, $"Fill Angle {_innerFillZoneAngle}", new Vector2(150, 100), Color.White);
            MainGame.SpriteBatch.DrawString(_font, $"Needle Angle {_needleAngle}", new Vector2(150, 150), Color.White);
            MainGame.SpriteBatch.DrawString(_font, $"Outer Start Angle: {_outerNeedleStartAngle}/m Needle Angle: {_outerNeedleAngle}", new Vector2(150, 200), Color.White);

            MainGame.SpriteBatch.Draw(_fillZone, _centerOrigin, null, Color.White, MathHelper.ToRadians(_innerFillZoneAngle), _fillZoneOrigin, 1, SpriteEffects.None, 0);
            MainGame.SpriteBatch.Draw(_needle, _needlePos, null, Color.White, MathHelper.ToRadians(_needleAngle), _needleOrigin, 1, SpriteEffects.None, 0);

            Color color = Color.Black;
            if (!_innerActive) {
                color = Color.Black;
            }
            else {
                color = Color.Gray;
            }
            MainGame.SpriteBatch.DrawArc(_centerOrigin, 200, MathHelper.ToRadians(_outerStartAngle), MathHelper.ToRadians(_outerSweepAngle), 20, Color.White,10);
            MainGame.SpriteBatch.DrawArc(_centerOrigin, 200, MathHelper.ToRadians(_outer1FillZoneAngle), MathHelper.ToRadians(_fillZoneSweepAngle), 20, color, 10.5f);
            MainGame.SpriteBatch.DrawArc(_centerOrigin, 200, MathHelper.ToRadians(_outerStartAngle + 90), MathHelper.ToRadians(_outerSweepAngle), 20, Color.White, 10);
            MainGame.SpriteBatch.DrawArc(_centerOrigin, 200, MathHelper.ToRadians(_outer2FillZoneAngle), MathHelper.ToRadians(_fillZoneSweepAngle), 20, color, 10.5f);
            MainGame.SpriteBatch.DrawArc(_centerOrigin, 200, MathHelper.ToRadians(_outerStartAngle + 180), MathHelper.ToRadians(_outerSweepAngle), 20, Color.White, 10);
            MainGame.SpriteBatch.DrawArc(_centerOrigin, 200, MathHelper.ToRadians(_outer3FillZoneAngle), MathHelper.ToRadians(_fillZoneSweepAngle), 20, color, 10.5f);
            MainGame.SpriteBatch.DrawArc(_centerOrigin, 200, MathHelper.ToRadians(_outerStartAngle + 270), MathHelper.ToRadians(_outerSweepAngle), 20, Color.White, 10);
            MainGame.SpriteBatch.DrawArc(_centerOrigin, 200, MathHelper.ToRadians(_outer4FillZoneAngle), MathHelper.ToRadians(_fillZoneSweepAngle), 20, color, 10.5f);
            MainGame.SpriteBatch.FillRectangle(860, 800, 200, 15, Color.DarkGray);
            MainGame.SpriteBatch.FillRectangle(860, 800, _progressValue, 15, Color.GreenYellow);

            if (!_innerActive) MainGame.SpriteBatch.DrawArc(_centerOrigin, 210, MathHelper.ToRadians(_outerNeedleAngle), MathHelper.ToRadians(_needleSweepAngle), 20, Color.Red, 30f);
            MainGame.SpriteBatch.End();
        }
    
        public void StartSkillCheck() {
            _progressValue = 0f;
            SkillCheckIsActive = true;
            _needleAngle = 0f;
            _innerActive = true;
            RandomZone();
        }

        private void RandomZone() {
            Random rand = new Random();
            if (_innerActive) {
                _innerFillZoneAngle = rand.Next(0, 360);
            }
            else {
                _outer1FillZoneAngle = rand.Next(_outerStartAngle + 0, _outerSweepAngle + _outerStartAngle + 0 - _fillZoneSweepAngle);
                _outer2FillZoneAngle = rand.Next(_outerStartAngle + 90, _outerSweepAngle + _outerStartAngle + 90 - _fillZoneSweepAngle);
                _outer3FillZoneAngle = rand.Next(_outerStartAngle + 180, _outerSweepAngle + _outerStartAngle + 180 - _fillZoneSweepAngle);
                _outer4FillZoneAngle = rand.Next(_outerStartAngle + 270, _outerSweepAngle + _outerStartAngle + 270 - _fillZoneSweepAngle);
            }
            
            //_fillZone.rectTransform.rotation = Quaternion.Euler(0f, 0f, -_fillZoneStartAngle);
            //_fillZone.fillAmount = 45f / 360f;
        }

        private void EvaluateInput() {
            Random rand = new Random();
            float normalizedAngle = _needleAngle % 360f;
            if (_needleAngle < 0f) normalizedAngle += 360f;
            if (_innerActive) {
                if (normalizedAngle >= _innerFillZoneAngle && normalizedAngle <= _innerFillZoneAngle + 45) {
                    //^^^^^change these if decided to swap to arc
                    //_outerNeedleAngle = normalizedAngle
                    //_progressBar.Value += 0.2f;
                    //if (playerCollision.obj == "Pipe") {
                    //    playerActions.PlayPipeSkillCheckSound(true);
                    //}
                    //else {
                    //    playerActions.PlayElecSkillCheckSound(true);
                    //}
                    if (normalizedAngle >= 0 && normalizedAngle < 90) {
                        _outerNeedleAngle = rand.Next(_outerStartAngle + 270, _outerSweepAngle + _outerStartAngle + 270 - _needleSweepAngle);
                        _outerNeedleStartAngle = _outerStartAngle + 270;
                    }
                    else if (normalizedAngle >= 90 && normalizedAngle < 180) {
                        _outerNeedleAngle = rand.Next(_outerStartAngle + 0, _outerSweepAngle + _outerStartAngle + 0 - _needleSweepAngle);
                        _outerNeedleStartAngle = _outerStartAngle + 0;
                    }
                    else if (normalizedAngle >= 180 && normalizedAngle < 270) {
                        _outerNeedleAngle = rand.Next(_outerStartAngle + 90, _outerSweepAngle + _outerStartAngle + 90 - _needleSweepAngle);
                        _outerNeedleStartAngle = _outerStartAngle + 90;
                    }
                    else if (normalizedAngle >= 270 && normalizedAngle < 360) {
                        _outerNeedleAngle = rand.Next(_outerStartAngle + 180, _outerSweepAngle + _outerStartAngle + 180 - _needleSweepAngle);
                        _outerNeedleStartAngle = _outerStartAngle + 180;
                    }
                    _innerActive = false;
                }
                else {
                    //inner wrong input
                    //_progressBar.Value -= 0.25f;
                    //if (playerCollision.obj == "Pipe") {
                    //    playerActions.PlayPipeSkillCheckSound(false);
                    //}
                    //else {
                    //    playerActions.PlayElecSkillCheckSound(false);
                    //}
                }
                
            }
            else {
                if ((_outerNeedleAngle >= _outer1FillZoneAngle && _outerNeedleAngle < _outer1FillZoneAngle + _fillZoneSweepAngle - _needleSweepAngle) ||
                       (_outerNeedleAngle >= _outer2FillZoneAngle && _outerNeedleAngle < _outer2FillZoneAngle + _fillZoneSweepAngle - _needleSweepAngle) ||
                       (_outerNeedleAngle >= _outer3FillZoneAngle && _outerNeedleAngle < _outer3FillZoneAngle + _fillZoneSweepAngle - _needleSweepAngle) ||
                       (_outerNeedleAngle >= _outer4FillZoneAngle && _outerNeedleAngle < _outer4FillZoneAngle + _fillZoneSweepAngle - _needleSweepAngle)) {
                    if(_progressValue <= 150) {
                        _progressValue += 50;
                    }
                }
                else {
                    if(_progressValue > 0) {
                        _progressValue -= 50;
                    }
                }
                _innerActive = true;
            }

            _rotationSpeed *= -1f;
            RandomZone();
        }
    }
}
