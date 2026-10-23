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
    public class SpinTheValveScreen : Screen {
        private Vector2 _centerOrigin = new Vector2(960,540);
        private Texture2D _valveTexture;
        private Vector2 _valveOrigin;
        private float _rotationSpeed = MathHelper.ToRadians(45);
        private float _valveAngle;
        private float _targetAngle;
        private float _mouseAngle;
        private float _lastMouseAngle;
        private bool _isSpinning;
        private float _needleAngle;
        private float _needleSpeed;
        private float _angularSpeed;
        private float _progressValue;
        private Player _player;
        private Trigger _trigger;
        private Random _rand;

        public bool MinigameActive;
        public bool MinigameVisible;
        public SpinTheValveScreen(Player player, Trigger trigger) {
            _valveTexture = MainGame.Content.Load<Texture2D>("UI/sprite_valve");
            _valveOrigin = new Vector2(_valveTexture.Width / 2f, _valveTexture.Height / 2f);
            MinigameActive = true;
            MinigameVisible = true;
            _player = player;
            _trigger = trigger;
            _progressValue = _trigger.Progress;
            _rand = new Random();

            RandomNeedleSpeed();
            //temp
            ObjectiveManager.Complete("valve-open");
        }

        public override void Update(GameTime gameTime) {
            if (MinigameActive) {
                CalculateInput();

                _valveAngle = MathHelper.Lerp(_valveAngle, _targetAngle, _rotationSpeed * Time.DeltaTime);
                _targetAngle = MathHelper.Lerp(_targetAngle, _valveAngle, _rotationSpeed * 2 * Time.DeltaTime);


                _needleAngle += (_needleSpeed / 38f) + (_angularSpeed / 100f);
                _needleAngle = MathHelper.Clamp(_needleAngle, MathHelper.ToRadians(225), MathHelper.ToRadians(313));

                if (_needleAngle > MathHelper.ToRadians(255) && _needleAngle < MathHelper.ToRadians(285)) {
                    _progressValue += 20f * Time.DeltaTime;
                }
                else if (_needleAngle >= MathHelper.ToRadians(285) && _needleAngle <= MathHelper.ToRadians(313)) {
                    _progressValue -= 35f * Time.DeltaTime;
                    AudioManager.Instance.PlaySFX(SoundEffectName.PipeFail, false, true);
                }
                else if (_needleAngle >= MathHelper.ToRadians(225) && _needleAngle <= MathHelper.ToRadians(255)) {
                    _progressValue -= 15f * Time.DeltaTime;
                    AudioManager.Instance.PlaySFX(SoundEffectName.PipeFail, false, true);
                }

                _progressValue = MathHelper.Clamp(_progressValue, 0, 200);

                if (_progressValue >= 200) {
                    MinigameActive = false;
                    AudioManager.Instance.PlaySFX(SoundEffectName.PipeComplete);
                    _player.ToggleRepair();
                    _trigger.Progress = _progressValue;
                    _trigger.Interactable = false;
                    ObjectiveManager.Complete("valve-completed");
                    ObjectiveManager.RepairValve(_trigger.Id);
                    this.ScreenManager.CloseScreen();
                }
            }
            //(_player.CurrentAction == CharacterAction.Checking || _player.CurrentAction == CharacterAction.Repairing)
            if (KeyboardExtended.GetState().WasKeyPressed(Keys.Q)) {
                MinigameActive = false;
                _player.ToggleRepair();
                _trigger.Progress = _progressValue;
                this.ScreenManager.CloseScreen();
            }

            if (_player.CurrentAction == CharacterAction.Checking) {
                MinigameActive = false;
            }
            else {
                MinigameActive = true;
            }
        }

        public override void Draw(GameTime gameTime) {
            if (MinigameVisible && MinigameActive) {
                MainGame.SpriteBatch.Begin();

                MainGame.SpriteBatch.DrawArc(_centerOrigin, 300, MathHelper.ToRadians(-135), MathHelper.ToRadians(30), 20, Color.Yellow, 10);
                MainGame.SpriteBatch.DrawArc(_centerOrigin, 300, MathHelper.ToRadians(-105), MathHelper.ToRadians(30), 20, Color.Green, 10);
                MainGame.SpriteBatch.DrawArc(_centerOrigin, 300, MathHelper.ToRadians(-75), MathHelper.ToRadians(30), 20, Color.Red, 10);
                MainGame.SpriteBatch.DrawArc(_centerOrigin, 315, _needleAngle, MathHelper.ToRadians(2), 20, Color.White, 40f);
                MainGame.SpriteBatch.Draw(_valveTexture, _centerOrigin, null, Color.White, _valveAngle, _valveOrigin, 8, SpriteEffects.None, 0);
                MainGame.SpriteBatch.FillRectangle(860, 800, 200, 15, Color.DarkGray);
                MainGame.SpriteBatch.FillRectangle(860, 800, _progressValue, 15, Color.GreenYellow);

                MainGame.SpriteBatch.End();
            }
            
        }

        private void Debug() {
            BitmapFont _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            //MainGame.SpriteBatch.DrawString(_font, $"Fill Angle {_valveAngle}", new Vector2(150, 100), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Mouse Pos {MouseExtended.GetState().Position}", new Vector2(150, 150), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Outer Start Angle: {_outerNeedleStartAngle}/m Needle Angle: {_outerNeedleAngle}", new Vector2(150, 200), Color.White);
        }

        private void RandomNeedleSpeed() {
            float randomSpeed;
            do {
                randomSpeed = _rand.Next(-65, 65);
            } while (randomSpeed >= -20 && randomSpeed <= 20);
             
            _needleSpeed = MathHelper.ToRadians(randomSpeed/10f);
            //do {
            //    randomSpeed = _rand.NextSingle(-6.5f, 6.5f);
            //} while (randomSpeed >= -2f && randomSpeed <= 2f);

            //_needleSpeed = MathHelper.ToRadians(randomSpeed);

            Time.AddTimer(RandomNeedleSpeed, 5);
        }

        private void CalculateInput() {
            MouseStateExtended mouseStateExtended = MouseExtended.GetState();
            float deltaX = mouseStateExtended.X - _centerOrigin.X;
            float deltaY = mouseStateExtended.Y - _centerOrigin.Y;

            if (mouseStateExtended.LeftButton == ButtonState.Pressed) {
                _mouseAngle = (float)Math.Atan2(deltaY, deltaX);
                if (!_isSpinning) {
                    float distance = new Vector2(deltaX, deltaY).Length();
                    float valveRadius = _valveTexture.Width * 8 / 2f;

                    if (distance < valveRadius) {
                        _isSpinning = true;
                        _lastMouseAngle = _mouseAngle;
                    }
                }
                else {
                    _angularSpeed = MathHelper.WrapAngle(_mouseAngle - _lastMouseAngle);
                    _targetAngle += _angularSpeed;
                    _lastMouseAngle = _mouseAngle;
                    AudioManager.Instance.PlaySFX(SoundEffectName.PipeRepairing, false, true);
                }
            }
            else {
                _isSpinning = false;
            }
        }


    }
}
