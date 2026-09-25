using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public static class UiFactory
    {
        public static RectTransform Rect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
            return rect;
        }

        public static void Stretch(RectTransform rect, float left, float bottom, float right, float top)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(-right, -top);
        }

        public static void AnchorCenter(RectTransform rect, Vector2 anchoredPosition, Vector2 size)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
        }

        public static void AnchorTop(RectTransform rect, float height, float left, float right)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.offsetMin = new Vector2(left, -height);
            rect.offsetMax = new Vector2(-right, 0f);
        }

        public static void AnchorBottom(RectTransform rect, float height, float left, float right)
        {
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.offsetMin = new Vector2(left, 0f);
            rect.offsetMax = new Vector2(-right, height);
        }

        static Sprite _rounded;
        static Sprite _circle;
        static TMP_FontAsset _playFont;

        public static Sprite RoundedSprite()
        {
            if (_rounded != null)
                return _rounded;
            const int size = 64;
            const float border = 22f;
            var raster = new Raster(size);
            raster.RoundRect(0.5f, 0.5f, 0.98f, 0.98f, 0.34f, Color.white);
            var texture = raster.ToTexture("soft-round");
            _rounded = Sprite.Create(
                texture,
                new Rect(0f, 0f, size, size),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect,
                new Vector4(border, border, border, border));
            _rounded.name = "soft-round";
            return _rounded;
        }

        public static Sprite CircleSprite()
        {
            if (_circle != null)
                return _circle;
            const int size = 128;
            var raster = new Raster(size);
            raster.Clear(new Color(1f, 1f, 1f, 0f));
            raster.Circle(0.5f, 0.5f, 0.48f, Color.white);
            var texture = raster.ToTexture("flat-circle");
            _circle = Sprite.Create(
                texture,
                new Rect(0f, 0f, size, size),
                new Vector2(0.5f, 0.5f),
                100f);
            _circle.name = "flat-circle";
            return _circle;
        }

        public static TMP_FontAsset PlayFont()
        {
            if (_playFont != null)
                return _playFont;
            var source = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (source == null)
                source = UiFont.Builtin;
            if (source == null)
                return null;
            _playFont = TMP_FontAsset.CreateFontAsset(source, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
            if (_playFont != null)
                _playFont.name = "play-latin";
            return _playFont;
        }

        public static TextMeshProUGUI Tmp(string name, Transform parent, string value, float size, Color color, bool bold)
        {
            var rect = Rect(name, parent);
            var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            var font = PlayFont();
            if (font != null)
                text.font = font;
            text.text = value;
            text.fontSize = size;
            text.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
            text.color = color;
            text.alignment = TextAlignmentOptions.Center;
            text.textWrappingMode = TextWrappingModes.Normal;
            text.overflowMode = TextOverflowModes.Overflow;
            text.richText = false;
            text.raycastTarget = false;
            return text;
        }

        public static Image Picture(string name, Transform parent, Sprite sprite, Color color, bool sliced, bool raycast)
        {
            var rect = Rect(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.type = sliced ? Image.Type.Sliced : Image.Type.Simple;
            image.raycastTarget = raycast;
            image.preserveAspect = !sliced;
            return image;
        }

        public static Text Label(string name, Transform parent, string value, int size, Color color, TextAnchor anchor)
        {
            var rect = Rect(name, parent);
            var text = rect.gameObject.AddComponent<Text>();
            text.font = UiFont.Builtin;
            text.text = value;
            text.fontSize = size;
            text.color = color;
            text.alignment = anchor;
            text.fontStyle = FontStyle.Normal;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            text.supportRichText = false;
            text.alignByGeometry = true;
            return text;
        }

        public static CanvasGroup AddGroup(GameObject target)
        {
            var group = target.GetComponent<CanvasGroup>();
            if (group == null)
                group = target.AddComponent<CanvasGroup>();
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
            return group;
        }

        public static RectTransform WoodStack(string name, Transform parent, Sprite shadow, Sprite face, Color faceColor, bool sliced, out Image faceImage)
        {
            var root = Rect(name, parent);
            var shadowImage = Picture("Ombre", root, shadow, MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.28f), false, false);
            Stretch(shadowImage.rectTransform, -18f, -30f, -18f, -8f);
            faceImage = Picture("Face", root, face, faceColor, sliced, true);
            Stretch(faceImage.rectTransform, 0f, 0f, 0f, 0f);
            faceImage.preserveAspect = false;
            return root;
        }

        public static Button CreateButton(GameObject target)
        {
            var image = target.GetComponent<Image>();
            if (image != null)
                image.raycastTarget = true;
            var button = target.GetComponent<Button>();
            if (button == null)
                button = target.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;
            var navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;
            return button;
        }

        public static RectTransform RoundButton(string name, Transform parent, ThemeAssets theme, string glyph, int glyphSize, NavigationTarget target)
        {
            var root = Rect(name, parent);
            var image = Picture("Face", root, theme.Pearl, MontessoriPalette.Maple, false, true);
            Stretch(image.rectTransform, 0f, 0f, 0f, 0f);
            image.preserveAspect = false;
            var label = Label("Glyph", image.transform, glyph, glyphSize, MontessoriPalette.Ink, TextAnchor.MiddleCenter);
            Stretch(label.rectTransform, 0f, 0f, 0f, 0f);
            image.gameObject.AddComponent<Pressable>();
            var navigation = image.gameObject.AddComponent<NavigationButton>();
            navigation.Configure(target);
            image.raycastTarget = true;
            return root;
        }

        public static void BuildGameHeader(Transform parent, ThemeAssets theme, out Text title, out Text progress)
        {
            var header = Rect("Entete", parent);
            AnchorTop(header, 118f, 20f, 20f);
            var back = RoundButton("Retour", header, theme, "<", 52, NavigationTarget.Category);
            back.anchorMin = new Vector2(0f, 0.5f);
            back.anchorMax = new Vector2(0f, 0.5f);
            back.pivot = new Vector2(0f, 0.5f);
            back.sizeDelta = new Vector2(86f, 86f);
            back.anchoredPosition = new Vector2(4f, 0f);

            title = Label("Titre", header, "", 40, MontessoriPalette.Ink, TextAnchor.MiddleCenter);
            Stretch(title.rectTransform, 120f, 28f, 160f, 8f);
            progress = Label("Progression", header, "", 28, MontessoriPalette.InkSoft, TextAnchor.MiddleRight);
            progress.rectTransform.anchorMin = new Vector2(1f, 0.5f);
            progress.rectTransform.anchorMax = new Vector2(1f, 0.5f);
            progress.rectTransform.pivot = new Vector2(1f, 0.5f);
            progress.rectTransform.sizeDelta = new Vector2(220f, 60f);
            progress.rectTransform.anchoredPosition = new Vector2(-12f, 0f);
        }
    }
}
