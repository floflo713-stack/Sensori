using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class PuzzleSlot : MonoBehaviour
    {
        LearningItem _item;
        RectTransform _rect;
        bool _filled;
        float _magnet = 110f;
        Image _glow;

        public LearningItem Item => _item;
        public bool Filled => _filled;
        public float Magnet => _magnet;
        public Vector2 AnchoredPosition => _rect != null ? _rect.anchoredPosition : Vector2.zero;

        public void Build(RectTransform rect, LearningItem item, float size, ThemeAssets theme)
        {
            _rect = rect;
            _item = item;
            _magnet = size * 0.48f;
            var shadow = UiFactory.Picture("Ombre", rect, theme.Shadow, MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.22f), false, false);
            UiFactory.Stretch(shadow.rectTransform, -10f, -16f, -10f, -4f);
            var inset = UiFactory.Picture("Empreinte", rect, theme.Inset, Color.white, true, false);
            UiFactory.Stretch(inset.rectTransform, 0f, 0f, 0f, 0f);
            inset.preserveAspect = false;
            var ghost = UiFactory.Rect("Fantome", rect);
            UiFactory.Stretch(ghost, size * 0.12f, size * 0.12f, size * 0.12f, size * 0.12f);
            ItemPlate.Paint(ghost, item, theme, true);
            _glow = UiFactory.Picture("Halo", rect, theme.Pearl, MontessoriPalette.WithAlpha(MontessoriPalette.Sun, 0f), false, false);
            UiFactory.Stretch(_glow.rectTransform, -8f, -8f, -8f, -8f);
            _glow.transform.SetAsFirstSibling();
        }

        public void SetHot(bool hot)
        {
            if (_glow == null)
                return;
            var color = _glow.color;
            color.a = hot ? 0.35f : 0f;
            _glow.color = color;
            _rect.localScale = hot ? Vector3.one * 1.04f : Vector3.one;
        }

        public void MarkFilled()
        {
            _filled = true;
            SetHot(false);
        }
    }

    public sealed class WoodenPiece : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        RectTransform _rect;
        Image _raycast;
        LearningItem _item;
        WoodenPuzzleGame _game;
        Vector2 _home;
        Vector2 _grabOffset;
        int _pointer = -1;
        bool _locked;

        public LearningItem Item => _item;
        public Vector2 AnchoredPosition => _rect != null ? _rect.anchoredPosition : Vector2.zero;

        public void Build(RectTransform rect, Image raycast, LearningItem item, WoodenPuzzleGame game, Vector2 home)
        {
            _rect = rect;
            _raycast = raycast;
            _item = item;
            _game = game;
            _home = home;
            _rect.anchoredPosition = home;
        }

        public void Nudge()
        {
            if (_locked || _rect == null)
                return;
            Motion.Anchored(_rect, _home + new Vector2(0f, 28f), 0.55f, Ease.InOutSine)
                .SetLoops(2, LoopKind.Yoyo)
                .SetDelay(0.45f);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_locked || eventData.button != PointerEventData.InputButton.Left)
                return;
            if (_pointer >= 0)
                return;
            _pointer = eventData.pointerId;
            _rect.SetAsLastSibling();
            Motion.Kill(_rect, "pos");
            Motion.Scale(_rect, Vector3.one * 1.06f, 0.12f, Ease.OutQuad);
            WoodenAudio.PlayTap();
            if (TryLocal(eventData, out var local))
                _grabOffset = _rect.anchoredPosition - local;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_locked || eventData.pointerId != _pointer)
                return;
            if (!TryLocal(eventData, out var local))
                return;
            _rect.anchoredPosition = local + _grabOffset;
            if (_game != null)
                _game.HighlightFor(this);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != _pointer)
                return;
            _pointer = -1;
            if (_locked)
                return;
            Motion.Scale(_rect, Vector3.one, 0.16f, Ease.OutQuad);
            if (_game != null)
                _game.Release(this);
        }

        public void MoveTo(Vector2 position, bool lockPiece)
        {
            Motion.Anchored(_rect, position, 0.22f, Ease.OutBack);
            if (!lockPiece)
                return;
            _locked = true;
            _home = position;
            if (_raycast != null)
                _raycast.raycastTarget = false;
            Motion.PunchScale(_rect, 0.08f, 0.28f);
        }

        public void ReturnHome()
        {
            WoodenAudio.PlayTock();
            Motion.Shake(_rect, 10f, 0.28f);
            Motion.Anchored(_rect, _home, 0.35f, Ease.OutCubic).SetDelay(0.12f);
        }

        bool TryLocal(PointerEventData eventData, out Vector2 local)
        {
            local = Vector2.zero;
            if (_rect == null || _rect.parent is not RectTransform parent)
                return false;
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, eventData.position, eventData.pressEventCamera, out local);
        }
    }

    public static class ItemPlate
    {
        public static void Paint(RectTransform host, LearningItem item, ThemeAssets theme, bool ghost)
        {
            if (host == null || item == null || theme == null)
                return;
            float alpha = ghost ? 0.32f : 1f;
            if (item.Visual == ItemVisual.Swatch)
            {
                var bead = UiFactory.Picture("Pastille", host, theme.Pearl, MontessoriPalette.WithAlpha(item.Accent, alpha), false, false);
                UiFactory.Stretch(bead.rectTransform, 0f, 0f, 0f, 0f);
                bead.preserveAspect = false;
                return;
            }

            if (item.Visual == ItemVisual.Pictogram)
            {
                var silhouette = PictogramPainter.GetSilhouette(item.PictogramId);
                if (silhouette != null)
                {
                    var color = ghost
                        ? MontessoriPalette.WithAlpha(MontessoriPalette.Walnut, 0.4f)
                        : MontessoriPalette.Maple;
                    var shape = UiFactory.Picture("Forme", host, silhouette, color, false, false);
                    UiFactory.Stretch(shape.rectTransform, 0f, 0f, 0f, 0f);
                    shape.preserveAspect = true;
                    return;
                }
            }

            var wood = UiFactory.Picture("Bois", host, theme.Piece, MontessoriPalette.WithAlpha(Color.white, alpha), true, false);
            UiFactory.Stretch(wood.rectTransform, 0f, 0f, 0f, 0f);
            wood.preserveAspect = false;
            var label = UiFactory.Label("Symbole", wood.transform, string.IsNullOrEmpty(item.Symbol) ? item.DisplayName : item.Symbol, 64, MontessoriPalette.WithAlpha(item.SymbolColor, alpha), TextAnchor.MiddleCenter);
            UiFactory.Stretch(label.rectTransform, 8f, 8f, 8f, 8f);
            label.resizeTextForBestFit = true;
            label.resizeTextMinSize = 18;
            label.resizeTextMaxSize = 120;
            label.alignment = TextAnchor.MiddleCenter;
        }
    }
}
