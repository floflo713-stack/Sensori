using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class RibbonGraphic : MaskableGraphic
    {
        readonly List<Vector2[]> _paths = new List<Vector2[]>();
        float _width = 36f;
        Color _core = Color.white;

        public void SetPaths(List<Vector2[]> paths, float width, Color core)
        {
            _paths.Clear();
            if (paths != null)
            {
                for (int i = 0; i < paths.Count; i++)
                {
                    var path = paths[i];
                    if (path == null || path.Length < 2)
                        continue;
                    var copy = new Vector2[path.Length];
                    Array.Copy(path, copy, path.Length);
                    _paths.Add(copy);
                }
            }
            _width = Mathf.Max(2f, width);
            _core = core;
            SetVerticesDirty();
        }

        public void ClearPaths()
        {
            _paths.Clear();
            SetVerticesDirty();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            raycastTarget = false;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (_paths.Count == 0)
                return;
            Color edge = _core;
            edge.a = 0f;
            float half = _width * 0.5f;
            float glow = half * 1.85f;
            for (int p = 0; p < _paths.Count; p++)
                AppendPath(vh, _paths[p], half, glow, _core, edge);
        }

        static void AppendPath(VertexHelper vh, Vector2[] points, float half, float glow, Color core, Color edge)
        {
            int count = points.Length;
            if (count < 2)
                return;
            var normals = new Vector2[count];
            for (int i = 0; i < count; i++)
            {
                Vector2 tangent;
                if (i == 0)
                    tangent = points[1] - points[0];
                else if (i == count - 1)
                    tangent = points[count - 1] - points[count - 2];
                else
                    tangent = points[i + 1] - points[i - 1];
                if (tangent.sqrMagnitude < 0.0001f)
                    tangent = Vector2.right;
                tangent.Normalize();
                normals[i] = new Vector2(-tangent.y, tangent.x);
            }

            int start = vh.currentVertCount;
            for (int i = 0; i < count; i++)
            {
                Vector2 point = points[i];
                Vector2 normal = normals[i];
                AddVert(vh, point + normal * glow, edge);
                AddVert(vh, point + normal * half, core);
                AddVert(vh, point - normal * half, core);
                AddVert(vh, point - normal * glow, edge);
            }

            for (int i = 0; i < count - 1; i++)
            {
                int a = start + i * 4;
                int b = start + (i + 1) * 4;
                Quad(vh, a, a + 1, b + 1, b);
                Quad(vh, a + 1, a + 2, b + 2, b + 1);
                Quad(vh, a + 2, a + 3, b + 3, b + 2);
            }

            AppendDisc(vh, points[0], glow, core, edge);
            AppendDisc(vh, points[count - 1], glow, core, edge);
        }

        static void AppendDisc(VertexHelper vh, Vector2 point, float radius, Color core, Color edge)
        {
            int center = vh.currentVertCount;
            AddVert(vh, point, core);
            const int segments = 10;
            int previous = -1;
            for (int i = 0; i <= segments; i++)
            {
                float angle = (i / (float)segments) * Mathf.PI * 2f;
                var rim = point + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                int index = vh.currentVertCount;
                AddVert(vh, rim, edge);
                if (i > 0)
                    vh.AddTriangle(center, previous, index);
                previous = index;
            }
        }

        static void AddVert(VertexHelper vh, Vector2 position, Color color)
        {
            var vertex = UIVertex.simpleVert;
            vertex.position = position;
            vertex.color = color;
            vertex.uv0 = new Vector2(0.5f, 0.5f);
            vh.AddVert(vertex);
        }

        static void Quad(VertexHelper vh, int a, int b, int c, int d)
        {
            vh.AddTriangle(a, b, c);
            vh.AddTriangle(a, c, d);
        }
    }
}
