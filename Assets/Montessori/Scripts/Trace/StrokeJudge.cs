using System.Collections.Generic;
using UnityEngine;

namespace Sensori.Montessori
{
    public enum StrokeFault
    {
        None = 0,
        Incomplete,
        WrongStart,
        WrongDirection,
        Scribble,
        OffPath
    }

    public readonly struct StrokeVerdict
    {
        public readonly bool Accepted;
        public readonly StrokeFault Fault;
        public readonly float Recall;
        public readonly float Precision;
        public readonly float LengthRatio;
        public readonly float Alignment;
        public readonly float StartDistance;

        public StrokeVerdict(
            bool accepted,
            StrokeFault fault,
            float recall,
            float precision,
            float lengthRatio,
            float alignment,
            float startDistance)
        {
            Accepted = accepted;
            Fault = fault;
            Recall = recall;
            Precision = precision;
            LengthRatio = lengthRatio;
            Alignment = alignment;
            StartDistance = startDistance;
        }
    }

    public static class StrokeJudge
    {
        const float BackWindow = 0.10f;
        const float ForwardWindow = 0.22f;
        const float CorridorSlack = 0.06f;

        public static StrokeVerdict Evaluate(IList<Vector2> guide, IList<Vector2> user, TracingThresholds thresholds)
        {
            thresholds = thresholds.Sanitized();
            if (guide == null || user == null || guide.Count < 2 || user.Count < 2)
                return new StrokeVerdict(false, StrokeFault.Incomplete, 0f, 0f, 0f, 0f, 0f);

            float guideLength = PolylineMath.Length(guide);
            float userLength = PolylineMath.Length(user);
            float lengthRatio = guideLength > 0.000001f ? userLength / guideLength : 0f;
            float startDistance = Vector2.Distance(user[0], guide[0]);
            float alignment = Alignment(guide, user);
            float endProgress = Farthest(guide, user);
            float recall = PolylineMath.Recall(guide, user, thresholds.MatchRadius);
            float precision = PolylineMath.Precision(guide, user, thresholds.MatchRadius);

            StrokeFault fault = StrokeFault.None;
            if (startDistance > thresholds.StartRadius)
                fault = StrokeFault.WrongStart;
            else if (alignment < thresholds.MinAlignment)
                fault = lengthRatio >= thresholds.MinLengthRatio && recall >= 0.55f && alignment > -0.05f
                    ? StrokeFault.Scribble
                    : StrokeFault.WrongDirection;
            else if (lengthRatio < thresholds.MinLengthRatio || recall < thresholds.MinRecall || endProgress < thresholds.MinEndProgress)
                fault = StrokeFault.Incomplete;
            else if (precision < thresholds.MinPrecision)
                fault = StrokeFault.OffPath;

            return new StrokeVerdict(fault == StrokeFault.None, fault, recall, precision, lengthRatio, alignment, startDistance);
        }

        static float Alignment(IList<Vector2> guide, IList<Vector2> user)
        {
            if (guide.Count < 2 || user.Count < 2)
                return 0f;
            float[] cumulative = Cumulative(guide);
            float progress = 0f;
            float travel = 0f;
            float previous = 0f;
            bool havePrevious = false;
            for (int i = 1; i < user.Count; i++)
            {
                Vector2 segment = user[i] - user[i - 1];
                float segmentLength = segment.magnitude;
                if (segmentLength < 0.0001f)
                    continue;
                Vector2 mid = (user[i] + user[i - 1]) * 0.5f;
                Projection projection = Project(guide, cumulative, mid, havePrevious, previous);
                float tangentLength = projection.Tangent.magnitude;
                if (tangentLength < 0.000001f)
                    continue;
                progress += Vector2.Dot(segment, projection.Tangent / tangentLength);
                travel += segmentLength;
                previous = projection.Arc;
                havePrevious = true;
            }
            if (travel < 0.000001f)
                return 0f;
            return progress / travel;
        }

        static float Farthest(IList<Vector2> guide, IList<Vector2> user)
        {
            float[] cumulative = Cumulative(guide);
            float total = cumulative[cumulative.Length - 1];
            if (total < 0.000001f)
                return 0f;
            float best = 0f;
            for (int i = 0; i < user.Count; i++)
            {
                Projection projection = Project(guide, cumulative, user[i], false, 0f);
                if (projection.Arc > best)
                    best = projection.Arc;
            }
            return best / total;
        }

        static Projection Project(IList<Vector2> guide, float[] cumulative, Vector2 point, bool havePrevious, float previous)
        {
            float globalDistance = float.MaxValue;
            float globalArc = 0f;
            Vector2 globalTangent = Vector2.right;
            float bestDistance = float.MaxValue;
            float bestArc = 0f;
            Vector2 bestTangent = Vector2.right;
            bool found = false;
            float total = cumulative[cumulative.Length - 1];
            float back = total * BackWindow;
            float forward = total * ForwardWindow;

            for (int i = 1; i < guide.Count; i++)
            {
                Vector2 a = guide[i - 1];
                Vector2 b = guide[i];
                Vector2 ab = b - a;
                float denom = ab.sqrMagnitude;
                float t = 0f;
                if (denom > 0.00000001f)
                    t = Mathf.Clamp01(Vector2.Dot(point - a, ab) / denom);
                Vector2 on = a + ab * t;
                float spatial = Vector2.Distance(on, point);
                float segment = denom > 0.00000001f ? Mathf.Sqrt(denom) : 0f;
                float arc = cumulative[i - 1] + segment * t;
                Vector2 tangent = denom > 0.00000001f ? ab : Vector2.right;
                if (spatial < globalDistance)
                {
                    globalDistance = spatial;
                    globalArc = arc;
                    globalTangent = tangent;
                }
                if (havePrevious && (arc < previous - back || arc > previous + forward))
                    continue;
                if (spatial < bestDistance)
                {
                    bestDistance = spatial;
                    bestArc = arc;
                    bestTangent = tangent;
                    found = true;
                }
            }

            if (!havePrevious || !found || bestDistance > globalDistance + CorridorSlack)
                return new Projection { Arc = globalArc, Tangent = globalTangent };
            return new Projection { Arc = bestArc, Tangent = bestTangent };
        }

        static float[] Cumulative(IList<Vector2> guide)
        {
            var cumulative = new float[guide.Count];
            for (int i = 1; i < guide.Count; i++)
                cumulative[i] = cumulative[i - 1] + Vector2.Distance(guide[i - 1], guide[i]);
            return cumulative;
        }

        struct Projection
        {
            public float Arc;
            public Vector2 Tangent;
        }
    }
}
