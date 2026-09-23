using UnityEngine;

namespace Sensori.Montessori
{
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        RectTransform _rect;
        Rect _applied;

        void Awake()
        {
            _rect = (RectTransform)transform;
            Apply();
        }

        void Update()
        {
            if (_applied != Screen.safeArea)
                Apply();
        }

        void Apply()
        {
            if (_rect == null)
                _rect = (RectTransform)transform;
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
                return;
            var pixelRect = canvas.pixelRect;
            if (pixelRect.width <= 1f || pixelRect.height <= 1f)
                return;
            var safe = Screen.safeArea;
            _applied = safe;
            var min = safe.position;
            var max = safe.position + safe.size;
            _rect.anchorMin = new Vector2(min.x / pixelRect.width, min.y / pixelRect.height);
            _rect.anchorMax = new Vector2(max.x / pixelRect.width, max.y / pixelRect.height);
            _rect.offsetMin = Vector2.zero;
            _rect.offsetMax = Vector2.zero;
        }
    }
}
