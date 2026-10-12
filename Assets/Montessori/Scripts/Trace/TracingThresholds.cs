using System;

namespace Sensori.Montessori
{
    [Serializable]
    public struct TracingThresholds
    {
        public float MinRecall;
        public float MinPrecision;
        public float MinLengthRatio;
        public float MatchRadius;
        public float StartRadius;
        public float MinAlignment;
        public float MinEndProgress;

        public static TracingThresholds Standard()
        {
            return new TracingThresholds
            {
                MinRecall = 0.78f,
                MinPrecision = 0.50f,
                MinLengthRatio = 0.70f,
                MatchRadius = 0.11f,
                StartRadius = 0.16f,
                MinAlignment = 0.40f,
                MinEndProgress = 0.66f
            };
        }

        public TracingThresholds Sanitized()
        {
            TracingThresholds standard = Standard();
            TracingThresholds copy = this;
            if (copy.MinRecall <= 0f)
                copy.MinRecall = standard.MinRecall;
            if (copy.MinPrecision <= 0f)
                copy.MinPrecision = standard.MinPrecision;
            if (copy.MinLengthRatio <= 0f)
                copy.MinLengthRatio = standard.MinLengthRatio;
            if (copy.MatchRadius <= 0f)
                copy.MatchRadius = standard.MatchRadius;
            if (copy.StartRadius <= 0f)
                copy.StartRadius = standard.StartRadius;
            if (copy.MinAlignment <= 0f)
                copy.MinAlignment = standard.MinAlignment;
            if (copy.MinEndProgress <= 0f)
                copy.MinEndProgress = standard.MinEndProgress;
            return copy;
        }
    }
}
