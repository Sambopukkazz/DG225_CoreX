using MonoGame.Extended;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.Input;

namespace Imaginophobia {
    public class Cell {
        public Color Color { get; set; }
        public Rectangle Bound { get; set; }
        public bool IsEndpoint { get; set; }
        public bool IsWired { get; set; }
        private bool _mouseLeft;

        public Cell(Rectangle rect) {
            Bound = rect;
        }

        public void Update() {
            if (Bound.Contains(MouseExtended.GetState().Position) && _mouseLeft == false) {
                if (MouseExtended.GetState().LeftButton == ButtonState.Pressed) {
                    IsWired = !IsWired;
                    _mouseLeft = true;
                }
            }
            else if (!Bound.Contains(MouseExtended.GetState().Position)) {
                _mouseLeft = false;
            }

            Color = IsWired ? Color.Red : Color.Green;
        }

        public void Draw() {
            MainGame.SpriteBatch.FillRectangle(Bound, Color);
            //MainGame.SpriteBatch.Draw();
        }
    }
}
