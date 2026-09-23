using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class CategoryPresenter : MonoBehaviour
    {
        [SerializeField] Text _title;
        [SerializeField] Text _subtitle;
        [SerializeField] RectTransform _area;
        readonly List<GameCard> _cards = new List<GameCard>();

        public void Construct(ThemeAssets theme, MiniGameDefinition[] games)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);

            var header = UiFactory.Rect("Entete", transform);
            UiFactory.AnchorTop(header, 140f, 28f, 28f);
            var back = UiFactory.RoundButton("Retour", header, theme, "<", 54, NavigationTarget.Home);
            UiFactory.AnchorCenter(back, new Vector2(-820f, -10f), new Vector2(92f, 92f));
            back.anchorMin = new Vector2(0f, 0.5f);
            back.anchorMax = new Vector2(0f, 0.5f);
            back.pivot = new Vector2(0f, 0.5f);
            back.anchoredPosition = new Vector2(8f, 0f);

            _title = UiFactory.Label("Titre", header, "Alphabet", 52, MontessoriPalette.Ink, TextAnchor.MiddleCenter);
            UiFactory.Stretch(_title.rectTransform, 140f, 36f, 140f, 8f);
            _subtitle = UiFactory.Label("SousTitre", header, "", 26, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            UiFactory.Stretch(_subtitle.rectTransform, 140f, 8f, 140f, 78f);

            _area = UiFactory.Rect("ZoneJeux", transform);
            UiFactory.Stretch(_area, 48f, 48f, 48f, 160f);

            if (games != null)
            {
                for (int i = 0; i < games.Length; i++)
                {
                    if (games[i] != null)
                        CreateCard(theme, games[i]);
                }
            }

            Canvas.ForceUpdateCanvases();
            Layout();
        }

        void CreateCard(ThemeAssets theme, MiniGameDefinition game)
        {
            var root = UiFactory.Rect("Jeu_" + game.GameId, _area);
            UiFactory.AnchorCenter(root, Vector2.zero, new Vector2(420f, 520f));
            var group = UiFactory.AddGroup(root.gameObject);
            var shadow = UiFactory.Picture("Ombre", root, theme.Shadow, MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.26f), false, false);
            UiFactory.Stretch(shadow.rectTransform, -16f, -28f, -16f, -6f);
            var face = UiFactory.Picture("Face", root, theme.Panel, Color.white, true, true);
            UiFactory.Stretch(face.rectTransform, 0f, 0f, 0f, 0f);
            face.preserveAspect = false;
            face.gameObject.AddComponent<Pressable>();

            var accent = UiFactory.Picture("Accent", face.transform, theme.Chip, game.Accent, false, false);
            accent.rectTransform.anchorMin = new Vector2(0.12f, 1f);
            accent.rectTransform.anchorMax = new Vector2(0.88f, 1f);
            accent.rectTransform.pivot = new Vector2(0.5f, 1f);
            accent.rectTransform.sizeDelta = new Vector2(0f, 14f);
            accent.rectTransform.anchoredPosition = new Vector2(0f, -26f);

            var icon = UiFactory.Picture("Icone", face.transform, theme.IconForGame(game.GameId), Color.white, false, false);
            UiFactory.AnchorCenter(icon.rectTransform, new Vector2(0f, 90f), new Vector2(200f, 200f));
            icon.preserveAspect = true;

            var title = UiFactory.Label("Titre", face.transform, game.Title, 36, MontessoriPalette.Ink, TextAnchor.MiddleCenter);
            title.rectTransform.anchorMin = new Vector2(0.08f, 0.24f);
            title.rectTransform.anchorMax = new Vector2(0.92f, 0.42f);
            title.rectTransform.offsetMin = Vector2.zero;
            title.rectTransform.offsetMax = Vector2.zero;

            var description = UiFactory.Label("Description", face.transform, game.Description, 24, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            description.rectTransform.anchorMin = new Vector2(0.1f, 0.08f);
            description.rectTransform.anchorMax = new Vector2(0.9f, 0.26f);
            description.rectTransform.offsetMin = Vector2.zero;
            description.rectTransform.offsetMax = Vector2.zero;

            var card = face.gameObject.AddComponent<GameCard>();
            card.Bind(game, title, description, root, group);
        }

        public void Show(LearningCategory category)
        {
            Cache();
            if (_title != null)
                _title.text = category != null ? category.Title : string.Empty;
            if (_subtitle != null)
                _subtitle.text = category != null ? category.Subtitle : string.Empty;
            for (int i = 0; i < _cards.Count; i++)
            {
                var card = _cards[i];
                if (card == null || card.Root == null)
                    continue;
                bool available = category != null && category.HasGame(card.GameId);
                card.Root.gameObject.SetActive(available);
                card.Refresh();
            }
            Layout();
            PlayIntro();
        }

        void PlayIntro()
        {
            int visibleIndex = 0;
            for (int i = 0; i < _cards.Count; i++)
            {
                var card = _cards[i];
                if (card == null || card.Root == null || !card.Root.gameObject.activeSelf)
                    continue;
                var root = card.Root;
                var group = card.Group;
                if (!Application.isPlaying)
                {
                    root.localScale = Vector3.one;
                    if (group != null)
                        group.alpha = 1f;
                    continue;
                }
                root.localScale = Vector3.one * 0.92f;
                if (group != null)
                    group.alpha = 0f;
                float delay = 0.04f + visibleIndex * 0.08f;
                Motion.Scale(root, Vector3.one, 0.46f, Ease.OutBack).SetDelay(delay);
                if (group != null)
                    Motion.Fade(group, 1f, 0.3f, Ease.OutQuad).SetDelay(delay);
                visibleIndex++;
            }
        }

        void OnRectTransformDimensionsChange()
        {
            Layout();
        }

        void Cache()
        {
            _cards.Clear();
            var found = GetComponentsInChildren<GameCard>(true);
            for (int i = 0; i < found.Length; i++)
                _cards.Add(found[i]);
        }

        void Layout()
        {
            if (_area == null)
                return;
            var visible = new List<RectTransform>();
            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i] != null && _cards[i].Root != null && _cards[i].Root.gameObject.activeSelf)
                    visible.Add(_cards[i].Root);
            }
            if (visible.Count == 0)
                return;
            float width = _area.rect.width;
            float height = _area.rect.height;
            if (width < 80f)
                width = 1600f;
            if (height < 80f)
                height = 700f;
            int count = visible.Count;
            float gap = 32f;
            float cardW = Mathf.Min(460f, (width - gap * (count - 1)) / count);
            float cardH = Mathf.Min(height - 12f, cardW * 1.18f);
            float total = count * cardW + (count - 1) * gap;
            float x = -total * 0.5f + cardW * 0.5f;
            for (int i = 0; i < count; i++)
            {
                visible[i].anchoredPosition = new Vector2(x, -10f);
                visible[i].sizeDelta = new Vector2(cardW, cardH);
                x += cardW + gap;
            }
        }
    }
}
