using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class CelebrationView : MonoBehaviour
    {
        [SerializeField] CanvasGroup _group;
        [SerializeField] RectTransform _plaque;
        [SerializeField] Text _emblem;
        [SerializeField] Text _title;
        [SerializeField] Text _subtitle;
        [SerializeField] SimpleClick _continue;
        Action _onContinue;
        string _buttonLabel = "Encore";
        int _continuedFrame = -1;

        public void Construct(ThemeAssets theme)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);

            _group = UiFactory.AddGroup(gameObject);
            var dim = UiFactory.Picture("Voile", transform, theme.Solid != null ? theme.Solid : theme.Chip, MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.38f), false, true);
            UiFactory.Stretch(dim.rectTransform, 0f, 0f, 0f, 0f);
            dim.preserveAspect = false;

            _plaque = UiFactory.Rect("Plaque", transform);
            UiFactory.AnchorCenter(_plaque, Vector2.zero, new Vector2(720f, 460f));
            var shadow = UiFactory.Picture("Ombre", _plaque, theme.Shadow, MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.35f), false, false);
            UiFactory.Stretch(shadow.rectTransform, -24f, -36f, -24f, -8f);
            var face = UiFactory.Picture("Face", _plaque, theme.Panel, Color.white, true, false);
            UiFactory.Stretch(face.rectTransform, 0f, 0f, 0f, 0f);
            face.preserveAspect = false;

            _emblem = UiFactory.Label("Embleme", face.transform, "", 92, MontessoriPalette.VowelBlue, TextAnchor.MiddleCenter);
            UiFactory.Stretch(_emblem.rectTransform, 40f, 250f, 40f, 36f);
            _title = UiFactory.Label("Titre", face.transform, "Bravo !", 64, MontessoriPalette.Ink, TextAnchor.MiddleCenter);
            UiFactory.Stretch(_title.rectTransform, 36f, 150f, 36f, 150f);
            _subtitle = UiFactory.Label("SousTitre", face.transform, "", 28, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            UiFactory.Stretch(_subtitle.rectTransform, 48f, 110f, 48f, 250f);

            var button = UiFactory.Rect("Encore", face.transform);
            UiFactory.AnchorCenter(button, new Vector2(0f, -150f), new Vector2(320f, 112f));
            var buttonFace = UiFactory.Picture("Face", button, theme.Panel, Color.white, true, true);
            UiFactory.Stretch(buttonFace.rectTransform, 0f, 0f, 0f, 0f);
            buttonFace.preserveAspect = false;
            buttonFace.gameObject.AddComponent<Pressable>();
            _continue = buttonFace.gameObject.AddComponent<SimpleClick>();
            var buttonLabel = UiFactory.Label("Libelle", buttonFace.transform, "Encore", 36, MontessoriPalette.Ink, TextAnchor.MiddleCenter);
            UiFactory.Stretch(buttonLabel.rectTransform, 8f, 8f, 8f, 8f);
            Hook(true);
        }

        void OnEnable()
        {
            Hook(true);
            ArmContinue();
        }

        void OnDisable()
        {
            Hook(false);
            Motion.KillOwner(this);
        }

        public void Show(string title, string subtitle, string emblem, Color emblemColor, Action onContinue, string buttonLabel = null)
        {
            _onContinue = onContinue;
            _buttonLabel = string.IsNullOrEmpty(buttonLabel) ? "Encore" : buttonLabel;
            _continuedFrame = -1;
            if (_title != null)
            {
                _title.text = title;
                _title.fontStyle = FontStyle.Bold;
                _title.alignByGeometry = false;
                _title.font = CategoryPresenter.ReadableFont();
            }
            if (_subtitle != null)
            {
                _subtitle.text = subtitle ?? string.Empty;
                _subtitle.alignByGeometry = false;
                _subtitle.font = CategoryPresenter.ReadableFont();
            }
            bool customEmblem = !string.IsNullOrEmpty(emblem);
            if (_emblem != null)
            {
                _emblem.text = customEmblem ? emblem : string.Empty;
                _emblem.color = emblemColor;
                _emblem.font = CategoryPresenter.ReadableFont();
                _emblem.fontStyle = FontStyle.Bold;
                _emblem.alignByGeometry = false;
                _emblem.gameObject.SetActive(customEmblem);
                _emblem.rectTransform.localScale = customEmblem ? Vector3.zero : Vector3.one;
            }
            var seal = EnsureSeal(emblemColor);
            if (seal != null)
            {
                seal.gameObject.SetActive(!customEmblem);
                seal.localScale = Vector3.zero;
            }

            transform.SetAsLastSibling();
            DimVeil();
            gameObject.SetActive(true);
            ArmContinue();
            if (_group != null)
            {
                _group.alpha = 0f;
                _group.interactable = true;
                _group.blocksRaycasts = true;
            }
            if (_plaque != null)
            {
                _plaque.localScale = Vector3.one * 0.78f;
                _plaque.anchoredPosition = new Vector2(0f, -48f);
                _plaque.localRotation = Quaternion.Euler(0f, 0f, -2.5f);
            }
            var button = ContinueFace();
            if (button != null)
                button.localScale = Vector3.one * 0.82f;
            if (!Application.isPlaying)
            {
                if (_group != null)
                    _group.alpha = 1f;
                if (_plaque != null)
                {
                    _plaque.localScale = Vector3.one;
                    _plaque.anchoredPosition = Vector2.zero;
                    _plaque.localRotation = Quaternion.identity;
                }
                if (_emblem != null)
                    _emblem.rectTransform.localScale = Vector3.one;
                if (seal != null)
                    seal.localScale = Vector3.one;
                if (button != null)
                    button.localScale = Vector3.one;
                return;
            }

            if (_group != null)
                Motion.Fade(_group, 1f, 0.28f, Ease.OutQuad);
            if (_plaque != null)
            {
                Motion.Scale(_plaque, Vector3.one, 0.48f, Ease.OutCubic);
                Motion.Anchored(_plaque, Vector2.zero, 0.48f, Ease.OutCubic);
                var plaque = _plaque;
                Motion.Float(0f, 1f, 0.48f, u =>
                {
                    if (plaque != null)
                        plaque.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(-2.5f, 0f, u));
                }, Ease.OutCubic, plaque, "rot");
            }
            if (customEmblem && _emblem != null)
                Motion.Scale(_emblem.rectTransform, Vector3.one, 0.42f, Ease.OutCubic).SetDelay(0.12f);
            else if (seal != null)
                Motion.Scale(seal, Vector3.one, 0.42f, Ease.OutCubic).SetDelay(0.1f);
            if (button != null)
                Motion.Scale(button, Vector3.one, 0.36f, Ease.OutCubic).SetDelay(0.2f);
            Burst();
            WoodenAudio.PlaySuccess();
        }

        void Hook(bool enabled)
        {
            if (_continue == null)
                return;
            _continue.Clicked -= HandleContinue;
            if (enabled)
                _continue.Clicked += HandleContinue;
        }

        void HandleContinue()
        {
            if (_continuedFrame == Time.frameCount)
                return;
            _continuedFrame = Time.frameCount;
            var callback = _onContinue;
            _onContinue = null;
            gameObject.SetActive(false);
            callback?.Invoke();
        }

        void ArmContinue()
        {
            var face = ContinueFace();
            if (face == null)
                return;
            var image = face.GetComponent<Image>();
            if (image != null)
            {
                image.raycastTarget = true;
                image.color = new Color(1f, 0.96f, 0.9f, 1f);
            }
            var label = face.Find("Libelle");
            var text = label != null ? label.GetComponent<Text>() : null;
            if (text != null)
            {
                text.text = _buttonLabel;
                text.raycastTarget = false;
                text.font = CategoryPresenter.ReadableFont();
                text.fontStyle = FontStyle.Bold;
                text.alignByGeometry = false;
            }
            var button = UiFactory.CreateButton(face.gameObject);
            button.onClick.RemoveListener(HandleContinue);
            button.onClick.AddListener(HandleContinue);
            var plate = face.GetComponent<ContinuePlate>();
            if (plate == null)
                plate = face.gameObject.AddComponent<ContinuePlate>();
            plate.Bind(HandleContinue);
        }

        RectTransform ContinueFace()
        {
            var face = transform.Find("Plaque/Face/Encore/Face") as RectTransform;
            if (face != null)
                return face;
            var named = transform.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < named.Length; i++)
            {
                if (named[i].name == "Encore")
                    return named[i].Find("Face") as RectTransform;
            }
            return null;
        }

        void DimVeil()
        {
            var veil = transform.Find("Voile");
            var image = veil != null ? veil.GetComponent<Image>() : null;
            if (image == null)
                return;
            image.color = MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.62f);
            image.raycastTarget = true;
            image.type = Image.Type.Simple;
        }

        RectTransform EnsureSeal(Color color)
        {
            var face = transform.Find("Plaque/Face");
            if (face == null)
                return null;
            var existing = face.Find("Sceau") as RectTransform;
            if (existing != null)
            {
                var current = existing.GetComponent<Image>();
                if (current != null)
                    current.color = color;
                return existing;
            }
            var pearl = FindPearl();
            var image = UiFactory.Picture("Sceau", face, pearl, color, false, false);
            image.raycastTarget = false;
            image.preserveAspect = true;
            UiFactory.AnchorCenter(image.rectTransform, new Vector2(0f, 118f), new Vector2(108f, 108f));
            return image.rectTransform;
        }

        void Burst()
        {
            if (_plaque == null)
                return;
            for (int i = _plaque.childCount - 1; i >= 0; i--)
            {
                var child = _plaque.GetChild(i);
                if (child.name == "Etincelle")
                    Destroy(child.gameObject);
            }
            var pearl = FindPearl();
            if (pearl == null)
                return;
            Color[] tones =
            {
                MontessoriPalette.Sun,
                MontessoriPalette.Honey,
                MontessoriPalette.ConsonantRose,
                MontessoriPalette.Moss,
                MontessoriPalette.Sky
            };
            for (int i = 0; i < 10; i++)
            {
                var tone = tones[i % tones.Length];
                var dot = UiFactory.Picture("Etincelle", _plaque, pearl, tone, false, false);
                float angle = (i * 36f + 8f) * Mathf.Deg2Rad;
                float distance = 210f + (i % 3) * 28f;
                var target = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
                float size = 18f + (i % 4) * 6f;
                UiFactory.AnchorCenter(dot.rectTransform, target * 0.15f, new Vector2(size, size));
                dot.raycastTarget = false;
                var group = dot.gameObject.AddComponent<CanvasGroup>();
                group.blocksRaycasts = false;
                var rect = dot.rectTransform;
                Motion.Anchored(rect, target, 0.72f, Ease.OutCubic).SetDelay(0.04f);
                Motion.Fade(group, 0f, 0.55f, Ease.InQuad).SetDelay(0.22f).OnComplete(() =>
                {
                    if (rect != null)
                        Destroy(rect.gameObject);
                });
            }
        }

        Sprite FindPearl()
        {
            var images = transform.root.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < images.Length; i++)
            {
                var sprite = images[i] != null ? images[i].sprite : null;
                if (sprite != null && sprite.name.IndexOf("pearl", StringComparison.OrdinalIgnoreCase) >= 0)
                    return sprite;
            }
            return null;
        }
    }

    public sealed class ContinuePlate : MonoBehaviour, IPointerClickHandler
    {
        Action _handler;

        public void Bind(Action handler)
        {
            _handler = handler;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData == null || eventData.button != PointerEventData.InputButton.Left)
                return;
            _handler?.Invoke();
        }
    }
}
