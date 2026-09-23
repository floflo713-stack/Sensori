using System.Collections.Generic;
using UnityEngine;

namespace Sensori.Montessori
{
    public static class PolylineMath
    {
        public static void Smooth(IList<Vector2> source, List<Vector2> destination, int subdivisions)
        {
            destination.Clear();
            if (source == null || source.Count == 0)
                return;
            if (source.Count == 1)
            {
                destination.Add(source[0]);
                return;
            }

            subdivisions = Mathf.Max(1, subdivisions);
            for (int i = 0; i < source.Count - 1; i++)
            {
                Vector2 p0 = source[Mathf.Max(i - 1, 0)];
                Vector2 p1 = source[i];
                Vector2 p2 = source[i + 1];
                Vector2 p3 = source[Mathf.Min(i + 2, source.Count - 1)];
                for (int s = 0; s < subdivisions; s++)
                {
                    float t = s / (float)subdivisions;
                    destination.Add(Catmull(p0, p1, p2, p3, t));
                }
            }
            destination.Add(source[source.Count - 1]);
        }

        public static void Resample(IList<Vector2> source, float spacing, List<Vector2> destination)
        {
            destination.Clear();
            if (source == null || source.Count == 0)
                return;
            destination.Add(source[0]);
            if (source.Count == 1 || spacing <= 0.0001f)
                return;

            float accumulated = 0f;
            Vector2 previous = source[0];
            int guard = 0;
            for (int i = 1; i < source.Count; i++)
            {
                Vector2 current = source[i];
                float segment = Vector2.Distance(previous, current);
                if (segment < 0.000001f)
                    continue;
                while (accumulated + segment >= spacing && guard < 8000)
                {
                    float remain = spacing - accumulated;
                    float t = remain / segment;
                    Vector2 point = Vector2.Lerp(previous, current, t);
                    destination.Add(point);
                    previous = point;
                    segment = Vector2.Distance(previous, current);
                    accumulated = 0f;
                    guard++;
                    if (segment < 0.000001f)
                        break;
                }
                accumulated += segment;
                previous = current;
            }

            if (Vector2.Distance(destination[destination.Count - 1], source[source.Count - 1]) > spacing * 0.35f)
                destination.Add(source[source.Count - 1]);
        }

        public static float Recall(IList<Vector2> guide, IList<Vector2> user, float radius)
        {
            if (guide == null || guide.Count == 0)
                return 1f;
            if (user == null || user.Count == 0)
                return 0f;
            float radiusSq = radius * radius;
            int hits = 0;
            for (int i = 0; i < guide.Count; i++)
            {
                if (Near(guide[i], user, radiusSq))
                    hits++;
            }
            return hits / (float)guide.Count;
        }

        public static float Precision(IList<Vector2> guide, IList<Vector2> user, float radius)
        {
            if (user == null || user.Count == 0)
                return 0f;
            if (guide == null || guide.Count == 0)
                return 0f;
            float radiusSq = radius * radius;
            int hits = 0;
            for (int i = 0; i < user.Count; i++)
            {
                if (Near(user[i], guide, radiusSq))
                    hits++;
            }
            return hits / (float)user.Count;
        }

        public static float Length(IList<Vector2> points)
        {
            if (points == null || points.Count < 2)
                return 0f;
            float total = 0f;
            for (int i = 1; i < points.Count; i++)
                total += Vector2.Distance(points[i - 1], points[i]);
            return total;
        }

        public static Vector2 PointAt(IList<Vector2> points, float distance)
        {
            if (points == null || points.Count == 0)
                return Vector2.zero;
            if (points.Count == 1 || distance <= 0f)
                return points[0];
            float walked = 0f;
            for (int i = 1; i < points.Count; i++)
            {
                float segment = Vector2.Distance(points[i - 1], points[i]);
                if (walked + segment >= distance)
                {
                    float t = segment <= 0.0001f ? 0f : (distance - walked) / segment;
                    return Vector2.Lerp(points[i - 1], points[i], t);
                }
                walked += segment;
            }
            return points[points.Count - 1];
        }

        static bool Near(Vector2 point, IList<Vector2> others, float radiusSq)
        {
            for (int i = 0; i < others.Count; i++)
            {
                if ((others[i] - point).sqrMagnitude <= radiusSq)
                    return true;
            }
            return false;
        }

        static Vector2 Catmull(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            return 0.5f * (
                (2f * p1) +
                (-p0 + p2) * t +
                (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
                (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
        }
    }
}
