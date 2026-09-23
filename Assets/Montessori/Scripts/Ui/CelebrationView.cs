using System;
using UnityEngine;
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
        }

        void OnDisable()
        {
            Hook(false);
            Motion.KillOwner(this);
        }

        public void Show(string title, string subtitle, string emblem, Color emblemColor, Action onContinue)
        {
            _onContinue = onContinue;
            if (_title != null)
                _title.text = title;
            if (_subtitle != null)
                _subtitle.text = subtitle ?? string.Empty;
            if (_emblem != null)
            {
                _emblem.text = emblem ?? string.Empty;
                _emblem.color = emblemColor;
                _emblem.gameObject.SetActive(!string.IsNullOrEmpty(emblem));
            }
            gameObject.SetActive(true);
            if (_group != null)
                _group.alpha = 0f;
            if (_plaque != null)
                _plaque.localScale = Vector3.one * 0.86f;
            if (!Application.isPlaying)
                return;
            if (_group != null)
                Motion.Fade(_group, 1f, 0.22f, Ease.OutQuad);
            if (_plaque != null)
                Motion.Scale(_plaque, Vector3.one, 0.42f, Ease.OutBack);
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
            var callback = _onContinue;
            _onContinue = null;
            gameObject.SetActive(false);
            callback?.Invoke();
        }
    }
}
