using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class HomePresenter : MonoBehaviour
    {
        public static readonly Color WarmShadow = new Color(0.52f, 0.30f, 0.12f, 0.32f);
        public static readonly Color PlateCream = new Color(1f, 0.9764706f, 0.9019608f, 1f);
        public static readonly Color InkCoral = new Color(1f, 0.29411766f, 0.29411766f, 1f);
        public static readonly Color InkAzure = new Color(0.1764706f, 0.6117647f, 0.85882354f, 1f);
        public static readonly Color InkMustard = new Color(0.9490196f, 0.7882353f, 0.29803923f, 1f);
        public static readonly Color InkApple = new Color(0.15294118f, 0.68235296f, 0.3764706f, 1f);
        public static readonly Color SoftDrop = new Color(0f, 0f, 0f, 0.28f);

        const float PlateWidth = 880f;
        const float PlateHeight = 360f;
        const float LetterLift = 52f;

        [SerializeField] Button _button;
        Sprite _keptPanel;
        Sprite _keptShadow;
        static Sprite _softRounded;

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

            if (theme != null)
            {
                if (_keptPanel == null)
                    _keptPanel = theme.Panel;
                if (_keptShadow == null)
                    _keptShadow = theme.Shadow;
            }

            var plate = UiFactory.Picture("Sensori", sign, SoftRounded(), PlateCream, true, true);
            UiFactory.AnchorCenter(plate.rectTransform, Vector2.zero, new Vector2(PlateWidth, PlateHeight));
            plate.preserveAspect = false;
            plate.raycastTarget = true;
            ApplyDropShadow(plate, -8f);
            _button = UiFactory.CreateButton(plate.gameObject);
            plate.gameObject.AddComponent<HomePlateMotion>();

            var title = UiFactory.Label("Titre", plate.transform, "Sensori", 84, InkCoral, TextAnchor.MiddleCenter);
            UiFactory.Stretch(title.rectTransform, 28f, 28f, 28f, 28f);
            title.font = ReadableFont();
            title.raycastTarget = false;
            var titleShadow = title.gameObject.AddComponent<Shadow>();
            titleShadow.effectColor = SoftDrop;
            titleShadow.effectDistance = new Vector2(0f, -3f);
            titleShadow.useGraphicAlpha = true;
            EnsureTitleLetters();
            EnsureJouer(plate.transform);

            BuildTokens();
            SealWelcome();
            EnsureMark();
            EnsureLandscape();

            if (catalog == null)
                Debug.LogWarning("[Montessori] Catalogue absent pendant la construction de l'accueil.");
        }

        public void SealWelcome()
        {
            var tokens = transform.Find("Jetons");
            if (tokens == null)
                return;
            tokens.SetAsFirstSibling();
            var group = tokens.GetComponent<CanvasGroup>();
            if (group == null)
                group = tokens.gameObject.AddComponent<CanvasGroup>();
            group.alpha = 1f;
            group.interactable = false;
            group.blocksRaycasts = false;
            var graphics = tokens.GetComponentsInChildren<Graphic>(true);
            for (int i = 0; i < graphics.Length; i++)
            {
                if (graphics[i] != null)
                    graphics[i].raycastTarget = false;
            }
        }

        public Button EnsureButton()
        {
            if (SensoriButton != null)
            {
                var ready = SensoriButton.GetComponent<Image>();
                if (ready != null)
                    ready.raycastTarget = true;
                RetirePressable(SensoriButton.gameObject);
                if (SensoriButton.GetComponent<HomePlateMotion>() == null)
                    SensoriButton.gameObject.AddComponent<HomePlateMotion>();
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
            RetirePressable(plate.gameObject);
            if (plate.GetComponent<HomePlateMotion>() == null)
                plate.gameObject.AddComponent<HomePlateMotion>();

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
            KeepThemeSprites();
            var sign = transform.Find("Enseigne") as RectTransform;
            if (sign != null)
                Center(sign, new Vector2(0f, 36f), new Vector2(980f, 420f));

            RetirePath("Enseigne/Cadre");
            RetirePath("Enseigne/Sensori/SousTitre");
            RetirePath("Enseigne/Plaque/SousTitre");
            StripBrandCaptions();

            var plate = transform.Find("Enseigne/Sensori") as RectTransform;
            if (plate == null)
                plate = transform.Find("Enseigne/Plaque") as RectTransform;
            if (plate != null)
            {
                plate.name = "Sensori";
                UiFactory.AnchorCenter(plate, Vector2.zero, new Vector2(PlateWidth, PlateHeight));
                var title = plate.Find("Titre") as RectTransform;
                if (title != null)
                    UiFactory.Stretch(title, 28f, 28f, 28f, 28f);
                var face = plate.GetComponent<Image>();
                if (face != null)
                {
                    ApplySoftFace(face, PlateCream, -8f);
                    face.raycastTarget = true;
                }
                RetirePressable(plate.gameObject);
                if (plate.GetComponent<HomePlateMotion>() == null)
                    plate.gameObject.AddComponent<HomePlateMotion>();
                EnsureJouer(plate);
            }

            StyleShadow();
            RetireLegend();
            EnsureTitleLetters();
            EnsureMark();

            var cards = transform.Find("ZoneCartes");
            if (cards != null)
                cards.gameObject.SetActive(false);

            var staleTokens = transform.Find("Jetons");
            if (staleTokens != null)
            {
                staleTokens.name = "JetonsAnciens";
                Retire(staleTokens.gameObject);
            }
            BuildTokens();
            SealWelcome();
            EnsureLandscape();
        }

        void EnsureLandscape()
        {
            ScreenBackdrop.Ensure(transform, "accueil");
        }

        public void BeginDeparture(Action continueWith)
        {
            var motion = PlateMotion();
            if (motion == null)
            {
                WoodenAudio.PlayTap();
                continueWith?.Invoke();
                return;
            }
            motion.Depart(continueWith);
        }

        public void Refresh()
        {
        }

        Coroutine _intro;

        public void PlayIntro()
        {
            if (!Application.isPlaying || !isActiveAndEnabled)
            {
                PlayIntroNow();
                return;
            }
            if (_intro != null)
                StopCoroutine(_intro);
            _intro = StartCoroutine(PlayIntroSoon());
        }

        System.Collections.IEnumerator PlayIntroSoon()
        {
            yield return null;
            PlayIntroNow();
            _intro = null;
        }

        void PlayIntroNow()
        {
            var motion = PlateMotion();
            if (motion != null)
                motion.ResetForWelcome();
            EnsureTitleLetters();
            TintTitle();

            var sign = transform.Find("Enseigne");
            if (sign == null || !Application.isPlaying)
                return;
            sign.localScale = Vector3.one * 0.94f;
            Motion.Scale(sign, Vector3.one, 0.5f, Ease.OutBack);
            if (motion != null)
                motion.HoldBreath(0.55f);

            var title = transform.Find("Enseigne/Sensori/Titre");
            if (title == null)
                title = transform.Find("Enseigne/Plaque/Titre");
            var letters = title != null ? title.GetComponent<WelcomeLetters>() : null;
            if (letters != null)
                letters.Play();

            var tokens = GetComponentsInChildren<FloatingToken>(true);
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] != null && tokens[i].gameObject.activeInHierarchy)
                    tokens[i].PlayArrival(0.18f + i * 0.07f);
            }

            var mascot = PeanutMascot.Ensure(transform);
            if (mascot != null)
                mascot.PlayHomeWelcome(SensoriButton);
        }

        public Sprite PlateSprite()
        {
            if (_keptPanel != null)
                return _keptPanel;
            KeepThemeSprites();
            return _keptPanel;
        }

        public Sprite ShadowSprite()
        {
            if (_keptShadow != null)
                return _keptShadow;
            KeepThemeSprites();
            return _keptShadow;
        }

        void KeepThemeSprites()
        {
            if (_keptPanel == null)
            {
                var button = SensoriButton;
                var image = button != null ? button.GetComponent<Image>() : null;
                if (image != null && image.sprite != null && image.sprite != SoftRounded())
                    _keptPanel = image.sprite;
            }
            if (_keptShadow == null)
            {
                var shadow = transform.Find("Enseigne/Ombre");
                var image = shadow != null ? shadow.GetComponent<Image>() : null;
                if (image != null && image.sprite != null)
                    _keptShadow = image.sprite;
            }
        }

        static Sprite SoftRounded()
        {
            if (_softRounded != null)
                return _softRounded;
            _softRounded = UiFactory.RoundedSprite();
            return _softRounded;
        }

        static void ApplySoftFace(Image image, Color color, float shadowY)
        {
            if (image == null)
                return;
            var sprite = SoftRounded();
            if (sprite != null)
            {
                image.sprite = sprite;
                image.type = Image.Type.Sliced;
            }
            image.color = color;
            image.material = null;
            image.preserveAspect = false;
            ApplyDropShadow(image, shadowY);
        }

        static void ApplyDropShadow(Graphic graphic, float shadowY)
        {
            if (graphic == null)
                return;
            var shadow = graphic.GetComponent<Shadow>();
            if (shadow == null || shadow is Outline)
                shadow = graphic.gameObject.AddComponent<Shadow>();
            shadow.effectColor = SoftDrop;
            shadow.effectDistance = new Vector2(0f, shadowY);
            shadow.useGraphicAlpha = true;
        }

        void EnsureJouer(Transform plate)
        {
            if (plate == null)
                return;
            var existing = plate.Find("Jouer");
            Image face;
            if (existing == null)
            {
                face = UiFactory.Picture("Jouer", plate, SoftRounded(), InkCoral, true, true);
                var label = UiFactory.Label("Libelle", face.transform, "JOUER", 42, PlateCream, TextAnchor.MiddleCenter);
                UiFactory.Stretch(label.rectTransform, 12f, 8f, 12f, 8f);
                label.font = ReadableFont();
                label.fontStyle = FontStyle.Bold;
                label.raycastTarget = false;
            }
            else
            {
                face = existing.GetComponent<Image>();
                if (face == null)
                    face = existing.gameObject.AddComponent<Image>();
                var label = existing.Find("Libelle");
                var text = label != null ? label.GetComponent<Text>() : null;
                if (text != null)
                {
                    text.text = "JOUER";
                    text.color = PlateCream;
                    text.fontStyle = FontStyle.Bold;
                    text.fontSize = 42;
                    text.raycastTarget = false;
                    text.font = ReadableFont();
                }
            }
            UiFactory.AnchorCenter(face.rectTransform, new Vector2(0f, -108f), new Vector2(320f, 84f));
            ApplySoftFace(face, InkCoral, -6f);
            face.raycastTarget = true;
        }

        static void Center(RectTransform rect, Vector2 position, Vector2 size)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        void EnsureMark()
        {
            var existing = transform.Find("Marque");
            Text mark;
            if (existing != null)
                mark = existing.GetComponent<Text>();
            else
                mark = UiFactory.Label("Marque", transform, "FM", 18, MontessoriPalette.WithAlpha(MontessoriPalette.InkSoft, 0.55f), TextAnchor.MiddleRight);
            if (mark == null)
                return;
            mark.text = "FM";
            mark.fontSize = 18;
            mark.color = MontessoriPalette.WithAlpha(MontessoriPalette.InkSoft, 0.55f);
            mark.alignment = TextAnchor.MiddleRight;
            mark.raycastTarget = false;
            mark.font = ReadableFont();
            var rect = mark.rectTransform;
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.sizeDelta = new Vector2(72f, 32f);
            rect.anchoredPosition = new Vector2(-36f, 22f);
            mark.transform.SetAsLastSibling();
        }

        public static Font ReadableFont()
        {
            return UiFont.Builtin;
        }

        void EnsureTitleLetters()
        {
            var plate = transform.Find("Enseigne/Sensori");
            if (plate == null)
                plate = transform.Find("Enseigne/Plaque");
            if (plate == null)
                return;
            var title = plate.Find("Titre");
            if (title == null)
                return;
            if (title.GetComponent<WelcomeLetters>() == null)
                title.gameObject.AddComponent<WelcomeLetters>();
            if (title.Find("Lettre") != null)
            {
                LayoutTitleLetters(title);
                return;
            }

            var existing = title.GetComponent<Text>();
            if (existing != null)
            {
                existing.text = string.Empty;
                existing.raycastTarget = false;
                existing.enabled = false;
            }
            var shadow = title.GetComponent<Shadow>();
            if (shadow != null)
                shadow.enabled = false;

            const string word = "Sensori";
            const float slot = 90f;
            float origin = -word.Length * slot * 0.5f + slot * 0.5f;
            var font = ReadableFont();
            for (int i = 0; i < word.Length; i++)
            {
                var label = UiFactory.Label("Lettre", title, word[i].ToString(), 90, PrimaryInk(i), TextAnchor.MiddleCenter);
                UiFactory.AnchorCenter(label.rectTransform, new Vector2(origin + i * slot, LetterLift), new Vector2(slot, 148f));
                label.font = font;
                label.raycastTarget = false;
                var letterShadow = label.gameObject.AddComponent<Shadow>();
                letterShadow.effectColor = new Color(0f, 0f, 0f, 0.25f);
                letterShadow.effectDistance = new Vector2(0f, -2f);
                letterShadow.useGraphicAlpha = true;
            }
        }

        void TintTitle()
        {
            var title = transform.Find("Enseigne/Sensori/Titre");
            if (title == null)
                title = transform.Find("Enseigne/Plaque/Titre");
            TintTitle(title);
        }

        static void TintTitle(Transform title)
        {
            LayoutTitleLetters(title);
        }

        static void LayoutTitleLetters(Transform title)
        {
            if (title == null)
                return;
            const float slot = 90f;
            int count = 0;
            for (int i = 0; i < title.childCount; i++)
            {
                if (title.GetChild(i).name == "Lettre")
                    count++;
            }
            float origin = -count * slot * 0.5f + slot * 0.5f;
            int index = 0;
            for (int i = 0; i < title.childCount; i++)
            {
                var child = title.GetChild(i);
                if (child.name != "Lettre")
                    continue;
                var rect = child as RectTransform;
                if (rect != null)
                    UiFactory.AnchorCenter(rect, new Vector2(origin + index * slot, LetterLift), new Vector2(slot, 148f));
                var label = child.GetComponent<Text>();
                if (label != null && !string.IsNullOrEmpty(label.text))
                    label.color = PrimaryInk(index);
                var letterShadow = child.GetComponent<Shadow>();
                if (letterShadow != null)
                {
                    letterShadow.effectColor = new Color(0f, 0f, 0f, 0.25f);
                    letterShadow.effectDistance = new Vector2(0f, -2f);
                    letterShadow.useGraphicAlpha = true;
                }
                index++;
            }
        }

        static Color PrimaryInk(int index)
        {
            switch (Mathf.Abs(index) % 4)
            {
                case 0:
                    return InkCoral;
                case 1:
                    return InkAzure;
                case 2:
                    return InkMustard;
                default:
                    return InkApple;
            }
        }

        void StyleShadow()
        {
            var shadow = transform.Find("Enseigne/Ombre");
            if (shadow == null)
                return;
            var image = shadow.GetComponent<Image>();
            if (image != null)
                image.raycastTarget = false;
            shadow.gameObject.SetActive(false);
        }

        void RetireLegend()
        {
            var legend = transform.Find("Legende");
            if (legend != null)
                Retire(legend.gameObject);
        }

        void StripBrandCaptions()
        {
            var texts = GetComponentsInChildren<Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                var label = texts[i];
                if (label == null || string.IsNullOrEmpty(label.text))
                    continue;
                if (!label.gameObject.activeInHierarchy)
                    continue;
                string value = label.text;
                if (value.IndexOf("Montessori", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("Voyelles", StringComparison.OrdinalIgnoreCase) >= 0
                    || value.IndexOf("Consonnes", StringComparison.OrdinalIgnoreCase) >= 0)
                    Retire(label.gameObject);
            }
        }

        HomePlateMotion PlateMotion()
        {
            var plate = transform.Find("Enseigne/Sensori");
            if (plate == null)
                plate = transform.Find("Enseigne/Plaque");
            if (plate == null)
                return null;
            var motion = plate.GetComponent<HomePlateMotion>();
            if (motion == null)
                motion = plate.gameObject.AddComponent<HomePlateMotion>();
            return motion;
        }

        void RetirePath(string path)
        {
            var found = transform.Find(path);
            if (found != null)
                Retire(found.gameObject);
        }

        static void RetirePressable(GameObject target)
        {
            if (target == null)
                return;
            var pressable = target.GetComponent<Pressable>();
            if (pressable != null)
                Retire(pressable);
        }

        static void Retire(UnityEngine.Object target)
        {
            if (target == null)
                return;
            var go = target as GameObject;
            if (go == null && target is Component component)
                go = component.gameObject;
            if (target is Behaviour behaviour)
                behaviour.enabled = false;
            if (go != null && target is GameObject)
                go.SetActive(false);
            if (Application.isPlaying)
                UnityEngine.Object.Destroy(target);
            else
                UnityEngine.Object.DestroyImmediate(target);
        }

        struct TokenSeed
        {
            public string Glyph;
            public bool Shape;
            public TokenShape Form;
            public Vector2 Position;
            public float Size;
            public Color Wash;
            public Color Ink;
            public float Phase;
            public float Amplitude;
            public float Bob;
            public float Tilt;

            public TokenSeed(string glyph, Vector2 position, float size, Color wash, Color ink, float phase, float amplitude, float bob, float tilt)
            {
                Glyph = glyph;
                Shape = false;
                Form = TokenShape.Disc;
                Position = position;
                Size = size;
                Wash = wash;
                Ink = ink;
                Phase = phase;
                Amplitude = amplitude;
                Bob = bob;
                Tilt = tilt;
            }

            public static TokenSeed FormOf(TokenShape form, Vector2 position, float size, Color wash, Color ink, float phase, float amplitude, float bob, float tilt)
            {
                return new TokenSeed(string.Empty, position, size, wash, ink, phase, amplitude, bob, tilt)
                {
                    Shape = true,
                    Form = form
                };
            }
        }

        static readonly Color SoftBlue = new Color(0.62f, 0.78f, 0.90f, 1f);
        static readonly Color SoftRose = new Color(0.93f, 0.62f, 0.72f, 1f);
        static readonly Color SoftHoney = new Color(0.94f, 0.80f, 0.55f, 1f);
        static readonly Color SoftMoss = new Color(0.70f, 0.84f, 0.72f, 1f);
        static readonly Color SoftSky = new Color(0.68f, 0.82f, 0.90f, 1f);
        static readonly Color SoftCoral = new Color(0.93f, 0.72f, 0.64f, 1f);
        static readonly Color GlyphBlue = new Color(0.20f, 0.42f, 0.76f, 1f);
        static readonly Color GlyphRose = new Color(0.80f, 0.26f, 0.46f, 1f);
        static readonly Color GlyphHoney = new Color(0.66f, 0.38f, 0.12f, 1f);
        static readonly Color GlyphMoss = new Color(0.28f, 0.50f, 0.36f, 1f);
        static readonly Color GlyphSky = new Color(0.24f, 0.48f, 0.72f, 1f);
        static readonly Color GlyphCoral = new Color(0.78f, 0.38f, 0.30f, 1f);

        static TokenSeed[] TokenSeeds()
        {
            return new[]
            {
                new TokenSeed("a", new Vector2(-530f, 228f), 156f, SoftBlue, GlyphBlue, 0.4f, 16f, 1.35f, 4.5f),
                new TokenSeed("m", new Vector2(560f, 118f), 150f, SoftRose, GlyphRose, 1.8f, 18f, 1.2f, 5f),
                new TokenSeed("1", new Vector2(-600f, -16f), 144f, SoftHoney, GlyphHoney, 2.6f, 14f, 1.45f, 3.8f),
                new TokenSeed("3", new Vector2(620f, -64f), 148f, SoftMoss, GlyphMoss, 0.9f, 15f, 1.28f, 4.2f),
                new TokenSeed("e", new Vector2(-410f, -248f), 136f, SoftBlue, GlyphBlue, 2.2f, 12f, 1.15f, 3.4f),
                new TokenSeed("s", new Vector2(455f, 268f), 140f, SoftRose, GlyphRose, 3.3f, 16f, 1.32f, 5.5f),
                new TokenSeed("2", new Vector2(168f, -268f), 140f, SoftHoney, GlyphHoney, 1.3f, 13f, 1.4f, 4f),
                new TokenSeed("o", new Vector2(-150f, 308f), 134f, SoftBlue, GlyphBlue, 2.4f, 11f, 1.1f, 3.6f)
            };
        }

        void BuildTokens()
        {
            var root = UiFactory.Rect("Jetons", transform);
            UiFactory.Stretch(root, 0f, 0f, 0f, 0f);
            var group = root.gameObject.AddComponent<CanvasGroup>();
            group.alpha = 1f;
            group.interactable = false;
            group.blocksRaycasts = false;

            var seeds = TokenSeeds();
            for (int i = 0; i < seeds.Length; i++)
                BuildToken(root, seeds[i], i);
            root.SetAsFirstSibling();
        }

        static void BuildToken(RectTransform parent, TokenSeed seed, int index)
        {
            var token = UiFactory.Rect("Jeton", parent);
            UiFactory.AnchorCenter(token, seed.Position, new Vector2(seed.Size, seed.Size));

            var face = UiFactory.Picture("Jeton", token, SoftRounded(), PlateCream, true, false);
            UiFactory.Stretch(face.rectTransform, 0f, 0f, 0f, 0f);
            ApplySoftFace(face, PlateCream, -6f);
            face.raycastTarget = false;
            var ink = PrimaryInk(index);

            if (seed.Shape)
            {
                var shapeRect = UiFactory.Rect("Forme", face.transform);
                UiFactory.Stretch(shapeRect, 16f, 20f, 16f, 14f);
                var shape = shapeRect.gameObject.AddComponent<SoftShape>();
                shape.Configure(seed.Form, ink);
                shape.raycastTarget = false;
            }
            else
            {
                var label = UiFactory.Label("Glyph", face.transform, seed.Glyph, Mathf.RoundToInt(seed.Size * 0.62f), ink, TextAnchor.MiddleCenter);
                UiFactory.Stretch(label.rectTransform, 0f, 0f, 0f, 0f);
                label.font = ReadableFont();
                label.fontStyle = FontStyle.Bold;
                label.raycastTarget = false;
            }

            var floaty = token.gameObject.AddComponent<FloatingToken>();
            floaty.Boot(seed.Phase, seed.Amplitude, seed.Bob, seed.Tilt, seed.Bob * 0.72f);
            var graphics = token.GetComponentsInChildren<Graphic>(true);
            for (int i = 0; i < graphics.Length; i++)
                graphics[i].raycastTarget = false;
        }
    }
}
