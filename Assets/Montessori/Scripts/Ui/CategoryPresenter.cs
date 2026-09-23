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
        [SerializeField] RectTransform _choices;
        readonly List<GameCard> _cards = new List<GameCard>();
        readonly List<CategoryCard> _choiceCards = new List<CategoryCard>();
        bool _showingGames;

        public bool ShowingGames => _showingGames;

        public void Construct(ThemeAssets theme, ContentCatalog catalog, MiniGameDefinition[] games)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);

            BuildHeader(theme);
            _choices = UiFactory.Rect("ZoneChoix", transform);
            UiFactory.Stretch(_choices, 36f, 48f, 36f, 168f);
            _area = UiFactory.Rect("ZoneJeux", transform);
            UiFactory.Stretch(_area, 48f, 48f, 48f, 168f);

            BuildChoices(catalog, theme != null ? theme.Panel : null, theme != null ? theme.Shadow : null, theme);
            if (games == null || games.Length == 0)
                games = GamesFrom(catalog);
            BuildGames(games, theme != null ? theme.Panel : null, theme != null ? theme.Shadow : null, theme);

            _area.gameObject.SetActive(false);
            _choices.gameObject.SetActive(true);
            _showingGames = false;
            Canvas.ForceUpdateCanvases();
            LayoutChoices();
        }

        public void EnsureMenus(ContentCatalog catalog, Sprite panel, Sprite shadow)
        {
            EnsureRefs();
            if (_choices == null)
            {
                _choices = UiFactory.Rect("ZoneChoix", transform);
                UiFactory.Stretch(_choices, 36f, 48f, 36f, 168f);
            }
            if (_area == null)
            {
                _area = UiFactory.Rect("ZoneJeux", transform);
                UiFactory.Stretch(_area, 48f, 48f, 48f, 168f);
            }

            CacheChoices();
            if (_choiceCards.Count == 0)
                BuildChoices(catalog, panel, shadow, null);

            Cache();
            if (_cards.Count == 0)
                BuildGames(GamesFrom(catalog), panel, shadow, null);

            if (!_showingGames)
            {
                _area.gameObject.SetActive(false);
                _choices.gameObject.SetActive(true);
            }
            ApplyChoiceFonts();
        }

        public void ShowChoices()
        {
            EnsureRefs();
            _showingGames = false;
            if (_title != null)
                _title.text = "Catégories";
            if (_subtitle != null)
                _subtitle.text = "Alphabet, chiffres, formes et couleurs";
            ApplyFont(_title);
            ApplyFont(_subtitle);
            if (_area != null)
                _area.gameObject.SetActive(false);
            if (_choices != null)
                _choices.gameObject.SetActive(true);

            CacheChoices();
            for (int i = 0; i < _choiceCards.Count; i++)
            {
                if (_choiceCards[i] != null)
                    _choiceCards[i].Refresh();
            }
            ApplyChoiceFonts();
            Canvas.ForceUpdateCanvases();
            LayoutChoices();
            PlayChoiceIntro();
        }

        public void Show(LearningCategory category)
        {
            EnsureRefs();
            Cache();
            _showingGames = true;
            if (_choices != null)
                _choices.gameObject.SetActive(false);
            if (_area != null)
                _area.gameObject.SetActive(true);
            if (_title != null)
                _title.text = category != null ? category.Title : string.Empty;
            if (_subtitle != null)
                _subtitle.text = category != null ? category.Subtitle : string.Empty;
            ApplyFont(_title);
            ApplyFont(_subtitle);

            for (int i = 0; i < _cards.Count; i++)
            {
                var card = _cards[i];
                if (card == null || card.Root == null)
                    continue;
                bool available = category != null && category.HasGame(card.GameId);
                card.Root.gameObject.SetActive(available);
                card.Refresh();
                ApplyFont(card.Root.GetComponentInChildren<Text>(true));
            }
            Canvas.ForceUpdateCanvases();
            Layout();
            PlayIntro();
        }

        void BuildHeader(ThemeAssets theme)
        {
            var header = UiFactory.Rect("Entete", transform);
            UiFactory.AnchorTop(header, 140f, 28f, 28f);
            if (theme != null)
                BuildBackButton(header, theme);

            _title = UiFactory.Label("Titre", header, "Catégories", 52, MontessoriPalette.Ink, TextAnchor.MiddleCenter);
            UiFactory.Stretch(_title.rectTransform, 140f, 36f, 140f, 8f);
            _title.font = ReadableFont();
            _subtitle = UiFactory.Label("SousTitre", header, "Alphabet, chiffres, formes et couleurs", 26, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            UiFactory.Stretch(_subtitle.rectTransform, 140f, 8f, 140f, 78f);
            _subtitle.font = ReadableFont();
        }

        static void BuildBackButton(Transform header, ThemeAssets theme)
        {
            var back = UiFactory.Rect("Retour", header);
            back.anchorMin = new Vector2(0f, 0.5f);
            back.anchorMax = new Vector2(0f, 0.5f);
            back.pivot = new Vector2(0f, 0.5f);
            back.sizeDelta = new Vector2(92f, 92f);
            back.anchoredPosition = new Vector2(8f, 0f);
            var image = UiFactory.Picture("Face", back, theme.Pearl, MontessoriPalette.Maple, false, true);
            UiFactory.Stretch(image.rectTransform, 0f, 0f, 0f, 0f);
            image.preserveAspect = false;
            image.raycastTarget = true;
            var label = UiFactory.Label("Glyph", image.transform, "<", 54, MontessoriPalette.Ink, TextAnchor.MiddleCenter);
            UiFactory.Stretch(label.rectTransform, 0f, 0f, 0f, 0f);
            label.font = ReadableFont();
            label.raycastTarget = false;
            image.gameObject.AddComponent<Pressable>();
            UiFactory.CreateButton(image.gameObject);
        }

        void BuildChoices(ContentCatalog catalog, Sprite panel, Sprite shadow, ThemeAssets theme)
        {
            if (_choices == null || catalog == null)
                return;
            var categories = catalog.Categories;
            for (int i = 0; i < categories.Length; i++)
            {
                if (categories[i] != null)
                    CreateChoice(categories[i], panel, shadow, theme);
            }
        }

        void BuildGames(MiniGameDefinition[] games, Sprite panel, Sprite shadow, ThemeAssets theme)
        {
            if (_area == null || games == null)
                return;
            for (int i = 0; i < games.Length; i++)
            {
                if (games[i] != null)
                    CreateGame(games[i], panel, shadow, theme);
            }
        }

        void CreateChoice(LearningCategory category, Sprite panel, Sprite shadow, ThemeAssets theme)
        {
            var root = UiFactory.Rect("Choix_" + category.CategoryId, _choices);
            UiFactory.AnchorCenter(root, Vector2.zero, new Vector2(340f, 420f));
            var group = UiFactory.AddGroup(root.gameObject);
            if (shadow != null)
            {
                var shadowImage = UiFactory.Picture("Ombre", root, shadow, MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.26f), false, false);
                UiFactory.Stretch(shadowImage.rectTransform, -16f, -28f, -16f, -6f);
                shadowImage.raycastTarget = false;
            }

            var face = UiFactory.Picture("Face", root, panel, Color.white, panel != null, true);
            UiFactory.Stretch(face.rectTransform, 0f, 0f, 0f, 0f);
            face.preserveAspect = false;
            face.raycastTarget = true;
            face.gameObject.AddComponent<Pressable>();

            var accent = UiFactory.Picture("Accent", face.transform, null, category.Accent, false, false);
            accent.rectTransform.anchorMin = new Vector2(0.1f, 1f);
            accent.rectTransform.anchorMax = new Vector2(0.9f, 1f);
            accent.rectTransform.pivot = new Vector2(0.5f, 1f);
            accent.rectTransform.sizeDelta = new Vector2(0f, 14f);
            accent.rectTransform.anchoredPosition = new Vector2(0f, -22f);
            accent.raycastTarget = false;

            if (theme != null)
            {
                var icon = UiFactory.Picture("Icone", face.transform, theme.IconForCategory(category.CategoryId), Color.white, false, false);
                UiFactory.AnchorCenter(icon.rectTransform, new Vector2(0f, 70f), new Vector2(150f, 150f));
                icon.preserveAspect = true;
                icon.raycastTarget = false;
            }

            var title = UiFactory.Label("Etiquette", face.transform, category.Title, 40, MontessoriPalette.Ink, TextAnchor.MiddleCenter);
            title.rectTransform.anchorMin = new Vector2(0.08f, theme != null ? 0.16f : 0.28f);
            title.rectTransform.anchorMax = new Vector2(0.92f, theme != null ? 0.42f : 0.72f);
            title.rectTransform.offsetMin = Vector2.zero;
            title.rectTransform.offsetMax = Vector2.zero;
            title.font = ReadableFont();
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 22;
            title.resizeTextMaxSize = 42;
            title.raycastTarget = false;

            var count = UiFactory.Label("Compte", face.transform, "", 24, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            count.rectTransform.anchorMin = new Vector2(0.08f, 0.06f);
            count.rectTransform.anchorMax = new Vector2(0.92f, 0.18f);
            count.rectTransform.offsetMin = Vector2.zero;
            count.rectTransform.offsetMax = Vector2.zero;
            count.font = ReadableFont();
            count.raycastTarget = false;

            var card = face.gameObject.AddComponent<CategoryCard>();
            card.Bind(category, title, count, root, face.rectTransform, group);
            card.Refresh();
            _choiceCards.Add(card);
        }

        void CreateGame(MiniGameDefinition game, Sprite panel, Sprite shadow, ThemeAssets theme)
        {
            var root = UiFactory.Rect("Jeu_" + game.GameId, _area);
            UiFactory.AnchorCenter(root, Vector2.zero, new Vector2(420f, 520f));
            var group = UiFactory.AddGroup(root.gameObject);
            if (shadow != null)
            {
                var shadowImage = UiFactory.Picture("Ombre", root, shadow, MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.26f), false, false);
                UiFactory.Stretch(shadowImage.rectTransform, -16f, -28f, -16f, -6f);
                shadowImage.raycastTarget = false;
            }

            var face = UiFactory.Picture("Face", root, panel, Color.white, panel != null, true);
            UiFactory.Stretch(face.rectTransform, 0f, 0f, 0f, 0f);
            face.preserveAspect = false;
            face.raycastTarget = true;
            face.gameObject.AddComponent<Pressable>();

            var accent = UiFactory.Picture("Accent", face.transform, null, game.Accent, false, false);
            accent.rectTransform.anchorMin = new Vector2(0.12f, 1f);
            accent.rectTransform.anchorMax = new Vector2(0.88f, 1f);
            accent.rectTransform.pivot = new Vector2(0.5f, 1f);
            accent.rectTransform.sizeDelta = new Vector2(0f, 14f);
            accent.rectTransform.anchoredPosition = new Vector2(0f, -26f);
            accent.raycastTarget = false;

            if (theme != null)
            {
                var icon = UiFactory.Picture("Icone", face.transform, theme.IconForGame(game.GameId), Color.white, false, false);
                UiFactory.AnchorCenter(icon.rectTransform, new Vector2(0f, 90f), new Vector2(180f, 180f));
                icon.preserveAspect = true;
                icon.raycastTarget = false;
            }

            var title = UiFactory.Label("Titre", face.transform, game.Title, 36, MontessoriPalette.Ink, TextAnchor.MiddleCenter);
            title.rectTransform.anchorMin = new Vector2(0.08f, 0.24f);
            title.rectTransform.anchorMax = new Vector2(0.92f, 0.42f);
            title.rectTransform.offsetMin = Vector2.zero;
            title.rectTransform.offsetMax = Vector2.zero;
            title.font = ReadableFont();
            title.raycastTarget = false;

            var description = UiFactory.Label("Description", face.transform, game.Description, 24, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            description.rectTransform.anchorMin = new Vector2(0.1f, 0.08f);
            description.rectTransform.anchorMax = new Vector2(0.9f, 0.26f);
            description.rectTransform.offsetMin = Vector2.zero;
            description.rectTransform.offsetMax = Vector2.zero;
            description.font = ReadableFont();
            description.raycastTarget = false;

            var card = face.gameObject.AddComponent<GameCard>();
            card.Bind(game, title, description, root, group);
        }

        static MiniGameDefinition[] GamesFrom(ContentCatalog catalog)
        {
            if (catalog == null)
                return new MiniGameDefinition[0];
            var unique = new List<MiniGameDefinition>();
            var categories = catalog.Categories;
            for (int c = 0; c < categories.Length; c++)
            {
                var category = categories[c];
                if (category == null)
                    continue;
                var games = category.Games;
                for (int i = 0; i < games.Length; i++)
                {
                    var game = games[i];
                    if (game == null || ContainsGame(unique, game.GameId))
                        continue;
                    unique.Add(game);
                }
            }
            return unique.ToArray();
        }

        static bool ContainsGame(List<MiniGameDefinition> games, string gameId)
        {
            for (int i = 0; i < games.Count; i++)
            {
                if (games[i] != null && games[i].GameId == gameId)
                    return true;
            }
            return false;
        }

        void PlayChoiceIntro()
        {
            CacheChoices();
            for (int i = 0; i < _choiceCards.Count; i++)
            {
                var card = _choiceCards[i];
                if (card == null || card.Root == null)
                    continue;
                if (!Application.isPlaying)
                {
                    card.Root.localScale = Vector3.one;
                    if (card.Group != null)
                        card.Group.alpha = 1f;
                    continue;
                }
                if (card.Group != null)
                {
                    card.Group.alpha = 1f;
                    card.Group.interactable = true;
                    card.Group.blocksRaycasts = true;
                }
                card.Root.localScale = Vector3.one * 0.94f;
                float delay = 0.04f + i * 0.06f;
                Motion.Scale(card.Root, Vector3.one, 0.42f, Ease.OutBack).SetDelay(delay);
            }
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
            if (_showingGames)
                Layout();
            else
                LayoutChoices();
        }

        void EnsureRefs()
        {
            if (_choices == null)
            {
                var choices = transform.Find("ZoneChoix") as RectTransform;
                if (choices != null)
                    _choices = choices;
            }
            if (_area == null)
            {
                var area = transform.Find("ZoneJeux") as RectTransform;
                if (area != null)
                    _area = area;
            }
            if (_title == null)
            {
                var title = transform.Find("Entete/Titre");
                if (title != null)
                    _title = title.GetComponent<Text>();
            }
            if (_subtitle == null)
            {
                var subtitle = transform.Find("Entete/SousTitre");
                if (subtitle != null)
                    _subtitle = subtitle.GetComponent<Text>();
            }
        }

        void Cache()
        {
            _cards.Clear();
            if (_area == null)
                return;
            var found = _area.GetComponentsInChildren<GameCard>(true);
            for (int i = 0; i < found.Length; i++)
                _cards.Add(found[i]);
        }

        void CacheChoices()
        {
            _choiceCards.Clear();
            if (_choices == null)
                return;
            var found = _choices.GetComponentsInChildren<CategoryCard>(true);
            for (int i = 0; i < found.Length; i++)
                _choiceCards.Add(found[i]);
        }

        void LayoutChoices()
        {
            CacheChoices();
            if (_choices == null || _choiceCards.Count == 0)
                return;
            float width = _choices.rect.width;
            float height = _choices.rect.height;
            if (width < 80f)
                width = 1700f;
            if (height < 80f)
                height = 760f;

            int count = _choiceCards.Count;
            float gap = 28f;
            float cardW = Mathf.Min(380f, (width - gap * (count - 1)) / count);
            float cardH = Mathf.Min(height - 8f, cardW * 1.22f);
            float total = count * cardW + (count - 1) * gap;
            float x = -total * 0.5f + cardW * 0.5f;
            for (int i = 0; i < count; i++)
            {
                var root = _choiceCards[i].Root;
                if (root == null)
                    continue;
                root.gameObject.SetActive(true);
                root.anchoredPosition = new Vector2(x, 0f);
                root.sizeDelta = new Vector2(cardW, cardH);
                x += cardW + gap;
            }
        }

        void Layout()
        {
            if (_area == null)
                return;
            Cache();
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

        void ApplyChoiceFonts()
        {
            if (_choices == null)
                return;
            var texts = _choices.GetComponentsInChildren<Text>(true);
            for (int i = 0; i < texts.Length; i++)
                ApplyFont(texts[i]);
        }

        static void ApplyFont(Text text)
        {
            if (text == null)
                return;
            var font = ReadableFont();
            if (font != null)
                text.font = font;
            text.raycastTarget = false;
        }

        public static Font ReadableFont()
        {
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return font != null ? font : UiFont.Builtin;
        }
    }
}
