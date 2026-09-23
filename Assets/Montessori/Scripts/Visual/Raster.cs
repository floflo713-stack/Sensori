using UnityEngine;

namespace Sensori.Montessori
{
    public sealed class Raster
    {
        readonly int _size;
        readonly Color32[] _pixels;

        public int Size => _size;

        public Raster(int size)
        {
            _size = Mathf.Max(8, size);
            _pixels = new Color32[_size * _size];
        }

        public void Clear(Color32 color)
        {
            for (int i = 0; i < _pixels.Length; i++)
                _pixels[i] = color;
        }

        public void Circle(float cx, float cy, float radius, Color32 color)
        {
            float cxp = cx * _size;
            float cyp = cy * _size;
            float rp = radius * _size;
            int x0 = Mathf.Clamp(Mathf.FloorToInt(cxp - rp - 2f), 0, _size - 1);
            int x1 = Mathf.Clamp(Mathf.CeilToInt(cxp + rp + 2f), 0, _size - 1);
            int y0 = Mathf.Clamp(Mathf.FloorToInt(cyp - rp - 2f), 0, _size - 1);
            int y1 = Mathf.Clamp(Mathf.CeilToInt(cyp + rp + 2f), 0, _size - 1);
            for (int y = y0; y <= y1; y++)
            {
                for (int x = x0; x <= x1; x++)
                {
                    float dx = (x + 0.5f) - cxp;
                    float dy = (y + 0.5f) - cyp;
                    float sd = Mathf.Sqrt(dx * dx + dy * dy) - rp;
                    float coverage = Mathf.Clamp01(0.75f - sd);
                    if (coverage <= 0f)
                        continue;
                    Blend(x, y, color, coverage);
                }
            }
        }

        public void Ellipse(float cx, float cy, float rx, float ry, Color32 color, float rotationDegrees = 0f)
        {
            float cxp = cx * _size;
            float cyp = cy * _size;
            float rxp = Mathf.Max(0.5f, rx * _size);
            float ryp = Mathf.Max(0.5f, ry * _size);
            float reach = Mathf.Max(rxp, ryp) + 2f;
            int x0 = Mathf.Clamp(Mathf.FloorToInt(cxp - reach), 0, _size - 1);
            int x1 = Mathf.Clamp(Mathf.CeilToInt(cxp + reach), 0, _size - 1);
            int y0 = Mathf.Clamp(Mathf.FloorToInt(cyp - reach), 0, _size - 1);
            int y1 = Mathf.Clamp(Mathf.CeilToInt(cyp + reach), 0, _size - 1);
            float rad = rotationDegrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            for (int y = y0; y <= y1; y++)
            {
                for (int x = x0; x <= x1; x++)
                {
                    float dx = (x + 0.5f) - cxp;
                    float dy = (y + 0.5f) - cyp;
                    float lx = dx * cos + dy * sin;
                    float ly = -dx * sin + dy * cos;
                    float nx = lx / rxp;
                    float ny = ly / ryp;
                    float sd = Mathf.Sqrt(nx * nx + ny * ny) - 1f;
                    float coverage = Mathf.Clamp01(0.75f - sd * Mathf.Min(rxp, ryp));
                    if (coverage <= 0f)
                        continue;
                    Blend(x, y, color, coverage);
                }
            }
        }

        public void RoundRect(float cx, float cy, float width, float height, float radius, Color32 color, float rotationDegrees = 0f)
        {
            float cxp = cx * _size;
            float cyp = cy * _size;
            float hw = Mathf.Max(1f, width * _size * 0.5f);
            float hh = Mathf.Max(1f, height * _size * 0.5f);
            float rad = Mathf.Min(radius * _size, Mathf.Min(hw, hh));
            float reach = Mathf.Max(hw, hh) + 2f;
            int x0 = Mathf.Clamp(Mathf.FloorToInt(cxp - reach), 0, _size - 1);
            int x1 = Mathf.Clamp(Mathf.CeilToInt(cxp + reach), 0, _size - 1);
            int y0 = Mathf.Clamp(Mathf.FloorToInt(cyp - reach), 0, _size - 1);
            int y1 = Mathf.Clamp(Mathf.CeilToInt(cyp + reach), 0, _size - 1);
            float angle = rotationDegrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);
            for (int y = y0; y <= y1; y++)
            {
                for (int x = x0; x <= x1; x++)
                {
                    float dx = (x + 0.5f) - cxp;
                    float dy = (y + 0.5f) - cyp;
                    float lx = dx * cos + dy * sin;
                    float ly = -dx * sin + dy * cos;
                    float sd = RoundedBox(lx, ly, hw, hh, rad);
                    float coverage = Mathf.Clamp01(0.75f - sd);
                    if (coverage <= 0f)
                        continue;
                    Blend(x, y, color, coverage);
                }
            }
        }

        public void Polygon(Color32 color, params Vector2[] points01)
        {
            if (points01 == null || points01.Length < 3)
                return;
            var points = new Vector2[points01.Length];
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;
            for (int i = 0; i < points01.Length; i++)
            {
                points[i] = new Vector2(points01[i].x * _size, points01[i].y * _size);
                minX = Mathf.Min(minX, points[i].x);
                minY = Mathf.Min(minY, points[i].y);
                maxX = Mathf.Max(maxX, points[i].x);
                maxY = Mathf.Max(maxY, points[i].y);
            }
            int x0 = Mathf.Clamp(Mathf.FloorToInt(minX - 1f), 0, _size - 1);
            int x1 = Mathf.Clamp(Mathf.CeilToInt(maxX + 1f), 0, _size - 1);
            int y0 = Mathf.Clamp(Mathf.FloorToInt(minY - 1f), 0, _size - 1);
            int y1 = Mathf.Clamp(Mathf.CeilToInt(maxY + 1f), 0, _size - 1);
            for (int y = y0; y <= y1; y++)
            {
                for (int x = x0; x <= x1; x++)
                {
                    int inside = 0;
                    for (int sy = 0; sy < 2; sy++)
                    {
                        for (int sx = 0; sx < 2; sx++)
                        {
                            float px = x + 0.25f + sx * 0.5f;
                            float py = y + 0.25f + sy * 0.5f;
                            if (Contains(points, px, py))
                                inside++;
                        }
                    }
                    if (inside == 0)
                        continue;
                    Blend(x, y, color, inside / 4f);
                }
            }
        }

        public void Stroke(Color32 color, float width, params Vector2[] points01)
        {
            if (points01 == null || points01.Length == 0)
                return;
            float radius = Mathf.Max(0.004f, width * 0.5f);
            for (int i = 0; i < points01.Length; i++)
                Circle(points01[i].x, points01[i].y, radius, color);
            for (int i = 1; i < points01.Length; i++)
                Capsule(points01[i - 1], points01[i], radius, color);
        }

        public Texture2D ToTexture(string textureName)
        {
            var texture = new Texture2D(_size, _size, TextureFormat.RGBA32, false);
            texture.name = textureName;
            texture.SetPixels32(_pixels);
            texture.Apply(false, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            return texture;
        }

        void Capsule(Vector2 a, Vector2 b, float radius01, Color32 color)
        {
            float ax = a.x * _size;
            float ay = a.y * _size;
            float bx = b.x * _size;
            float by = b.y * _size;
            float rp = radius01 * _size;
            float minX = Mathf.Min(ax, bx) - rp - 2f;
            float maxX = Mathf.Max(ax, bx) + rp + 2f;
            float minY = Mathf.Min(ay, by) - rp - 2f;
            float maxY = Mathf.Max(ay, by) + rp + 2f;
            int x0 = Mathf.Clamp(Mathf.FloorToInt(minX), 0, _size - 1);
            int x1 = Mathf.Clamp(Mathf.CeilToInt(maxX), 0, _size - 1);
            int y0 = Mathf.Clamp(Mathf.FloorToInt(minY), 0, _size - 1);
            int y1 = Mathf.Clamp(Mathf.CeilToInt(maxY), 0, _size - 1);
            for (int y = y0; y <= y1; y++)
            {
                for (int x = x0; x <= x1; x++)
                {
                    float sd = DistanceToSegment(x + 0.5f, y + 0.5f, ax, ay, bx, by) - rp;
                    float coverage = Mathf.Clamp01(0.75f - sd);
                    if (coverage <= 0f)
                        continue;
                    Blend(x, y, color, coverage);
                }
            }
        }

        void Blend(int x, int y, Color32 src, float coverage)
        {
            float sa = (src.a / 255f) * Mathf.Clamp01(coverage);
            if (sa <= 0.001f)
                return;
            int index = y * _size + x;
            Color32 dst = _pixels[index];
            float da = dst.a / 255f;
            float outA = sa + da * (1f - sa);
            if (outA <= 0.0001f)
            {
                _pixels[index] = new Color32(0, 0, 0, 0);
                return;
            }
            float sr = src.r / 255f;
            float sg = src.g / 255f;
            float sb = src.b / 255f;
            float dr = dst.r / 255f;
            float dg = dst.g / 255f;
            float db = dst.b / 255f;
            float inv = 1f - sa;
            float r = (sr * sa + dr * da * inv) / outA;
            float g = (sg * sa + dg * da * inv) / outA;
            float b = (sb * sa + db * da * inv) / outA;
            _pixels[index] = new Color32(ToByte(r), ToByte(g), ToByte(b), ToByte(outA));
        }

        static float RoundedBox(float x, float y, float halfW, float halfH, float radius)
        {
            float qx = Mathf.Abs(x) - (halfW - radius);
            float qy = Mathf.Abs(y) - (halfH - radius);
            float outside = new Vector2(Mathf.Max(qx, 0f), Mathf.Max(qy, 0f)).magnitude;
            float inside = Mathf.Min(Mathf.Max(qx, qy), 0f);
            return outside + inside - radius;
        }

        static bool Contains(Vector2[] poly, float x, float y)
        {
            bool inside = false;
            for (int i = 0, j = poly.Length - 1; i < poly.Length; j = i++)
            {
                float yi = poly[i].y;
                float yj = poly[j].y;
                float xi = poly[i].x;
                float xj = poly[j].x;
                if (((yi > y) != (yj > y)) &&
                    (x < (xj - xi) * (y - yi) / (yj - yi + 0.00001f) + xi))
                    inside = !inside;
            }
            return inside;
        }

        static float DistanceToSegment(float px, float py, float ax, float ay, float bx, float by)
        {
            float abx = bx - ax;
            float aby = by - ay;
            float apx = px - ax;
            float apy = py - ay;
            float ab2 = abx * abx + aby * aby;
            float t = ab2 < 0.0001f ? 0f : Mathf.Clamp01((apx * abx + apy * aby) / ab2);
            float dx = px - (ax + abx * t);
            float dy = py - (ay + aby * t);
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        static byte ToByte(float value)
        {
            return (byte)Mathf.Clamp(Mathf.RoundToInt(value * 255f), 0, 255);
        }
    }
}
