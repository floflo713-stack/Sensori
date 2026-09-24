using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public static class ScreenBackdrop
    {
        public static void Ensure(Transform parent, string scene)
        {
            if (!Application.isPlaying || parent == null)
                return;
            var found = parent.Find("Paysage");
            Image image;
            if (found == null)
            {
                image = UiFactory.Picture("Paysage", parent, HomeLandscape.For(scene), Color.white, false, false);
                UiFactory.Stretch(image.rectTransform, 0f, 0f, 0f, 0f);
            }
            else
            {
                image = found.GetComponent<Image>();
                if (image == null)
                    image = found.gameObject.AddComponent<Image>();
                if (image.sprite == null)
                    image.sprite = HomeLandscape.For(scene);
                image.color = Color.white;
                image.type = Image.Type.Simple;
            }
            image.preserveAspect = false;
            image.raycastTarget = false;
            image.transform.SetAsFirstSibling();
        }
    }

    public static class HomeLandscape
    {
        static Sprite _sprite;
        static readonly Dictionary<string, Sprite> Scenes = new Dictionary<string, Sprite>();

        public static Sprite Sprite
        {
            get
            {
                if (_sprite != null)
                    return _sprite;
                var texture = Paint(1280, 720);
                _sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
                _sprite.name = "paysage-accueil";
                return _sprite;
            }
        }

        public static Sprite For(string scene)
        {
            if (string.IsNullOrEmpty(scene) || scene == "accueil")
                return Sprite;
            if (Scenes.TryGetValue(scene, out var cached) && cached != null)
                return cached;
            var texture = PaintScene(scene);
            var sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            sprite.name = "paysage-" + scene;
            Scenes[scene] = sprite;
            return sprite;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetCache()
        {
            Release(_sprite);
            _sprite = null;
            foreach (var pair in Scenes)
                Release(pair.Value);
            Scenes.Clear();
        }

        static void Release(Sprite sprite)
        {
            if (sprite == null)
                return;
            var texture = sprite.texture;
            Object.DestroyImmediate(sprite);
            if (texture != null)
                Object.DestroyImmediate(texture);
        }

        static Texture2D Paint(int width, int height)
        {
            var park = new Park(width, height);
            park.Sky(0.14f, 0.84f, new Color(0.74f, 0.84f, 0.92f), new Color(0.98f, 0.91f, 0.82f), new Color(1f, 0.86f, 0.62f));
            park.Sun(0.14f, 0.84f);
            park.Cloud(0.34f, 0.78f, 1f);
            park.Cloud(0.58f, 0.86f, 0.72f);
            park.Cloud(0.78f, 0.74f, 0.88f);
            park.Bird(0.46f, 0.80f, 0.8f);
            park.Bird(0.62f, 0.88f, 0.55f);

            var far = Rgb(176, 206, 184);
            var mid = Rgb(132, 178, 138);
            var near = Rgb(168, 196, 124);
            var meadow = Rgb(214, 226, 176);
            var clearing = Rgb(236, 232, 206);
            park.Ellipse(0.18f, 0.36f, 0.48f, 0.16f, far);
            park.Ellipse(0.62f, 0.38f, 0.58f, 0.17f, far);
            park.Ellipse(1.02f, 0.34f, 0.36f, 0.14f, Rgb(188, 214, 196));
            park.Ellipse(0.08f, 0.28f, 0.42f, 0.15f, mid);
            park.Ellipse(0.48f, 0.26f, 0.5f, 0.14f, near);
            park.Ellipse(0.92f, 0.29f, 0.4f, 0.15f, mid);
            park.Ellipse(0.5f, 0.06f, 0.78f, 0.22f, meadow);
            park.Ellipse(0.5f, 0.05f, 0.34f, 0.1f, clearing);

            park.Pond(0.24f, 0.1f);
            park.DistantTree(0.30f, 0.40f, 0.55f);
            park.DistantTree(0.70f, 0.42f, 0.48f);
            park.DistantTree(0.40f, 0.37f, 0.36f);

            park.Tree(0.05f, 0.22f, 1.15f, true);
            park.Tree(0.13f, 0.20f, 0.82f, false);
            park.Tree(0.20f, 0.24f, 0.62f, false);
            park.Tree(0.95f, 0.21f, 1.2f, false);
            park.Tree(0.86f, 0.23f, 0.9f, true);
            park.Tree(0.78f, 0.19f, 0.58f, false);
            park.Bush(0.17f, 0.18f, 0.9f);
            park.Bush(0.82f, 0.17f, 1f);

            park.Flower(0.07f, 0.08f, Rgb(226, 120, 140), 1.1f);
            park.Flower(0.12f, 0.14f, Rgb(232, 176, 78), 0.85f);
            park.Flower(0.17f, 0.06f, Rgb(120, 168, 210), 0.95f);
            park.Flower(0.31f, 0.09f, Rgb(226, 120, 140), 0.75f);
            park.Flower(0.70f, 0.08f, Rgb(232, 176, 78), 0.8f);
            park.Flower(0.84f, 0.07f, Rgb(226, 120, 140), 1f);
            park.Flower(0.90f, 0.14f, Rgb(120, 168, 210), 0.9f);
            park.Flower(0.96f, 0.08f, Rgb(232, 176, 78), 0.7f);

            park.Finish();
            return park.ToTexture("paysage-accueil");
        }

        static Texture2D PaintScene(string scene)
        {
            var park = new Park(1280, 720);
            if (scene == "puzzle")
                PaintPuzzle(park);
            else if (scene == "imagier")
                PaintImagier(park);
            else if (scene == "trace")
                PaintTracing(park);
            else
                PaintCategories(park);
            park.Finish();
            return park.ToTexture("paysage-" + scene);
        }

        static void PaintCategories(Park park)
        {
            park.Sky(0.84f, 0.86f, new Color(0.78f, 0.86f, 0.90f), new Color(0.97f, 0.92f, 0.84f), new Color(1f, 0.90f, 0.70f));
            park.Sun(0.84f, 0.86f);
            park.Cloud(0.22f, 0.80f, 0.9f);
            park.Cloud(0.48f, 0.88f, 0.6f);
            park.Bird(0.36f, 0.84f, 0.7f);
            park.Ellipse(0.2f, 0.34f, 0.5f, 0.15f, Rgb(186, 210, 186));
            park.Ellipse(0.78f, 0.32f, 0.48f, 0.14f, Rgb(150, 188, 146));
            park.Ellipse(0.5f, 0.08f, 0.8f, 0.24f, Rgb(226, 232, 198));
            park.Ellipse(0.5f, 0.2f, 0.42f, 0.28f, Rgb(244, 238, 220));
            park.Tree(0.02f, 0.16f, 1.05f, true);
            park.Tree(0.98f, 0.15f, 1.1f, false);
            park.Bush(0.06f, 0.12f, 0.8f);
            park.Bush(0.94f, 0.11f, 0.85f);
            park.Flower(0.03f, 0.05f, Rgb(226, 120, 140), 1f);
            park.Flower(0.08f, 0.09f, Rgb(232, 176, 78), 0.8f);
            park.Flower(0.94f, 0.06f, Rgb(120, 168, 210), 0.9f);
            park.Flower(0.97f, 0.12f, Rgb(226, 120, 140), 0.75f);
            park.HeaderWash();
        }

        static void PaintPuzzle(Park park)
        {
            park.Sky(0.16f, 0.78f, new Color(0.93f, 0.78f, 0.62f), new Color(0.98f, 0.90f, 0.78f), new Color(1f, 0.82f, 0.48f));
            park.Sun(0.16f, 0.78f);
            park.Cloud(0.42f, 0.84f, 0.7f);
            park.Cloud(0.68f, 0.76f, 0.85f);
            park.Ellipse(0.15f, 0.30f, 0.46f, 0.16f, Rgb(196, 168, 112));
            park.Ellipse(0.7f, 0.28f, 0.55f, 0.15f, Rgb(176, 154, 102));
            park.Ellipse(0.5f, 0.06f, 0.82f, 0.22f, Rgb(232, 214, 170));
            park.Ellipse(0.5f, 0.18f, 0.4f, 0.26f, Rgb(246, 236, 214));
            park.Tree(0.03f, 0.18f, 1.15f, true);
            park.Tree(0.1f, 0.2f, 0.7f, true);
            park.Tree(0.96f, 0.17f, 1.05f, true);
            park.Bush(0.94f, 0.12f, 0.7f);
            park.Flower(0.04f, 0.06f, Rgb(214, 96, 72), 0.7f);
            park.Flower(0.96f, 0.07f, Rgb(232, 176, 78), 0.85f);
            park.HeaderWash();
        }

        static void PaintImagier(Park park)
        {
            park.Sky(0.5f, 0.9f, new Color(0.86f, 0.78f, 0.88f), new Color(0.99f, 0.92f, 0.90f), new Color(1f, 0.84f, 0.78f));
            park.Sun(0.5f, 0.9f);
            park.Cloud(0.22f, 0.74f, 0.8f);
            park.Cloud(0.78f, 0.76f, 0.75f);
            park.Butterfly(0.12f, 0.62f);
            park.Butterfly(0.9f, 0.58f);
            park.Ellipse(0.5f, 0.22f, 0.7f, 0.2f, Rgb(220, 206, 196));
            park.Ellipse(0.12f, 0.16f, 0.28f, 0.12f, Rgb(186, 206, 154));
            park.Ellipse(0.9f, 0.14f, 0.26f, 0.12f, Rgb(196, 176, 186));
            park.Ellipse(0.5f, 0.16f, 0.36f, 0.24f, Rgb(248, 240, 232));
            park.Flower(0.04f, 0.2f, Rgb(226, 120, 140), 1.2f);
            park.Flower(0.08f, 0.1f, Rgb(186, 140, 206), 1f);
            park.Flower(0.03f, 0.05f, Rgb(232, 176, 78), 0.8f);
            park.Flower(0.94f, 0.18f, Rgb(226, 120, 140), 1.15f);
            park.Flower(0.97f, 0.08f, Rgb(120, 168, 210), 0.9f);
            park.Flower(0.9f, 0.05f, Rgb(232, 176, 78), 0.75f);
            park.Bush(0.06f, 0.28f, 0.7f);
            park.Bush(0.95f, 0.26f, 0.65f);
            park.HeaderWash();
        }

        static void PaintTracing(Park park)
        {
            park.Sky(0.8f, 0.82f, new Color(0.70f, 0.82f, 0.90f), new Color(0.94f, 0.94f, 0.90f), new Color(1f, 0.92f, 0.74f));
            park.Sun(0.8f, 0.82f);
            park.Cloud(0.28f, 0.78f, 0.85f);
            park.Cloud(0.52f, 0.86f, 0.55f);
            park.Bird(0.4f, 0.8f, 0.6f);
            park.Ellipse(0.2f, 0.3f, 0.4f, 0.12f, Rgb(176, 198, 186));
            park.Ellipse(0.82f, 0.28f, 0.36f, 0.11f, Rgb(160, 188, 176));
            park.Ellipse(0.5f, 0.08f, 0.9f, 0.16f, Rgb(214, 226, 214));
            park.Ellipse(0.5f, 0.045f, 0.55f, 0.035f, Rgb(150, 196, 214));
            park.Ellipse(0.42f, 0.055f, 0.08f, 0.012f, new Color32(230, 244, 248, 160));
            park.Tree(0.02f, 0.18f, 0.9f, false);
            park.Tree(0.98f, 0.17f, 0.85f, false);
            park.Reed(0.06f, 0.08f);
            park.Reed(0.1f, 0.06f);
            park.Reed(0.92f, 0.09f);
            park.Reed(0.96f, 0.05f);
            park.Ellipse(0.5f, 0.2f, 0.38f, 0.26f, Rgb(242, 242, 234));
            park.HeaderWash();
        }

        static Color32 Rgb(byte r, byte g, byte b)
        {
            return new Color32(r, g, b, 255);
        }

        sealed class Park
        {
            readonly int _width;
            readonly int _height;
            readonly Color32[] _pixels;

            public Park(int width, int height)
            {
                _width = width;
                _height = height;
                _pixels = new Color32[width * height];
            }

            public void Sky(float sunX, float sunY, Color top, Color horizon, Color blush)
            {
                for (int y = 0; y < _height; y++)
                {
                    float ny = y / (float)(_height - 1);
                    float lift = Mathf.SmoothStep(0.28f, 1f, ny);
                    var row = Color.Lerp(horizon, top, lift);
                    for (int x = 0; x < _width; x++)
                    {
                        float nx = x / (float)(_width - 1);
                        float dx = nx - sunX;
                        float dy = ny - sunY;
                        float sun = Mathf.Exp(-(dx * dx * 5.5f + dy * dy * 7f));
                        var color = Color.Lerp(row, blush, sun * 0.45f);
                        _pixels[y * _width + x] = (Color32)color;
                    }
                }
            }

            public void HeaderWash()
            {
                Ellipse(0.5f, 1.02f, 0.72f, 0.16f, new Color32(248, 242, 230, 150));
            }

            public void Butterfly(float x, float y)
            {
                var wing = Rgb(226, 150, 176);
                Ellipse(x - 0.012f, y, 0.016f, 0.01f, wing, -30f);
                Ellipse(x + 0.012f, y, 0.016f, 0.01f, wing, 30f);
                Ellipse(x, y, 0.004f, 0.012f, Rgb(110, 74, 48));
            }

            public void Reed(float x, float y)
            {
                Ellipse(x, y + 0.04f, 0.004f, 0.045f, Rgb(96, 140, 88), -8f);
                Circle(x + 0.008f, y + 0.08f, 0.012f, Rgb(140, 176, 120));
            }

            public void Sun(float x, float y)
            {
                Circle(x, y, 0.11f, new Color32(255, 214, 140, 70));
                Circle(x, y, 0.055f, new Color32(255, 214, 120, 210));
                Circle(x, y, 0.032f, Rgb(255, 236, 170));
            }

            public void Cloud(float x, float y, float scale)
            {
                var shade = new Color32(186, 206, 220, 90);
                var puff = Rgb(255, 252, 247);
                Ellipse(x, y - 0.012f * scale, 0.09f * scale, 0.032f * scale, shade);
                Ellipse(x, y, 0.075f * scale, 0.034f * scale, puff);
                Ellipse(x - 0.05f * scale, y - 0.008f * scale, 0.048f * scale, 0.026f * scale, puff);
                Ellipse(x + 0.055f * scale, y - 0.006f * scale, 0.044f * scale, 0.024f * scale, puff);
                Ellipse(x + 0.012f * scale, y + 0.014f * scale, 0.04f * scale, 0.026f * scale, puff);
            }

            public void Bird(float x, float y, float scale)
            {
                var ink = new Color32(96, 72, 58, 170);
                Ellipse(x - 0.012f * scale, y, 0.018f * scale, 0.0055f * scale, ink, -26f);
                Ellipse(x + 0.012f * scale, y, 0.018f * scale, 0.0055f * scale, ink, 26f);
            }

            public void Pond(float x, float y)
            {
                Ellipse(x, y, 0.075f, 0.026f, Rgb(150, 196, 214));
                Ellipse(x - 0.012f, y + 0.004f, 0.028f, 0.008f, new Color32(230, 244, 248, 180));
            }

            public void DistantTree(float x, float ground, float scale)
            {
                var leaf = Rgb(150, 186, 164);
                Ellipse(x, ground + 0.03f * scale, 0.008f * scale, 0.035f * scale, Rgb(168, 140, 112));
                Circle(x, ground + 0.07f * scale, 0.028f * scale, leaf);
            }

            public void Tree(float x, float ground, float scale, bool fruit)
            {
                var trunk = Rgb(110, 74, 48);
                var leaf = Rgb(78, 140, 86);
                var deep = Rgb(58, 112, 72);
                var light = Rgb(156, 196, 112);
                Ellipse(x, ground + 0.055f * scale, 0.012f * scale, 0.07f * scale, trunk);
                Circle(x - 0.01f * scale, ground + 0.1f * scale, 0.048f * scale, deep);
                Circle(x + 0.028f * scale, ground + 0.115f * scale, 0.046f * scale, leaf);
                Circle(x - 0.03f * scale, ground + 0.13f * scale, 0.04f * scale, leaf);
                Circle(x, ground + 0.155f * scale, 0.042f * scale, light);
                if (!fruit)
                    return;
                Circle(x + 0.02f * scale, ground + 0.12f * scale, 0.008f * scale, Rgb(226, 96, 96));
                Circle(x - 0.02f * scale, ground + 0.14f * scale, 0.007f * scale, Rgb(232, 176, 72));
            }

            public void Bush(float x, float y, float scale)
            {
                var leaf = Rgb(96, 154, 96);
                Circle(x, y, 0.028f * scale, leaf);
                Circle(x - 0.03f * scale, y - 0.004f, 0.02f * scale, Rgb(72, 130, 80));
                Circle(x + 0.028f * scale, y - 0.002f, 0.018f * scale, Rgb(140, 184, 110));
            }

            public void Flower(float x, float y, Color32 petal, float scale)
            {
                float r = 0.009f * scale;
                Circle(x, y + r * 1.05f, r, petal);
                Circle(x - r * 0.95f, y - r * 0.15f, r * 0.9f, petal);
                Circle(x + r * 0.95f, y - r * 0.15f, r * 0.9f, petal);
                Circle(x, y, r * 0.5f, Rgb(255, 236, 170));
            }

            public void Finish()
            {
                for (int y = 0; y < _height; y++)
                {
                    float ny = y / (float)(_height - 1);
                    float dy = ny - 0.5f;
                    for (int x = 0; x < _width; x++)
                    {
                        float nx = x / (float)(_width - 1);
                        float dx = nx - 0.5f;
                        float vignette = Mathf.Clamp01(dx * dx * 1.15f + dy * dy * 1.35f);
                        float grain = (Hash(x / 2, y / 2) - 0.5f) * 0.03f;
                        int index = y * _width + x;
                        var pixel = _pixels[index];
                        float shade = 1f - vignette * 0.07f + grain;
                        _pixels[index] = new Color32(Channel(pixel.r, shade), Channel(pixel.g, shade), Channel(pixel.b, shade), 255);
                    }
                }
            }

            public Texture2D ToTexture(string textureName)
            {
                var texture = new Texture2D(_width, _height, TextureFormat.RGBA32, false);
                texture.name = textureName;
                texture.SetPixels32(_pixels);
                texture.Apply(false, false);
                texture.wrapMode = TextureWrapMode.Clamp;
                texture.filterMode = FilterMode.Bilinear;
                return texture;
            }

            public void Circle(float cx, float cy, float radius, Color32 color)
            {
                float px = radius * _height;
                FillEllipse(cx * _width, cy * _height, px, px, color, 0f);
            }

            public void Ellipse(float cx, float cy, float rx, float ry, Color32 color, float rotationDegrees = 0f)
            {
                FillEllipse(cx * _width, cy * _height, Mathf.Max(0.6f, rx * _width), Mathf.Max(0.6f, ry * _height), color, rotationDegrees);
            }

            void FillEllipse(float cxp, float cyp, float rxp, float ryp, Color32 color, float rotationDegrees)
            {
                float reachX = rxp + ryp + 2f;
                float reachY = ryp + rxp + 2f;
                int x0 = Mathf.Clamp(Mathf.FloorToInt(cxp - reachX), 0, _width - 1);
                int x1 = Mathf.Clamp(Mathf.CeilToInt(cxp + reachX), 0, _width - 1);
                int y0 = Mathf.Clamp(Mathf.FloorToInt(cyp - reachY), 0, _height - 1);
                int y1 = Mathf.Clamp(Mathf.CeilToInt(cyp + reachY), 0, _height - 1);
                float rad = rotationDegrees * Mathf.Deg2Rad;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);
                float edge = Mathf.Min(rxp, ryp);
                for (int y = y0; y <= y1; y++)
                {
                    for (int x = x0; x <= x1; x++)
                    {
                        float dx = (x + 0.5f) - cxp;
                        float dy = (y + 0.5f) - cyp;
                        float lx = dx * cos + dy * sin;
                        float ly = -dx * sin + dy * cos;
                        float sd = Mathf.Sqrt((lx / rxp) * (lx / rxp) + (ly / ryp) * (ly / ryp)) - 1f;
                        float coverage = Mathf.Clamp01(0.85f - sd * edge);
                        if (coverage <= 0f)
                            continue;
                        Blend(x, y, color, coverage);
                    }
                }
            }

            void Blend(int x, int y, Color32 src, float coverage)
            {
                float sa = (src.a / 255f) * coverage;
                if (sa <= 0.001f)
                    return;
                int index = y * _width + x;
                if (sa >= 0.995f)
                {
                    _pixels[index] = new Color32(src.r, src.g, src.b, 255);
                    return;
                }
                Color32 dst = _pixels[index];
                float inv = 1f - sa;
                _pixels[index] = new Color32(
                    (byte)(src.r * sa + dst.r * inv),
                    (byte)(src.g * sa + dst.g * inv),
                    (byte)(src.b * sa + dst.b * inv),
                    255);
            }

            static byte Channel(byte source, float shade)
            {
                return (byte)Mathf.Clamp(Mathf.RoundToInt(source * shade), 0, 255);
            }

            static float Hash(int x, int y)
            {
                int n = x * 374761393 + y * 668265263;
                n = (n ^ (n >> 13)) * 1274126177;
                return ((n ^ (n >> 16)) & 0x7fffffff) / (float)int.MaxValue;
            }
        }
    }
}
