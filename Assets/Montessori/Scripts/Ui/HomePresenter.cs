using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class HomePresenter : MonoBehaviour
    {
        [SerializeField] RectTransform _area;
        readonly List<CategoryCard> _cards = new List<CategoryCard>();

        public void Construct(ThemeAssets theme, ContentCatalog catalog)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);

            var sign = UiFactory.Rect("Enseigne", transform);
            UiFactory.AnchorTop(sign, 210f, 80f, 80f);
            var signFace = UiFactory.Picture("Plaque", sign, theme.Panel, Color.white, true, false);
            UiFactory.AnchorCenter(signFace.rectTransform, new Vector2(0f, -8f), new Vector2(640f, 168f));
            var shadow = UiFactory.Picture("Ombre", sign, theme.Shadow, MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.22f), false, false);
            shadow.rectTransform.SetSiblingIndex(0);
            UiFactory.AnchorCenter(shadow.rectTransform, new Vector2(0f, -24f), new Vector2(700f, 150f));

            var title = UiFactory.Label("Titre", signFace.transform, "Sensori", 78, MontessoriPalette.WalnutDeep, TextAnchor.MiddleCenter);
            UiFactory.Stretch(title.rectTransform, 24f, 48f, 24f, 18f);
            var titleShadow = title.gameObject.AddComponent<Shadow>();
            titleShadow.effectColor = MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.18f);
            titleShadow.effectDistance = new Vector2(0f, -3f);
            titleShadow.useGraphicAlpha = true;

            var subtitle = UiFactory.Label("SousTitre", signFace.transform, "Atelier Montessori", 28, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            UiFactory.Stretch(subtitle.rectTransform, 24f, 16f, 24f, 96f);

            _area = UiFactory.Rect("ZoneCartes", transform);
            UiFactory.Stretch(_area, 36f, 92f, 36f, 220f);

            var footer = UiFactory.Label("Legende", transform, "Voyelles en bleu    ·    Consonnes en rose", 26, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            UiFactory.AnchorBottom(footer.rectTransform, 72f, 40f, 40f);

            if (catalog != null)
            {
                var categories = catalog.Categories;
                for (int i = 0; i < categories.Length; i++)
                {
                    if (categories[i] != null)
                        CreateCard(theme, categories[i]);
                }
            }

            Canvas.ForceUpdateCanvases();
            Layout();
        }

        void CreateCard(ThemeAssets theme, LearningCategory category)
        {
            var root = UiFactory.Rect("Carte_" + category.CategoryId, _area);
            UiFactory.AnchorCenter(root, Vector2.zero, new Vector2(320f, 430f));
            var group = UiFactory.AddGroup(root.gameObject);
            var shadow = UiFactory.Picture("Ombre", root, theme.Shadow, MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.26f), false, false);
            UiFactory.Stretch(shadow.rectTransform, -16f, -28f, -16f, -6f);
            var face = UiFactory.Picture("Face", root, theme.Panel, Color.white, true, true);
            UiFactory.Stretch(face.rectTransform, 0f, 0f, 0f, 0f);
            face.preserveAspect = false;
            face.gameObject.AddComponent<Pressable>();

            var accent = UiFactory.Picture("Accent", face.transform, theme.Chip, category.Accent, false, false);
            accent.rectTransform.anchorMin = new Vector2(0.1f, 1f);
            accent.rectTransform.anchorMax = new Vector2(0.9f, 1f);
            accent.rectTransform.pivot = new Vector2(0.5f, 1f);
            accent.rectTransform.sizeDelta = new Vector2(0f, 14f);
            accent.rectTransform.anchoredPosition = new Vector2(0f, -22f);

            var icon = UiFactory.Picture("Icone", face.transform, theme.IconForCategory(category.CategoryId), Color.white, false, false);
            UiFactory.AnchorCenter(icon.rectTransform, new Vector2(0f, 70f), new Vector2(168f, 168f));
            icon.preserveAspect = true;

            var title = UiFactory.Label("Titre", face.transform, category.Title, 40, MontessoriPalette.Ink, TextAnchor.MiddleCenter);
            title.rectTransform.anchorMin = new Vector2(0.08f, 0.22f);
            title.rectTransform.anchorMax = new Vector2(0.92f, 0.40f);
            title.rectTransform.offsetMin = Vector2.zero;
            title.rectTransform.offsetMax = Vector2.zero;

            var count = UiFactory.Label("Compte", face.transform, "", 24, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            count.rectTransform.anchorMin = new Vector2(0.08f, 0.08f);
            count.rectTransform.anchorMax = new Vector2(0.92f, 0.22f);
            count.rectTransform.offsetMin = Vector2.zero;
            count.rectTransform.offsetMax = Vector2.zero;

            var card = face.gameObject.AddComponent<CategoryCard>();
            card.Bind(category, title, count, root, icon.rectTransform, group);
            card.Refresh();
        }

        public void Refresh()
        {
            Cache();
            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i] != null)
                    _cards[i].Refresh();
            }
        }

        public void PlayIntro()
        {
            Refresh();
            Layout();
            for (int i = 0; i < _cards.Count; i++)
            {
                var card = _cards[i];
                if (card == null || card.Root == null)
                    continue;
                var root = card.Root;
                var group = card.Group;
                var icon = card.Icon;
                if (!Application.isPlaying)
                {
                    root.localScale = Vector3.one;
                    if (group != null)
                        group.alpha = 1f;
                    continue;
                }

                root.localScale = Vector3.one * 0.9f;
                if (group != null)
                    group.alpha = 0f;
                float delay = 0.05f + i * 0.07f;
                Motion.Scale(root, Vector3.one, 0.48f, Ease.OutBack).SetDelay(delay);
                if (group != null)
                    Motion.Fade(group, 1f, 0.35f, Ease.OutQuad).SetDelay(delay);
                if (icon != null)
                {
                    icon.localScale = Vector3.one;
                    Motion.Scale(icon, Vector3.one * 1.035f, 1.7f, Ease.InOutSine)
                        .SetDelay(delay + 0.45f)
                        .SetLoops(-1, LoopKind.Yoyo);
                }
            }
        }

        void OnRectTransformDimensionsChange()
        {
            Layout();
        }

        void Cache()
        {
            _cards.Clear();
            var found = GetComponentsInChildren<CategoryCard>(true);
            for (int i = 0; i < found.Length; i++)
                _cards.Add(found[i]);
        }

        void Layout()
        {
            Cache();
            if (_area == null || _cards.Count == 0)
                return;
            float width = _area.rect.width;
            float height = _area.rect.height;
            if (width < 80f)
                width = 1600f;
            if (height < 80f)
                height = 680f;

            int count = _cards.Count;
            float gap = 28f;
            float available = width - 8f;
            bool grid = count > 2 && available < count * 250f + (count - 1) * gap;
            if (!grid)
            {
                float cardW = Mathf.Min(360f, (available - gap * (count - 1)) / count);
                float cardH = Mathf.Min(height - 8f, cardW * 1.32f);
                float total = count * cardW + (count - 1) * gap;
                float x = -total * 0.5f + cardW * 0.5f;
                for (int i = 0; i < count; i++)
                {
                    var root = _cards[i].Root;
                    root.anchoredPosition = new Vector2(x, 0f);
                    root.sizeDelta = new Vector2(cardW, cardH);
                    x += cardW + gap;
                }
                return;
            }

            int columns = 2;
            int rows = Mathf.CeilToInt(count / 2f);
            float cardW2 = Mathf.Min(420f, (available - gap) * 0.5f);
            float cardH2 = Mathf.Min(340f, (height - gap * (rows - 1)) / rows);
            for (int i = 0; i < count; i++)
            {
                int column = i % columns;
                int row = i / columns;
                float x = (column == 0 ? -0.5f : 0.5f) * (cardW2 + gap);
                float y = ((rows - 1) * 0.5f - row) * (cardH2 + gap);
                var root = _cards[i].Root;
                root.anchoredPosition = new Vector2(x, y);
                root.sizeDelta = new Vector2(cardW2, cardH2);
            }
        }
    }
}
