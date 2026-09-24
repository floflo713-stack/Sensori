using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class ImagierGame : MiniGameController
    {
        [SerializeField] Text _title;
        [SerializeField] Text _progress;
        [SerializeField] RectTransform _card;
        [SerializeField] CanvasGroup _front;
        [SerializeField] CanvasGroup _back;
        [SerializeField] Text _frontSymbol;
        [SerializeField] Text _frontCaption;
        [SerializeField] Image _frontIcon;
        [SerializeField] Image _backImage;
        [SerializeField] Text _backWord;
        [SerializeField] Text _backHint;
        [SerializeField] Sprite _pearl;
        [SerializeField] SimpleClick _previousClick;
        [SerializeField] SimpleClick _nextClick;
        [SerializeField] SimpleClick _flipClick;

        GameRequest _request;
        int _index;
        bool _showingBack;
        bool _turning;
        bool _awaitRelease;
        Coroutine _flip;
        RectTransform _vocabulary;
        Text _backLetter;
        Text _caption;
        Text[] _wordLines;
        string[] _words;
        string _openWord;
        int _actedFrame = -1;

        public override string GameId => GameIds.Imagier;

        public void Construct(ThemeAssets theme)
        {
            _pearl = theme.Pearl;
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);

            UiFactory.BuildGameHeader(transform, theme, out _title, out _progress);

            var previous = UiFactory.RoundButton("Precedent", transform, theme, "<", 48, NavigationTarget.Category);
            previous.anchorMin = new Vector2(0f, 0.5f);
            previous.anchorMax = new Vector2(0f, 0.5f);
            previous.pivot = new Vector2(0.5f, 0.5f);
            previous.sizeDelta = new Vector2(96f, 96f);
            previous.anchoredPosition = new Vector2(90f, -20f);
            _previousClick = SwapNavigation(previous);

            var next = UiFactory.RoundButton("Suivant", transform, theme, ">", 48, NavigationTarget.Category);
            next.anchorMin = new Vector2(1f, 0.5f);
            next.anchorMax = new Vector2(1f, 0.5f);
            next.pivot = new Vector2(0.5f, 0.5f);
            next.sizeDelta = new Vector2(96f, 96f);
            next.anchoredPosition = new Vector2(-90f, -20f);
            _nextClick = SwapNavigation(next);

            _card = UiFactory.Rect("Carte", transform);
            UiFactory.AnchorCenter(_card, new Vector2(0f, -10f), new Vector2(640f, 760f));
            var shadow = UiFactory.Picture("Ombre", _card, theme.Shadow, MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.3f), false, false);
            UiFactory.Stretch(shadow.rectTransform, -20f, -34f, -20f, -8f);
            var faceCard = UiFactory.Picture("Bois", _card, theme.Panel, Color.white, true, true);
            UiFactory.Stretch(faceCard.rectTransform, 0f, 0f, 0f, 0f);
            faceCard.preserveAspect = false;
            faceCard.gameObject.AddComponent<Pressable>();
            _flipClick = faceCard.gameObject.AddComponent<SimpleClick>();

            var frontRoot = UiFactory.Rect("Face", faceCard.transform);
            UiFactory.Stretch(frontRoot, 28f, 28f, 28f, 28f);
            _front = UiFactory.AddGroup(frontRoot.gameObject);
            _front.blocksRaycasts = false;
            _frontSymbol = UiFactory.Label("Symbole", frontRoot, "A", 220, MontessoriPalette.VowelBlue, TextAnchor.MiddleCenter);
            UiFactory.Stretch(_frontSymbol.rectTransform, 20f, 180f, 20f, 40f);
            _frontIcon = UiFactory.Picture("Icone", frontRoot, theme.Pearl, Color.white, false, false);
            UiFactory.AnchorCenter(_frontIcon.rectTransform, new Vector2(0f, 40f), new Vector2(340f, 340f));
            _frontIcon.preserveAspect = true;
            _frontCaption = UiFactory.Label("Legende", frontRoot, "Touche la carte", 32, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            UiFactory.AnchorBottom(_frontCaption.rectTransform, 90f, 20f, 20f);

            var backRoot = UiFactory.Rect("Dos", faceCard.transform);
            UiFactory.Stretch(backRoot, 28f, 28f, 28f, 28f);
            _back = UiFactory.AddGroup(backRoot.gameObject);
            _back.blocksRaycasts = false;
            _back.alpha = 0f;
            backRoot.gameObject.SetActive(false);
            _backImage = UiFactory.Picture("Image", backRoot, theme.Pearl, Color.white, false, false);
            UiFactory.AnchorCenter(_backImage.rectTransform, new Vector2(0f, 70f), new Vector2(420f, 420f));
            _backImage.preserveAspect = true;
            _backWord = UiFactory.Label("Mot", backRoot, "", 54, MontessoriPalette.Ink, TextAnchor.MiddleCenter);
            UiFactory.AnchorBottom(_backWord.rectTransform, 130f, 16f, 16f);
            _backHint = UiFactory.Label("Voix", backRoot, "Dis le mot doucement", 24, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            UiFactory.AnchorBottom(_backHint.rectTransform, 64f, 16f, 16f);
            _vocabulary = null;
            EnsureVocabulary();
        }

        public override void Begin(GameRequest request)
        {
            ScreenBackdrop.Ensure(transform, "imagier");
            _request = request;
            _index = 0;
            _showingBack = false;
            _turning = false;
            _awaitRelease = true;
            ArmClicks();
            if (_title != null)
            {
                string name = request != null && request.Definition != null ? request.Definition.Title : "Imagier";
                string category = request != null && request.Category != null ? request.Category.Title : string.Empty;
                _title.text = string.IsNullOrEmpty(category) ? name : name + " · " + category;
            }
            ShowCurrent(false);
        }

        void OnEnable()
        {
            ArmClicks();
        }

        void OnDisable()
        {
            if (_previousClick != null)
                _previousClick.Clicked -= ShowPrevious;
            if (_nextClick != null)
                _nextClick.Clicked -= ShowNext;
            if (_flipClick != null)
                _flipClick.Clicked -= ToggleFlip;
            StopFlip();
            Motion.Kill(_card, "scale");
            Motion.KillOwner(this);
        }

        void Update()
        {
            if (_card == null)
                return;
            var pointer = Pointer.current;
            if (pointer == null)
                return;
            if (_awaitRelease)
            {
                if (!pointer.press.isPressed)
                    _awaitRelease = false;
                return;
            }
            if (!pointer.press.wasPressedThisFrame)
                return;
            Vector2 screen = pointer.position.ReadValue();
            var canvas = _card.GetComponentInParent<Canvas>();
            Camera cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
            var previous = SideButton("Precedent", _previousClick);
            var next = SideButton("Suivant", _nextClick);
            if (Contains(previous, screen, cam) || Near(previous, screen, cam, 88f))
            {
                ShowPrevious();
                return;
            }
            if (Contains(next, screen, cam) || Near(next, screen, cam, 88f))
            {
                ShowNext();
                return;
            }
            if (!Contains(_card, screen, cam))
                return;
            if (_showingBack && !string.IsNullOrEmpty(_openWord))
            {
                ClosePortrait();
                return;
            }
            if (_showingBack && WordUnder(screen, cam, out string word))
            {
                OpenPortrait(word);
                return;
            }
            RequestFlip();
        }

        void ArmClicks()
        {
            if (_flipClick == null && _card != null)
            {
                var wood = _card.Find("Bois");
                if (wood != null)
                    _flipClick = wood.GetComponent<SimpleClick>();
            }
            if (_previousClick != null)
            {
                _previousClick.Clicked -= ShowPrevious;
                _previousClick.Clicked += ShowPrevious;
            }
            if (_nextClick != null)
            {
                _nextClick.Clicked -= ShowNext;
                _nextClick.Clicked += ShowNext;
            }
            if (_flipClick != null)
            {
                _flipClick.Clicked -= ToggleFlip;
                _flipClick.enabled = false;
            }
            if (_previousClick != null && _previousClick.transform.parent != null)
                _previousClick.transform.parent.SetAsLastSibling();
            if (_nextClick != null && _nextClick.transform.parent != null)
                _nextClick.transform.parent.SetAsLastSibling();
            PrepareCardHit();
        }

        void PrepareCardHit()
        {
            if (_card == null)
                return;
            var graphics = _card.GetComponentsInChildren<Graphic>(true);
            for (int i = 0; i < graphics.Length; i++)
            {
                if (graphics[i] != null && graphics[i].gameObject != (_flipClick != null ? _flipClick.gameObject : null))
                    graphics[i].raycastTarget = false;
            }
            var wood = _card.Find("Bois");
            if (wood == null)
                return;
            var face = wood.GetComponent<Image>();
            if (face != null)
                face.raycastTarget = true;
            var press = wood.GetComponent<Pressable>();
            if (press != null && Application.isPlaying)
                Destroy(press);
        }

        static SimpleClick SwapNavigation(RectTransform button)
        {
            var navigation = button.GetComponentInChildren<NavigationButton>();
            var image = button.GetComponentInChildren<Image>();
            if (navigation != null)
                DestroyImmediate(navigation);
            if (image == null)
                return null;
            var click = image.GetComponent<SimpleClick>();
            if (click == null)
                click = image.gameObject.AddComponent<SimpleClick>();
            return click;
        }

        void ShowPrevious()
        {
            Step(-1);
        }

        void ShowNext()
        {
            Step(1);
        }

        void Step(int direction)
        {
            if (_actedFrame == Time.frameCount)
                return;
            var items = Items();
            if (items.Length == 0)
                return;
            _actedFrame = Time.frameCount;
            _index = (_index + direction + items.Length) % items.Length;
            _showingBack = false;
            _openWord = null;
            ShowCurrent(true);
            WoodenAudio.PlayTap();
        }

        void ToggleFlip()
        {
            RequestFlip();
        }

        void RequestFlip()
        {
            if (_actedFrame == Time.frameCount || _turning || _card == null || !isActiveAndEnabled)
                return;
            _actedFrame = Time.frameCount;
            _turning = true;
            StopFlip();
            _turning = true;
            _flip = StartCoroutine(Flip());
        }

        void StopFlip()
        {
            if (_flip != null)
            {
                StopCoroutine(_flip);
                _flip = null;
            }
            _turning = false;
            if (_card != null)
                _card.localScale = Vector3.one;
        }

        IEnumerator Flip()
        {
            _showingBack = !_showingBack;
            bool reveal = _showingBack;
            WoodenAudio.PlayTap();
            yield return Squeeze(Vector3.one, new Vector3(0.02f, 1f, 1f), 0.18f, true);
            ApplySide();
            if (reveal)
                DiscoverCurrent();
            yield return Squeeze(new Vector3(0.02f, 1f, 1f), Vector3.one, 0.24f, false);
            if (_card != null)
                _card.localScale = Vector3.one;
            _turning = false;
            _flip = null;
        }

        IEnumerator Squeeze(Vector3 from, Vector3 to, float duration, bool easeIn)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float u = duration <= 0.0001f ? 1f : Mathf.Clamp01(elapsed / duration);
                u = easeIn ? u * u : 1f - (1f - u) * (1f - u);
                if (_card != null)
                    _card.localScale = Vector3.LerpUnclamped(from, to, u);
                yield return null;
            }
        }

        void ShowCurrent(bool animate)
        {
            var items = Items();
            if (items.Length == 0)
                return;
            StopFlip();
            Motion.Kill(_card, "scale");
            _index = Mathf.Clamp(_index, 0, items.Length - 1);
            var item = items[_index];
            if (_progress != null)
                _progress.text = (_index + 1) + " / " + items.Length;
            PaintFront(item);
            PaintBack(item);
            _showingBack = false;
            ApplySide();
            if (!animate || _card == null || !Application.isPlaying)
                return;
            _card.localScale = Vector3.one * 0.94f;
            Motion.Scale(_card, Vector3.one, 0.28f, Ease.OutBack);
        }

        void PaintFront(LearningItem item)
        {
            if (item == null)
                return;
            bool glyph = item.Visual == ItemVisual.Glyph;
            if (_frontSymbol != null)
            {
                _frontSymbol.gameObject.SetActive(glyph);
                _frontSymbol.text = item.Symbol;
                _frontSymbol.color = item.SymbolColor;
            }
            if (_frontIcon != null)
            {
                _frontIcon.gameObject.SetActive(!glyph);
                if (!glyph && item.Visual == ItemVisual.Swatch)
                {
                    _frontIcon.sprite = _pearl;
                    _frontIcon.color = item.Accent;
                }
                else if (!glyph)
                {
                    var silhouette = PictogramPainter.GetSilhouette(item.PictogramId);
                    if (silhouette != null)
                    {
                        _frontIcon.sprite = silhouette;
                        _frontIcon.color = MontessoriPalette.Walnut;
                    }
                    else
                    {
                        _frontIcon.sprite = PictogramPainter.Get(item.PictogramId);
                        _frontIcon.color = Color.white;
                    }
                }
            }
            if (_frontCaption != null)
            {
                if (glyph && item.Kind == ItemKind.Digit)
                    _frontCaption.text = item.DisplayName;
                else if (glyph)
                    _frontCaption.text = "Touche la carte";
                else
                    _frontCaption.text = item.DisplayName;
            }
        }

        void PaintBack(LearningItem item)
        {
            if (item == null)
                return;
            _openWord = null;
            EnsureVocabulary();
            string[] words = item.Kind == ItemKind.Letter ? LetterVocabulary.For(item.Symbol) : null;
            if ((words == null || words.Length == 0) && item.Kind == ItemKind.Letter && !string.IsNullOrEmpty(item.AssociatedWord))
                words = new[] { item.AssociatedWord.ToLowerInvariant() };
            _words = words;
            bool list = words != null && words.Length > 0;
            if (_vocabulary != null)
                _vocabulary.gameObject.SetActive(list);
            HidePortraitChrome();
            if (!list)
            {
                if (_backImage != null)
                {
                    _backImage.gameObject.SetActive(true);
                    _backImage.sprite = PictogramPainter.Get(item.PictogramId);
                    _backImage.color = Color.white;
                    UiFactory.AnchorCenter(_backImage.rectTransform, new Vector2(0f, 70f), new Vector2(420f, 420f));
                }
                if (_backWord != null)
                {
                    _backWord.gameObject.SetActive(true);
                    _backWord.supportRichText = false;
                    _backWord.color = MontessoriPalette.Ink;
                    _backWord.fontSize = 54;
                    _backWord.text = item.AssociatedWord;
                }
                if (_backHint != null)
                {
                    _backHint.gameObject.SetActive(true);
                    _backHint.text = "Dis le mot doucement";
                }
                return;
            }
            if (_backLetter != null)
            {
                _backLetter.gameObject.SetActive(true);
                _backLetter.text = item.Symbol;
                _backLetter.color = item.SymbolColor;
                UiFactory.AnchorCenter(_backLetter.rectTransform, new Vector2(0f, 268f), new Vector2(240f, 110f));
            }
            var rule = _vocabulary != null ? _vocabulary.Find("Filet") as RectTransform : null;
            if (rule != null)
                UiFactory.AnchorCenter(rule, new Vector2(0f, 198f), new Vector2(64f, 6f));
            if (_caption != null)
            {
                _caption.gameObject.SetActive(true);
                _caption.text = "Touche un mot";
                UiFactory.AnchorCenter(_caption.rectTransform, new Vector2(0f, -286f), new Vector2(460f, 44f));
            }
            PlaceWords(words);
        }

        void EnsureVocabulary()
        {
            if (_back == null)
                return;
            if (_vocabulary == null)
                _vocabulary = _back.transform.Find("Vocabulaire") as RectTransform;
            if (_vocabulary == null)
            {
                BuildVocabulary();
                return;
            }
            if (_backLetter == null)
            {
                var letter = _vocabulary.Find("Lettre");
                if (letter != null)
                    _backLetter = letter.GetComponent<Text>();
            }
            if (_caption == null)
            {
                var caption = _vocabulary.Find("Retour");
                if (caption != null)
                    _caption = caption.GetComponent<Text>();
            }
            if (_wordLines == null || _wordLines.Length != 4 || _wordLines[0] == null)
            {
                _wordLines = new Text[4];
                for (int i = 0; i < _wordLines.Length; i++)
                {
                    var line = _vocabulary.Find("Mot" + i);
                    _wordLines[i] = line != null ? line.GetComponent<Text>() : null;
                }
            }
        }

        void BuildVocabulary()
        {
            _vocabulary = UiFactory.Rect("Vocabulaire", _back.transform);
            UiFactory.Stretch(_vocabulary, 18f, 12f, 18f, 12f);
            _backLetter = UiFactory.Label("Lettre", _vocabulary, "", 84, MontessoriPalette.Ink, TextAnchor.MiddleCenter);
            UiFactory.AnchorCenter(_backLetter.rectTransform, new Vector2(0f, 268f), new Vector2(240f, 110f));
            var rule = UiFactory.Picture("Filet", _vocabulary, _pearl, MontessoriPalette.WithAlpha(MontessoriPalette.Walnut, 0.4f), false, false);
            rule.preserveAspect = false;
            UiFactory.AnchorCenter(rule.rectTransform, new Vector2(0f, 198f), new Vector2(64f, 6f));
            _wordLines = new Text[4];
            for (int i = 0; i < _wordLines.Length; i++)
            {
                _wordLines[i] = UiFactory.Label("Mot" + i, _vocabulary, "", 50, Color.white, TextAnchor.MiddleCenter);
                _wordLines[i].supportRichText = true;
                UiFactory.AnchorCenter(_wordLines[i].rectTransform, new Vector2(0f, 108f - i * 96f), new Vector2(520f, 88f));
            }
            _caption = UiFactory.Label("Retour", _vocabulary, "Touche pour revenir", 24, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            UiFactory.AnchorCenter(_caption.rectTransform, new Vector2(0f, -286f), new Vector2(460f, 44f));
        }

        void PlaceWords(string[] words)
        {
            if (_wordLines == null || words == null)
                return;
            int count = Mathf.Min(words.Length, _wordLines.Length);
            const float spacing = 96f;
            const float center = -36f;
            float start = center + (count - 1) * spacing * 0.5f;
            for (int i = 0; i < _wordLines.Length; i++)
            {
                if (_wordLines[i] == null)
                    continue;
                bool visible = i < count;
                _wordLines[i].gameObject.SetActive(visible);
                if (!visible)
                    continue;
                _wordLines[i].fontSize = 50;
                _wordLines[i].supportRichText = true;
                _wordLines[i].color = Color.white;
                _wordLines[i].text = ColorLetters(words[i]);
                UiFactory.AnchorCenter(_wordLines[i].rectTransform, new Vector2(0f, start - i * spacing), new Vector2(520f, 88f));
            }
        }

        void OpenPortrait(string word)
        {
            if (string.IsNullOrEmpty(word) || _turning)
                return;
            _actedFrame = Time.frameCount;
            _openWord = word;
            if (_wordLines != null)
            {
                for (int i = 0; i < _wordLines.Length; i++)
                {
                    if (_wordLines[i] != null)
                        _wordLines[i].gameObject.SetActive(false);
                }
            }
            if (_caption != null)
                _caption.gameObject.SetActive(false);
            if (_backImage != null)
            {
                _backImage.gameObject.SetActive(true);
                _backImage.sprite = PictogramPainter.Get(word);
                _backImage.color = Color.white;
                _backImage.preserveAspect = true;
                UiFactory.AnchorCenter(_backImage.rectTransform, new Vector2(0f, 8f), new Vector2(430f, 430f));
            }
            if (_backWord != null)
            {
                _backWord.gameObject.SetActive(true);
                _backWord.supportRichText = true;
                _backWord.color = Color.white;
                _backWord.fontSize = 48;
                _backWord.text = ColorLetters(word);
                UiFactory.AnchorCenter(_backWord.rectTransform, new Vector2(0f, -236f), new Vector2(520f, 72f));
            }
            if (_backHint != null)
            {
                _backHint.gameObject.SetActive(true);
                _backHint.text = "Touche pour les mots";
                UiFactory.AnchorCenter(_backHint.rectTransform, new Vector2(0f, -292f), new Vector2(460f, 40f));
            }
            WoodenAudio.PlayTap();
        }

        void ClosePortrait()
        {
            _openWord = null;
            HidePortraitChrome();
            if (_caption != null)
            {
                _caption.gameObject.SetActive(true);
                _caption.text = "Touche un mot";
            }
            PlaceWords(_words);
        }

        void HidePortraitChrome()
        {
            if (_backImage != null)
                _backImage.gameObject.SetActive(false);
            if (_backWord != null)
                _backWord.gameObject.SetActive(false);
            if (_backHint != null)
                _backHint.gameObject.SetActive(false);
        }

        static bool Contains(RectTransform rect, Vector2 screen, Camera cam)
        {
            return rect != null && RectTransformUtility.RectangleContainsScreenPoint(rect, screen, cam);
        }

        RectTransform SideButton(string name, SimpleClick click)
        {
            var found = transform.Find(name) as RectTransform;
            if (found != null)
                return found;
            return NavRect(click);
        }

        static RectTransform NavRect(SimpleClick click)
        {
            if (click == null)
                return null;
            var own = click.transform as RectTransform;
            var parent = click.transform.parent as RectTransform;
            if (parent != null && parent.rect.width > 8f && parent.rect.height > 8f)
                return parent;
            return own;
        }

        static bool Near(RectTransform rect, Vector2 screen, Camera cam, float radius)
        {
            if (rect == null)
                return false;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screen, cam, out var local))
                return false;
            return (local - rect.rect.center).sqrMagnitude <= radius * radius;
        }

        bool WordUnder(Vector2 screen, Camera cam, out string word)
        {
            word = null;
            if (_wordLines == null || _words == null)
                return false;
            int count = Mathf.Min(_words.Length, _wordLines.Length);
            for (int i = 0; i < count; i++)
            {
                if (_wordLines[i] == null || !_wordLines[i].gameObject.activeInHierarchy)
                    continue;
                if (!Contains(_wordLines[i].rectTransform, screen, cam))
                    continue;
                word = _words[i];
                return true;
            }
            return false;
        }

        static string ColorLetters(string word)
        {
            if (string.IsNullOrEmpty(word))
                return string.Empty;
            const string blue = "#1F6FBF";
            const string rose = "#E24B6A";
            const string ink = "#3C2E24";
            string result = string.Empty;
            int i = 0;
            while (i < word.Length)
            {
                char current = word[i];
                bool letter = char.IsLetter(current);
                bool vowel = letter && IsVowel(current);
                string color = !letter ? ink : vowel ? blue : rose;
                int j = i + 1;
                while (j < word.Length)
                {
                    char next = word[j];
                    bool nextLetter = char.IsLetter(next);
                    if (nextLetter != letter)
                        break;
                    if (letter && IsVowel(next) != vowel)
                        break;
                    j++;
                }
                result += "<color=" + color + ">" + word.Substring(i, j - i) + "</color>";
                i = j;
            }
            return result;
        }

        static bool IsVowel(char c)
        {
            switch (char.ToLowerInvariant(c))
            {
                case 'a':
                case 'à':
                case 'â':
                case 'ä':
                case 'e':
                case 'é':
                case 'è':
                case 'ê':
                case 'ë':
                case 'i':
                case 'î':
                case 'ï':
                case 'o':
                case 'ô':
                case 'ö':
                case 'u':
                case 'ù':
                case 'û':
                case 'ü':
                case 'y':
                case 'ÿ':
                case 'œ':
                case 'æ':
                    return true;
                default:
                    return false;
            }
        }

        void ApplySide()
        {
            if (_front != null)
            {
                _front.gameObject.SetActive(!_showingBack);
                _front.alpha = _showingBack ? 0f : 1f;
            }
            if (_back != null)
            {
                _back.gameObject.SetActive(_showingBack);
                _back.alpha = _showingBack ? 1f : 0f;
            }
        }

        void DiscoverCurrent()
        {
            var items = Items();
            if (_index < 0 || _index >= items.Length)
                return;
            var item = items[_index];
            var category = _request != null ? _request.Category : null;
            if (item == null || category == null)
                return;
            bool known = LearningProgress.IsDone(category.CategoryId, GameId, item.ItemId);
            LearningProgress.Mark(category.CategoryId, GameId, item.ItemId);
            if (!known && MontessoriApp.Instance != null && _card != null)
                MontessoriApp.Instance.Sparkle(_card.position, item.SymbolColor);
        }

        LearningItem[] Items()
        {
            if (_request == null || _request.Category == null)
                return new LearningItem[0];
            return _request.Category.Items;
        }
    }

    static class LetterVocabulary
    {
        static readonly string[] None = new string[0];

        public static string[] For(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
                return None;
            switch (char.ToUpperInvariant(symbol[0]))
            {
                case 'A': return A;
                case 'B': return B;
                case 'C': return C;
                case 'D': return D;
                case 'E': return E;
                case 'F': return F;
                case 'G': return G;
                case 'H': return H;
                case 'I': return I;
                case 'J': return J;
                case 'K': return K;
                case 'L': return L;
                case 'M': return M;
                case 'N': return N;
                case 'O': return O;
                case 'P': return P;
                case 'Q': return Q;
                case 'R': return R;
                case 'S': return S;
                case 'T': return T;
                case 'U': return U;
                case 'V': return V;
                case 'W': return W;
                case 'X': return X;
                case 'Y': return Y;
                case 'Z': return Z;
                default: return None;
            }
        }

        static readonly string[] A = { "abricot", "ami", "arbre", "avion" };
        static readonly string[] B = { "ballon", "banane", "bateau", "bébé" };
        static readonly string[] C = { "cadeau", "chat", "ciel", "cœur" };
        static readonly string[] D = { "danse", "dauphin", "doudou", "dragon" };
        static readonly string[] E = { "eau", "école", "éléphant", "étoile" };
        static readonly string[] F = { "famille", "fleur", "forêt", "fraise" };
        static readonly string[] G = { "gâteau", "girafe", "glace", "grenouille" };
        static readonly string[] H = { "hamac", "hélicoptère", "hérisson", "hibou" };
        static readonly string[] I = { "igloo", "île", "insecte", "iris" };
        static readonly string[] J = { "jardin", "jouet", "jupe", "jus" };
        static readonly string[] K = { "kangourou", "kayak", "kiwi", "koala" };
        static readonly string[] L = { "lait", "lapin", "livre", "lune" };
        static readonly string[] M = { "maison", "maman", "miel", "mouton" };
        static readonly string[] N = { "neige", "nid", "nounours", "nuage" };
        static readonly string[] O = { "oiseau", "orange", "oreille", "ours" };
        static readonly string[] P = { "pain", "papa", "poisson", "pomme" };
        static readonly string[] Q = { "quatre", "queue", "quille", "quinze" };
        static readonly string[] R = { "raisin", "rivière", "robe", "robot" };
        static readonly string[] S = { "sable", "sapin", "sirène", "soleil" };
        static readonly string[] T = { "table", "tigre", "tomate", "train" };
        static readonly string[] U = { "ukulélé", "uniforme", "univers", "usine" };
        static readonly string[] V = { "vache", "vélo", "vent", "voiture" };
        static readonly string[] W = { "wagon", "wallaby", "wombat" };
        static readonly string[] X = { "xylophone", "xérus" };
        static readonly string[] Y = { "yacht", "yaourt", "yoga", "yoyo" };
        static readonly string[] Z = { "zèbre", "zéro", "zigzag", "zoo" };
    }
}
