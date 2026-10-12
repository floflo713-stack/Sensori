using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Sensori.Montessori.Tests
{
    public sealed class PolylineMathTests
    {
        [Test]
        public void Length_OfASegment_IsItsDistance()
        {
            var points = new List<Vector2> { new Vector2(0f, 0f), new Vector2(0.3f, 0.4f) };
            Assert.AreEqual(0.5f, PolylineMath.Length(points), 0.0001f);
        }

        [Test]
        public void Length_OfAShortPath_IsZero()
        {
            Assert.AreEqual(0f, PolylineMath.Length(null));
            Assert.AreEqual(0f, PolylineMath.Length(new List<Vector2> { new Vector2(0.2f, 0.2f) }));
        }

        [Test]
        public void Resample_SpacesPointsAlongTheSegment()
        {
            var source = new List<Vector2> { new Vector2(0f, 0.2f), new Vector2(0.3f, 0.2f) };
            var sampled = new List<Vector2>();
            PolylineMath.Resample(source, 0.1f, sampled);

            Assert.GreaterOrEqual(sampled.Count, 3);
            Assert.AreEqual(source[0], sampled[0]);
            Assert.AreEqual(source[source.Count - 1], sampled[sampled.Count - 1]);
            for (int i = 1; i < sampled.Count; i++)
                Assert.LessOrEqual(Vector2.Distance(sampled[i - 1], sampled[i]), 0.1f + 0.0001f);
        }

        [Test]
        public void Resample_KeepsASinglePoint()
        {
            var source = new List<Vector2> { new Vector2(0.4f, 0.6f) };
            var sampled = new List<Vector2>();
            PolylineMath.Resample(source, 0.05f, sampled);
            Assert.AreEqual(1, sampled.Count);
            Assert.AreEqual(source[0], sampled[0]);
        }

        [Test]
        public void Smooth_AddsPointsBetweenCorners()
        {
            var source = new List<Vector2>
            {
                new Vector2(0.1f, 0.1f),
                new Vector2(0.2f, 0.8f),
                new Vector2(0.8f, 0.7f),
                new Vector2(0.7f, 0.2f)
            };
            var smooth = new List<Vector2>();
            PolylineMath.Smooth(source, smooth, 3);
            Assert.Greater(smooth.Count, source.Count);
            Assert.AreEqual(source[source.Count - 1], smooth[smooth.Count - 1]);
        }

        [Test]
        public void Recall_IsOneWhenTheFingerCoversTheGuide()
        {
            var guide = new List<Vector2> { new Vector2(0.2f, 0.2f), new Vector2(0.8f, 0.2f) };
            Assert.AreEqual(1f, PolylineMath.Recall(guide, guide, 0.02f), 0.0001f);
        }

        [Test]
        public void Recall_IsZeroWhenTheFingerMissesTheGuide()
        {
            var guide = new List<Vector2> { new Vector2(0.2f, 0.2f), new Vector2(0.8f, 0.2f) };
            var user = new List<Vector2> { new Vector2(0.2f, 0.8f), new Vector2(0.8f, 0.8f) };
            Assert.AreEqual(0f, PolylineMath.Recall(guide, user, 0.05f), 0.0001f);
        }

        [Test]
        public void Precision_DropsWhenTheFingerLeavesTheGuide()
        {
            var guide = new List<Vector2> { new Vector2(0.2f, 0.4f), new Vector2(0.8f, 0.4f) };
            var user = new List<Vector2> { new Vector2(0.2f, 0.4f), new Vector2(0.8f, 0.9f) };
            Assert.AreEqual(0.5f, PolylineMath.Precision(guide, user, 0.05f), 0.0001f);
        }

        [Test]
        public void PointAt_WalksThePolyline()
        {
            var points = new List<Vector2> { new Vector2(0.2f, 0.1f), new Vector2(0.2f, 0.9f) };
            Vector2 middle = PolylineMath.PointAt(points, 0.4f);
            Assert.AreEqual(0.2f, middle.x, 0.0001f);
            Assert.AreEqual(0.5f, middle.y, 0.0001f);
            Assert.AreEqual(points[0], PolylineMath.PointAt(points, 0f));
            Assert.AreEqual(points[1], PolylineMath.PointAt(points, 5f));
        }
    }
}
