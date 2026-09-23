using UnityEngine;

namespace Sensori.Montessori
{
    public static class WoodTextures
    {
        public static Texture2D CreatePlate(int size, int radius, Color32 baseColor, bool inset, float grain)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = inset ? "wood-inset" : "wood-plate";
            var pixels = new Color32[size * size];
            float half = size * 0.5f;
            float bevel = Mathf.Max(8f, radius * 0.45f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float px = x + 0.5f;
                    float py = y + 0.5f;
                    float sd = RoundedBox(px - half, py - half, half - 2f, half - 2f, radius);
                    float alpha = Mathf.Clamp01(0.85f - sd);
                    if (alpha <= 0.001f)
                    {
                        pixels[y * size + x] = new Color32(0, 0, 0, 0);
                        continue;
                    }

                    float ndx = SampleSd(px + 1.25f, py, half, radius) - SampleSd(px - 1.25f, py, half, radius);
                    float ndy = SampleSd(px, py + 1.25f, half, radius) - SampleSd(px, py - 1.25f, half, radius);
                    float nLen = Mathf.Sqrt(ndx * ndx + ndy * ndy);
                    float nx = nLen > 0.0001f ? ndx / nLen : 0f;
                    float ny = nLen > 0.0001f ? ndy / nLen : 1f;
                    float light = Mathf.Clamp01(0.5f + nx * -0.55f + ny * 0.72f);
                    float edge = Mathf.Clamp01(1f - (-sd) / bevel);
                    float raised = Mathf.Lerp(0.78f, 1.18f, light);
                    float shade = Mathf.Lerp(1f, inset ? Mathf.Lerp(1.12f, 0.62f, light) : raised, edge);
                    if (inset)
                    {
                        float center = Mathf.Clamp01((-sd) / (size * 0.42f));
                        shade *= Mathf.Lerp(0.96f, 0.78f, center);
                    }
                    else if (-sd > 2f && -sd < 7f && ny > 0.35f)
                    {
                        shade += 0.08f;
                    }

                    float streak = Mathf.PerlinNoise(px * 0.018f, py * 0.006f);
                    float wave = Mathf.Sin(py * 0.42f + Mathf.Sin(px * 0.025f) * 3.2f);
                    float pores = Hash(x / 2, y) * 2f - 1f;
                    float grainMix = (streak - 0.5f) * 0.16f + wave * 0.045f + pores * 0.035f;
                    shade *= 1f + grainMix * grain;

                    pixels[y * size + x] = new Color32(
                        Channel(baseColor.r, shade),
                        Channel(baseColor.g, shade),
                        Channel(baseColor.b, shade),
                        (byte)Mathf.Clamp(Mathf.RoundToInt(alpha * 255f), 0, 255));
                }
            }
            texture.SetPixels32(pixels);
            texture.Apply(false, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            return texture;
        }

        public static Texture2D CreatePearl(int size)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = "pearl";
            var pixels = new Color32[size * size];
            float half = size * 0.5f;
            float radius = half - 1.5f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x + 0.5f - half) / radius;
                    float dy = (y + 0.5f - half) / radius;
                    float d = Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = Mathf.Clamp01((1f - d) * 10f);
                    if (alpha <= 0.001f)
                    {
                        pixels[y * size + x] = new Color32(0, 0, 0, 0);
                        continue;
                    }
                    float light = Mathf.Clamp01(1.12f - d * 0.38f + (-dx) * 0.16f + dy * 0.18f);
                    float rim = Mathf.Clamp01((d - 0.72f) / 0.28f);
                    light *= Mathf.Lerp(1f, 0.72f, rim);
                    byte c = (byte)Mathf.Clamp(Mathf.RoundToInt(light * 255f), 0, 255);
                    pixels[y * size + x] = new Color32(c, c, c, (byte)Mathf.Clamp(Mathf.RoundToInt(alpha * 255f), 0, 255));
                }
            }
            texture.SetPixels32(pixels);
            texture.Apply(false, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            return texture;
        }

        public static Texture2D CreateShadow(int size)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = "shadow";
            var pixels = new Color32[size * size];
            float half = size * 0.5f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = (x + 0.5f - half) / (half * 0.92f);
                    float dy = (y + 0.5f - half) / (half * 0.72f);
                    float d = dx * dx + dy * dy;
                    float alpha = Mathf.Exp(-d * 2.15f);
                    byte a = (byte)Mathf.Clamp(Mathf.RoundToInt(alpha * 255f), 0, 255);
                    pixels[y * size + x] = new Color32(255, 255, 255, a);
                }
            }
            texture.SetPixels32(pixels);
            texture.Apply(false, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            return texture;
        }

        public static Texture2D CreateChip(int size, int radius)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = "chip";
            var pixels = new Color32[size * size];
            float half = size * 0.5f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float sd = RoundedBox(x + 0.5f - half, y + 0.5f - half, half - 1.5f, half - 1.5f, radius);
                    float alpha = Mathf.Clamp01(0.8f - sd);
                    byte a = (byte)Mathf.Clamp(Mathf.RoundToInt(alpha * 255f), 0, 255);
                    pixels[y * size + x] = new Color32(255, 255, 255, a);
                }
            }
            texture.SetPixels32(pixels);
            texture.Apply(false, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            return texture;
        }

        public static Texture2D CreateBackground(int width, int height)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.name = "paper";
            var pixels = new Color32[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float nx = x / (float)(width - 1);
                    float ny = y / (float)(height - 1);
                    float dx = nx - 0.5f;
                    float dy = ny - 0.45f;
                    float vignette = Mathf.Clamp01(dx * dx * 1.3f + dy * dy * 1.6f);
                    float fiber = Mathf.Sin(y * 0.85f + Mathf.Sin(x * 0.03f)) * 0.012f;
                    float noise = (Hash(x / 3, y / 2) - 0.5f) * 0.035f;
                    float sun = Mathf.Exp(-((nx - 0.18f) * (nx - 0.18f) + (ny - 0.82f) * (ny - 0.82f)) * 8f);
                    float warm = Mathf.Exp(-((nx - 0.86f) * (nx - 0.86f) + (ny - 0.12f) * (ny - 0.12f)) * 7f);
                    float r = 0.965f + fiber + noise - vignette * 0.08f + sun * 0.03f + warm * 0.02f;
                    float g = 0.941f + fiber + noise - vignette * 0.07f + sun * 0.025f + warm * 0.012f;
                    float b = 0.902f + fiber * 0.5f + noise - vignette * 0.05f + sun * 0.01f;
                    pixels[y * width + x] = new Color32(Channel(255, r), Channel(255, g), Channel(255, b), 255);
                }
            }
            texture.SetPixels32(pixels);
            texture.Apply(false, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            return texture;
        }

        public static Texture2D CreateSolid(int size)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = "solid";
            var pixels = new Color32[size * size];
            var white = new Color32(255, 255, 255, 255);
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = white;
            texture.SetPixels32(pixels);
            texture.Apply(false, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            return texture;
        }

        static float SampleSd(float px, float py, float half, float radius)
        {
            return RoundedBox(px - half, py - half, half - 2f, half - 2f, radius);
        }

        static float RoundedBox(float x, float y, float halfW, float halfH, float radius)
        {
            float qx = Mathf.Abs(x) - (halfW - radius);
            float qy = Mathf.Abs(y) - (halfH - radius);
            float outside = new Vector2(Mathf.Max(qx, 0f), Mathf.Max(qy, 0f)).magnitude;
            float inside = Mathf.Min(Mathf.Max(qx, qy), 0f);
            return outside + inside - radius;
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
