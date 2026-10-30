using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using Penumbra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Imaginophobia {
    public static class InputManager {
        private static Vector2 s_direction;
        public static Vector2 Direction => s_direction;
        private static Keys s_keyPressed;
        private static KeyboardStateExtended s_keyboardState;

        public static void Update() {
            KeyboardExtended.Update();
            MouseExtended.Update();

            s_keyboardState = KeyboardExtended.GetState();
            s_direction = Vector2.Zero;

            if (s_keyboardState.IsKeyDown(Keys.A)) s_direction.X--;
            if (s_keyboardState.IsKeyDown(Keys.D)) s_direction.X++;

            if (s_direction != Vector2.Zero) s_direction = Vector2.Normalize(s_direction);
        }

        public static bool IsKeyDown(Keys key) {
            return s_keyboardState.IsKeyDown(key);
        }
    }
}
