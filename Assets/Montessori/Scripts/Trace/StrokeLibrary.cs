using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Sensori.Montessori
{
    public static partial class StrokeLibrary
    {
        public static StrokePath[] For(string key)
        {
            if (string.IsNullOrEmpty(key))
                return new[] { Circle(0.5f, 0.5f, 0.34f, 0.34f, 28) };
            switch (key)
            {
                case "letter-a": return Cursive('a');
                case "letter-b": return Cursive('b');
                case "letter-c": return Cursive('c');
                case "letter-d": return Cursive('d');
                case "letter-e": return Cursive('e');
                case "letter-f": return Cursive('f');
                case "letter-g": return Cursive('g');
                case "letter-h": return Cursive('h');
                case "letter-i": return Cursive('i');
                case "letter-j": return Cursive('j');
                case "letter-k": return Cursive('k');
                case "letter-l": return Cursive('l');
                case "letter-m": return Cursive('m');
                case "letter-n": return Cursive('n');
                case "letter-o": return Cursive('o');
                case "letter-p": return Cursive('p');
                case "letter-q": return Cursive('q');
                case "letter-r": return Cursive('r');
                case "letter-s": return Cursive('s');
                case "letter-t": return Cursive('t');
                case "letter-u": return Cursive('u');
                case "letter-v": return Cursive('v');
                case "letter-w": return Cursive('w');
                case "letter-x": return Cursive('x');
                case "letter-y": return Cursive('y');
                case "letter-z": return Cursive('z');
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

        public static StrokePath[] Resolve(StrokePath[] authored, string itemId)
        {
            if (HasPoints(authored))
                return authored;
            if (!string.IsNullOrEmpty(itemId))
                return For(itemId);
            return For("shape-circle");
        }

        static bool HasPoints(StrokePath[] strokes)
        {
            if (strokes == null || strokes.Length == 0)
                return false;
            for (int i = 0; i < strokes.Length; i++)
            {
                StrokePath stroke = strokes[i];
                if (stroke != null && stroke.Points != null && stroke.Points.Length >= 2)
                    return true;
            }
            return false;
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
