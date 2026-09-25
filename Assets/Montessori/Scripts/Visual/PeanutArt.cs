using UnityEngine;

namespace Sensori.Montessori
{
    public static class PeanutArt
    {
        static Sprite _body;
        static Sprite _arm;
        static Sprite _leg;
        static Sprite _sprout;
        static Sprite _eyeOpen;
        static Sprite _eyeShut;
        static Sprite _smile;
        static Sprite _grin;
        static Sprite _cheek;
        static Sprite _shadow;
        static Sprite _bubble;
        static Sprite _board;
        static Sprite _field;
        static Sprite _button;
        static Sprite _solid;

        public static Sprite Body => _body ?? (_body = SpriteOf(PaintBody(), "cacahuete-corps"));
        public static Sprite Arm => _arm ?? (_arm = SpriteOf(PaintArm(), "cacahuete-bras"));
        public static Sprite Leg => _leg ?? (_leg = SpriteOf(PaintLeg(), "cacahuete-jambe"));
        public static Sprite Sprout => _sprout ?? (_sprout = SpriteOf(PaintSprout(), "cacahuete-pousse"));
        public static Sprite EyeOpen => _eyeOpen ?? (_eyeOpen = SpriteOf(PaintEye(true), "cacahuete-oeil"));
        public static Sprite EyeShut => _eyeShut ?? (_eyeShut = SpriteOf(PaintEye(false), "cacahuete-clin"));
        public static Sprite Smile => _smile ?? (_smile = SpriteOf(PaintFace(false), "cacahuete-sourire"));
        public static Sprite Grin => _grin ?? (_grin = SpriteOf(PaintFace(true), "cacahuete-rire"));
        public static Sprite Cheek => _cheek ?? (_cheek = SpriteOf(PaintCheek(), "cacahuete-joue"));
        public static Sprite Shadow => _shadow ?? (_shadow = SpriteOf(PaintShadow(), "cacahuete-ombre"));
        public static Sprite Bubble => _bubble ?? (_bubble = SpriteOf(PaintBubble(), "cacahuete-bulle"));
        public static Sprite Board => _board ?? (_board = SpriteOf(PaintBoard(), "cacahuete-panneau"));
        public static Sprite Field => _field ?? (_field = SpriteOf(PaintField(), "cacahuete-champ"));
        public static Sprite ButtonFace => _button ?? (_button = SpriteOf(PaintButton(), "cacahuete-bouton"));
        public static Sprite Solid => _solid ?? (_solid = SpriteOf(PaintSolid(), "cacahuete-aplat"));

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetCache()
        {
            _body = _arm = _leg = _sprout = null;
            _eyeOpen = _eyeShut = _smile = _grin = null;
            _cheek = _shadow = _bubble = _board = _field = _button = _solid = null;
        }

        static Sprite SpriteOf(Texture2D texture, string spriteName)
        {
            var sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            sprite.name = spriteName;
            return sprite;
        }

        static Texture2D PaintBody()
        {
            const int w = 440;
            const int h = 600;
            var paint = new Painter(w, h);
            float cx = 220f;
            float lowerY = 248f;
            float upperY = 418f;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    float px = x + 0.5f;
                    float py = y + 0.5f;
                    if (px < cx - 190f || px > cx + 190f || py < 70f || py > 560f)
                        continue;
                    float d = Shell(px, py, cx, lowerY, upperY);
                    if (d > 1.35f)
                        continue;
                    float dx = Shell(px + 1.4f, py, cx, lowerY, upperY) - d;
                    float dy = Shell(px, py + 1.4f, cx, lowerY, upperY) - d;
                    float len = Mathf.Sqrt(dx * dx + dy * dy);
                    float nx = len > 0.0001f ? dx / len : 0f;
                    float ny = len > 0.0001f ? dy / len : 1f;
                    float light = Mathf.Clamp01(Vector2.Dot(new Vector2(nx, ny), new Vector2(-0.42f, 0.9f).normalized) * 0.55f + 0.58f);
                    Color deep = new Color(0.62f, 0.36f, 0.16f, 1f);
                    Color mid = new Color(0.90f, 0.64f, 0.32f, 1f);
                    Color lit = new Color(0.99f, 0.90f, 0.70f, 1f);
                    Color col = Color.Lerp(deep, mid, Mathf.Clamp01(light * 1.15f));
                    col = Color.Lerp(col, lit, Mathf.Clamp01((light - 0.62f) * 2.4f));
                    float edge = Mathf.Clamp01(1f + d / 8f);
                    col = Color.Lerp(col, new Color(0.40f, 0.20f, 0.09f, 1f), edge * edge * 0.9f);
                    float hx = (px - (cx - 48f)) / 78f;
                    float hy = (py - (upperY + 18f)) / 64f;
                    float hi = Mathf.Clamp01(1f - (hx * hx + hy * hy));
                    col = Color.Lerp(col, new Color(1f, 0.97f, 0.90f, 1f), hi * hi * 0.62f);
                    float bx = (px - (cx + 18f)) / 90f;
                    float by = (py - (lowerY - 10f)) / 80f;
                    float belly = Mathf.Clamp01(1f - (bx * bx + by * by));
                    col = Color.Lerp(col, new Color(0.96f, 0.78f, 0.48f, 1f), belly * 0.28f);
                    paint.Blend(x, y, col, Mathf.Clamp01(0.9f - d));
                }
            }

            paint.Ellipse(cx + 2f, 330f, 7f, 78f, new Color(0.45f, 0.24f, 0.11f, 0.28f), 7f);
            var scarf = new Color(0.86f, 0.32f, 0.28f, 1f);
            var knot = new Color(0.62f, 0.18f, 0.16f, 1f);
            paint.Ellipse(cx - 62f, 318f, 40f, 16f, scarf, 28f);
            paint.Ellipse(cx + 66f, 326f, 38f, 15f, scarf, -22f);
            paint.Ellipse(cx, 324f, 18f, 14f, knot, 0f);
            paint.Ellipse(cx - 6f, 330f, 6f, 5f, new Color(1f, 0.82f, 0.74f, 0.7f), 0f);
            paint.Ellipse(168f, 392f, 3.2f, 2.4f, new Color(0.55f, 0.30f, 0.14f, 0.55f), 20f);
            paint.Ellipse(184f, 374f, 2.4f, 1.8f, new Color(0.55f, 0.30f, 0.14f, 0.45f), -10f);
            paint.Ellipse(262f, 386f, 2.8f, 2f, new Color(0.55f, 0.30f, 0.14f, 0.5f), 15f);
            return paint.ToTexture("cacahuete-corps");
        }

        static float Shell(float px, float py, float cx, float lowerY, float upperY)
        {
            float dLower = EllipseSd(Rotate(px - cx, py - lowerY, 7f), 156f, 132f);
            float dUpper = EllipseSd(Rotate(px - cx, py - upperY, -11f), 116f, 108f);
            return SmoothMin(dLower, dUpper, 30f);
        }

        static Texture2D PaintArm()
        {
            var paint = new Painter(140, 300);
            var shell = new Color(0.90f, 0.66f, 0.36f, 1f);
            var deep = new Color(0.72f, 0.44f, 0.20f, 1f);
            var lit = new Color(0.99f, 0.90f, 0.72f, 1f);
            paint.Ellipse(70f, 168f, 32f, 92f, deep, 0f);
            paint.Ellipse(66f, 172f, 26f, 86f, shell, 0f);
            paint.Ellipse(58f, 196f, 10f, 40f, lit, 0f);
            paint.Ellipse(70f, 62f, 46f, 40f, deep, 0f);
            paint.Ellipse(68f, 66f, 40f, 34f, shell, 0f);
            paint.Ellipse(54f, 74f, 12f, 10f, new Color(1f, 0.95f, 0.82f, 0.85f), 0f);
            paint.Ellipse(52f, 48f, 10f, 8f, deep, 18f);
            paint.Ellipse(70f, 40f, 9f, 7f, deep, 0f);
            paint.Ellipse(88f, 48f, 10f, 8f, deep, -18f);
            return paint.ToTexture("cacahuete-bras");
        }

        static Texture2D PaintLeg()
        {
            var paint = new Painter(170, 250);
            var shell = new Color(0.86f, 0.60f, 0.32f, 1f);
            var boot = new Color(0.95f, 0.78f, 0.28f, 1f);
            var bootDeep = new Color(0.78f, 0.52f, 0.12f, 1f);
            var sole = new Color(0.55f, 0.32f, 0.10f, 1f);
            paint.Ellipse(85f, 205f, 28f, 36f, shell, 0f);
            paint.RoundRect(85f, 118f, 108f, 150f, 36f, bootDeep);
            paint.RoundRect(85f, 126f, 96f, 132f, 32f, boot);
            paint.RoundRect(85f, 78f, 112f, 36f, 16f, sole);
            paint.RoundRect(85f, 168f, 78f, 18f, 8f, new Color(0.99f, 0.96f, 0.90f, 1f));
            paint.Ellipse(62f, 142f, 14f, 28f, new Color(1f, 0.95f, 0.72f, 0.55f), 8f);
            return paint.ToTexture("cacahuete-jambe");
        }

        static Texture2D PaintSprout()
        {
            var paint = new Painter(220, 180);
            var stem = new Color(0.36f, 0.58f, 0.30f, 1f);
            var leaf = new Color(0.48f, 0.74f, 0.40f, 1f);
            var leafDeep = new Color(0.28f, 0.52f, 0.28f, 1f);
            var vein = new Color(0.22f, 0.42f, 0.22f, 0.65f);
            paint.RoundRect(108f, 42f, 16f, 70f, 8f, stem);
            paint.Ellipse(74f, 108f, 48f, 22f, leafDeep, 42f);
            paint.Ellipse(70f, 114f, 40f, 16f, leaf, 42f);
            paint.Ellipse(78f, 116f, 22f, 3f, vein, 40f);
            paint.Ellipse(148f, 116f, 40f, 18f, leafDeep, -36f);
            paint.Ellipse(150f, 122f, 32f, 13f, new Color(0.62f, 0.82f, 0.46f, 1f), -36f);
            paint.Ellipse(146f, 120f, 16f, 2.5f, vein, -34f);
            paint.Ellipse(86f, 124f, 8f, 4f, new Color(1f, 1f, 0.9f, 0.45f), 40f);
            return paint.ToTexture("cacahuete-pousse");
        }

        static Texture2D PaintEye(bool open)
        {
            var paint = new Painter(180, 220);
            var brow = new Color(0.38f, 0.20f, 0.10f, 1f);
            paint.Ellipse(90f, 176f, 48f, 9f, brow, open ? -8f : -4f);
            paint.Ellipse(118f, 168f, 16f, 5f, brow, 24f);
            if (!open)
            {
                paint.Ellipse(90f, 108f, 52f, 8f, new Color(0.30f, 0.16f, 0.09f, 1f), 6f);
                paint.Ellipse(90f, 114f, 40f, 4f, new Color(0.95f, 0.86f, 0.74f, 1f), 6f);
                return paint.ToTexture("cacahuete-clin");
            }

            var white = new Color(1f, 0.98f, 0.94f, 1f);
            var iris = new Color(0.43f, 0.26f, 0.14f, 1f);
            var irisLit = new Color(0.62f, 0.40f, 0.22f, 1f);
            var pupil = new Color(0.14f, 0.08f, 0.05f, 1f);
            paint.Ellipse(90f, 104f, 58f, 62f, new Color(0.35f, 0.18f, 0.09f, 1f), 0f);
            paint.Ellipse(90f, 106f, 52f, 56f, white, 0f);
            paint.Ellipse(90f, 132f, 46f, 22f, new Color(0.55f, 0.32f, 0.18f, 0.28f), 0f);
            paint.Ellipse(90f, 100f, 30f, 32f, iris, 0f);
            paint.Ellipse(84f, 108f, 18f, 16f, irisLit, 0f);
            paint.Ellipse(92f, 96f, 14f, 16f, pupil, 0f);
            paint.Ellipse(78f, 116f, 8f, 6f, Color.white, 20f);
            paint.Ellipse(104f, 88f, 4f, 3f, new Color(1f, 1f, 1f, 0.9f), 0f);
            paint.Ellipse(58f, 142f, 5f, 10f, brow, 30f);
            paint.Ellipse(74f, 152f, 4f, 9f, brow, 12f);
            return paint.ToTexture("cacahuete-oeil");
        }

        static Texture2D PaintFace(bool grin)
        {
            var paint = new Painter(200, 170);
            paint.Ellipse(100f, 132f, 12f, 8f, new Color(0.72f, 0.42f, 0.24f, 0.9f), 0f);
            paint.Ellipse(96f, 136f, 4f, 3f, new Color(1f, 0.86f, 0.74f, 0.7f), 0f);
            if (!grin)
            {
                SmileArc(paint, 100f, 78f, 46f, 7f, new Color(0.48f, 0.22f, 0.14f, 1f));
                return paint.ToTexture("cacahuete-sourire");
            }

            paint.Ellipse(100f, 62f, 40f, 28f, new Color(0.55f, 0.18f, 0.16f, 1f), 0f);
            paint.Ellipse(100f, 60f, 32f, 20f, new Color(0.90f, 0.48f, 0.48f, 1f), 0f);
            paint.RoundRect(100f, 74f, 36f, 14f, 4f, new Color(1f, 0.97f, 0.92f, 1f));
            SmileArc(paint, 100f, 86f, 52f, 6f, new Color(0.42f, 0.16f, 0.12f, 1f));
            return paint.ToTexture("cacahuete-rire");
        }

        static void SmileArc(Painter paint, float cx, float cy, float radius, float thickness, Color color)
        {
            int steps = 16;
            for (int i = 0; i <= steps; i++)
            {
                float u = i / (float)steps;
                float angle = Mathf.Lerp(200f, 340f, u) * Mathf.Deg2Rad;
                float x = cx + Mathf.Cos(angle) * radius;
                float y = cy + Mathf.Sin(angle) * radius;
                paint.Ellipse(x, y, thickness, thickness * 0.85f, color, 0f);
            }
        }

        static Texture2D PaintCheek()
        {
            var paint = new Painter(120, 80);
            paint.Ellipse(60f, 40f, 48f, 28f, new Color(0.95f, 0.55f, 0.52f, 0.55f), 0f);
            paint.Ellipse(48f, 46f, 16f, 10f, new Color(1f, 0.75f, 0.72f, 0.35f), 0f);
            return paint.ToTexture("cacahuete-joue");
        }

        static Texture2D PaintShadow()
        {
            var paint = new Painter(280, 90);
            paint.Ellipse(140f, 42f, 120f, 24f, new Color(0.32f, 0.16f, 0.08f, 0.28f), 0f);
            return paint.ToTexture("cacahuete-ombre");
        }

        static Texture2D PaintBubble()
        {
            var paint = new Painter(500, 190);
            var ink = new Color(0.40f, 0.24f, 0.12f, 1f);
            var paper = new Color(1f, 0.98f, 0.94f, 1f);
            paint.RoundRect(260f, 108f, 430f, 130f, 46f, ink);
            paint.Ellipse(92f, 42f, 28f, 22f, ink, 0f);
            paint.RoundRect(260f, 112f, 408f, 108f, 40f, paper);
            paint.Ellipse(96f, 48f, 20f, 15f, paper, 0f);
            return paint.ToTexture("cacahuete-bulle");
        }

        static Texture2D PaintBoard()
        {
            var paint = new Painter(700, 460);
            var border = new Color(0.48f, 0.28f, 0.14f, 1f);
            var wood = new Color(0.93f, 0.78f, 0.55f, 1f);
            var inset = new Color(0.99f, 0.95f, 0.88f, 1f);
            paint.RoundRect(350f, 230f, 660f, 420f, 48f, border);
            paint.RoundRect(350f, 234f, 628f, 388f, 40f, wood);
            paint.Grain(0.05f);
            paint.RoundRect(350f, 214f, 588f, 300f, 36f, inset);
            var leaf = new Color(0.42f, 0.66f, 0.36f, 1f);
            paint.Ellipse(600f, 390f, 22f, 10f, leaf, 30f);
            paint.Ellipse(628f, 398f, 16f, 8f, new Color(0.30f, 0.52f, 0.28f, 1f), -20f);
            return paint.ToTexture("cacahuete-panneau");
        }

        static Texture2D PaintField()
        {
            var paint = new Painter(560, 100);
            paint.RoundRect(280f, 50f, 540f, 88f, 28f, new Color(0.72f, 0.55f, 0.36f, 1f));
            paint.RoundRect(280f, 52f, 522f, 72f, 24f, new Color(1f, 0.99f, 0.97f, 1f));
            return paint.ToTexture("cacahuete-champ");
        }

        static Texture2D PaintSolid()
        {
            var paint = new Painter(4, 4);
            paint.RoundRect(2f, 2f, 8f, 8f, 0f, Color.white);
            return paint.ToTexture("cacahuete-aplat");
        }

        static Texture2D PaintButton()
        {
            var paint = new Painter(340, 100);
            paint.RoundRect(170f, 46f, 310f, 78f, 32f, new Color(0.62f, 0.34f, 0.12f, 1f));
            paint.RoundRect(170f, 52f, 298f, 66f, 28f, new Color(0.93f, 0.62f, 0.28f, 1f));
            paint.Ellipse(120f, 64f, 40f, 10f, new Color(1f, 0.86f, 0.62f, 0.45f), 0f);
            return paint.ToTexture("cacahuete-bouton");
        }

        static Vector2 Rotate(float x, float y, float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            return new Vector2(x * cos + y * sin, -x * sin + y * cos);
        }

        static float EllipseSd(Vector2 point, float rx, float ry)
        {
            return EllipseSd(point.x, point.y, rx, ry);
        }

        static float EllipseSd(float x, float y, float rx, float ry)
        {
            rx = Mathf.Max(0.5f, rx);
            ry = Mathf.Max(0.5f, ry);
            float nx = x / rx;
            float ny = y / ry;
            return (Mathf.Sqrt(nx * nx + ny * ny) - 1f) * Mathf.Min(rx, ry);
        }

        static float SmoothMin(float a, float b, float k)
        {
            float h = Mathf.Clamp01(0.5f + 0.5f * (b - a) / Mathf.Max(0.001f, k));
            return Mathf.Lerp(b, a, h) - k * h * (1f - h);
        }

        sealed class Painter
        {
            readonly int _width;
            readonly int _height;
            readonly Color32[] _pixels;

            public Painter(int width, int height)
            {
                _width = width;
                _height = height;
                _pixels = new Color32[width * height];
            }

            public void Ellipse(float cx, float cy, float rx, float ry, Color color, float rotationDegrees)
            {
                float reach = Mathf.Max(rx, ry) + 2f;
                int x0 = Mathf.Clamp(Mathf.FloorToInt(cx - reach), 0, _width - 1);
                int x1 = Mathf.Clamp(Mathf.CeilToInt(cx + reach), 0, _width - 1);
                int y0 = Mathf.Clamp(Mathf.FloorToInt(cy - reach), 0, _height - 1);
                int y1 = Mathf.Clamp(Mathf.CeilToInt(cy + reach), 0, _height - 1);
                float rad = rotationDegrees * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);
                for (int y = y0; y <= y1; y++)
                {
                    for (int x = x0; x <= x1; x++)
                    {
                        float dx = (x + 0.5f) - cx;
                        float dy = (y + 0.5f) - cy;
                        float lx = dx * cos + dy * sin;
                        float ly = -dx * sin + dy * cos;
                        float sd = EllipseSd(lx, ly, rx, ry);
                        float coverage = Mathf.Clamp01(0.8f - sd);
                        if (coverage > 0f)
                            Blend(x, y, color, coverage);
                    }
                }
            }

            public void RoundRect(float cx, float cy, float width, float height, float radius, Color color)
            {
                float hw = width * 0.5f;
                float hh = height * 0.5f;
                float rad = Mathf.Min(radius, Mathf.Min(hw, hh));
                int x0 = Mathf.Clamp(Mathf.FloorToInt(cx - hw - 2f), 0, _width - 1);
                int x1 = Mathf.Clamp(Mathf.CeilToInt(cx + hw + 2f), 0, _width - 1);
                int y0 = Mathf.Clamp(Mathf.FloorToInt(cy - hh - 2f), 0, _height - 1);
                int y1 = Mathf.Clamp(Mathf.CeilToInt(cy + hh + 2f), 0, _height - 1);
                for (int y = y0; y <= y1; y++)
                {
                    for (int x = x0; x <= x1; x++)
                    {
                        float sd = RoundedBox((x + 0.5f) - cx, (y + 0.5f) - cy, hw, hh, rad);
                        float coverage = Mathf.Clamp01(0.8f - sd);
                        if (coverage > 0f)
                            Blend(x, y, color, coverage);
                    }
                }
            }

            public void Grain(float amount)
            {
                for (int y = 0; y < _height; y++)
                {
                    float wave = 0.94f + amount * Mathf.Sin(y * 0.47f);
                    if (Mathf.Abs(Mathf.Sin(y * 0.29f + 0.4f)) > 0.94f)
                        wave *= 0.9f;
                    for (int x = 0; x < _width; x++)
                    {
                        int index = y * _width + x;
                        Color32 pixel = _pixels[index];
                        if (pixel.a == 0)
                            continue;
                        float n = wave + Mathf.Sin(x * 0.05f + y * 0.01f) * amount * 0.25f;
                        _pixels[index] = new Color32(Scale(pixel.r, n), Scale(pixel.g, n), Scale(pixel.b, n), pixel.a);
                    }
                }
            }

            public void Blend(int x, int y, Color src, float coverage)
            {
                float sa = src.a * Mathf.Clamp01(coverage);
                if (sa <= 0.001f)
                    return;
                int index = y * _width + x;
                Color32 dst = _pixels[index];
                float da = dst.a / 255f;
                float outA = sa + da * (1f - sa);
                if (outA <= 0.0001f)
                {
                    _pixels[index] = new Color32(0, 0, 0, 0);
                    return;
                }
                float inv = 1f - sa;
                float r = (src.r * sa + (dst.r / 255f) * da * inv) / outA;
                float g = (src.g * sa + (dst.g / 255f) * da * inv) / outA;
                float b = (src.b * sa + (dst.b / 255f) * da * inv) / outA;
                _pixels[index] = new Color32(ToByte(r), ToByte(g), ToByte(b), ToByte(outA));
            }

            public Texture2D ToTexture(string textureName)
            {
                var texture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
                texture.name = textureName;
                texture.SetPixels32(_pixels);
                texture.Apply(false, false);
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.filterMode = FilterMode.Bilinear;
                texture.hideFlags = HideFlags.HideAndDontSave;
                return texture;
            }

            static float RoundedBox(float x, float y, float halfW, float halfH, float radius)
            {
                float qx = Mathf.Abs(x) - (halfW - radius);
                float qy = Mathf.Abs(y) - (halfH - radius);
                float outside = new Vector2(Mathf.Max(qx, 0f), Mathf.Max(qy, 0f)).magnitude;
                float inside = Mathf.Min(Mathf.Max(qx, qy), 0f);
                return outside + inside - radius;
            }

            static byte Scale(byte channel, float factor)
            {
                return ToByte((channel / 255f) * factor);
            }

            static byte ToByte(float value)
            {
                return (byte)Mathf.Clamp(Mathf.RoundToInt(value * 255f), 0, 255);
            }
        }
    }
}
