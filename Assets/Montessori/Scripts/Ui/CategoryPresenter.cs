using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class CategoryPresenter : MonoBehaviour
    {
        [SerializeField] TMP_Text _title;
        [SerializeField] TMP_Text _subtitle;
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

            BuildHeader();
            _choices = UiFactory.Rect("ZoneChoix", transform);
            _area = UiFactory.Rect("ZoneJeux", transform);
            FitZones();

            BuildChoices(catalog);
            if (games == null || games.Length == 0)
                games = GamesFrom(catalog);
            BuildGames(games);

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
                _choices = UiFactory.Rect("ZoneChoix", transform);
            if (_area == null)
                _area = UiFactory.Rect("ZoneJeux", transform);
            FitZones();
            RebuildHeader();

            RetireChildren(_choices);
            _choiceCards.Clear();
            BuildChoices(catalog);

            RetireChildren(_area);
            _cards.Clear();
            BuildGames(GamesFrom(catalog));

            if (!_showingGames)
            {
                _area.gameObject.SetActive(false);
                _choices.gameObject.SetActive(true);
            }
            ApplyChoiceFonts();
            Canvas.ForceUpdateCanvases();
            _choicesIntroPending = false;
            LayoutChoices();
            ScreenBackdrop.Ensure(transform, "categories");
        }

        public void ShowChoices()
        {
            EnsureRefs();
            _showingGames = false;
            if (_title != null)
                _title.text = "Catégories";
            if (_subtitle != null)
                _subtitle.text = "Alphabet, chiffres, formes, couleurs et mots";
            ApplyFont(_title);
            ApplyFont(_subtitle);
            StyleHeader();
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
            ScreenBackdrop.Ensure(transform, "categories");
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
            StyleHeader();

            for (int i = 0; i < _cards.Count; i++)
            {
                var card = _cards[i];
                if (card == null || card.Root == null)
                    continue;
                bool available = category != null && category.HasGame(card.GameId);
                card.Root.gameObject.SetActive(available);
                card.Refresh();
                var labels = card.Root.GetComponentsInChildren<TMP_Text>(true);
                for (int t = 0; t < labels.Length; t++)
                    ApplyFont(labels[t]);
            }
            if (_area != null)
                _area.SetSiblingIndex(1);
            _gamesIntroPending = true;
            Canvas.ForceUpdateCanvases();
            if (!Layout())
                ParkUntilLaidOut(_cards);
            ScreenBackdrop.Ensure(transform, "categories");
        }

        void BuildHeader()
        {
            var header = UiFactory.Rect("Entete", transform);
            UiFactory.AnchorTop(header, 156f, 28f, 28f);
            BuildBackButton(header);

            _title = UiFactory.Tmp("Titre", header, "Catégories", 56f, Color.white, true);
            UiFactory.Stretch(_title.rectTransform, 120f, 64f, 120f, 6f);
            _subtitle = UiFactory.Tmp("SousTitre", header, "Alphabet, chiffres, formes, couleurs et mots", 26f, Color.white, false);
            UiFactory.Stretch(_subtitle.rectTransform, 120f, 10f, 120f, 96f);
            StyleHeader();
        }

        void RebuildHeader()
        {
            var existing = transform.Find("Entete");
            if (existing != null)
            {
                existing.gameObject.SetActive(false);
                existing.SetParent(null, false);
                if (Application.isPlaying)
                    Destroy(existing.gameObject);
                else
                    DestroyImmediate(existing.gameObject);
            }
            _title = null;
            _subtitle = null;
            BuildHeader();
        }

        static void BuildBackButton(Transform header)
        {
            var back = UiFactory.Rect("Retour", header);
            back.anchorMin = new Vector2(0f, 0.5f);
            back.anchorMax = new Vector2(0f, 0.5f);
            back.pivot = new Vector2(0f, 0.5f);
            back.sizeDelta = new Vector2(92f, 92f);
            back.anchoredPosition = new Vector2(8f, 0f);
            var image = UiFactory.Picture("Face", back, UiFactory.CircleSprite(), CoralButton, false, true);
            UiFactory.Stretch(image.rectTransform, 0f, 0f, 0f, 0f);
            image.preserveAspect = true;
            image.raycastTarget = true;
            var drop = image.gameObject.AddComponent<Shadow>();
            drop.effectColor = new Color(0f, 0f, 0f, 0.28f);
            drop.effectDistance = new Vector2(0f, -5f);
            drop.useGraphicAlpha = true;
            var label = UiFactory.Tmp("Glyph", image.transform, "<", 64f, Color.white, true);
            UiFactory.Stretch(label.rectTransform, 0f, 4f, 6f, 0f);
            image.gameObject.AddComponent<Pressable>();
            UiFactory.CreateButton(image.gameObject);
        }

        void BuildChoices(ContentCatalog catalog)
        {
            if (_choices == null || catalog == null)
                return;
            var categories = catalog.Categories;
            for (int i = 0; i < categories.Length; i++)
            {
                if (categories[i] != null)
                    CreateChoice(categories[i]);
            }
        }

        void BuildGames(MiniGameDefinition[] games)
        {
            if (_area == null || games == null)
                return;
            for (int i = 0; i < games.Length; i++)
            {
                if (games[i] != null)
                    CreateGame(games[i]);
            }
        }

        void CreateChoice(LearningCategory category)
        {
            var root = UiFactory.Rect("Choix_" + category.CategoryId, _choices);
            UiFactory.AnchorCenter(root, Vector2.zero, new Vector2(300f, 392f));
            var group = UiFactory.AddGroup(root.gameObject);

            var face = BuildFace(root, BorderFor(category.CategoryId));
            var disc = BuildDisc(face.transform, PastelFor(category.CategoryId));
            BuildMark(disc, category.CategoryId);

            var title = BuildTitle(face.transform, category.Title, 36f);
            var count = BuildCaption(face.transform, "Compte", "", 26f, MontessoriPalette.Ink);

            face.gameObject.AddComponent<Pressable>();
            root.gameObject.AddComponent<ShelfTile>().Apply();
            var card = face.gameObject.AddComponent<CategoryCard>();
            card.Bind(category, title, count, root, disc, group);
            card.Refresh();
            _choiceCards.Add(card);
        }

        void CreateGame(MiniGameDefinition game)
        {
            var root = UiFactory.Rect("Jeu_" + game.GameId, _area);
            UiFactory.AnchorCenter(root, Vector2.zero, new Vector2(360f, 460f));
            var group = UiFactory.AddGroup(root.gameObject);

            var face = BuildFace(root, BorderFor(game.GameId));
            var disc = BuildDisc(face.transform, PastelFor(game.GameId));
            BuildMark(disc, game.GameId);

            var title = BuildTitle(face.transform, game.Title, 34f);
            var description = BuildCaption(face.transform, "Description", game.Description, 22f, MontessoriPalette.Ink);
            description.enableAutoSizing = true;
            description.fontSizeMin = 16f;
            description.fontSizeMax = 24f;

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
                    _title = title.GetComponent<TMP_Text>();
            }
            if (_subtitle == null)
            {
                var subtitle = transform.Find("Entete/SousTitre");
                if (subtitle != null)
                    _subtitle = subtitle.GetComponent<TMP_Text>();
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

        void StyleHeader()
        {
            var header = transform.Find("Entete") as RectTransform;
            if (header != null)
                UiFactory.AnchorTop(header, 156f, 28f, 28f);
            if (_title != null)
            {
                _title.fontSize = 56f;
                _title.fontStyle = FontStyles.Bold;
                _title.color = Color.white;
                _title.alignment = TextAlignmentOptions.Center;
                UiFactory.Stretch(_title.rectTransform, 120f, 64f, 120f, 8f);
                ApplyFont(_title);
                var outline = _title.GetComponent<Outline>();
                if (outline == null)
                    outline = _title.gameObject.AddComponent<Outline>();
                outline.effectColor = Navy;
                outline.effectDistance = new Vector2(4f, -4f);
                outline.useGraphicAlpha = true;
            }
            if (_subtitle != null)
            {
                _subtitle.fontSize = 26f;
                _subtitle.color = Color.white;
                _subtitle.alignment = TextAlignmentOptions.Center;
                _subtitle.textWrappingMode = TextWrappingModes.Normal;
                _subtitle.overflowMode = TextOverflowModes.Overflow;
                UiFactory.Stretch(_subtitle.rectTransform, 120f, 10f, 120f, 92f);
                ApplyFont(_subtitle);
                var shadow = _subtitle.GetComponent<Shadow>();
                if (shadow == null || shadow is Outline)
                    shadow = _subtitle.gameObject.AddComponent<Shadow>();
                shadow.effectColor = new Color(0f, 0f, 0f, 0.35f);
                shadow.effectDistance = new Vector2(0f, -2f);
                shadow.useGraphicAlpha = true;
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
            if (theme.IconParlant == null && spriteName.Contains("parlant"))
                theme.IconParlant = image.sprite;
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
            else if (categoryId == WordThemes.CategoryId && theme.IconParlant == null)
                theme.IconParlant = sprite;
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

        static readonly Color CardPaper = new Color(1f, 0.99215686f, 0.96862745f, 1f);
        static readonly Color Navy = new Color(0.102f, 0.212f, 0.365f, 1f);
        static readonly Color CoralButton = new Color(1f, 0.420f, 0.345f, 1f);

        static Image BuildFace(RectTransform root, Color border)
        {
            var sprite = UiFactory.RoundedSprite();
            var face = UiFactory.Picture("Face", root, sprite, CardPaper, sprite != null, true);
            UiFactory.Stretch(face.rectTransform, 0f, 0f, 0f, 0f);
            face.preserveAspect = false;
            face.material = null;
            face.raycastTarget = true;
            var outline = face.gameObject.AddComponent<Outline>();
            outline.effectColor = border;
            outline.effectDistance = new Vector2(7f, -7f);
            outline.useGraphicAlpha = true;
            var drop = face.gameObject.AddComponent<Shadow>();
            drop.effectColor = new Color(0f, 0f, 0f, 0.2f);
            drop.effectDistance = new Vector2(0f, -8f);
            drop.useGraphicAlpha = true;
            return face;
        }

        static RectTransform BuildDisc(Transform parent, Color pastel)
        {
            var disc = UiFactory.Picture("Disque", parent, UiFactory.CircleSprite(), pastel, false, false);
            disc.preserveAspect = true;
            disc.raycastTarget = false;
            return disc.rectTransform;
        }

        static void BuildMark(RectTransform disc, string id)
        {
            if (disc == null)
                return;
            if (id == "formes" || id == GameIds.Puzzle)
            {
                BuildShapeCluster(disc);
                return;
            }
            if (id == "couleurs")
            {
                BuildColorCluster(disc);
                return;
            }
            if (id == GameIds.Tracing)
            {
                BuildGlyph(disc, "A", MontessoriPalette.VowelBlue, 72f);
                var bar = UiFactory.Rect("Trait", disc);
                bar.anchorMin = new Vector2(0.22f, 0.18f);
                bar.anchorMax = new Vector2(0.78f, 0.30f);
                bar.offsetMin = Vector2.zero;
                bar.offsetMax = Vector2.zero;
                var line = bar.gameObject.AddComponent<Image>();
                line.color = MontessoriPalette.Coral;
                line.raycastTarget = false;
                return;
            }
            string glyph = "Aa";
            Color ink = MontessoriPalette.Hex("#6B3FA0");
            float size = 64f;
            if (id == "alphabet")
            {
                glyph = "ABC";
                ink = MontessoriPalette.Hex("#E23B4A");
                size = 52f;
            }
            else if (id == "chiffres")
            {
                glyph = "1 2 3";
                ink = MontessoriPalette.VowelBlue;
                size = 44f;
            }
            BuildGlyph(disc, glyph, ink, size);
        }

        static void BuildGlyph(RectTransform disc, string value, Color color, float size)
        {
            var label = UiFactory.Tmp("Marque", disc, value, size, color, true);
            UiFactory.Stretch(label.rectTransform, 10f, 12f, 10f, 8f);
            label.enableAutoSizing = true;
            label.fontSizeMin = 22f;
            label.fontSizeMax = size;
        }

        static void BuildShapeCluster(RectTransform disc)
        {
            var circle = UiFactory.Picture("Rond", disc, UiFactory.CircleSprite(), MontessoriPalette.Hex("#2F74D0"), false, false);
            AnchorIn(circle.rectTransform, new Vector2(0.30f, 0.58f), new Vector2(0.34f, 0.34f));
            var square = UiFactory.Rect("Carre", disc);
            AnchorIn(square, new Vector2(0.62f, 0.40f), new Vector2(0.30f, 0.30f));
            var squareImage = square.gameObject.AddComponent<Image>();
            squareImage.color = new Color(0.12f, 0.12f, 0.14f, 1f);
            squareImage.raycastTarget = false;
            var triangle = UiFactory.Rect("Triangle", disc);
            AnchorIn(triangle, new Vector2(0.70f, 0.68f), new Vector2(0.32f, 0.32f));
            var shape = triangle.gameObject.AddComponent<SoftShape>();
            shape.Configure(TokenShape.Triangle, MontessoriPalette.Hex("#E24B4B"));
        }

        static void BuildColorCluster(RectTransform disc)
        {
            PlaceSwatch(disc, "Rouge", MontessoriPalette.Hex("#E24B4B"), new Vector2(0.38f, 0.58f));
            PlaceSwatch(disc, "Jaune", MontessoriPalette.Hex("#F2C14E"), new Vector2(0.62f, 0.58f));
            PlaceSwatch(disc, "Bleu", MontessoriPalette.Hex("#2F74D0"), new Vector2(0.50f, 0.36f));
        }

        static void PlaceSwatch(RectTransform disc, string name, Color color, Vector2 anchor)
        {
            var swatch = UiFactory.Picture(name, disc, UiFactory.CircleSprite(), color, false, false);
            AnchorIn(swatch.rectTransform, anchor, new Vector2(0.38f, 0.38f));
        }

        static void AnchorIn(RectTransform rect, Vector2 center, Vector2 size)
        {
            rect.anchorMin = center - size * 0.5f;
            rect.anchorMax = center + size * 0.5f;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.pivot = new Vector2(0.5f, 0.5f);
        }

        static Color BorderFor(string id)
        {
            if (id == "alphabet" || id == GameIds.Puzzle)
                return MontessoriPalette.Hex("#E23D3D");
            if (id == "chiffres" || id == GameIds.Tracing)
                return MontessoriPalette.Hex("#2F74D0");
            if (id == "formes")
                return MontessoriPalette.Hex("#2EAE5B");
            if (id == "couleurs")
                return MontessoriPalette.Hex("#E2B043");
            return MontessoriPalette.Hex("#7B4FD0");
        }

        static Color PastelFor(string id)
        {
            if (id == "alphabet" || id == GameIds.Puzzle)
                return MontessoriPalette.Hex("#FFD5DC");
            if (id == "chiffres" || id == GameIds.Tracing)
                return MontessoriPalette.Hex("#D4ECFF");
            if (id == "formes")
                return MontessoriPalette.Hex("#D9F6E3");
            if (id == "couleurs")
                return MontessoriPalette.Hex("#FFF4D2");
            return MontessoriPalette.Hex("#E6D8FF");
        }

        static TMP_Text BuildTitle(Transform parent, string value, float size)
        {
            var title = UiFactory.Tmp("Etiquette", parent, value, size, MontessoriPalette.Ink, true);
            title.enableAutoSizing = true;
            title.fontSizeMin = 20f;
            title.fontSizeMax = size;
            title.overflowMode = TextOverflowModes.Ellipsis;
            return title;
        }

        static TMP_Text BuildCaption(Transform parent, string name, string value, float size, Color color)
        {
            var caption = UiFactory.Tmp(name, parent, value, size, color, true);
            caption.enableAutoSizing = true;
            caption.fontSizeMin = 16f;
            caption.fontSizeMax = size;
            caption.overflowMode = TextOverflowModes.Ellipsis;
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
            var texts = _choices.GetComponentsInChildren<TMP_Text>(true);
            for (int i = 0; i < texts.Length; i++)
                ApplyFont(texts[i]);
        }

        static void ApplyFont(TMP_Text text)
        {
            if (text == null)
                return;
            var font = UiFactory.PlayFont();
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

            float medal = Mathf.Min(w * 0.52f, h * 0.40f);
            float medalY = h * 0.16f;
            var medalRect = FindRect("Disque");
            if (medalRect == null)
                medalRect = FindRect("Medaillon");
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
