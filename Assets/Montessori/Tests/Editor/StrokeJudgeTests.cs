using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Sensori.Montessori.Tests
{
    public sealed class StrokeJudgeTests
    {
        [Test]
        public void ForwardStroke_IsAccepted()
        {
            List<Vector2> line = VerticalLine(24);
            StrokeVerdict verdict = StrokeJudge.Evaluate(line, line, TracingThresholds.Standard());
            Assert.IsTrue(verdict.Accepted, verdict.Fault.ToString());
        }

        [Test]
        public void ReversedStroke_IsRejected()
        {
            List<Vector2> line = VerticalLine(24);
            StrokeVerdict verdict = StrokeJudge.Evaluate(line, Reverse(line), TracingThresholds.Standard());
            Assert.IsFalse(verdict.Accepted);
            Assert.AreEqual(StrokeFault.WrongStart, verdict.Fault);
        }

        [Test]
        public void ClosedShape_TracedBackwards_IsRejected()
        {
            StrokePath[] zero = StrokeLibrary.For("digit-0");
            List<Vector2> circle = Copy(zero[0].Points);
            StrokeVerdict forward = StrokeJudge.Evaluate(circle, circle, TracingThresholds.Standard());
            StrokeVerdict backward = StrokeJudge.Evaluate(circle, Reverse(circle), TracingThresholds.Standard());
            Assert.IsTrue(forward.Accepted, forward.Fault.ToString());
            Assert.IsFalse(backward.Accepted);
            Assert.AreEqual(StrokeFault.WrongDirection, backward.Fault);
        }

        [Test]
        public void ScribbleCoveringTheModel_IsRejected()
        {
            List<Vector2> line = VerticalLine(24);
            StrokeVerdict verdict = StrokeJudge.Evaluate(line, BackAndForth(line, 3), TracingThresholds.Standard());
            Assert.IsFalse(verdict.Accepted);
            Assert.AreEqual(StrokeFault.Scribble, verdict.Fault);
            Assert.GreaterOrEqual(verdict.Recall, 0.78f);
            Assert.GreaterOrEqual(verdict.Precision, 0.50f);
            Assert.GreaterOrEqual(verdict.LengthRatio, 0.70f);
        }

        [Test]
        public void StrokeThatStopsHalfway_IsIncomplete()
        {
            List<Vector2> line = VerticalLine(24);
            StrokeVerdict verdict = StrokeJudge.Evaluate(line, Partial(line, 0.45f), TracingThresholds.Standard());
            Assert.IsFalse(verdict.Accepted);
            Assert.AreEqual(StrokeFault.Incomplete, verdict.Fault);
        }

        [Test]
        public void StrokeThatStartsAwayFromTheDot_IsWrongStart()
        {
            List<Vector2> line = VerticalLine(24);
            List<Vector2> shifted = new List<Vector2>(line);
            shifted[0] = line[0] + new Vector2(0.28f, 0f);
            StrokeVerdict verdict = StrokeJudge.Evaluate(line, shifted, TracingThresholds.Standard());
            Assert.IsFalse(verdict.Accepted);
            Assert.AreEqual(StrokeFault.WrongStart, verdict.Fault);
        }

        [Test]
        public void WobblyForwardStroke_StaysAccepted()
        {
            List<Vector2> line = VerticalLine(32);
            StrokeVerdict verdict = StrokeJudge.Evaluate(line, Wobble(line, 0.03f), TracingThresholds.Standard());
            Assert.IsTrue(verdict.Accepted, verdict.Fault.ToString());
        }

        [Test]
        public void Thresholds_AreConfigurable()
        {
            List<Vector2> line = VerticalLine(24);
            List<Vector2> half = Partial(line, 0.55f);
            TracingThresholds strict = TracingThresholds.Standard();
            Assert.IsFalse(StrokeJudge.Evaluate(line, half, strict).Accepted);

            TracingThresholds loose = new TracingThresholds
            {
                MinRecall = 0.40f,
                MinPrecision = 0.40f,
                MinLengthRatio = 0.40f,
                MinEndProgress = 0.40f
            };
            StrokeVerdict eased = StrokeJudge.Evaluate(line, half, loose);
            Assert.IsTrue(eased.Accepted, eased.Fault.ToString());
        }

        [Test]
        public void MissingThresholds_FallBackToTheTabletDefaults()
        {
            TracingThresholds cleaned = new TracingThresholds().Sanitized();
            Assert.AreEqual(0.78f, cleaned.MinRecall, 0.0001f);
            Assert.AreEqual(0.50f, cleaned.MinPrecision, 0.0001f);
            Assert.AreEqual(0.70f, cleaned.MinLengthRatio, 0.0001f);
        }

        [Test]
        public void LaterStroke_DoesNotCountForTheExpectedOne()
        {
            StrokePath[] tee = StrokeLibrary.For("letter-t");
            Assert.AreEqual(2, tee.Length);
            List<Vector2> stem = Copy(tee[0].Points);
            List<Vector2> bar = Copy(tee[1].Points);
            StrokeVerdict wrongOrder = StrokeJudge.Evaluate(stem, bar, TracingThresholds.Standard());
            Assert.IsFalse(wrongOrder.Accepted);
            StrokeVerdict barAlone = StrokeJudge.Evaluate(bar, bar, TracingThresholds.Standard());
            Assert.IsTrue(barAlone.Accepted, barAlone.Fault.ToString());
        }

        [Test]
        public void Resolve_UsesTheLearningItemStrokesWhenPresent()
        {
            var authored = new[]
            {
                new StrokePath
                {
                    Points = new[] { new Vector2(0.2f, 0.3f), new Vector2(0.7f, 0.6f) }
                }
            };
            Assert.AreSame(authored, StrokeLibrary.Resolve(authored, "letter-a"));
        }

        [Test]
        public void Resolve_FallsBackToTheLibraryById()
        {
            StrokePath[] fromLibrary = StrokeLibrary.For("digit-1");
            StrokePath[] resolved = StrokeLibrary.Resolve(null, "digit-1");
            Assert.AreEqual(fromLibrary.Length, resolved.Length);
            Assert.AreEqual(fromLibrary[0].Points[0], resolved[0].Points[0]);

            var blank = new[] { new StrokePath { Points = new Vector2[0] } };
            StrokePath[] letter = StrokeLibrary.Resolve(blank, "letter-c");
            Assert.AreEqual(1, letter.Length);
            Assert.Greater(letter[0].Points.Length, 8);
        }

        [Test]
        public void CursiveLetters_FollowSchoolStrokeOrder()
        {
            const string twoStrokes = "fijtx";
            for (char letter = 'a'; letter <= 'z'; letter++)
            {
                string id = "letter-" + letter;
                StrokePath[] strokes = StrokeLibrary.For(id);
                int expected = twoStrokes.IndexOf(letter) >= 0 ? 2 : 1;
                Assert.AreEqual(expected, strokes.Length, id);
                for (int s = 0; s < strokes.Length; s++)
                {
                    List<Vector2> guide = Copy(strokes[s].Points);
                    Assert.Greater(guide.Count, 4, id);
                    AssertInsideBoard(guide, id);
                    StrokeVerdict forward = StrokeJudge.Evaluate(guide, guide, TracingThresholds.Standard());
                    Assert.IsTrue(forward.Accepted, id + " trait " + s + " " + forward.Fault);
                    var sampled = new List<Vector2>();
                    PolylineMath.Resample(guide, 0.012f, sampled);
                    StrokeVerdict drawn = StrokeJudge.Evaluate(sampled, sampled, TracingThresholds.Standard());
                    Assert.IsTrue(drawn.Accepted, id + " rééchantillonné " + drawn.Fault);
                    StrokeVerdict backward = StrokeJudge.Evaluate(guide, Reverse(guide), TracingThresholds.Standard());
                    Assert.IsFalse(backward.Accepted, id + " à l'envers");
                    StrokeVerdict scribble = StrokeJudge.Evaluate(guide, BackAndForth(guide, 3), TracingThresholds.Standard());
                    Assert.IsFalse(scribble.Accepted, id + " gribouillis");
                    StrokeVerdict wobbly = StrokeJudge.Evaluate(guide, Wobble(guide, 0.02f), TracingThresholds.Standard());
                    Assert.IsTrue(wobbly.Accepted, id + " tremblé " + wobbly.Fault);
                }

                if (strokes.Length < 2)
                    continue;
                float gap = Vector2.Distance(strokes[0].Points[0], strokes[1].Points[0]);
                Assert.Greater(gap, 0.18f, id + " départs trop proches");
            }
        }

        [Test]
        public void Letters_AreLowercaseCursive_AndDigitsStayPrint()
        {
            StrokePath[] a = StrokeLibrary.For("letter-a");
            Assert.AreEqual(1, a.Length);
            Assert.Greater(a[0].Points[0].y, 0.35f);
            Assert.Less(Highest(a), 0.75f);

            Assert.AreEqual(1, StrokeLibrary.For("letter-e").Length);
            Assert.AreEqual(1, StrokeLibrary.For("letter-b").Length);
            Assert.Less(StrokeLibrary.For("letter-t")[0].Points[0].y, 0.6f);
            Assert.Greater(Highest(StrokeLibrary.For("letter-l")), 0.84f);
            Assert.Greater(Highest(StrokeLibrary.For("letter-b")), 0.84f);
            Assert.Less(Lowest(StrokeLibrary.For("letter-g")), 0.2f);
            Assert.Less(Lowest(StrokeLibrary.For("letter-y")), 0.2f);

            StrokePath[] one = StrokeLibrary.For("digit-1");
            Assert.AreEqual(2, one.Length);
            Assert.AreEqual(0.36f, one[0].Points[0].x, 0.001f);
            Assert.AreEqual(0.68f, one[0].Points[0].y, 0.001f);
            Assert.AreEqual(2, StrokeLibrary.For("digit-8").Length);
        }

        static void AssertInsideBoard(List<Vector2> points, string id)
        {
            for (int i = 0; i < points.Count; i++)
            {
                Assert.Greater(points[i].x, 0.02f, id);
                Assert.Less(points[i].x, 0.98f, id);
                Assert.Greater(points[i].y, 0.02f, id);
                Assert.Less(points[i].y, 0.98f, id);
            }
        }

        static float Highest(StrokePath[] strokes)
        {
            float max = 0f;
            for (int s = 0; s < strokes.Length; s++)
            {
                Vector2[] points = strokes[s].Points;
                for (int i = 0; i < points.Length; i++)
                {
                    if (points[i].y > max)
                        max = points[i].y;
                }
            }
            return max;
        }

        static float Lowest(StrokePath[] strokes)
        {
            float min = 1f;
            for (int s = 0; s < strokes.Length; s++)
            {
                Vector2[] points = strokes[s].Points;
                for (int i = 0; i < points.Length; i++)
                {
                    if (points[i].y < min)
                        min = points[i].y;
                }
            }
            return min;
        }

        static List<Vector2> VerticalLine(int count)
        {
            var line = new List<Vector2>(count);
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)(count - 1);
                line.Add(new Vector2(0.5f, Mathf.Lerp(0.15f, 0.85f, t)));
            }
            return line;
        }

        static List<Vector2> Copy(IList<Vector2> source)
        {
            return new List<Vector2>(source);
        }

        static List<Vector2> Reverse(IList<Vector2> source)
        {
            var result = new List<Vector2>(source.Count);
            for (int i = source.Count - 1; i >= 0; i--)
                result.Add(source[i]);
            return result;
        }

        static List<Vector2> Partial(IList<Vector2> source, float fraction)
        {
            float limit = PolylineMath.Length(source) * fraction;
            var result = new List<Vector2> { source[0] };
            float walked = 0f;
            for (int i = 1; i < source.Count; i++)
            {
                walked += Vector2.Distance(source[i - 1], source[i]);
                result.Add(source[i]);
                if (walked >= limit)
                    break;
            }
            return result;
        }

        static List<Vector2> BackAndForth(IList<Vector2> source, int times)
        {
            var result = new List<Vector2>();
            for (int n = 0; n < times; n++)
            {
                bool forward = n % 2 == 0;
                int begin = result.Count == 0 ? 0 : 1;
                if (forward)
                {
                    for (int i = begin; i < source.Count; i++)
                        result.Add(source[i]);
                }
                else
                {
                    for (int i = source.Count - 1 - begin; i >= 0; i--)
                        result.Add(source[i]);
                }
            }
            return result;
        }

        static List<Vector2> Wobble(IList<Vector2> source, float amplitude)
        {
            var result = new List<Vector2>(source.Count);
            for (int i = 0; i < source.Count; i++)
            {
                if (i == 0 || i == source.Count - 1)
                {
                    result.Add(source[i]);
                    continue;
                }
                Vector2 delta = source[i + 1] - source[i - 1];
                float length = delta.magnitude;
                if (length < 0.0001f)
                    length = 1f;
                Vector2 normal = new Vector2(-delta.y / length, delta.x / length);
                result.Add(source[i] + normal * (Mathf.Sin(i * 0.85f) * amplitude));
            }
            return result;
        }
    }
}
