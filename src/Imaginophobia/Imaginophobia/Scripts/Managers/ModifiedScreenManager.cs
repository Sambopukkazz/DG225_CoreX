using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.ViewportAdapters;
using MonoSound.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class ModifiedScreenManager : ScreenManager {
        private RenderTarget2D _frameRT;
        private Effect _grade;
        private float _fadeValue;
        private float _fadeCurrentTime;
        private float _fadeTargetTime;
        private TransitionState _transitionState;

        // 0 = grayscale, 1 = normal
        public float Saturation { get; set; } = 1f;

        protected override void LoadContent() {
            _grade = MainGame.Content.Load<Effect>("Shaders/Saturation");
            _grade.CurrentTechnique = _grade.Techniques["ApplySaturation"];
            base.LoadContent();
        }

        public override void Update(GameTime gameTime) {

            if (_transitionState == TransitionState.In && _fadeCurrentTime > 0) {
                _fadeCurrentTime -= Time.DeltaTime;
            }
            else if (_transitionState == TransitionState.Out && _fadeCurrentTime < _fadeTargetTime) {
                _fadeCurrentTime += Time.DeltaTime;
            }
            
            _fadeValue = _fadeCurrentTime / _fadeTargetTime;
            _fadeValue = MathHelper.Clamp(_fadeValue, 0f, 1f);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime) {
            EnsureFrameTarget();
            MainGame.GraphicsDevice.SetRenderTarget(_frameRT);

            base.Draw(gameTime);
        }

        public void DrawFade() {
            SpriteBatch spriteBatch = MainGame.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.FillRectangle(0, 0, MainGame.GraphicsDevice.Viewport.Width, MainGame.GraphicsDevice.Viewport.Height, Color.Black * _fadeValue);
            spriteBatch.End();
        }

        public void DrawGraded() {
            _grade.Parameters["Saturation"]?.SetValue(Saturation);
            SpriteBatch spriteBatch = MainGame.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Immediate, Microsoft.Xna.Framework.Graphics.BlendState.Opaque, SamplerState.PointClamp, effect: _grade);
            spriteBatch.Draw(_frameRT, MainGame.GraphicsDevice.Viewport.Bounds, Color.White);
            spriteBatch.End();
        }

        private void EnsureFrameTarget() {
            Viewport viewport = MainGame.GraphicsDevice.Viewport;

            if (_frameRT != null && _frameRT.Width == viewport.Width && _frameRT.Height == viewport.Height) {
                return;
            }

            _frameRT?.Dispose();
            _frameRT = new RenderTarget2D(MainGame.GraphicsDevice, viewport.Width, viewport.Height, false, SurfaceFormat.Color, DepthFormat.None);
        }

        public void FadeIn(float seconds) {
            _transitionState = TransitionState.In;
            _fadeTargetTime = seconds;
            _fadeCurrentTime = seconds;
        }
        public void FadeOut(float seconds) {
            _transitionState = TransitionState.Out;
            _fadeTargetTime = seconds;
            _fadeCurrentTime = 0;
        }

        public void ShowScreen(Screen screen, float fadeInSeconds) {
            ShowScreen(screen);
            FadeIn(fadeInSeconds);
            AudioManager.Instance.UnmuteAudio();
        }

        public void CloseScreen(float fadeOutSeconds) {
            FadeOut(fadeOutSeconds);
            Time.AddTimer(CloseScreen, fadeOutSeconds);
            Time.AddTimer(AudioManager.Instance.MuteAudio, fadeOutSeconds);
        }

        public void ClearScreens(Screen screen, float fadeOutSeconds, float fadeInSeconds, float waitSeconds = 0) {
            FadeOut(fadeOutSeconds);
            Time.AddTimer(ClearScreens, fadeOutSeconds);
            Time.AddTimer(AudioManager.Instance.MuteAudio, fadeOutSeconds);

            if (waitSeconds > 0) {
                Time.AddTimer(() => ShowScreen(screen, fadeInSeconds), waitSeconds + fadeOutSeconds);
            }
            else {
                Time.AddTimer(() => ShowScreen(screen, fadeInSeconds), fadeOutSeconds);
            }
        }

        public void ReplaceScreen(Screen screen, float fadeOutSeconds, float fadeInSeconds, float waitSeconds = 0) {
            CloseScreen(fadeOutSeconds);
            if (waitSeconds > 0) {
                Time.AddTimer(() => ShowScreen(screen, fadeInSeconds), waitSeconds + fadeOutSeconds);
            }
            else {
                Time.AddTimer(() => ShowScreen(screen, fadeInSeconds), fadeOutSeconds);
            }
        }

        //public void ReplaceScreen(float fadeOutSeconds, float fadeInSeconds, float waitSeconds = 0) {
            
        //    CloseScreen(fadeOutSeconds);
        //    if (waitSeconds > 0) {
        //        Time.AddTimer(() => ShowScreen(new GameplayScreen(viewportAdapter), fadeInSeconds), waitSeconds + fadeOutSeconds);
        //    }
        //    else {
        //        Time.AddTimer(() => ShowScreen(new GameplayScreen(viewportAdapter), fadeInSeconds), fadeOutSeconds);
        //    }
        //}
    }
}
