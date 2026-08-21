using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Imaginophonia {
    public static class InputManager {
        private static Vector2 _direction;
        public static Vector2 Direction { get { return _direction; } }

        public static void Update() {
            KeyboardExtended.Update();

            KeyboardStateExtended keyboardState = KeyboardExtended.GetState();
            _direction = Vector2.Zero;

            if (keyboardState.WasKeyPressed(Keys.A)) _direction.X--;
            if (keyboardState.WasKeyPressed(Keys.D)) _direction.X++;

            if(_direction != Vector2.Zero) _direction = Vector2.Normalize(_direction);
        }
    }
}
