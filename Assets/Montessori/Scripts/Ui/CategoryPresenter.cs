using System;
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
        bool _choiceLayoutDirty;
        bool _gameLayoutDirty;
        bool _choicesIntroPending;
        bool _gamesIntroPending;

        public bool ShowingGames => _showingGames;

        public void Construct(ThemeAssets theme, ContentCatalog catalog, MiniGameDefinition[] games)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);

            BuildHeader(theme);
            _choices = UiFactory.Rect("ZoneChoix", transform);
            _area = UiFactory.Rect("ZoneJeux", transform);
            FitZones();

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
            var theme = HarvestTheme(panel, shadow);
            if (_choices == null)
                _choices = UiFactory.Rect("ZoneChoix", transform);
            if (_area == null)
                _area = UiFactory.Rect("ZoneJeux", transform);
            FitZones();
            RestyleHeader();

            RetireChildren(_choices);
            _choiceCards.Clear();
            BuildChoices(catalog, theme.Panel, theme.Shadow, theme);

            RetireChildren(_area);
            _cards.Clear();
            BuildGames(GamesFrom(catalog), theme.Panel, theme.Shadow, theme);

            if (!_showingGames)
            {
                _area.gameObject.SetActive(false);
                _choices.gameObject.SetActive(true);
            }
            ApplyChoiceFonts();
            Canvas.ForceUpdateCanvases();
            _choicesIntroPending = false;
            LayoutChoices();
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
            if (_choices != null)
                _choices.SetSiblingIndex(1);
            _choicesIntroPending = true;
            Canvas.ForceUpdateCanvases();
            if (!LayoutChoices())
                ParkUntilLaidOut(_choiceCards);
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
                var labels = card.Root.GetComponentsInChildren<Text>(true);
                for (int t = 0; t < labels.Length; t++)
                    ApplyFont(labels[t]);
            }
            if (_area != null)
                _area.SetSiblingIndex(1);
            _gamesIntroPending = true;
            Canvas.ForceUpdateCanvases();
            if (!Layout())
                ParkUntilLaidOut(_cards);
        }

        void BuildHeader(ThemeAssets theme)
        {
            var header = UiFactory.Rect("Entete", transform);
            UiFactory.AnchorTop(header, 156f, 28f, 28f);
            if (theme != null)
                BuildBackButton(header, theme);

            _title = UiFactory.Label("Titre", header, "Catégories", 48, MontessoriPalette.WalnutDeep, TextAnchor.MiddleCenter);
            UiFactory.Stretch(_title.rectTransform, 120f, 64f, 120f, 6f);
            _title.font = ReadableFont();
            _title.fontStyle = FontStyle.Bold;
            _title.alignByGeometry = false;
            _subtitle = UiFactory.Label("SousTitre", header, "Alphabet, chiffres, formes et couleurs", 24, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            UiFactory.Stretch(_subtitle.rectTransform, 120f, 10f, 120f, 96f);
            _subtitle.font = ReadableFont();
            _subtitle.alignByGeometry = false;
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
            UiFactory.AnchorCenter(root, Vector2.zero, new Vector2(300f, 392f));
            var group = UiFactory.AddGroup(root.gameObject);
            BuildShadow(root, shadow);

            var face = BuildFace(root, panel);
            Sprite pearl = theme != null ? theme.Pearl : null;
            BuildHalo(face.transform, pearl, category.Accent);
            RectTransform medal;
            if (category.CategoryId == "alphabet")
            {
                medal = BuildMedallion(face.transform, pearl);
                BuildAlphabetMark(medal);
            }
            else
                medal = BuildBadge(face.transform, theme != null ? theme.IconForCategory(category.CategoryId) : null, pearl);

            BuildRibbon(face.transform, pearl, category.Accent);

            var title = BuildTitle(face.transform, category.Title, 36);
            var count = BuildCaption(face.transform, "Compte", "", 22, MontessoriPalette.InkSoft);

            face.gameObject.AddComponent<Pressable>();
            root.gameObject.AddComponent<ShelfTile>().Apply();
            var card = face.gameObject.AddComponent<CategoryCard>();
            card.Bind(category, title, count, root, medal, group);
            card.Refresh();
            _choiceCards.Add(card);
        }

        void CreateGame(MiniGameDefinition game, Sprite panel, Sprite shadow, ThemeAssets theme)
        {
            var root = UiFactory.Rect("Jeu_" + game.GameId, _area);
            UiFactory.AnchorCenter(root, Vector2.zero, new Vector2(360f, 460f));
            var group = UiFactory.AddGroup(root.gameObject);
            BuildShadow(root, shadow);

            var face = BuildFace(root, panel);
            Sprite pearl = theme != null ? theme.Pearl : null;
            BuildHalo(face.transform, pearl, game.Accent);
            var medal = BuildBadge(face.transform, theme != null ? theme.IconForGame(game.GameId) : null, pearl);
            BuildRibbon(face.transform, pearl, game.Accent);

            var title = BuildTitle(face.transform, game.Title, 34);
            var description = BuildCaption(face.transform, "Description", game.Description, 22, MontessoriPalette.InkSoft);
            description.resizeTextForBestFit = true;
            description.resizeTextMinSize = 16;
            description.resizeTextMaxSize = 24;

            face.gameObject.AddComponent<Pressable>();
            root.gameObject.AddComponent<ShelfTile>().Apply();
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

        void LateUpdate()
        {
            if (!isActiveAndEnabled)
                return;
            if (!_showingGames && _choiceLayoutDirty)
                LayoutChoices();
            else if (_showingGames && _gameLayoutDirty)
                Layout();
        }

        void PlayChoiceIntro()
        {
            CacheChoices();
            for (int i = 0; i < _choiceCards.Count; i++)
                PlayTileIntro(_choiceCards[i] != null ? _choiceCards[i].Root : null, _choiceCards[i] != null ? _choiceCards[i].Group : null, i);
        }

        void PlayIntro()
        {
            int visibleIndex = 0;
            for (int i = 0; i < _cards.Count; i++)
            {
                var card = _cards[i];
                if (card == null || card.Root == null || !card.Root.gameObject.activeSelf)
                    continue;
                PlayTileIntro(card.Root, card.Group, visibleIndex);
                visibleIndex++;
            }
        }

        static void PlayTileIntro(RectTransform root, CanvasGroup group, int index)
        {
            if (root == null)
                return;
            if (group != null)
            {
                group.alpha = 1f;
                group.interactable = true;
                group.blocksRaycasts = true;
            }
            if (!Application.isPlaying)
            {
                root.localScale = Vector3.one;
                return;
            }
            Motion.Kill(root, "scale");
            root.localScale = Vector3.one * 0.97f;
            Motion.Scale(root, Vector3.one, 0.36f, Ease.OutCubic).SetDelay(0.03f + index * 0.05f).OnComplete(() =>
            {
                if (root != null)
                    root.localScale = Vector3.one;
            });
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

        bool LayoutChoices()
        {
            CacheChoices();
            if (_choices == null || _choiceCards.Count == 0)
            {
                _choiceLayoutDirty = false;
                return false;
            }
            var tiles = new List<RectTransform>(_choiceCards.Count);
            for (int i = 0; i < _choiceCards.Count; i++)
            {
                if (_choiceCards[i] != null && _choiceCards[i].Root != null)
                    tiles.Add(_choiceCards[i].Root);
            }
            bool placed = PlaceAll(_choices, tiles, 360f, 1.22f);
            _choiceLayoutDirty = !placed;
            if (!placed)
                return false;
            if (_choicesIntroPending)
            {
                _choicesIntroPending = false;
                PlayChoiceIntro();
            }
            return true;
        }

        bool Layout()
        {
            if (_area == null)
            {
                _gameLayoutDirty = false;
                return false;
            }
            Cache();
            var visible = new List<RectTransform>();
            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i] != null && _cards[i].Root != null && _cards[i].Root.gameObject.activeSelf)
                    visible.Add(_cards[i].Root);
            }
            if (visible.Count == 0)
            {
                _gameLayoutDirty = false;
                return true;
            }
            bool placed = PlaceAll(_area, visible, 400f, 1.2f);
            _gameLayoutDirty = !placed;
            if (!placed)
                return false;
            if (_gamesIntroPending)
            {
                _gamesIntroPending = false;
                PlayIntro();
            }
            return true;
        }

        static bool PlaceAll(RectTransform area, List<RectTransform> tiles, float maxCardWidth, float heightRatio)
        {
            if (area == null || tiles == null || tiles.Count == 0)
                return false;
            float width = area.rect.width;
            float height = area.rect.height;
            if (width < 160f || height < 160f)
                return false;

            int count = tiles.Count;
            const float gap = 40f;
            int rows = 1;
            int columns = count;
            float cardW = (width - gap * (columns - 1)) / columns;
            if (cardW > maxCardWidth)
                cardW = maxCardWidth;
            if (count > 3 && cardW < 230f)
            {
                rows = 2;
                columns = Mathf.CeilToInt(count * 0.5f);
                cardW = (width - gap * (columns - 1)) / columns;
                if (cardW > maxCardWidth)
                    cardW = maxCardWidth;
            }

            float maxRowH = rows == 1 ? height : (height - gap) / rows;
            float cardH = cardW * heightRatio;
            if (cardH > maxRowH)
            {
                cardH = maxRowH;
                cardW = cardH / heightRatio;
            }
            float rowW = columns * cardW + (columns - 1) * gap;
            if (rowW > width)
            {
                cardW = (width - gap * (columns - 1)) / columns;
                cardH = Mathf.Min(cardW * heightRatio, maxRowH);
            }
            if (cardW < 48f || cardH < 48f)
                return false;

            float totalH = rows * cardH + (rows - 1) * gap;
            float yTop = totalH * 0.5f - cardH * 0.5f - 6f;
            for (int i = 0; i < count; i++)
            {
                var tile = tiles[i];
                if (tile == null)
                    continue;
                int row = i / columns;
                int col = i % columns;
                int inRow = Mathf.Min(columns, count - row * columns);
                float thisRow = inRow * cardW + (inRow - 1) * gap;
                float x = -thisRow * 0.5f + cardW * 0.5f + col * (cardW + gap);
                float y = yTop - row * (cardH + gap);
                tile.anchorMin = new Vector2(0.5f, 0.5f);
                tile.anchorMax = new Vector2(0.5f, 0.5f);
                tile.pivot = new Vector2(0.5f, 0.5f);
                if (tile.localScale.x < 0.2f)
                    tile.localScale = Vector3.one;
                tile.sizeDelta = new Vector2(cardW, cardH);
                tile.anchoredPosition = new Vector2(x, y);
                tile.SetSiblingIndex(i);
                var group = tile.GetComponent<CanvasGroup>();
                if (group != null)
                {
                    group.alpha = 1f;
                    group.interactable = true;
                    group.blocksRaycasts = true;
                }
                var shelf = tile.GetComponent<ShelfTile>();
                if (shelf != null)
                    shelf.Apply();
            }
            return true;
        }

        void FitZones()
        {
            if (_choices != null)
                UiFactory.Stretch(_choices, 72f, 36f, 72f, 180f);
            if (_area != null)
                UiFactory.Stretch(_area, 72f, 36f, 72f, 180f);
        }

        void RestyleHeader()
        {
            var header = transform.Find("Entete") as RectTransform;
            if (header != null)
                UiFactory.AnchorTop(header, 156f, 28f, 28f);
            if (_title != null)
            {
                _title.fontSize = 48;
                _title.fontStyle = FontStyle.Bold;
                _title.color = MontessoriPalette.WalnutDeep;
                _title.alignByGeometry = false;
                UiFactory.Stretch(_title.rectTransform, 120f, 64f, 120f, 8f);
                ApplyFont(_title);
            }
            if (_subtitle != null)
            {
                _subtitle.fontSize = 24;
                _subtitle.color = MontessoriPalette.InkSoft;
                _subtitle.alignByGeometry = false;
                _subtitle.horizontalOverflow = HorizontalWrapMode.Wrap;
                _subtitle.verticalOverflow = VerticalWrapMode.Overflow;
                UiFactory.Stretch(_subtitle.rectTransform, 120f, 10f, 120f, 92f);
                ApplyFont(_subtitle);
            }
        }

        ThemeAssets HarvestTheme(Sprite panel, Sprite shadow)
        {
            var theme = new ThemeAssets
            {
                Panel = panel,
                Shadow = shadow
            };
            var images = transform.root.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < images.Length; i++)
            {
                var image = images[i];
                if (image == null || image.sprite == null)
                    continue;
                string spriteName = image.sprite.name.ToLowerInvariant();
                if (theme.Panel == null && (spriteName.Contains("panel") || spriteName.Contains("wood-plate")) && spriteName.IndexOf("icon", StringComparison.Ordinal) < 0)
                    theme.Panel = image.sprite;
                if (theme.Shadow == null && spriteName.Contains("shadow"))
                    theme.Shadow = image.sprite;
                if (theme.Pearl == null && spriteName.Contains("pearl"))
                    theme.Pearl = image.sprite;
                NoteIcon(theme, image);
            }
            if (theme.Pearl == null)
            {
                var pearlFace = transform.Find("Entete/Retour/Face");
                var pearlImage = pearlFace != null ? pearlFace.GetComponent<Image>() : null;
                if (pearlImage != null)
                    theme.Pearl = pearlImage.sprite;
            }
            return theme;
        }

        static void NoteIcon(ThemeAssets theme, Image image)
        {
            string spriteName = image.sprite.name.ToLowerInvariant();
            if (theme.IconAlphabet == null && (spriteName.Contains("alphabet")))
                theme.IconAlphabet = image.sprite;
            if (theme.IconDigits == null && (spriteName.Contains("digit")))
                theme.IconDigits = image.sprite;
            if (theme.IconShapes == null && (spriteName.Contains("shape")))
                theme.IconShapes = image.sprite;
            if (theme.IconColors == null && (spriteName.Contains("color") || spriteName.Contains("colour")))
                theme.IconColors = image.sprite;
            if (theme.IconPuzzle == null && spriteName.Contains("puzzle"))
                theme.IconPuzzle = image.sprite;
            if (theme.IconImagier == null && spriteName.Contains("imagier"))
                theme.IconImagier = image.sprite;
            if (theme.IconTrace == null && (spriteName.Contains("trace") || spriteName.Contains("trac")))
                theme.IconTrace = image.sprite;

            if (image.gameObject.name != "Icone")
                return;
            Transform cursor = image.transform.parent;
            while (cursor != null)
            {
                if (cursor.name.StartsWith("Choix_"))
                {
                    AssignCategoryIcon(theme, cursor.name.Substring(6), image.sprite);
                    return;
                }
                if (cursor.name.StartsWith("Jeu_"))
                {
                    AssignGameIcon(theme, cursor.name.Substring(4), image.sprite);
                    return;
                }
                cursor = cursor.parent;
            }
        }

        static void AssignCategoryIcon(ThemeAssets theme, string categoryId, Sprite sprite)
        {
            if (categoryId == "alphabet" && theme.IconAlphabet == null)
                theme.IconAlphabet = sprite;
            else if (categoryId == "chiffres" && theme.IconDigits == null)
                theme.IconDigits = sprite;
            else if (categoryId == "formes" && theme.IconShapes == null)
                theme.IconShapes = sprite;
            else if (categoryId == "couleurs" && theme.IconColors == null)
                theme.IconColors = sprite;
        }

        static void AssignGameIcon(ThemeAssets theme, string gameId, Sprite sprite)
        {
            if (gameId == GameIds.Puzzle && theme.IconPuzzle == null)
                theme.IconPuzzle = sprite;
            else if (gameId == GameIds.Imagier && theme.IconImagier == null)
                theme.IconImagier = sprite;
            else if (gameId == GameIds.Tracing && theme.IconTrace == null)
                theme.IconTrace = sprite;
        }

        static void RetireChildren(RectTransform parent)
        {
            if (parent == null)
                return;
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                var child = parent.GetChild(i).gameObject;
                child.SetActive(false);
                child.transform.SetParent(null, false);
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(child);
                else
                    UnityEngine.Object.DestroyImmediate(child);
            }
        }

        static void BuildShadow(RectTransform root, Sprite shadow)
        {
            if (shadow == null)
                return;
            var shadowImage = UiFactory.Picture("Ombre", root, shadow, MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.33f), false, false);
            UiFactory.Stretch(shadowImage.rectTransform, -12f, -30f, -12f, 8f);
            shadowImage.preserveAspect = false;
            shadowImage.raycastTarget = false;
        }

        static Image BuildFace(RectTransform root, Sprite panel)
        {
            var face = UiFactory.Picture("Face", root, panel, Color.white, panel != null, true);
            UiFactory.Stretch(face.rectTransform, 0f, 0f, 0f, 0f);
            face.preserveAspect = false;
            face.raycastTarget = true;
            return face;
        }

        static void BuildHalo(Transform parent, Sprite pearl, Color accent)
        {
            if (pearl != null)
            {
                var halo = UiFactory.Picture("Halo", parent, pearl, accent, false, false);
                halo.preserveAspect = true;
                halo.raycastTarget = false;
                return;
            }
            var rect = UiFactory.Rect("Halo", parent);
            var shape = rect.gameObject.AddComponent<SoftShape>();
            shape.Configure(TokenShape.Disc, accent);
            shape.raycastTarget = false;
        }

        static RectTransform BuildMedallion(Transform parent, Sprite pearl)
        {
            if (pearl != null)
            {
                var medal = UiFactory.Picture("Medaillon", parent, pearl, MontessoriPalette.Cream, false, false);
                medal.preserveAspect = true;
                medal.raycastTarget = false;
                return medal.rectTransform;
            }
            var rect = UiFactory.Rect("Medaillon", parent);
            var shape = rect.gameObject.AddComponent<SoftShape>();
            shape.Configure(TokenShape.Disc, MontessoriPalette.Cream);
            shape.raycastTarget = false;
            return rect;
        }

        static RectTransform BuildBadge(Transform parent, Sprite icon, Sprite pearl)
        {
            if (icon != null)
            {
                var badge = UiFactory.Picture("Medaillon", parent, icon, Color.white, false, false);
                badge.preserveAspect = true;
                badge.raycastTarget = false;
                return badge.rectTransform;
            }
            return BuildMedallion(parent, pearl);
        }

        static void BuildAlphabetMark(RectTransform medal)
        {
            if (medal == null)
                return;
            var a = UiFactory.Label("Glyphe", medal, "A", 78, MontessoriPalette.VowelBlue, TextAnchor.MiddleCenter);
            a.rectTransform.anchorMin = new Vector2(0.04f, 0.14f);
            a.rectTransform.anchorMax = new Vector2(0.50f, 0.86f);
            a.rectTransform.offsetMin = Vector2.zero;
            a.rectTransform.offsetMax = Vector2.zero;
            StyleLetter(a);
            var m = UiFactory.Label("Glyphe", medal, "M", 78, MontessoriPalette.ConsonantRose, TextAnchor.MiddleCenter);
            m.rectTransform.anchorMin = new Vector2(0.46f, 0.14f);
            m.rectTransform.anchorMax = new Vector2(0.96f, 0.86f);
            m.rectTransform.offsetMin = Vector2.zero;
            m.rectTransform.offsetMax = Vector2.zero;
            StyleLetter(m);
        }

        static void StyleLetter(Text letter)
        {
            letter.font = ReadableFont();
            letter.fontStyle = FontStyle.Bold;
            letter.alignByGeometry = false;
            letter.resizeTextForBestFit = true;
            letter.resizeTextMinSize = 28;
            letter.resizeTextMaxSize = 96;
            letter.horizontalOverflow = HorizontalWrapMode.Wrap;
            letter.verticalOverflow = VerticalWrapMode.Truncate;
            letter.raycastTarget = false;
            var shadow = letter.gameObject.AddComponent<Shadow>();
            shadow.effectColor = MontessoriPalette.WithAlpha(letter.color, 0.18f);
            shadow.effectDistance = new Vector2(0f, -2f);
            shadow.useGraphicAlpha = true;
        }

        static void BuildRibbon(Transform parent, Sprite pearl, Color accent)
        {
            var ribbon = UiFactory.Picture("Ruban", parent, pearl, accent, false, false);
            ribbon.preserveAspect = false;
            ribbon.raycastTarget = false;
        }

        static Text BuildTitle(Transform parent, string value, int size)
        {
            var title = UiFactory.Label("Etiquette", parent, value, size, MontessoriPalette.WalnutDeep, TextAnchor.MiddleCenter);
            title.font = ReadableFont();
            title.fontStyle = FontStyle.Bold;
            title.alignByGeometry = false;
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 20;
            title.resizeTextMaxSize = size;
            title.horizontalOverflow = HorizontalWrapMode.Wrap;
            title.verticalOverflow = VerticalWrapMode.Truncate;
            title.raycastTarget = false;
            return title;
        }

        static Text BuildCaption(Transform parent, string name, string value, int size, Color color)
        {
            var caption = UiFactory.Label(name, parent, value, size, color, TextAnchor.MiddleCenter);
            caption.font = ReadableFont();
            caption.alignByGeometry = false;
            caption.resizeTextForBestFit = true;
            caption.resizeTextMinSize = 15;
            caption.resizeTextMaxSize = size;
            caption.horizontalOverflow = HorizontalWrapMode.Wrap;
            caption.verticalOverflow = VerticalWrapMode.Truncate;
            caption.raycastTarget = false;
            return caption;
        }

        static void ParkUntilLaidOut(List<CategoryCard> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i] != null && cards[i].Root != null)
                    cards[i].Root.localScale = Vector3.zero;
            }
        }

        static void ParkUntilLaidOut(List<GameCard> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i] != null && cards[i].Root != null)
                    cards[i].Root.localScale = Vector3.zero;
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

    public sealed class ShelfTile : MonoBehaviour
    {
        void OnEnable()
        {
            Apply();
        }

        void OnRectTransformDimensionsChange()
        {
            Apply();
        }

        public void Apply()
        {
            var root = (RectTransform)transform;
            float w = root.rect.width;
            float h = root.rect.height;
            if (w < 24f || h < 24f)
                return;

            float medal = Mathf.Min(w * 0.54f, h * 0.42f);
            float medalY = h * 0.14f;
            var medalRect = FindRect("Medaillon");
            Place(FindRect("Halo"), new Vector2(0f, medalY), new Vector2(medal * 1.18f, medal * 1.18f));
            Place(medalRect, new Vector2(0f, medalY), new Vector2(medal, medal));

            var title = FindRect("Etiquette");
            if (title == null)
                title = FindRect("Titre");
            float titleH = Mathf.Clamp(h * 0.13f, 40f, 60f);
            float titleY = -h * 0.18f;
            float medalBottom = medalY - medal * 0.5f;
            float titleTop = titleY + titleH * 0.5f;
            if (titleTop > medalBottom - 14f)
                titleY -= titleTop - (medalBottom - 14f);
            Place(title, new Vector2(0f, titleY), new Vector2(w * 0.9f, titleH));

            var ribbon = FindRect("Ruban");
            titleTop = titleY + titleH * 0.5f;
            if (ribbon != null)
            {
                float room = medalBottom - titleTop;
                if (room > 16f)
                {
                    ribbon.gameObject.SetActive(true);
                    float ribbonY = medalBottom - room * 0.45f;
                    Place(ribbon, new Vector2(0f, ribbonY), new Vector2(Mathf.Clamp(w * 0.28f, 42f, 96f), 12f));
                }
                else
                    ribbon.gameObject.SetActive(false);
            }

            var description = FindRect("Description");
            var count = FindRect("Compte");
            float floor = -h * 0.5f + 16f;
            if (description != null)
            {
                float descH = Mathf.Clamp(h * 0.16f, 44f, 78f);
                float descY = titleY - titleH * 0.5f - 10f - descH * 0.5f;
                if (descY - descH * 0.5f < floor)
                    descY = floor + descH * 0.5f;
                Place(description, new Vector2(0f, descY), new Vector2(w * 0.84f, descH));
            }
            else if (count != null)
            {
                float countH = Mathf.Clamp(h * 0.09f, 26f, 38f);
                float countY = titleY - titleH * 0.5f - h * 0.03f - countH * 0.5f;
                if (countY - countH * 0.5f < floor)
                    countY = floor + countH * 0.5f;
                Place(count, new Vector2(0f, countY), new Vector2(w * 0.84f, countH));
            }
        }

        RectTransform FindRect(string childName)
        {
            var transforms = GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i] != transform && transforms[i].name == childName)
                    return transforms[i] as RectTransform;
            }
            return null;
        }

        static void Place(RectTransform rect, Vector2 position, Vector2 size)
        {
            if (rect == null)
                return;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }
    }
}
