using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class LivingLandscape : MonoBehaviour
    {
        const float SunDegreesPerSecond = 4f;
        const float FarSpeed = 22f;
        const float NearSpeed = 46f;

        struct Slot
        {
            public RectTransform Rect;
            public float Along;
            public float Lift;
            public Vector2 Size;
            public bool Clone;
        }

        readonly List<Slot> _slots = new List<Slot>();
        RectTransform _sun;
        RectTransform _pond;
        RectTransform _glint;
        RectTransform _farBand;
        RectTransform _nearBand;
        float _spin;
        float _span;
        float _farOffset;
        float _nearOffset;
        float _farSeed = 0.12f;
        float _nearSeed = 0.48f;
        bool _seeded;
        bool _built;

        public static void Mount(Transform parent, string scene)
        {
            var found = parent.Find("Paysage");
            RectTransform rect;
            if (found == null)
            {
                rect = UiFactory.Rect("Paysage", parent);
                UiFactory.Stretch(rect, 0f, 0f, 0f, 0f);
            }
            else
            {
                rect = (RectTransform)found;
                UiFactory.Stretch(rect, 0f, 0f, 0f, 0f);
                var legacy = rect.GetComponent<Image>();
                if (legacy != null)
                {
                    legacy.sprite = null;
                    legacy.enabled = false;
                    Destroy(legacy);
                }
            }

            var stage = rect.GetComponent<LivingLandscape>();
            if (stage == null)
                stage = rect.gameObject.AddComponent<LivingLandscape>();
            var group = rect.GetComponent<CanvasGroup>();
            if (group == null)
                group = rect.gameObject.AddComponent<CanvasGroup>();
            group.alpha = 1f;
            group.interactable = false;
            group.blocksRaycasts = false;
            stage.EnsureBuilt(scene);
            rect.SetAsFirstSibling();
        }

        void EnsureBuilt(string scene)
        {
            if (_built)
                return;
            _built = true;
            _farSeed = 0.08f + Seed(scene) * 0.37f;
            _nearSeed = 0.41f + Seed(scene) * 0.29f;

            var decor = Picture("Decor", transform, LandscapeArt.Scenery);
            UiFactory.Stretch(decor.rectTransform, 0f, 0f, 0f, 0f);
            decor.preserveAspect = false;

            var sun = Picture("Soleil", transform, LandscapeArt.Sun);
            _sun = sun.rectTransform;
            _sun.anchorMin = new Vector2(0.17f, 0.80f);
            _sun.anchorMax = new Vector2(0.17f, 0.80f);
            _sun.pivot = new Vector2(0.5f, 0.5f);
            _sun.sizeDelta = new Vector2(430f, 430f);
            _sun.anchoredPosition = Vector2.zero;
            sun.preserveAspect = true;

            _farBand = Band("NuagesLoin", 0.84f);
            _nearBand = Band("NuagesPres", 0.73f);
            AddCloud(_farBand, 0.05f, 8f, new Vector2(250f, 108f));
            AddCloud(_farBand, 0.42f, -6f, new Vector2(310f, 128f));
            AddCloud(_farBand, 0.76f, 12f, new Vector2(270f, 114f));
            AddCloud(_nearBand, 0.18f, -8f, new Vector2(360f, 148f));
            AddCloud(_nearBand, 0.55f, 10f, new Vector2(300f, 124f));
            AddCloud(_nearBand, 0.86f, -2f, new Vector2(240f, 104f));

            var pond = Picture("Mare", transform, LandscapeArt.Pond);
            _pond = pond.rectTransform;
            _pond.anchorMin = new Vector2(0.15f, 0.105f);
            _pond.anchorMax = new Vector2(0.15f, 0.105f);
            _pond.pivot = new Vector2(0.5f, 0.5f);
            _pond.sizeDelta = new Vector2(340f, 136f);
            _pond.anchoredPosition = Vector2.zero;
            pond.preserveAspect = true;

            var glint = Picture("Reflet", _pond, LandscapeArt.Glint);
            _glint = glint.rectTransform;
            UiFactory.AnchorCenter(_glint, new Vector2(-22f, 12f), new Vector2(128f, 40f));
            glint.preserveAspect = true;
        }

        void LateUpdate()
        {
            if (!_built)
                return;
            float width = ((RectTransform)transform).rect.width;
            if (width > 8f && Mathf.Abs(width - _span) > 1.5f)
            {
                _span = width;
                if (!_seeded)
                {
                    _farOffset = Mathf.Repeat(_farSeed, 1f) * _span;
                    _nearOffset = Mathf.Repeat(_nearSeed, 1f) * _span;
                    _seeded = true;
                }
                else
                {
                    _farOffset = Mathf.Repeat(_farOffset, _span);
                    _nearOffset = Mathf.Repeat(_nearOffset, _span);
                }
                LayoutClouds();
            }

            _spin -= SunDegreesPerSecond * Time.unscaledDeltaTime;
            if (_sun != null)
                _sun.localRotation = Quaternion.Euler(0f, 0f, _spin);

            Drift(_farBand, ref _farOffset, FarSpeed);
            Drift(_nearBand, ref _nearOffset, NearSpeed);
            Ripple();
        }

        void Drift(RectTransform band, ref float offset, float speed)
        {
            if (band == null || _span < 8f)
                return;
            offset += speed * Time.unscaledDeltaTime;
            if (offset >= _span)
                offset -= _span;
            var pos = band.anchoredPosition;
            pos.x = -offset;
            band.anchoredPosition = pos;
        }

        void Ripple()
        {
            if (_pond == null)
                return;
            float t = Time.unscaledTime;
            float wave = Mathf.Sin(t * 1.7f);
            _pond.localScale = new Vector3(1f + wave * 0.035f, 1f - wave * 0.05f, 1f);
            if (_glint == null)
                return;
            _glint.anchoredPosition = new Vector2(-18f + Mathf.Sin(t * 1.25f) * 18f, 8f + Mathf.Cos(t * 1.05f) * 5f);
            float pulse = 0.86f + (Mathf.Sin(t * 2.3f) + 1f) * 0.09f;
            _glint.localScale = new Vector3(pulse, 0.72f + pulse * 0.18f, 1f);
        }

        void LayoutClouds()
        {
            _farBand.sizeDelta = new Vector2(_span * 2f, 20f);
            _nearBand.sizeDelta = new Vector2(_span * 2f, 20f);
            for (int i = 0; i < _slots.Count; i++)
            {
                var slot = _slots[i];
                if (slot.Rect == null)
                    continue;
                float x = slot.Along * _span + (slot.Clone ? _span : 0f);
                slot.Rect.anchoredPosition = new Vector2(x, slot.Lift);
                slot.Rect.sizeDelta = slot.Size;
            }
        }

        RectTransform Band(string name, float yAnchor)
        {
            var rect = UiFactory.Rect(name, transform);
            rect.anchorMin = new Vector2(0f, yAnchor);
            rect.anchorMax = new Vector2(0f, yAnchor);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(4000f, 0f);
            rect.sizeDelta = new Vector2(20f, 20f);
            return rect;
        }

        void AddCloud(RectTransform band, float along, float lift, Vector2 size)
        {
            AddSlot(band, along, lift, size, false);
            AddSlot(band, along, lift, size, true);
        }

        void AddSlot(RectTransform band, float along, float lift, Vector2 size, bool clone)
        {
            var image = Picture(clone ? "NuageBis" : "Nuage", band, LandscapeArt.Cloud);
            image.preserveAspect = true;
            var rect = image.rectTransform;
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(4000f, lift);
            rect.sizeDelta = size;
            _slots.Add(new Slot
            {
                Rect = rect,
                Along = along,
                Lift = lift,
                Size = size,
                Clone = clone
            });
        }

        static Image Picture(string name, Transform parent, Sprite sprite)
        {
            var image = UiFactory.Picture(name, parent, sprite, Color.white, false, false);
            image.raycastTarget = false;
            return image;
        }

        static float Seed(string scene)
        {
            if (string.IsNullOrEmpty(scene))
                return 0.2f;
            float value = 0.13f;
            for (int i = 0; i < scene.Length; i++)
                value = Mathf.Repeat(value + scene[i] * 0.017f, 1f);
            return value;
        }
    }

    static class LandscapeArt
    {
        static Sprite _scenery;
        static Sprite _sun;
        static Sprite _cloud;
        static Sprite _pond;
        static Sprite _glint;

        public static Sprite Scenery => _scenery ?? (_scenery = SpriteOf(PaintScenery(), "paysage-vivant"));
        public static Sprite Sun => _sun ?? (_sun = SpriteOf(PaintSun(), "soleil"));
        public static Sprite Cloud => _cloud ?? (_cloud = SpriteOf(PaintCloud(), "nuage"));
        public static Sprite Pond => _pond ?? (_pond = SpriteOf(PaintPond(), "mare"));
        public static Sprite Glint => _glint ?? (_glint = SpriteOf(PaintGlint(), "reflet"));

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetCache()
        {
            Release(ref _scenery);
            Release(ref _sun);
            Release(ref _cloud);
            Release(ref _pond);
            Release(ref _glint);
        }

        static Sprite SpriteOf(Texture2D texture, string spriteName)
        {
            var sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            sprite.name = spriteName;
            return sprite;
        }

        static void Release(ref Sprite sprite)
        {
            if (sprite == null)
                return;
            var texture = sprite.texture;
            Object.DestroyImmediate(sprite);
            if (texture != null)
                Object.DestroyImmediate(texture);
            sprite = null;
        }

        static Texture2D PaintScenery()
        {
            var park = new Raster(1280, 720);
            park.Fill(Rgb(0, 174, 239));

            var far = Rgb(42, 176, 48);
            var farLip = Rgb(170, 226, 74);
            var mid = Rgb(58, 196, 44);
            var midLip = Rgb(190, 232, 82);
            var apple = Rgb(126, 220, 42);
            var soil = Rgb(48, 168, 30);

            park.Hill(0.20f, 0.47f, 0.38f, 0.09f, far, farLip);
            park.Hill(0.64f, 0.485f, 0.44f, 0.095f, far, farLip);
            park.Hill(1.04f, 0.46f, 0.30f, 0.085f, far, farLip);

            park.Hill(0.06f, 0.40f, 0.36f, 0.085f, mid, midLip);
            park.Hill(0.52f, 0.39f, 0.42f, 0.08f, mid, midLip);
            park.Hill(0.94f, 0.405f, 0.36f, 0.088f, mid, midLip);

            park.Tree(0.33f, 0.46f, 0.5f, false);
            park.Tree(0.40f, 0.47f, 0.4f, false);
            park.Tree(0.63f, 0.47f, 0.48f, false);
            park.Tree(0.71f, 0.465f, 0.38f, false);

            park.Tree(0.07f, 0.30f, 1.7f, true);
            park.Tree(0.16f, 0.31f, 1.15f, true);
            park.Bush(0.015f, 0.30f, 1.35f);
            park.Bush(0.21f, 0.29f, 0.95f);

            park.Tree(0.81f, 0.31f, 1.2f, true);
            park.Tree(0.93f, 0.30f, 1.75f, true);
            park.Bush(0.985f, 0.29f, 1.2f);
            park.Bush(0.75f, 0.30f, 0.9f);

            park.Ellipse(0.5f, 0.02f, 0.98f, 0.34f, apple);
            park.Ellipse(0.5f, -0.08f, 1.2f, 0.16f, soil);
            return park.ToTexture("paysage-vivant");
        }

        static Texture2D PaintSun()
        {
            var park = new Raster(512, 512);
            var ray = Rgb(255, 196, 12);
            var gold = Rgb(255, 214, 0);
            var core = Rgb(255, 164, 8);
            for (int i = 0; i < 12; i++)
            {
                float ang = i * 30f;
                float rad = ang * Mathf.Deg2Rad;
                float cx = 0.5f + Mathf.Cos(rad) * 0.30f;
                float cy = 0.5f + Mathf.Sin(rad) * 0.30f;
                park.Ellipse(cx, cy, 0.10f, 0.036f, ray, ang);
            }
            park.Circle(0.5f, 0.5f, 0.185f, gold);
            park.Circle(0.5f, 0.5f, 0.105f, core);
            return park.ToTexture("soleil");
        }

        static Texture2D PaintCloud()
        {
            var park = new Raster(512, 256);
            var puff = Rgb(255, 255, 255);
            park.Circle(0.50f, 0.40f, 0.34f, puff);
            park.Circle(0.28f, 0.38f, 0.26f, puff);
            park.Circle(0.74f, 0.40f, 0.24f, puff);
            park.Circle(0.40f, 0.58f, 0.22f, puff);
            park.Circle(0.64f, 0.56f, 0.20f, puff);
            return park.ToTexture("nuage");
        }

        static Texture2D PaintPond()
        {
            var park = new Raster(480, 200);
            park.Ellipse(0.5f, 0.42f, 0.48f, 0.42f, Rgb(16, 112, 190));
            park.Ellipse(0.5f, 0.48f, 0.40f, 0.30f, Rgb(32, 168, 232));
            return park.ToTexture("mare");
        }

        static Texture2D PaintGlint()
        {
            var park = new Raster(256, 96);
            park.Ellipse(0.5f, 0.5f, 0.42f, 0.34f, new Color32(214, 244, 255, 230));
            return park.ToTexture("reflet");
        }

        static Color32 Rgb(byte r, byte g, byte b)
        {
            return new Color32(r, g, b, 255);
        }

        sealed class Raster
        {
            readonly int _width;
            readonly int _height;
            readonly Color32[] _pixels;

            public Raster(int width, int height)
            {
                _width = width;
                _height = height;
                _pixels = new Color32[width * height];
            }

            public void Fill(Color32 color)
            {
                for (int i = 0; i < _pixels.Length; i++)
                    _pixels[i] = color;
            }

            public void Hill(float x, float y, float rx, float ry, Color32 body, Color32 lip)
            {
                Ellipse(x, y + ry * 0.22f, rx, ry, lip);
                Ellipse(x, y, rx, ry, body);
            }

            public void Tree(float x, float ground, float scale, bool full)
            {
                var trunk = Rgb(168, 98, 42);
                var leaf = Rgb(64, 176, 40);
                var light = Rgb(148, 214, 56);
                var deep = Rgb(34, 146, 32);
                Ellipse(x, ground + 0.048f * scale, 0.014f * scale, 0.062f * scale, trunk);
                if (full)
                {
                    Ellipse(x - 0.02f * scale, ground + 0.09f * scale, 0.046f * scale, 0.015f * scale, trunk, 62f);
                    Ellipse(x + 0.022f * scale, ground + 0.095f * scale, 0.042f * scale, 0.014f * scale, trunk, -56f);
                }
                float crown = full ? 0.125f : 0.085f;
                Circle(x - 0.008f * scale, ground + crown * scale, 0.052f * scale, deep);
                Circle(x + 0.03f * scale, ground + (crown + 0.012f) * scale, 0.048f * scale, leaf);
                if (full)
                    Circle(x - 0.018f * scale, ground + (crown + 0.028f) * scale, 0.04f * scale, light);
            }

            public void Bush(float x, float y, float scale)
            {
                var leaf = Rgb(52, 168, 36);
                var deep = Rgb(30, 132, 28);
                var light = Rgb(132, 206, 52);
                Circle(x, y, 0.032f * scale, leaf);
                Circle(x - 0.028f * scale, y - 0.004f, 0.024f * scale, deep);
                Circle(x + 0.026f * scale, y + 0.006f, 0.022f * scale, light);
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
                float edge = Mathf.Max(1.25f, Mathf.Min(rxp, ryp));
                for (int y = y0; y <= y1; y++)
                {
                    for (int x = x0; x <= x1; x++)
                    {
                        float dx = (x + 0.5f) - cxp;
                        float dy = (y + 0.5f) - cyp;
                        float lx = dx * cos + dy * sin;
                        float ly = -dx * sin + dy * cos;
                        float sd = Mathf.Sqrt((lx / rxp) * (lx / rxp) + (ly / ryp) * (ly / ryp)) - 1f;
                        float coverage = Mathf.Clamp01(0.65f - sd * edge);
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
                Color32 dst = _pixels[index];
                float da = dst.a / 255f;
                float outA = sa + da * (1f - sa);
                if (outA <= 0.001f)
                    return;
                float srcW = sa / outA;
                float dstW = da * (1f - sa) / outA;
                _pixels[index] = new Color32(
                    Mix(src.r, dst.r, srcW, dstW),
                    Mix(src.g, dst.g, srcW, dstW),
                    Mix(src.b, dst.b, srcW, dstW),
                    (byte)Mathf.Clamp(Mathf.RoundToInt(outA * 255f), 0, 255));
            }

            static byte Mix(byte a, byte b, float wa, float wb)
            {
                return (byte)Mathf.Clamp(Mathf.RoundToInt(a * wa + b * wb), 0, 255);
            }
        }
    }
}
