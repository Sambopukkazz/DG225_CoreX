using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Imaginophobia {
    // One eye variant: the three sprite layers, their measured bounds, and the clamp field
    // shared by every eye placed from it.
    public class EyeSpriteSet {
        public Texture2D Sclera { get; set; }
        public Texture2D Pupil { get; set; }
        public Texture2D Eyelid { get; set; }
        public Vector2 ScleraPivot { get; set; }
        public Vector2 PupilPivot { get; set; }
        public Rectangle ScleraBounds { get; set; }
        public Rectangle PupilBounds { get; set; }
        public Rectangle EyelidBounds { get; set; }
        public ScleraField Field { get; set; }
    }

    // One placed eye.
    public class EyeInstance {
        public EyeSpriteSet SpriteSet { get; set; }
        public Vector2 Center { get; set; }
        public float Scale { get; set; }
        public float Rotation { get; set; }
        public float FootprintRadius { get; set; }
        public float PupilRadius { get; set; }
        public float NextTwitch { get; set; }
        public EyeShape Shape { get; set; }
        public EyeMotion Motion { get; set; }
    }

    // Scatters eyes through the dark area of a background and animates them: they watch the
    // cursor, twitch on their own, and all flinch together when the mouse is clicked.
    public class EyeField {
        // Settings parameters
        public int EyeCount { get; set; } = 9;
        public float MinEyeWidth { get; set; } = 108f;
        public float MaxEyeWidth { get; set; } = 252f;
        public float RotationRange { get; set; } = MathHelper.TwoPi;
        public float EyeSpacing { get; set; } = 21f;
        public float TwitchMagnitude { get; set; } = 24f;
        public float TwitchDuration { get; set; } = 0.45f;
        public float ClickTwitchMagnitude { get; set; } = 42f;
        public bool FollowMouse { get; set; } = true;
        public bool RandomTwitch { get; set; } = true;
        public bool TwitchOnClick { get; set; } = true;
        public bool SynchronizedTwitch { get; set; } = true;

        // In pixels of the background texture, which is drawn 1:1. This is the black region
        // of UI/background_main.png.
        public Rectangle DarkArea { get; set; } = new Rectangle(558, 191, 780, 560);

        private static readonly string[] s_variants = { "A", "B", "C" };

        private readonly Texture2D _background;
        private readonly Color[] _backgroundPixels;
        private readonly int[] _darkIntegral;
        private readonly List<EyeSpriteSet> _spriteSets = new List<EyeSpriteSet>();
        private readonly List<EyeInstance> _eyes = new List<EyeInstance>();
        private readonly Random _random = new Random();
        private MouseState _previousMouse = new MouseState();

        private const string SpriteFolder = "UI/Eyes/";
        private const int DarkLuminance = 12;
        private const int InnerMargin = 6;

        public EyeField(Texture2D background) {
            _background = background;
            _backgroundPixels = new Color[background.Width * background.Height];
            _background.GetData(_backgroundPixels);
            _darkIntegral = new int[(background.Width + 1) * (background.Height + 1)];

            BuildDarkMask();
            LoadSpriteSets();
            PlaceEyes();
        }

        public void Update() {
            MouseState mouse = Mouse.GetState();
            Vector2 mousePosition = new Vector2(mouse.X, mouse.Y);

            if (TwitchOnClick) {
                bool clicked = mouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released;
                if (clicked) {
                    TwitchAll();
                }
            }

            _previousMouse = mouse;

            foreach (EyeInstance eye in _eyes) {
                if (FollowMouse) {
                    eye.Motion.SetGazeOffset(GazeOffset(eye, mousePosition));
                }
                else {
                    eye.Motion.SetGazeOffset(Vector2.Zero);
                }

                if (RandomTwitch) {
                    eye.NextTwitch -= Time.DeltaTime;
                    if (eye.NextTwitch <= 0f) {
                        eye.Motion.Twitch(TwitchMagnitude, TwitchDuration);
                        eye.NextTwitch = 2f + (float)_random.NextDouble() * 9f;
                    }
                }

                eye.Motion.Update();
            }
        }

        public void Draw() {
            SpriteBatch spriteBatch = MainGame.SpriteBatch;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);

            foreach (EyeInstance eye in _eyes) {
                spriteBatch.Draw(eye.SpriteSet.Sclera, eye.Center, null, Color.White, eye.Rotation, eye.SpriteSet.ScleraPivot, eye.Scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(eye.SpriteSet.Pupil, eye.Motion.PupilPosition, null, Color.White, eye.Rotation, eye.SpriteSet.PupilPivot, eye.Scale, SpriteEffects.None, 0f);
                spriteBatch.Draw(eye.SpriteSet.Eyelid, eye.Center, null, Color.White, eye.Rotation, eye.SpriteSet.ScleraPivot, eye.Scale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();
        }

        // Re-rolls the whole layout.
        public void PlaceEyes() {
            _eyes.Clear();

            if (_spriteSets.Count == 0) {
                return;
            }

            int misses = 0;
            while (_eyes.Count < EyeCount && misses < 400) {
                float gapScale = 1f;
                float sizeBias = 1f;

                if (misses > 120) {
                    gapScale = 0.25f;
                }
                else if (misses > 40) {
                    gapScale = 0.55f;
                }

                if (misses > 40) {
                    sizeBias = 0.6f;
                }

                // Best of 24 candidates: a turned eye's bounding box is up to 1.4x its size,
                // so taking the roomiest spot fills the area instead of coming up short.
                EyeInstance best = null;
                float bestSlack = float.MinValue;

                for (int i = 0; i < 24; i++) {
                    EyeInstance candidate = SampleCandidate(sizeBias);
                    if (candidate == null) {
                        continue;
                    }

                    float slack = 0f;
                    foreach (EyeInstance other in _eyes) {
                        float required = candidate.FootprintRadius + other.FootprintRadius + EyeSpacing * gapScale;
                        slack = MathF.Min(slack, Vector2.Distance(candidate.Center, other.Center) - required);
                    }

                    if (slack > bestSlack) {
                        bestSlack = slack;
                        best = candidate;
                    }
                }

                if (best != null && bestSlack >= 0f) {
                    _eyes.Add(best);
                    misses = 0;
                }
                else {
                    misses++;
                }
            }

            // Painter's order: the far, smaller eyes first.
            _eyes.Sort((first, second) => first.Scale.CompareTo(second.Scale));
        }

        // Every eye flinches on the same frame. They share one direction so the group snaps
        // as a single thing; set SynchronizedTwitch false for a random direction per eye.
        public void TwitchAll() {
            Vector2 direction = RandomDirection();

            foreach (EyeInstance eye in _eyes) {
                if (SynchronizedTwitch) {
                    eye.Motion.Twitch(direction, ClickTwitchMagnitude, TwitchDuration);
                }
                else {
                    eye.Motion.Twitch(ClickTwitchMagnitude, TwitchDuration);
                }
            }
        }

        private void BuildDarkMask() {
            int width = _background.Width;
            int height = _background.Height;
            byte[] darkMask = new byte[width * height];

            for (int i = 0; i < darkMask.Length; i++) {
                Color color = _backgroundPixels[i];
                if (color.R + color.G + color.B <= DarkLuminance * 3) {
                    darkMask[i] = 1;
                }
            }

            // Summed-area table, so "is this whole rectangle black" is one lookup rather
            // than a loop that can step over a one-pixel bright line.
            int stride = width + 1;
            for (int y = 0; y < height; y++) {
                int rowSum = 0;
                for (int x = 0; x < width; x++) {
                    rowSum += darkMask[y * width + x];
                    _darkIntegral[(y + 1) * stride + (x + 1)] = _darkIntegral[y * stride + (x + 1)] + rowSum;
                }
            }
        }

        private void LoadSpriteSets() {
            foreach (string variant in s_variants) {
                EyeSpriteSet spriteSet = new EyeSpriteSet();

                spriteSet.Sclera = MainGame.Content.Load<Texture2D>(SpriteFolder + "sprite_sclera_" + variant);
                spriteSet.Pupil = MainGame.Content.Load<Texture2D>(SpriteFolder + "sprite_pupil_" + variant);
                spriteSet.Eyelid = MainGame.Content.Load<Texture2D>(SpriteFolder + "sprite_eyelid_" + variant);

                spriteSet.ScleraBounds = MeasureOpaque(spriteSet.Sclera);
                spriteSet.PupilBounds = MeasureOpaque(spriteSet.Pupil);
                spriteSet.EyelidBounds = MeasureOpaque(spriteSet.Eyelid);
                spriteSet.ScleraPivot = BoxCenter(spriteSet.ScleraBounds);
                spriteSet.PupilPivot = BoxCenter(spriteSet.PupilBounds);
                spriteSet.Field = new ScleraField(spriteSet.Sclera, spriteSet.ScleraPivot);

                _spriteSets.Add(spriteSet);
            }
        }

        private EyeInstance SampleCandidate(float sizeBias) {
            EyeSpriteSet spriteSet = _spriteSets[_random.Next(_spriteSets.Count)];

            float width = MathHelper.Lerp(MinEyeWidth, MaxEyeWidth, (float)_random.NextDouble() * sizeBias);
            float scale = width / spriteSet.ScleraBounds.Width;
            float rotation = ((float)_random.NextDouble() * 2f - 1f) * RotationRange * 0.5f;

            // A turned rectangle needs its rotated bounds, and that is what has to fit.
            float cos = MathF.Abs(MathF.Cos(rotation));
            float sin = MathF.Abs(MathF.Sin(rotation));
            float footprintWidth = (spriteSet.EyelidBounds.Width * cos + spriteSet.EyelidBounds.Height * sin) * scale;
            float footprintHeight = (spriteSet.EyelidBounds.Width * sin + spriteSet.EyelidBounds.Height * cos) * scale;

            if (footprintWidth + InnerMargin * 2 >= DarkArea.Width) {
                return null;
            }
            if (footprintHeight + InnerMargin * 2 >= DarkArea.Height) {
                return null;
            }

            float centerX = MathHelper.Lerp(DarkArea.Left + InnerMargin + footprintWidth * 0.5f, DarkArea.Right - InnerMargin - footprintWidth * 0.5f, (float)_random.NextDouble());
            float centerY = MathHelper.Lerp(DarkArea.Top + InnerMargin + footprintHeight * 0.5f, DarkArea.Bottom - InnerMargin - footprintHeight * 0.5f, (float)_random.NextDouble());

            Vector2 lidOffset = Rotate(BoxCenter(spriteSet.EyelidBounds) - spriteSet.ScleraPivot, rotation) * scale;

            int boxWidth = (int)footprintWidth;
            int boxHeight = (int)footprintHeight;
            int boxX = (int)(centerX + lidOffset.X - boxWidth * 0.5f);
            int boxY = (int)(centerY + lidOffset.Y - boxHeight * 0.5f);

            if (InsideDarkArea(boxX, boxY, boxWidth, boxHeight) == false) {
                return null;
            }

            if (BoxIsDark(boxX, boxY, boxWidth, boxHeight) == false) {
                return null;
            }

            Vector2 center = new Vector2(centerX, centerY);
            float pupilRadius = (spriteSet.PupilBounds.Width + spriteSet.PupilBounds.Height) * 0.25f * scale;
            EyeShape shape = new EyeShape(spriteSet.Field, spriteSet.ScleraBounds, center, scale, rotation);

            EyeInstance eye = new EyeInstance();
            eye.SpriteSet = spriteSet;
            eye.Center = center;
            eye.Scale = scale;
            eye.Rotation = rotation;
            eye.PupilRadius = pupilRadius;
            eye.Shape = shape;
            eye.Motion = new EyeMotion(shape, pupilRadius, _random);
            eye.NextTwitch = 1f + (float)_random.NextDouble() * 8f;

            // Equal-area radius, not the bounding box: the box corners are transparent and
            // it is up to 1.4x wider when turned, which would rule out the count entirely.
            eye.FootprintRadius = 0.5f * MathF.Sqrt(footprintWidth * footprintHeight);

            return eye;
        }

        // Offset from the eye centre that puts the pupil on the mouse, capped at how far it
        // can actually travel so a twitch around the look stays visible.
        private Vector2 GazeOffset(EyeInstance eye, Vector2 mousePosition) {
            Vector2 toMouse = mousePosition - eye.Center;
            float distance = toMouse.Length();
            if (distance < 0.001f) {
                return Vector2.Zero;
            }

            Vector2 direction = toMouse / distance;
            float travel = eye.Shape.MaxTravel(eye.Center, direction, eye.PupilRadius);
            return direction * MathF.Min(distance, travel);
        }

        private Vector2 RandomDirection() {
            float angle = (float)(_random.NextDouble() * Math.PI * 2.0);
            return new Vector2(MathF.Cos(angle), MathF.Sin(angle));
        }

        private bool InsideDarkArea(int x, int y, int width, int height) {
            if (x < DarkArea.Left + InnerMargin) {
                return false;
            }
            if (y < DarkArea.Top + InnerMargin) {
                return false;
            }
            if (x + width > DarkArea.Right - InnerMargin) {
                return false;
            }
            if (y + height > DarkArea.Bottom - InnerMargin) {
                return false;
            }

            return true;
        }

        private bool BoxIsDark(int x, int y, int width, int height) {
            if (width <= 0 || height <= 0) {
                return false;
            }

            if (x < 0 || y < 0) {
                return false;
            }

            if (x + width > _background.Width || y + height > _background.Height) {
                return false;
            }

            int stride = _background.Width + 1;
            int right = x + width;
            int bottom = y + height;

            int dark = _darkIntegral[bottom * stride + right]
                     - _darkIntegral[y * stride + right]
                     - _darkIntegral[bottom * stride + x]
                     + _darkIntegral[y * stride + x];

            return dark == width * height;
        }

        private static Rectangle MeasureOpaque(Texture2D texture) {
            Color[] pixels = new Color[texture.Width * texture.Height];
            texture.GetData(pixels);

            int minX = texture.Width;
            int minY = texture.Height;
            int maxX = -1;
            int maxY = -1;

            for (int y = 0; y < texture.Height; y++) {
                int row = y * texture.Width;
                for (int x = 0; x < texture.Width; x++) {
                    if (pixels[row + x].A <= 32) {
                        continue;
                    }

                    if (x < minX) {
                        minX = x;
                    }
                    if (y < minY) {
                        minY = y;
                    }
                    if (x > maxX) {
                        maxX = x;
                    }
                    if (y > maxY) {
                        maxY = y;
                    }
                }
            }

            if (maxX < 0) {
                return new Rectangle(0, 0, texture.Width, texture.Height);
            }

            return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }

        private static Vector2 BoxCenter(Rectangle rectangle) {
            return new Vector2(rectangle.X + rectangle.Width * 0.5f, rectangle.Y + rectangle.Height * 0.5f);
        }

        private static Vector2 Rotate(Vector2 vector, float angle) {
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);
            return new Vector2(vector.X * cos - vector.Y * sin, vector.X * sin + vector.Y * cos);
        }
    }
}
