using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class HomePresenter : MonoBehaviour
    {
        public static readonly Color WarmShadow = new Color(0.52f, 0.30f, 0.12f, 0.32f);
        public static readonly Color InstructionInk = new Color(0.42f, 0.26f, 0.14f, 1f);

        const string Instruction = "Touche la plaque pour commencer";

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

            var shadow = UiFactory.Picture("Ombre", sign, theme.Shadow, WarmShadow, false, false);
            UiFactory.AnchorCenter(shadow.rectTransform, new Vector2(0f, -48f), new Vector2(1100f, 390f));
            shadow.preserveAspect = false;
            shadow.raycastTarget = false;

            var plate = UiFactory.Picture("Sensori", sign, theme.Panel, Color.white, true, true);
            UiFactory.AnchorCenter(plate.rectTransform, Vector2.zero, new Vector2(840f, 250f));
            plate.preserveAspect = false;
            plate.raycastTarget = true;
            _button = UiFactory.CreateButton(plate.gameObject);
            plate.gameObject.AddComponent<HomePlateMotion>();

            var title = UiFactory.Label("Titre", plate.transform, "Sensori", 84, MontessoriPalette.WalnutDeep, TextAnchor.MiddleCenter);
            UiFactory.Stretch(title.rectTransform, 28f, 28f, 28f, 28f);
            title.font = ReadableFont();
            title.raycastTarget = false;
            var titleShadow = title.gameObject.AddComponent<Shadow>();
            titleShadow.effectColor = MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.18f);
            titleShadow.effectDistance = new Vector2(0f, -3f);
            titleShadow.useGraphicAlpha = true;
            EnsureTitleLetters();

            var footer = UiFactory.Label("Legende", transform, Instruction, 28, InstructionInk, TextAnchor.MiddleCenter);
            UiFactory.AnchorBottom(footer.rectTransform, 88f, 40f, 40f);
            footer.font = ReadableFont();
            footer.raycastTarget = false;

            BuildTokens(theme != null ? theme.Panel : null, theme != null ? theme.Shadow : null);
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
                UiFactory.AnchorCenter(plate, Vector2.zero, new Vector2(840f, 250f));
                var title = plate.Find("Titre") as RectTransform;
                if (title != null)
                    UiFactory.Stretch(title, 28f, 28f, 28f, 28f);
                var face = plate.GetComponent<Image>();
                if (face != null)
                    face.raycastTarget = true;
                RetirePressable(plate.gameObject);
                if (plate.GetComponent<HomePlateMotion>() == null)
                    plate.gameObject.AddComponent<HomePlateMotion>();
            }

            StyleShadow();
            StyleLegend();
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
            BuildTokens(PlateSprite(), ShadowSprite());
            SealWelcome();
            EnsureLandscape();
        }

        void EnsureLandscape()
        {
            if (!Application.isPlaying)
                return;
            var found = transform.Find("Paysage");
            Image image;
            if (found == null)
            {
                image = UiFactory.Picture("Paysage", transform, HomeLandscape.Sprite, Color.white, false, false);
                UiFactory.Stretch(image.rectTransform, 0f, 0f, 0f, 0f);
            }
            else
            {
                image = found.GetComponent<Image>();
                if (image == null)
                    image = found.gameObject.AddComponent<Image>();
                image.sprite = HomeLandscape.Sprite;
                image.color = Color.white;
                image.type = Image.Type.Simple;
            }
            image.preserveAspect = false;
            image.raycastTarget = false;
            image.transform.SetAsFirstSibling();
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
                TintTitle(title);
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
                var label = UiFactory.Label("Lettre", title, word[i].ToString(), 90, MontessoriPalette.WalnutDeep, TextAnchor.MiddleCenter);
                UiFactory.AnchorCenter(label.rectTransform, new Vector2(origin + i * slot, 4f), new Vector2(slot, 148f));
                label.font = font;
                label.color = LetterInk(word[i]);
                label.raycastTarget = false;
                var letterShadow = label.gameObject.AddComponent<Shadow>();
                letterShadow.effectColor = MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.16f);
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
            if (title == null)
                return;
            for (int i = 0; i < title.childCount; i++)
            {
                var child = title.GetChild(i);
                if (child.name != "Lettre")
                    continue;
                var label = child.GetComponent<Text>();
                if (label == null || string.IsNullOrEmpty(label.text))
                    continue;
                label.color = LetterInk(label.text[0]);
            }
        }

        static Color LetterInk(char glyph)
        {
            char c = char.ToLowerInvariant(glyph);
            if (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u' || c == 'y')
                return GlyphBlue;
            if (c >= '0' && c <= '9')
                return GlyphHoney;
            return GlyphRose;
        }

        void StyleShadow()
        {
            var shadow = transform.Find("Enseigne/Ombre") as RectTransform;
            if (shadow == null)
                return;
            UiFactory.AnchorCenter(shadow, new Vector2(0f, -48f), new Vector2(1100f, 390f));
            var image = shadow.GetComponent<Image>();
            if (image == null)
                return;
            image.color = WarmShadow;
            image.preserveAspect = false;
            image.raycastTarget = false;
        }

        void StyleLegend()
        {
            var legend = transform.Find("Legende");
            var legendText = legend != null ? legend.GetComponent<Text>() : null;
            if (legendText == null)
                return;
            legendText.text = Instruction;
            legendText.color = InstructionInk;
            legendText.fontSize = 28;
            legendText.raycastTarget = false;
            var font = ReadableFont();
            if (font != null)
                legendText.font = font;
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
                if (label.transform == transform.Find("Legende"))
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

        void BuildTokens(Sprite wood, Sprite shadow)
        {
            var root = UiFactory.Rect("Jetons", transform);
            UiFactory.Stretch(root, 0f, 0f, 0f, 0f);
            var group = root.gameObject.AddComponent<CanvasGroup>();
            group.alpha = 1f;
            group.interactable = false;
            group.blocksRaycasts = false;

            var seeds = TokenSeeds();
            for (int i = 0; i < seeds.Length; i++)
                BuildToken(root, seeds[i], wood, shadow);
            root.SetAsFirstSibling();
        }

        static void BuildToken(RectTransform parent, TokenSeed seed, Sprite wood, Sprite shadow)
        {
            var token = UiFactory.Rect("Jeton", parent);
            UiFactory.AnchorCenter(token, seed.Position, new Vector2(seed.Size, seed.Size));

            if (shadow != null)
            {
                var shade = UiFactory.Picture("Ombre", token, shadow, WarmShadow, false, false);
                UiFactory.Stretch(shade.rectTransform, -10f, -22f, -10f, 6f);
                shade.preserveAspect = false;
                shade.raycastTarget = false;
            }

            float washMix = seed.Shape ? 0.82f : 0.46f;
            var wash = Color.Lerp(new Color(1f, 0.97f, 0.92f, 1f), seed.Wash, washMix);
            var face = UiFactory.Picture("Jeton", token, wood, wash, wood != null, false);
            UiFactory.Stretch(face.rectTransform, 0f, 4f, 0f, 0f);
            face.preserveAspect = false;
            face.raycastTarget = false;

            if (seed.Shape)
            {
                var shapeRect = UiFactory.Rect("Forme", face.transform);
                UiFactory.Stretch(shapeRect, 16f, 20f, 16f, 14f);
                var shape = shapeRect.gameObject.AddComponent<SoftShape>();
                shape.Configure(seed.Form, seed.Ink);
                shape.raycastTarget = false;
            }
            else
            {
                var label = UiFactory.Label("Glyph", face.transform, seed.Glyph, Mathf.RoundToInt(seed.Size * 0.62f), seed.Ink, TextAnchor.MiddleCenter);
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
