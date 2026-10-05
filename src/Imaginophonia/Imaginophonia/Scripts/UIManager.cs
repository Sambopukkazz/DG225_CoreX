using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Timers;
using MonoGame.Extended.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class UIManager {
        private BitmapFont _font;
        private Texture2D _spacebar;
        private Vector2 _spaceBarPos;
        private Player _player;
        public VignetteOverlay VignetteOverlay;
        public VignetteOverlay VignetteOverlay2;
        private readonly Tweener _tweener;

        public UIManager(Player player) {
            _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            _spacebar = MainGame.Content.Load<Texture2D>("UI/ui_spacebar");

            VignetteOverlay = new();
            VignetteOverlay2 = new();
            _tweener = new Tweener();

            _player = player;
            _spaceBarPos = new Vector2(960 - 300, 540 + 400);
        }

        public void Update() {
            //VignetteOverlay.Scale = new Vector2(VignetteOverlay.Scale.X, MathHelper.Lerp(VignetteOverlay.Scale.Y, 0, 0.01f));
            if (_player.Blink) {
                BlinkFx();
                _player.Blink = false;
            }
            _tweener.Update(Time.DeltaTime);
            DebugUpdate();
        }

        public void Draw() {
            VignetteOverlay.Render();
            VignetteOverlay2.Render();

            MainGame.SpriteBatch.Begin();
            DebugDraw();

            MainGame.SpriteBatch.DrawString(_font, $"Objective : Fix everything and leave\nSanity : {_player.Sanity}\nBattery : {_player.Battery}", new Vector2(100, 100), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"PlayerPos {_player.Transform.Position} ", new Vector2(150, 400), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"PlayerPos {_player.Transform.WorldPosition} ", new Vector2(150, 420), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Listener {_player.Listener.Position.X}", new Vector2(150, 440), Color.White);

            if (_player.CanInteract && _player.CanRepair) {
                MainGame.SpriteBatch.Draw(_spacebar, _spaceBarPos, null, ColorHelper.FromHex("#70707041"), 0,Vector2.Zero,0.75f,SpriteEffects.None,0);
            }

            if (_player.CooldownTimer != null) {
                Color color;
                if (_player.CooldownTimer.Name == "cooldownTime") {
                    color = Color.DarkRed;
                }
                else {
                    color = Color.CornflowerBlue;
                }
                MainGame.SpriteBatch.FillRectangle(960 - 60, 540 + 300, 120, 15, Color.DarkGray);
                MainGame.SpriteBatch.FillRectangle(960 - 60, 540 + 300, _player.CooldownTimer.TimeLeft * 40, 15, color);
            }

            MainGame.SpriteBatch.End();
            

            //MainGame.SpriteBatch.FillRectangle(new Rectangle(0,0,1920,480),Color.Black);

            


            //foreach (Timer timer in Time.Timers) {
            //    MainGame.SpriteBatch.DrawString(_font, $"\nTimer:{timer.TimeLeft}", new Vector2(100, 500 + (40 * Time.Timers.IndexOf(timer))), Color.White);
            //}
        }

        private void DebugUpdate() {
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
                VignetteOverlay.Scale -= new Vector2(0, 0.01f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) {
                VignetteOverlay.Scale += new Vector2(0, 0.01f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
                VignetteOverlay.Scale += new Vector2(0.01f, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
                VignetteOverlay.Scale -= new Vector2(0.01f, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad6)) {
                VignetteOverlay.Softness += 0.01f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad4)) {
                VignetteOverlay.Softness -= 0.01f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.I)) {
                VignetteOverlay2.Scale -= new Vector2(0, 0.01f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.K)) {
                VignetteOverlay2.Scale += new Vector2(0, 0.01f);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.L)) {
                VignetteOverlay2.Scale += new Vector2(0.01f, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.J)) {
                VignetteOverlay2.Scale -= new Vector2(0.01f, 0);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.T)) {
                VignetteOverlay2.Softness += 0.01f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.G)) {
                VignetteOverlay2.Softness -= 0.01f;
            }
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad8)) {
            //    _vignetteOverlay.Sharpness += 0.01f;
            //}
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad2)) {
            //    _vignetteOverlay.Sharpness -= 0.01f;
            //}
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad1)) {
                VignetteOverlay.Color = Color.Red;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.NumPad0)) {
                VignetteOverlay.Color = Color.Black;
            }
            //if (Keyboard.GetState().IsKeyDown(Keys.NumPad3)) {
            //    _scotopicLight.Enabled = false;
            //}

        }

        public void BlinkFx() {
            _tweener.TweenTo(target: VignetteOverlay, expression: vignette => vignette.Scale, toValue: new Vector2(VignetteOverlay.Scale.X, 0), duration: 0.5f)
                .Repeat(1)
                .AutoReverse()
                .Easing(EasingFunctions.Linear);
        }

        private void DebugDraw() {
            BitmapFont _font = MainGame.Content.Load<BitmapFont>("Font/GenerationFonting");
            MainGame.SpriteBatch.DrawString(_font, $"Scale {VignetteOverlay.Scale} ", new Vector2(150, 500), Color.White);
            MainGame.SpriteBatch.DrawString(_font, $"Brightness: {VignetteOverlay.Softness}", new Vector2(150, 50), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Sharpenss {_vignetteOverlay.Sharpness}", new Vector2(150, 150), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Animation {_animatedSprite.CurrentAnimation}", new Vector2(150, 200), Color.White);
            //MainGame.SpriteBatch.DrawString(_font, $"Listener {Listener.Position.X}", new Vector2(150, 520), Color.White);

            //foreach (Timer timer in Time.Timers) {
            //    MainGame.SpriteBatch.DrawString(_font, $"\nTimer:{timer.TimeLeft}", new Vector2(100, 500 + (40 * Time.Timers.IndexOf(timer))), Color.White);
            //}
        }
    }
}
