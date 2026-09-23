using UnityEngine;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class HomePresenter : MonoBehaviour
    {
        [SerializeField] Button _button;

        public Button SensoriButton
        {
            get
            {
                if (_button != null)
                    return _button;
                var named = transform.Find("Enseigne/Sensori");
                if (named != null)
                    _button = named.GetComponent<Button>();
                return _button;
            }
        }

        public void Construct(ThemeAssets theme, ContentCatalog catalog)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);

            var sign = UiFactory.Rect("Enseigne", transform);
            Center(sign, new Vector2(0f, 36f), new Vector2(980f, 420f));

            var shadow = UiFactory.Picture("Ombre", sign, theme.Shadow, MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.22f), false, false);
            UiFactory.AnchorCenter(shadow.rectTransform, new Vector2(0f, -28f), new Vector2(900f, 250f));
            shadow.raycastTarget = false;

            var frame = UiFactory.Picture("Cadre", sign, theme.Panel, MontessoriPalette.WithAlpha(MontessoriPalette.Walnut, 0.45f), true, false);
            UiFactory.AnchorCenter(frame.rectTransform, new Vector2(0f, -6f), new Vector2(900f, 300f));
            frame.raycastTarget = false;
            frame.preserveAspect = false;

            var plate = UiFactory.Picture("Sensori", sign, theme.Panel, Color.white, true, true);
            UiFactory.AnchorCenter(plate.rectTransform, Vector2.zero, new Vector2(840f, 250f));
            plate.preserveAspect = false;
            plate.raycastTarget = true;
            _button = UiFactory.CreateButton(plate.gameObject);
            plate.gameObject.AddComponent<Pressable>();

            var title = UiFactory.Label("Titre", plate.transform, "Sensori", 84, MontessoriPalette.WalnutDeep, TextAnchor.MiddleCenter);
            UiFactory.Stretch(title.rectTransform, 24f, 70f, 24f, 28f);
            title.font = ReadableFont();
            title.raycastTarget = false;
            var titleShadow = title.gameObject.AddComponent<Shadow>();
            titleShadow.effectColor = MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.18f);
            titleShadow.effectDistance = new Vector2(0f, -3f);
            titleShadow.useGraphicAlpha = true;

            var subtitle = UiFactory.Label("SousTitre", plate.transform, "Atelier Montessori", 30, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            UiFactory.Stretch(subtitle.rectTransform, 24f, 28f, 24f, 150f);
            subtitle.font = ReadableFont();
            subtitle.raycastTarget = false;

            var footer = UiFactory.Label("Legende", transform, "Touche la plaque pour commencer", 28, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            UiFactory.AnchorBottom(footer.rectTransform, 88f, 40f, 40f);
            footer.font = ReadableFont();
            footer.raycastTarget = false;

            if (catalog == null)
                Debug.LogWarning("[Montessori] Catalogue absent pendant la construction de l'accueil.");
        }

        public Button EnsureButton()
        {
            if (SensoriButton != null)
            {
                var ready = SensoriButton.GetComponent<Image>();
                if (ready != null)
                    ready.raycastTarget = true;
                return SensoriButton;
            }

            Transform plate = transform.Find("Enseigne/Sensori");
            if (plate == null)
                plate = transform.Find("Enseigne/Plaque");
            if (plate == null)
            {
                var labels = GetComponentsInChildren<Text>(true);
                for (int i = 0; i < labels.Length; i++)
                {
                    if (labels[i] != null && labels[i].text == "Sensori")
                    {
                        plate = labels[i].transform.parent;
                        break;
                    }
                }
            }
            if (plate == null)
            {
                Debug.LogError("[Montessori] Plaque Sensori introuvable sous Accueil.");
                return null;
            }

            plate.name = "Sensori";
            var image = plate.GetComponent<Image>();
            if (image == null)
                image = plate.gameObject.AddComponent<Image>();
            image.raycastTarget = true;
            _button = UiFactory.CreateButton(plate.gameObject);
            if (plate.GetComponent<Pressable>() == null)
                plate.gameObject.AddComponent<Pressable>();

            var texts = plate.GetComponentsInChildren<Text>(true);
            var font = ReadableFont();
            for (int i = 0; i < texts.Length; i++)
            {
                texts[i].raycastTarget = false;
                if (font != null)
                    texts[i].font = font;
            }
            return _button;
        }

        public void PresentAsWelcome()
        {
            var sign = transform.Find("Enseigne") as RectTransform;
            if (sign != null)
                Center(sign, new Vector2(0f, 36f), new Vector2(980f, 420f));

            var plate = transform.Find("Enseigne/Sensori") as RectTransform;
            if (plate == null)
                plate = transform.Find("Enseigne/Plaque") as RectTransform;
            if (plate != null)
                UiFactory.AnchorCenter(plate, Vector2.zero, new Vector2(840f, 250f));

            var cards = transform.Find("ZoneCartes");
            if (cards != null)
                cards.gameObject.SetActive(false);

            var legend = transform.Find("Legende");
            var legendText = legend != null ? legend.GetComponent<Text>() : null;
            if (legendText != null)
            {
                legendText.text = "Touche la plaque pour commencer";
                legendText.raycastTarget = false;
                var font = ReadableFont();
                if (font != null)
                    legendText.font = font;
            }
        }

        public void Refresh()
        {
        }

        public void PlayIntro()
        {
            var sign = transform.Find("Enseigne");
            if (sign == null || !Application.isPlaying)
                return;
            sign.localScale = Vector3.one * 0.94f;
            Motion.Scale(sign, Vector3.one, 0.46f, Ease.OutBack);
        }

        public Sprite PlateSprite()
        {
            var button = SensoriButton;
            var image = button != null ? button.GetComponent<Image>() : null;
            return image != null ? image.sprite : null;
        }

        public Sprite ShadowSprite()
        {
            var shadow = transform.Find("Enseigne/Ombre");
            var image = shadow != null ? shadow.GetComponent<Image>() : null;
            return image != null ? image.sprite : null;
        }

        static void Center(RectTransform rect, Vector2 position, Vector2 size)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        public static Font ReadableFont()
        {
            return UiFont.Builtin;
        }
    }
}
