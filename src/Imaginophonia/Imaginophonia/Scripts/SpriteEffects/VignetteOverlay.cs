using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imaginophobia {
    public class VignetteOverlay {
        private readonly Effect _shader;
        private readonly Texture2D _blankTexture;

        // Settings parameters
        public Vector2 ScreenPosition { get; set; } = new Vector2(960, 540);
        public Vector2 Scale { get; set; } = new Vector2(0.5f, 0.5f);
        public float Softness { get; set; } = 0.25f;
        public Color Color { get; set; } = Color.Black;

        public VignetteOverlay() {
            _shader = MainGame.Content.Load<Effect>("Shaders/Vignette");

            _blankTexture = new Texture2D(MainGame.GraphicsDevice, 1, 1);
            _blankTexture.SetData(new[] { Color.White });
        }

        public void Render() {
            Viewport viewport = MainGame.GraphicsDevice.Viewport;
            // 1. Normalize pixel coordinates into standard UV texture space (0.0 to 1.0)
            Vector2 normalizedCenter = new Vector2(
                ScreenPosition.X / viewport.Width,
                ScreenPosition.Y / viewport.Height
            );

            // 2. Map structural values directly onto the shader uniforms
            _shader.Parameters["Center"]?.SetValue(normalizedCenter);
            _shader.Parameters["Scale"]?.SetValue(Scale);
            _shader.Parameters["FadeSoftness"]?.SetValue(Softness);
            _shader.Parameters["Color"]?.SetValue(Color.ToVector4());

            // 3. Execute the screen space blending pass immediately
            MainGame.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, effect: _shader);
            MainGame.SpriteBatch.Draw(_blankTexture, viewport.Bounds, Color.White);
            MainGame.SpriteBatch.End();
        }
    }
}
