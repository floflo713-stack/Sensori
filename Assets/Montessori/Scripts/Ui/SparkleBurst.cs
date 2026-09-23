using UnityEngine;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class SparkleBurst : MonoBehaviour
    {
        [SerializeField] Sprite _pearl;
        [SerializeField] RectTransform _layer;

        public void Construct(ThemeAssets theme)
        {
            _pearl = theme.Pearl;
            _layer = (RectTransform)transform;
            UiFactory.Stretch(_layer, 0f, 0f, 0f, 0f);
        }

        public void Play(Vector3 worldPosition, Color color)
        {
            if (_pearl == null || _layer == null || !Application.isPlaying)
                return;
            var canvas = GetComponentInParent<Canvas>();
            Camera camera = null;
            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                camera = canvas.worldCamera;
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(camera, worldPosition);
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_layer, screen, camera, out var origin))
                return;

            int count = 16;
            for (int i = 0; i < count; i++)
            {
                var image = UiFactory.Picture("Etincelle", _layer, _pearl, color, false, false);
                float size = Random.Range(14f, 28f);
                UiFactory.AnchorCenter(image.rectTransform, origin, new Vector2(size, size));
                image.rectTransform.localScale = Vector3.one * 0.4f;
                Vector2 target = origin + Random.insideUnitCircle * Random.Range(70f, 210f);
                var rect = image.rectTransform;
                var group = image.gameObject.AddComponent<CanvasGroup>();
                Motion.Anchored(rect, target, Random.Range(0.55f, 0.85f), Ease.OutCubic);
                Motion.Scale(rect, Vector3.one, 0.45f, Ease.OutBack);
                Motion.Fade(group, 0f, 0.7f, Ease.InQuad).SetDelay(0.15f).OnComplete(() =>
                {
                    if (rect != null)
                        Destroy(rect.gameObject);
                });
            }
        }
    }
}
