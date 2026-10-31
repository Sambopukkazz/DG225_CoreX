using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Imaginophobia {
    // Distance from every pixel of a sclera sprite to the nearest edge: positive inside
    // the drawn white, negative outside. Built once per sprite, shared by every eye that
    // uses it.
    public class ScleraField {
        private readonly float[] _signed;
        private readonly int _width;
        private readonly int _height;

        public Vector2 Pivot { get; private set; }
        public Vector2 DeepPoint { get; private set; }

        public ScleraField(Texture2D sclera, Vector2 pivot) {
            _width = sclera.Width;
            _height = sclera.Height;
            Pivot = pivot;

            Color[] pixels = new Color[_width * _height];
            sclera.GetData(pixels);

            bool[] inside = new bool[pixels.Length];
            for (int i = 0; i < pixels.Length; i++) {
                inside[i] = pixels[i].A > 64;
            }

            // Seeds are the opposite side: inside pixels measure their way out to the
            // nearest outside pixel, and back to front. Swapping the two makes the whole
            // positive half read zero and every clamp fails closed, silently.
            float[] toOutside = Chamfer(inside, true, _width, _height);
            float[] toInside = Chamfer(inside, false, _width, _height);

            _signed = new float[pixels.Length];
            int deepest = -1;

            for (int i = 0; i < _signed.Length; i++) {
                if (inside[i]) {
                    _signed[i] = toOutside[i];
                    if (deepest < 0 || toOutside[i] > toOutside[deepest]) {
                        deepest = i;
                    }
                }
                else {
                    _signed[i] = -toInside[i];
                }
            }

            if (deepest < 0) {
                deepest = (_height / 2) * _width + _width / 2;
            }

            DeepPoint = new Vector2(deepest % _width + 0.5f, deepest / _width + 0.5f);
        }

        public float Sample(Vector2 point) {
            float x = MathHelper.Clamp(point.X, 0f, _width - 1.001f);
            float y = MathHelper.Clamp(point.Y, 0f, _height - 1.001f);
            int x0 = (int)x;
            int y0 = (int)y;
            int x1 = Math.Min(x0 + 1, _width - 1);
            int y1 = Math.Min(y0 + 1, _height - 1);
            float blendX = x - x0;
            float blendY = y - y0;

            float top = MathHelper.Lerp(_signed[y0 * _width + x0], _signed[y0 * _width + x1], blendX);
            float bottom = MathHelper.Lerp(_signed[y1 * _width + x0], _signed[y1 * _width + x1], blendX);
            return MathHelper.Lerp(top, bottom, blendY);
        }

        // Two-pass chamfer distance transform: seeds start at zero and every other pixel
        // takes the distance to the nearest seed. The 1 / sqrt(2) weights keep the
        // gradient near unit length, which the clamp relies on when it steps to the edge.
        private static float[] Chamfer(bool[] inside, bool seedsAreOutside, int width, int height) {
            float[] distances = new float[width * height];

            for (int i = 0; i < distances.Length; i++) {
                bool isSeed;
                if (seedsAreOutside) {
                    isSeed = inside[i] == false;
                }
                else {
                    isSeed = inside[i];
                }

                if (isSeed) {
                    distances[i] = 0f;
                }
                else {
                    distances[i] = 1e9f;
                }
            }

            float diagonal = MathF.Sqrt(2f);

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    int i = y * width + x;
                    float value = distances[i];

                    if (x > 0) {
                        value = MathF.Min(value, distances[i - 1] + 1f);
                    }
                    if (y > 0) {
                        value = MathF.Min(value, distances[i - width] + 1f);
                    }
                    if (x > 0 && y > 0) {
                        value = MathF.Min(value, distances[i - width - 1] + diagonal);
                    }
                    if (x < width - 1 && y > 0) {
                        value = MathF.Min(value, distances[i - width + 1] + diagonal);
                    }

                    distances[i] = value;
                }
            }

            for (int y = height - 1; y >= 0; y--) {
                for (int x = width - 1; x >= 0; x--) {
                    int i = y * width + x;
                    float value = distances[i];

                    if (x < width - 1) {
                        value = MathF.Min(value, distances[i + 1] + 1f);
                    }
                    if (y < height - 1) {
                        value = MathF.Min(value, distances[i + width] + 1f);
                    }
                    if (x < width - 1 && y < height - 1) {
                        value = MathF.Min(value, distances[i + width + 1] + diagonal);
                    }
                    if (x > 0 && y < height - 1) {
                        value = MathF.Min(value, distances[i + width - 1] + diagonal);
                    }

                    distances[i] = value;
                }
            }

            return distances;
        }
    }

    // The area a pupil may occupy, taken from the sclera's own outline and carried in the
    // eye's own frame (centre, scale and rotation). A two-disk lens shape only fits a
    // symmetric almond; the half-lidded C sclera is a crescent, so the clamp reads the
    // sprite instead of assuming a shape.
    public class EyeShape {
        private readonly ScleraField _field;
        private readonly float _scale;

        public Vector2 Center { get; private set; }
        public float HalfWidth { get; private set; }
        public float HalfHeight { get; private set; }
        public float Rotation { get; private set; }

        public EyeShape(ScleraField field, Rectangle bounds, Vector2 center, float scale, float rotation) {
            _field = field;
            _scale = scale;

            Center = center;
            Rotation = rotation;
            HalfWidth = bounds.Width * 0.5f * scale;
            HalfHeight = bounds.Height * 0.5f * scale;
        }

        // Nearest point to position where a pupil of that radius is still fully inside the
        // sclera. The field's gradient is unit length, so one step of (radius - clearance)
        // lands on the boundary of the allowed set; a few passes settle the curvature.
        public Vector2 ClampPupil(Vector2 position, float pupilRadius) {
            Vector2 point = ToSprite(position);
            float clearance = pupilRadius / _scale - 0.3f;

            for (int i = 0; i < 12; i++) {
                float distance = _field.Sample(point);
                if (distance >= clearance) {
                    break;
                }

                Vector2 gradient = Gradient(point);
                float length = gradient.Length();
                if (length < 0.0001f) {
                    break;
                }

                point += gradient / length * (clearance - distance);
            }

            if (_field.Sample(point) < clearance) {
                // Far outside, or a flat spot killed the gradient: slide along the line
                // toward the deepest point, which always has clearance.
                if (_field.Sample(_field.DeepPoint) >= clearance) {
                    Vector2 low = point;
                    Vector2 high = _field.DeepPoint;

                    for (int i = 0; i < 24; i++) {
                        Vector2 middle = (low + high) * 0.5f;
                        if (_field.Sample(middle) >= clearance) {
                            high = middle;
                        }
                        else {
                            low = middle;
                        }
                    }

                    point = high;
                }
                else {
                    point = _field.DeepPoint;
                }
            }

            return ToScreen(point);
        }

        public float ClearanceAt(Vector2 position) {
            return _field.Sample(ToSprite(position));
        }

        public bool PupilFits(Vector2 position, float pupilRadius) {
            return ClearanceAt(position) >= pupilRadius / _scale - 0.5f;
        }

        // How far the pupil's centre may travel from a point along a direction before it
        // stops fitting. Searches the field, so it is exact for any outline and rotation.
        public float MaxTravel(Vector2 center, Vector2 direction, float pupilRadius) {
            if (direction.LengthSquared() < 0.000001f) {
                return 0f;
            }

            Vector2 normalized = direction / direction.Length();
            float radius = pupilRadius / _scale;
            float low = 0f;
            float high = MathF.Max(HalfWidth, HalfHeight) + 1f;

            for (int i = 0; i < 24; i++) {
                float middle = (low + high) * 0.5f;
                if (_field.Sample(ToSprite(center + normalized * middle)) >= radius) {
                    low = middle;
                }
                else {
                    high = middle;
                }
            }

            return low;
        }

        private Vector2 ToSprite(Vector2 world) {
            Vector2 scaled = (world - Center) / _scale;
            float cos = MathF.Cos(Rotation);
            float sin = MathF.Sin(Rotation);
            Vector2 rotated = new Vector2(scaled.X * cos + scaled.Y * sin, -scaled.X * sin + scaled.Y * cos);
            return rotated + _field.Pivot;
        }

        private Vector2 ToScreen(Vector2 sprite) {
            Vector2 scaled = (sprite - _field.Pivot) * _scale;
            float cos = MathF.Cos(Rotation);
            float sin = MathF.Sin(Rotation);
            return new Vector2(scaled.X * cos - scaled.Y * sin, scaled.X * sin + scaled.Y * cos) + Center;
        }

        private Vector2 Gradient(Vector2 point) {
            float offset = 1f;
            float dx = _field.Sample(point + new Vector2(offset, 0f)) - _field.Sample(point - new Vector2(offset, 0f));
            float dy = _field.Sample(point + new Vector2(0f, offset)) - _field.Sample(point - new Vector2(0f, offset));
            return new Vector2(dx, dy) / (2f * offset);
        }
    }
}
