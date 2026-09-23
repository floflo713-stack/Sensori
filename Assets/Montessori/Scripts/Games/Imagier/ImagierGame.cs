using UnityEngine;
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
        }

        public override void Begin(GameRequest request)
        {
            _request = request;
            _index = 0;
            _showingBack = false;
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
            if (_previousClick != null)
                _previousClick.Clicked += ShowPrevious;
            if (_nextClick != null)
                _nextClick.Clicked += ShowNext;
            if (_flipClick != null)
                _flipClick.Clicked += ToggleFlip;
        }

        void OnDisable()
        {
            if (_previousClick != null)
                _previousClick.Clicked -= ShowPrevious;
            if (_nextClick != null)
                _nextClick.Clicked -= ShowNext;
            if (_flipClick != null)
                _flipClick.Clicked -= ToggleFlip;
            Motion.Kill(_card, "scale");
            Motion.KillOwner(this);
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
            var items = Items();
            if (items.Length == 0)
                return;
            _index = (_index + direction + items.Length) % items.Length;
            _showingBack = false;
            ShowCurrent(true);
        }

        void ToggleFlip()
        {
            if (_card == null)
                return;
            _showingBack = !_showingBack;
            Motion.Scale(_card, new Vector3(0.04f, 1f, 1f), 0.16f, Ease.InQuad).OnComplete(() =>
            {
                ApplySide();
                if (_card != null)
                    Motion.Scale(_card, Vector3.one, 0.22f, Ease.OutBack);
            });
            if (_showingBack)
                DiscoverCurrent();
        }

        void ShowCurrent(bool animate)
        {
            var items = Items();
            if (items.Length == 0)
                return;
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
            if (_backImage != null)
            {
                _backImage.sprite = PictogramPainter.Get(item.PictogramId);
                _backImage.color = Color.white;
            }
            if (_backWord != null)
                _backWord.text = item.AssociatedWord;
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
            WoodenAudio.PlayTap();
        }

        LearningItem[] Items()
        {
            if (_request == null || _request.Category == null)
                return new LearningItem[0];
            return _request.Category.Items;
        }
    }
}
