using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Sensori.Montessori
{
    public static class StrokeLibrary
    {
        public static StrokePath[] For(string key)
        {
            if (string.IsNullOrEmpty(key))
                return new[] { Circle(0.5f, 0.5f, 0.34f, 0.34f, 28) };
            switch (key)
            {
                case "letter-a": return Lines(S("0.16,0.10 0.50,0.90 0.84,0.10"), S("0.30,0.42 0.70,0.42"));
                case "letter-b": return Lines(S("0.26,0.10 0.26,0.90"), S("0.26,0.90 0.58,0.90 0.74,0.78 0.74,0.62 0.58,0.50 0.26,0.50"), S("0.26,0.50 0.62,0.50 0.78,0.36 0.78,0.22 0.60,0.10 0.26,0.10"));
                case "letter-c": return Lines(Arc(0.52f, 0.50f, 0.34f, 0.40f, 55f, 305f, 24));
                case "letter-d": return Lines(S("0.26,0.10 0.26,0.90"), S("0.26,0.90 0.52,0.90 0.78,0.74 0.78,0.26 0.52,0.10 0.26,0.10"));
                case "letter-e": return Lines(S("0.28,0.10 0.28,0.90"), S("0.28,0.90 0.74,0.90"), S("0.28,0.50 0.64,0.50"), S("0.28,0.10 0.74,0.10"));
                case "letter-f": return Lines(S("0.30,0.10 0.30,0.90"), S("0.30,0.90 0.76,0.90"), S("0.30,0.50 0.66,0.50"));
                case "letter-g": return Lines(Arc(0.48f, 0.50f, 0.34f, 0.40f, 50f, 310f, 24), S("0.70,0.50 0.48,0.50"));
                case "letter-h": return Lines(S("0.24,0.10 0.24,0.90"), S("0.76,0.10 0.76,0.90"), S("0.24,0.50 0.76,0.50"));
                case "letter-i": return Lines(S("0.32,0.90 0.68,0.90"), S("0.50,0.90 0.50,0.10"), S("0.32,0.10 0.68,0.10"));
                case "letter-j": return Lines(S("0.38,0.90 0.72,0.90"), S("0.62,0.90 0.62,0.28 0.52,0.12 0.34,0.12 0.24,0.24"));
                case "letter-k": return Lines(S("0.28,0.10 0.28,0.90"), S("0.72,0.90 0.32,0.48"), S("0.44,0.58 0.78,0.10"));
                case "letter-l": return Lines(S("0.30,0.90 0.30,0.10 0.76,0.10"));
                case "letter-m": return Lines(S("0.12,0.10 0.12,0.90 0.50,0.42 0.88,0.90 0.88,0.10"));
                case "letter-n": return Lines(S("0.20,0.10 0.20,0.90 0.80,0.10 0.80,0.90"));
                case "letter-o": return Lines(Circle(0.50f, 0.50f, 0.32f, 0.40f, 28));
                case "letter-p": return Lines(S("0.28,0.10 0.28,0.90"), S("0.28,0.90 0.62,0.90 0.78,0.76 0.78,0.60 0.60,0.46 0.28,0.46"));
                case "letter-q": return Lines(Circle(0.46f, 0.50f, 0.28f, 0.38f, 28), S("0.62,0.34 0.84,0.10"));
                case "letter-r": return Lines(S("0.28,0.10 0.28,0.90"), S("0.28,0.90 0.62,0.90 0.78,0.76 0.78,0.60 0.60,0.46 0.28,0.46"), S("0.52,0.46 0.80,0.10"));
                case "letter-s": return Lines(S("0.74,0.78 0.60,0.90 0.38,0.90 0.24,0.78 0.26,0.64 0.40,0.54 0.62,0.46 0.76,0.34 0.74,0.20 0.58,0.10 0.36,0.10 0.22,0.22"));
                case "letter-t": return Lines(S("0.18,0.90 0.82,0.90"), S("0.50,0.90 0.50,0.10"));
                case "letter-u": return Lines(S("0.22,0.90 0.22,0.36 0.34,0.12 0.66,0.12 0.78,0.36 0.78,0.90"));
                case "letter-v": return Lines(S("0.14,0.90 0.50,0.10 0.86,0.90"));
                case "letter-w": return Lines(S("0.08,0.90 0.28,0.10 0.50,0.62 0.72,0.10 0.92,0.90"));
                case "letter-x": return Lines(S("0.18,0.90 0.82,0.10"), S("0.82,0.90 0.18,0.10"));
                case "letter-y": return Lines(S("0.16,0.90 0.50,0.48 0.84,0.90"), S("0.50,0.48 0.50,0.10"));
                case "letter-z": return Lines(S("0.18,0.90 0.82,0.90 0.18,0.10 0.82,0.10"));
                case "digit-0": return Lines(Circle(0.50f, 0.50f, 0.28f, 0.40f, 28));
                case "digit-1": return Lines(S("0.36,0.68 0.54,0.90 0.54,0.10"), S("0.34,0.10 0.74,0.10"));
                case "digit-2": return Lines(S("0.24,0.72 0.32,0.88 0.56,0.90 0.74,0.76 0.70,0.58 0.22,0.14 0.80,0.10"));
                case "digit-3": return Lines(S("0.26,0.78 0.40,0.90 0.64,0.90 0.78,0.76 0.72,0.60 0.52,0.50"), S("0.52,0.50 0.74,0.40 0.78,0.24 0.62,0.10 0.36,0.10 0.22,0.24"));
                case "digit-4": return Lines(S("0.70,0.10 0.70,0.90"), S("0.70,0.86 0.22,0.36 0.80,0.36"));
                case "digit-5": return Lines(S("0.76,0.88 0.30,0.88 0.28,0.54 0.48,0.58 0.70,0.50 0.76,0.32 0.62,0.12 0.34,0.12 0.22,0.26"));
                case "digit-6": return Lines(S("0.68,0.82 0.48,0.92 0.30,0.78 0.26,0.40 0.36,0.16 0.56,0.12 0.72,0.24 0.70,0.42 0.50,0.50 0.32,0.40"));
                case "digit-7": return Lines(S("0.22,0.86 0.78,0.86 0.40,0.10"), S("0.36,0.52 0.66,0.52"));
                case "digit-8": return Lines(Circle(0.50f, 0.68f, 0.22f, 0.18f, 18), Circle(0.50f, 0.32f, 0.26f, 0.20f, 18));
                case "digit-9": return Lines(Circle(0.48f, 0.66f, 0.24f, 0.20f, 18), S("0.70,0.66 0.68,0.28 0.50,0.10 0.30,0.16"));
                case "shape-circle": return Lines(Circle(0.50f, 0.50f, 0.36f, 0.36f, 32));
                case "shape-square": return Lines(S("0.18,0.18 0.82,0.18 0.82,0.82 0.18,0.82 0.18,0.18"));
                case "shape-triangle": return Lines(S("0.50,0.88 0.88,0.16 0.12,0.16 0.50,0.88"));
                case "shape-rectangle": return Lines(S("0.12,0.28 0.88,0.28 0.88,0.72 0.12,0.72 0.12,0.28"));
                case "shape-star": return Lines(Star());
                case "shape-heart": return Lines(Heart());
                case "shape-oval": return Lines(Circle(0.50f, 0.50f, 0.28f, 0.40f, 28));
                case "shape-diamond": return Lines(S("0.50,0.90 0.86,0.50 0.50,0.10 0.14,0.50 0.50,0.90"));
                default:
                    return new[] { Circle(0.5f, 0.5f, 0.34f, 0.34f, 28) };
            }
        }

        static StrokePath[] Lines(params StrokePath[] paths)
        {
            return paths;
        }

        static StrokePath S(string spec)
        {
            var parts = spec.Split(' ');
            var list = new List<Vector2>(parts.Length);
            var culture = CultureInfo.InvariantCulture;
            for (int i = 0; i < parts.Length; i++)
            {
                var xy = parts[i].Split(',');
                if (xy.Length != 2)
                    continue;
                if (float.TryParse(xy[0], NumberStyles.Float, culture, out float x) &&
                    float.TryParse(xy[1], NumberStyles.Float, culture, out float y))
                    list.Add(new Vector2(x, y));
            }
            return new StrokePath { Points = list.ToArray() };
        }

        static StrokePath Circle(float cx, float cy, float rx, float ry, int steps)
        {
            steps = Mathf.Max(8, steps);
            var points = new Vector2[steps + 1];
            for (int i = 0; i <= steps; i++)
            {
                float angle = Mathf.PI * 0.5f + (i / (float)steps) * Mathf.PI * 2f;
                points[i] = new Vector2(cx + Mathf.Cos(angle) * rx, cy + Mathf.Sin(angle) * ry);
            }
            return new StrokePath { Points = points };
        }

        static StrokePath Arc(float cx, float cy, float rx, float ry, float startDeg, float endDeg, int steps)
        {
            steps = Mathf.Max(4, steps);
            var points = new Vector2[steps + 1];
            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;
                float angle = Mathf.Lerp(startDeg, endDeg, t) * Mathf.Deg2Rad;
                points[i] = new Vector2(cx + Mathf.Cos(angle) * rx, cy + Mathf.Sin(angle) * ry);
            }
            return new StrokePath { Points = points };
        }

        static StrokePath Star()
        {
            var points = new Vector2[11];
            for (int i = 0; i <= 10; i++)
            {
                float angle = (90f - i * 36f) * Mathf.Deg2Rad;
                float radius = (i % 2 == 0) ? 0.40f : 0.17f;
                points[i] = new Vector2(0.5f + Mathf.Cos(angle) * radius, 0.5f + Mathf.Sin(angle) * radius);
            }
            return new StrokePath { Points = points };
        }

        static StrokePath Heart()
        {
            const int steps = 36;
            var points = new Vector2[steps + 1];
            for (int i = 0; i <= steps; i++)
            {
                float t = Mathf.Lerp(0f, Mathf.PI * 2f, i / (float)steps);
                float x = 16f * Mathf.Pow(Mathf.Sin(t), 3f);
                float y = 13f * Mathf.Cos(t) - 5f * Mathf.Cos(2f * t) - 2f * Mathf.Cos(3f * t) - Mathf.Cos(4f * t);
                points[i] = new Vector2(0.5f + x / 42f, 0.46f + y / 42f);
            }
            return new StrokePath { Points = points };
        }
    }
}
