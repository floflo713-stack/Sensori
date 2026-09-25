using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class FlashcardPresenter : MonoBehaviour
    {
        [SerializeField] WordCardDeck _deck;
        [SerializeField] Text _title;
        [SerializeField] Text _subtitle;
        [SerializeField] RectTransform _themes;
        [SerializeField] RectTransform _reader;
        [SerializeField] RectTransform _card;
        [SerializeField] Image _illustration;
        [SerializeField] Text _word;
        [SerializeField] Text _aide;
        [SerializeField] Text _progress;

        readonly List<ThemeTile> _tiles = new List<ThemeTile>();
        ThemeAssets _theme;
        WordTheme _currentTheme;
        int _index;
        bool _reading;
        bool _sliding;
        bool _layoutDirty = true;
        int _speakToken;
        Vector2 _rest = new Vector2(0f, -24f);

        public WordCardDeck Deck => _deck;
        public bool ShowingReader => _reading;
        public int CardCount => _deck != null ? _deck.CardCount : 0;

        public void Construct(ThemeAssets theme, WordCardDeck deck)
        {
            _theme = theme;
            _deck = deck;
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);

            BuildHeader();
            _themes = UiFactory.Rect("Themes", transform);
            UiFactory.Stretch(_themes, 48f, 36f, 48f, 168f);
            _reader = UiFactory.Rect("Lecteur", transform);
            UiFactory.Stretch(_reader, 0f, 0f, 0f, 0f);
            BuildThemes();
            BuildReader();
            _reader.gameObject.SetActive(false);
            _themes.gameObject.SetActive(true);
            _reading = false;
        }

        public void Bind(WordCardDeck deck)
        {
            _deck = deck;
        }

        void OnEnable()
        {
            if (!Application.isPlaying)
                return;
            EnsureRefs();
            RecoverTiles();
            ArmClicks();
            _layoutDirty = true;
        }

        void LateUpdate()
        {
            if (!isActiveAndEnabled || _reading || !_layoutDirty)
                return;
            _layoutDirty = !LayoutTiles();
        }

        public void ShowThemes()
        {
            _reading = false;
            _sliding = false;
            if (_reader != null)
                _reader.gameObject.SetActive(false);
            if (_themes != null)
                _themes.gameObject.SetActive(true);
            if (_title != null)
                _title.text = "Imagier Parlant";
            if (_subtitle != null)
            {
                _subtitle.text = "Choisis un thème";
                _subtitle.color = MontessoriPalette.WalnutDeep;
            }
            ApplyFont(_title);
            ApplyFont(_subtitle);
            RecoverTiles();
            ArmClicks();
            RefreshTiles();
            _layoutDirty = true;
            LayoutTiles();
            ScreenBackdrop.Ensure(transform, "imagier");
        }

        public void OpenTheme(WordTheme theme)
        {
            if (_deck == null || !_deck.Has(theme))
            {
                Debug.Log("[Imagier Parlant] Thème encore vide : " + WordThemes.Title(theme));
                WoodenAudio.PlayTock();
                if (_subtitle != null)
                    _subtitle.text = "Bientôt dans l'atelier";
                return;
            }

            _currentTheme = theme;
            _index = 0;
            _reading = true;
            if (_themes != null)
                _themes.gameObject.SetActive(false);
            if (_reader != null)
                _reader.gameObject.SetActive(true);
            if (_title != null)
                _title.text = WordThemes.Title(theme);
            if (_subtitle != null)
                _subtitle.text = WordThemes.Subtitle(theme);
            ApplyFont(_title);
            ApplyFont(_subtitle);
            Present(1, true);
        }

        public void Back()
        {
            if (_reading)
                ShowThemes();
            else if (MontessoriApp.Instance != null)
                MontessoriApp.Instance.OpenCategories();
        }

        void BuildHeader()
        {
            var header = UiFactory.Rect("Entete", transform);
            UiFactory.AnchorTop(header, 148f, 24f, 24f);
            var back = UiFactory.RoundButton("Retour", header, _theme, "<", 52, NavigationTarget.Category);
            back.anchorMin = new Vector2(0f, 0.5f);
            back.anchorMax = new Vector2(0f, 0.5f);
            back.pivot = new Vector2(0f, 0.5f);
            back.sizeDelta = new Vector2(104f, 104f);
            back.anchoredPosition = new Vector2(8f, -8f);
            var backClick = SwapNavigation(back);
            if (backClick != null)
                backClick.Clicked += Back;

            _title = UiFactory.Label("Titre", header, "Imagier Parlant", 46, MontessoriPalette.WalnutDeep, TextAnchor.MiddleCenter);
            UiFactory.Stretch(_title.rectTransform, 140f, 58f, 140f, 6f);
            _title.fontStyle = FontStyle.Bold;
            _title.raycastTarget = false;
            _subtitle = UiFactory.Label("SousTitre", header, "Choisis un thème", 26, MontessoriPalette.WalnutDeep, TextAnchor.MiddleCenter);
            UiFactory.Stretch(_subtitle.rectTransform, 140f, 8f, 140f, 88f);
            _subtitle.raycastTarget = false;
            ApplyFont(_title);
            ApplyFont(_subtitle);
        }

        void BuildThemes()
        {
            var themes = WordThemes.All();
            for (int i = 0; i < themes.Length; i++)
                _tiles.Add(CreateTile(themes[i], i));
            LayoutTiles();
        }

        ThemeTile CreateTile(WordTheme theme, int index)
        {
            var root = UiFactory.Rect("Theme_" + theme, _themes);
            var group = UiFactory.AddGroup(root.gameObject);
            if (_theme != null && _theme.Shadow != null)
            {
                var shadow = UiFactory.Picture("Ombre", root, _theme.Shadow, MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.28f), false, false);
                UiFactory.Stretch(shadow.rectTransform, -10f, -22f, -10f, 6f);
                shadow.raycastTarget = false;
            }

            var face = UiFactory.Picture("Face", root, _theme != null ? _theme.Panel : null, Color.white, _theme != null && _theme.Panel != null, true);
            UiFactory.Stretch(face.rectTransform, 0f, 0f, 0f, 0f);
            face.preserveAspect = false;
            face.raycastTarget = true;

            Color accent = WordThemes.Accent(theme);
            var ribbon = UiFactory.Picture("Ruban", face.transform, _theme != null ? _theme.Pearl : null, accent, false, false);
            UiFactory.AnchorCenter(ribbon.rectTransform, new Vector2(0f, 36f), new Vector2(88f, 14f));
            ribbon.raycastTarget = false;

            var title = UiFactory.Label("Etiquette", face.transform, WordThemes.Title(theme), 32, MontessoriPalette.WalnutDeep, TextAnchor.MiddleCenter);
            UiFactory.AnchorCenter(title.rectTransform, new Vector2(0f, -18f), new Vector2(240f, 64f));
            title.fontStyle = FontStyle.Bold;
            title.raycastTarget = false;
            ApplyFont(title);

            var count = UiFactory.Label("Compte", face.transform, "", 22, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            UiFactory.AnchorCenter(count.rectTransform, new Vector2(0f, -62f), new Vector2(220f, 36f));
            count.raycastTarget = false;
            ApplyFont(count);

            face.gameObject.AddComponent<Pressable>();
            var tile = new ThemeTile
            {
                Theme = theme,
                Root = root,
                Group = group,
                Title = title,
                Count = count
            };
            return tile;
        }

        bool LayoutTiles()
        {
            if (_themes == null)
                return false;
            if (_tiles.Count == 0)
                RecoverTiles();
            if (_tiles.Count == 0)
                return false;

            float width = _themes.rect.width;
            float height = _themes.rect.height;
            if (width < 240f || height < 280f)
                return false;

            const int columns = 2;
            const int rows = 3;
            const float gapX = 36f;
            const float gapY = 22f;
            float cardH = (height - gapY * (rows - 1)) / rows;
            float cardW = cardH * 0.82f;
            float maxW = (width - gapX * (columns - 1)) / columns;
            if (cardW > maxW)
                cardW = maxW;
            if (cardW > 360f)
                cardW = 360f;
            if (cardH > cardW * 1.35f)
                cardH = cardW * 1.35f;

            float totalW = columns * cardW + (columns - 1) * gapX;
            float totalH = rows * cardH + (rows - 1) * gapY;
            float yTop = totalH * 0.5f - cardH * 0.5f;
            for (int i = 0; i < _tiles.Count; i++)
            {
                var tile = _tiles[i];
                if (tile.Root == null)
                    continue;
                int col = i % columns;
                int row = i / columns;
                int inRow = Mathf.Min(columns, _tiles.Count - row * columns);
                float rowW = inRow * cardW + (inRow - 1) * gapX;
                float x = -rowW * 0.5f + cardW * 0.5f + col * (cardW + gapX);
                float y = yTop - row * (cardH + gapY);
                UiFactory.AnchorCenter(tile.Root, new Vector2(x, y), new Vector2(cardW, cardH));
                PlaceTileFace(tile, cardW, cardH);
            }
            return true;
        }

        static void PlaceTileFace(ThemeTile tile, float width, float height)
        {
            float disc = Mathf.Clamp(Mathf.Min(width, height) * 0.22f, 48f, 84f);
            Place(Find(tile.Root, "Ruban"), new Vector2(0f, height * 0.24f), new Vector2(disc, disc));
            Place(tile.Title != null ? tile.Title.rectTransform : Find(tile.Root, "Etiquette"), new Vector2(0f, -height * 0.02f), new Vector2(width * 0.86f, 64f));
            Place(tile.Count != null ? tile.Count.rectTransform : Find(tile.Root, "Compte"), new Vector2(0f, -height * 0.22f), new Vector2(width * 0.8f, 40f));
        }

        void EnsureRefs()
        {
            if (_themes == null)
                _themes = transform.Find("Themes") as RectTransform;
            if (_reader == null)
                _reader = transform.Find("Lecteur") as RectTransform;
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
            if (_card == null && _reader != null)
                _card = _reader.Find("Carte") as RectTransform;
        }

        void RecoverTiles()
        {
            EnsureRefs();
            if (_themes == null)
                return;
            _tiles.Clear();
            for (int i = 0; i < _themes.childCount; i++)
            {
                var root = _themes.GetChild(i) as RectTransform;
                if (root == null || !root.name.StartsWith("Theme_"))
                    continue;
                var themeName = root.name.Substring("Theme_".Length);
                if (!Enum.TryParse(themeName, out WordTheme theme))
                    continue;
                var title = Find(root, "Etiquette");
                var count = Find(root, "Compte");
                _tiles.Add(new ThemeTile
                {
                    Theme = theme,
                    Root = root,
                    Group = root.GetComponent<CanvasGroup>(),
                    Title = title != null ? title.GetComponent<Text>() : null,
                    Count = count != null ? count.GetComponent<Text>() : null
                });
            }
        }

        void ArmClicks()
        {
            EnsureRefs();
            Arm(transform.Find("Entete/Retour/Face"), ImagierTap.Action.Back, default);
            for (int i = 0; i < _tiles.Count; i++)
            {
                var face = _tiles[i].Root != null ? _tiles[i].Root.Find("Face") : null;
                Arm(face, ImagierTap.Action.Theme, _tiles[i].Theme);
            }
            if (_reader == null)
                return;
            Arm(Find(_reader, "Precedent"), ImagierTap.Action.Previous, default);
            Arm(Find(_reader, "Suivant"), ImagierTap.Action.Next, default);
            Arm(Find(_reader, "HautParleur"), ImagierTap.Action.Speak, default);
            var cardFace = _card != null ? _card.Find("Face") : null;
            Arm(cardFace, ImagierTap.Action.Effect, default);
        }

        static void Arm(Transform face, ImagierTap.Action action, WordTheme theme)
        {
            if (face == null)
                return;
            var image = face.GetComponent<Image>();
            if (image == null)
                image = face.GetComponentInChildren<Image>(true);
            var host = image != null ? image.transform : face;
            if (image != null)
                image.raycastTarget = true;
            var simple = host.GetComponent<SimpleClick>();
            if (simple != null)
                simple.enabled = false;
            var navigation = host.GetComponent<NavigationButton>();
            if (navigation != null)
                navigation.enabled = false;
            var tap = host.GetComponent<ImagierTap>();
            if (tap == null)
                tap = host.gameObject.AddComponent<ImagierTap>();
            tap.Configure(action, theme);
        }

        static RectTransform Find(Transform root, string childName)
        {
            if (root == null)
                return null;
            var transforms = root.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i] != root && transforms[i].name == childName)
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

        void RefreshTiles()
        {
            for (int i = 0; i < _tiles.Count; i++)
            {
                var tile = _tiles[i];
                int count = _deck != null ? _deck.Count(tile.Theme) : 0;
                if (tile.Count != null)
                    tile.Count.text = count > 0 ? count + " cartes" : "Bientôt";
                if (tile.Group != null)
                    tile.Group.alpha = count > 0 ? 1f : 0.62f;
            }
        }

        void BuildReader()
        {
            var previous = UiFactory.RoundButton("Precedent", _reader, _theme, "<", 64, NavigationTarget.Category);
            previous.anchorMin = new Vector2(0f, 0.5f);
            previous.anchorMax = new Vector2(0f, 0.5f);
            previous.pivot = new Vector2(0.5f, 0.5f);
            previous.sizeDelta = new Vector2(148f, 148f);
            previous.anchoredPosition = new Vector2(120f, -20f);
            var previousClick = SwapNavigation(previous);
            if (previousClick != null)
                previousClick.Clicked += ShowPrevious;

            var next = UiFactory.RoundButton("Suivant", _reader, _theme, ">", 64, NavigationTarget.Category);
            next.anchorMin = new Vector2(1f, 0.5f);
            next.anchorMax = new Vector2(1f, 0.5f);
            next.pivot = new Vector2(0.5f, 0.5f);
            next.sizeDelta = new Vector2(148f, 148f);
            next.anchoredPosition = new Vector2(-120f, -20f);
            var nextClick = SwapNavigation(next);
            if (nextClick != null)
                nextClick.Clicked += ShowNext;

            var speaker = UiFactory.RoundButton("HautParleur", _reader, _theme, ")", 42, NavigationTarget.Category);
            speaker.anchorMin = new Vector2(0.5f, 1f);
            speaker.anchorMax = new Vector2(0.5f, 1f);
            speaker.pivot = new Vector2(0.5f, 0.5f);
            speaker.sizeDelta = new Vector2(92f, 92f);
            speaker.anchoredPosition = new Vector2(250f, -210f);
            var speakerClick = SwapNavigation(speaker);
            if (speakerClick != null)
                speakerClick.Clicked += ReplayVoice;
            var speakerGlyph = speaker.GetComponentInChildren<Text>();
            if (speakerGlyph != null)
            {
                speakerGlyph.text = "Son";
                speakerGlyph.fontSize = 28;
                speakerGlyph.raycastTarget = false;
            }

            _card = UiFactory.Rect("Carte", _reader);
            UiFactory.AnchorCenter(_card, _rest, new Vector2(620f, 760f));
            if (_theme != null && _theme.Shadow != null)
            {
                var shadow = UiFactory.Picture("Ombre", _card, _theme.Shadow, MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.32f), false, false);
                UiFactory.Stretch(shadow.rectTransform, -16f, -28f, -16f, 8f);
                shadow.raycastTarget = false;
            }

            var wood = UiFactory.Picture("Face", _card, _theme != null ? _theme.Panel : null, Color.white, _theme != null && _theme.Panel != null, true);
            UiFactory.Stretch(wood.rectTransform, 0f, 0f, 0f, 0f);
            wood.preserveAspect = false;
            wood.raycastTarget = true;
            var tap = wood.gameObject.AddComponent<SimpleClick>();
            tap.Clicked += OnCardTapped;

            var paper = UiFactory.Picture("Papier", wood.transform, _theme != null ? _theme.Piece : null, MontessoriPalette.Cream, _theme != null && _theme.Piece != null, false);
            UiFactory.Stretch(paper.rectTransform, 28f, 150f, 28f, 28f);
            paper.preserveAspect = false;
            paper.raycastTarget = false;

            _illustration = UiFactory.Picture("Illustration", paper.transform, null, Color.white, false, false);
            UiFactory.Stretch(_illustration.rectTransform, 36f, 36f, 36f, 36f);
            _illustration.preserveAspect = true;
            _illustration.raycastTarget = false;

            _word = UiFactory.Label("Mot", wood.transform, "", 42, MontessoriPalette.WalnutDeep, TextAnchor.MiddleCenter);
            UiFactory.AnchorBottom(_word.rectTransform, 108f, 24f, 24f);
            _word.fontStyle = FontStyle.Bold;
            _word.raycastTarget = false;
            var wordRect = _word.rectTransform;
            wordRect.offsetMax = new Vector2(-24f, 148f);
            wordRect.offsetMin = new Vector2(24f, 72f);

            _aide = UiFactory.Label("Aide", wood.transform, "", 22, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            _aide.rectTransform.anchorMin = new Vector2(0f, 0f);
            _aide.rectTransform.anchorMax = new Vector2(1f, 0f);
            _aide.rectTransform.pivot = new Vector2(0.5f, 0f);
            _aide.rectTransform.offsetMin = new Vector2(28f, 22f);
            _aide.rectTransform.offsetMax = new Vector2(-28f, 70f);
            _aide.raycastTarget = false;

            _progress = UiFactory.Label("Progression", _reader, "", 26, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            _progress.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            _progress.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            _progress.rectTransform.pivot = new Vector2(0.5f, 0f);
            _progress.rectTransform.sizeDelta = new Vector2(220f, 48f);
            _progress.rectTransform.anchoredPosition = new Vector2(0f, 28f);
            _progress.raycastTarget = false;
            ApplyFont(_word);
            ApplyFont(_aide);
            ApplyFont(_progress);
        }

        public void ShowPrevious()
        {
            Step(-1);
        }

        public void ShowNext()
        {
            Step(1);
        }

        void Step(int direction)
        {
            if (_sliding || _deck == null || !_deck.Has(_currentTheme))
                return;
            _index = direction < 0
                ? _deck.Previous(_currentTheme, _index)
                : _deck.Next(_currentTheme, _index);
            Present(direction, false);
        }

        void Present(int direction, bool first)
        {
            var card = _deck != null ? _deck.At(_currentTheme, _index) : null;
            if (card == null)
            {
                if (_word != null)
                    _word.text = string.Empty;
                return;
            }

            if (_illustration != null)
            {
                _illustration.sprite = card.Visuel;
                _illustration.enabled = _illustration.sprite != null;
                _illustration.raycastTarget = false;
            }
            if (_word != null)
                _word.text = card.MotAffiche;
            if (_aide != null)
            {
                _aide.text = card.Aide;
                _aide.gameObject.SetActive(card.HasAide);
            }
            if (_progress != null && _deck != null)
                _progress.text = (_index + 1) + " / " + _deck.Count(_currentTheme);
            ApplyFont(_word);
            ApplyFont(_aide);
            ApplyFont(_progress);

            if (_card == null)
            {
                Speak(card);
                return;
            }

            Motion.Kill(_card, "pos");
            Motion.Kill(_card, "scale");
            _card.localScale = Vector3.one;
            float fromX = direction < 0 ? -780f : 780f;
            if (first)
                fromX = 780f;
            _card.anchoredPosition = new Vector2(fromX, _rest.y);
            _sliding = true;
            Motion.Anchored(_card, _rest, 0.46f, Ease.OutCubic).OnComplete(() =>
            {
                _sliding = false;
                if (_card != null)
                    _card.anchoredPosition = _rest;
            });
            var spoken = card;
            int token = ++_speakToken;
            float voiceWait = spoken.VoixPrononciation != null && spoken.VoixPrononciation.length > 0.2f
                ? spoken.VoixPrononciation.length
                : 1.6f;
            Motion.Delayed(0.28f, () =>
            {
                if (token == _speakToken && isActiveAndEnabled && _reading)
                    Speak(spoken);
            });
            Motion.Delayed(0.4f + voiceWait, () =>
            {
                if (token != _speakToken || !isActiveAndEnabled || !_reading || WoodenAudio.Instance == null)
                    return;
                WoodenAudio.Instance.PlayEffect(spoken.EffetSonore, spoken.MotAffiche);
            });
        }

        public void OnCardTapped()
        {
            if (_sliding)
                return;
            var card = _deck != null ? _deck.At(_currentTheme, _index) : null;
            if (_card != null)
            {
                Motion.Kill(_card, "scale");
                _card.localScale = Vector3.one * 0.95f;
                Motion.Scale(_card, Vector3.one, 0.2f, Ease.OutBack);
            }
            if (WoodenAudio.Instance != null)
                WoodenAudio.Instance.PlayEffect(card != null ? card.EffetSonore : null, card != null ? card.MotAffiche : "carte");
            else
                Debug.Log("[Imagier Parlant] Audio indisponible pour l'effet sonore.");
        }

        public void ReplayVoice()
        {
            var card = _deck != null ? _deck.At(_currentTheme, _index) : null;
            Speak(card);
        }

        static void Speak(WordCardData card)
        {
            if (WoodenAudio.Instance == null)
            {
                Debug.Log("[Imagier Parlant] Audio indisponible pour « " + (card != null ? card.MotAffiche : "carte") + " ».");
                return;
            }
            WoodenAudio.Instance.SpeakWord(card != null ? card.VoixPrononciation : null, card != null ? card.MotAffiche : "carte");
        }

        static SimpleClick SwapNavigation(RectTransform button)
        {
            if (button == null)
                return null;
            var navigation = button.GetComponentInChildren<NavigationButton>(true);
            if (navigation != null)
            {
                if (Application.isPlaying)
                    Destroy(navigation);
                else
                    DestroyImmediate(navigation);
            }
            var image = button.GetComponentInChildren<Image>(true);
            if (image == null)
                return null;
            image.raycastTarget = true;
            var click = image.GetComponent<SimpleClick>();
            if (click == null)
                click = image.gameObject.AddComponent<SimpleClick>();
            return click;
        }

        static void ApplyFont(Text text)
        {
            if (text == null)
                return;
            var font = CategoryPresenter.ReadableFont();
            if (font != null)
                text.font = font;
            text.raycastTarget = false;
        }

        sealed class ThemeTile
        {
            public WordTheme Theme;
            public RectTransform Root;
            public CanvasGroup Group;
            public Text Title;
            public Text Count;
        }
    }

    public sealed class ImagierTap : MonoBehaviour, IPointerDownHandler
    {
        public enum Action
        {
            Back,
            Theme,
            Previous,
            Next,
            Speak,
            Effect
        }

        [SerializeField] Action _action;
        [SerializeField] WordTheme _theme;

        public void Configure(Action action, WordTheme theme)
        {
            _action = action;
            _theme = theme;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData != null && eventData.button != PointerEventData.InputButton.Left)
                return;
            var presenter = GetComponentInParent<FlashcardPresenter>();
            if (presenter == null)
                return;
            if (_action == Action.Back)
                presenter.Back();
            else if (_action == Action.Theme)
                presenter.OpenTheme(_theme);
            else if (_action == Action.Previous)
                presenter.ShowPrevious();
            else if (_action == Action.Next)
                presenter.ShowNext();
            else if (_action == Action.Speak)
                presenter.ReplayVoice();
            else
                presenter.OnCardTapped();
        }
    }
}
