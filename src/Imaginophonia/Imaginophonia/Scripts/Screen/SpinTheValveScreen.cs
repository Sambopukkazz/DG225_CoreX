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
        

        public bool SkillCheckIsActive;
        public SpinTheValveScreen() {
            _valveTexture = MainGame.Content.Load<Texture2D>("UI/sprite_valve");
            _valveOrigin = new Vector2(_valveTexture.Width / 2f, _valveTexture.Height / 2f);
            StartSkillCheck();
            SkillCheckIsActive = true;
        }

        public override void Update(GameTime gameTime) {
            if (SkillCheckIsActive) {
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
                        float angularVelocity = MathHelper.WrapAngle(_mouseAngle - _lastMouseAngle);
                        _targetAngle += angularVelocity;
                        _lastMouseAngle = _mouseAngle;
                    }
                }
                else {
                    _isSpinning = false;
                }
                _valveAngle = MathHelper.Lerp(_valveAngle, _targetAngle, _rotationSpeed * Time.DeltaTime);
                _targetAngle = MathHelper.Lerp(_targetAngle, _valveAngle, _rotationSpeed * 2 * Time.DeltaTime);

                if (KeyboardExtended.GetState().WasKeyPressed(Keys.Q)) {
                    Time.AddTimer(this.ScreenManager.CloseScreen, 0.5f);
                    SkillCheckIsActive = false;
                }
            }

            
        }

        public override void Draw(GameTime gameTime) {
            MainGame.SpriteBatch.Begin();
            

            MainGame.SpriteBatch.Draw(_valveTexture, _centerOrigin, null, Color.White, _valveAngle, _valveOrigin, 8, SpriteEffects.None, 0);
            MainGame.SpriteBatch.End();
        }

        private void Debug() {
            BitmapFont _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            //MainGame.SpriteBatch.DrawString(_font, $"Fill Angle {_valveAngle}", new Vector2(150, 100), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Mouse Pos {MouseExtended.GetState().Position}", new Vector2(150, 150), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Outer Start Angle: {_outerNeedleStartAngle}/m Needle Angle: {_outerNeedleAngle}", new Vector2(150, 200), Color.White);
        }

        public void StartSkillCheck() {
            
        }

        private void RandomZone() {
            
        }

        private void EvaluateInput() {
            
        }
    }
}
