using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class PeanutMascot : MonoBehaviour
    {
        static readonly Vector2 Entrance = new Vector2(-1180f, -280f);
        static readonly Vector2 Stage = new Vector2(-400f, -280f);
        static readonly Vector2 Corner = new Vector2(-760f, -400f);
        static readonly Vector2 SignShown = new Vector2(150f, 80f);
        static readonly Vector2 SignHidden = new Vector2(-80f, -220f);
        const float StageScale = 1f;
        const float CornerScale = 0.72f;

        RectTransform _root;
        RectTransform _corps;
        RectTransform _armL;
        RectTransform _armR;
        RectTransform _legL;
        RectTransform _legR;
        RectTransform _sprout;
        RectTransform _shadow;
        Image _eyeL;
        Image _eyeR;
        Image _mouth;
        RectTransform _bubble;
        Text _bubbleText;
        GameObject _veil;
        CanvasGroup _veilGroup;
        RectTransform _sign;
        InputField _input;
        Text _hint;
        RectTransform _field;
        Button _pet;
        Button _homeButton;
        Coroutine _ritual;
        bool _busy;
        bool _committed;
        bool _blinking;
        bool _idleMotion = true;
        float _blinkTimer = 1.4f;

        public static PeanutMascot Ensure(Transform home)
        {
            if (home == null)
                return null;
            var found = home.Find("Cacahuete");
            if (found != null)
            {
                var existing = found.GetComponent<PeanutMascot>();
                if (existing != null)
                    return existing;
            }
            var root = UiFactory.Rect("Cacahuete", home);
            var mascot = root.gameObject.AddComponent<PeanutMascot>();
            mascot.Build(root, home);
            return mascot;
        }

        public void PlayHomeWelcome(Button homeButton)
        {
            if (!Application.isPlaying || !isActiveAndEnabled)
                return;
            _homeButton = homeButton;
            if (_ritual != null)
                StopCoroutine(_ritual);
            HideGate();
            _busy = true;
            SetPettable(false);
            _ritual = StartCoroutine(ChildProfile.HasName ? ReturnHello() : FirstMeeting());
        }

        void Build(RectTransform root, Transform home)
        {
            _root = root;
            UiFactory.AnchorCenter(root, Entrance, new Vector2(420f, 560f));
            root.localScale = Vector3.one * StageScale;

            _shadow = Art("OmbreSol", root, PeanutArt.Shadow, new Vector2(10f, -214f), new Vector2(210f, 52f), new Vector2(0.5f, 0.5f)).rectTransform;

            var corps = Art("Corps", root, PeanutArt.Body, Vector2.zero, new Vector2(242f, 330f), new Vector2(0.5f, 0.5f));
            _corps = corps.rectTransform;
            corps.raycastTarget = true;
            _pet = UiFactory.CreateButton(corps.gameObject);
            _pet.onClick.AddListener(OnTapped);
            _pet.interactable = false;

            Art("Joue", _corps, PeanutArt.Cheek, new Vector2(-62f, 52f), new Vector2(54f, 34f), new Vector2(0.5f, 0.5f));
            Art("Joue", _corps, PeanutArt.Cheek, new Vector2(66f, 60f), new Vector2(48f, 30f), new Vector2(0.5f, 0.5f));
            _eyeL = Art("Oeil", _corps, PeanutArt.EyeOpen, new Vector2(-38f, 72f), new Vector2(74f, 90f), new Vector2(0.5f, 0.5f));
            _eyeR = Art("Oeil", _corps, PeanutArt.EyeOpen, new Vector2(36f, 84f), new Vector2(64f, 78f), new Vector2(0.5f, 0.5f));
            _mouth = Art("Bouche", _corps, PeanutArt.Smile, new Vector2(2f, 16f), new Vector2(78f, 66f), new Vector2(0.5f, 0.5f));
            _sprout = Art("Pousse", _corps, PeanutArt.Sprout, new Vector2(6f, 158f), new Vector2(92f, 76f), new Vector2(0.5f, 0.12f)).rectTransform;
            _sprout.localRotation = Quaternion.Euler(0f, 0f, -12f);

            _armL = Limb("EpauleGauche", _corps, new Vector2(-78f, 24f), 22f, false, false);
            _armR = Limb("EpauleDroite", _corps, new Vector2(76f, 18f), -20f, false, true);
            _legL = Limb("HancheGauche", _corps, new Vector2(-36f, -112f), 10f, true, false);
            _legR = Limb("HancheDroite", _corps, new Vector2(40f, -116f), -12f, true, true);

            _bubble = UiFactory.Rect("Bulle", root);
            UiFactory.AnchorCenter(_bubble, new Vector2(36f, 248f), new Vector2(460f, 150f));
            var bubbleFace = Art("FondBulle", _bubble, PeanutArt.Bubble, Vector2.zero, new Vector2(460f, 150f), new Vector2(0.5f, 0.5f));
            UiFactory.Stretch(bubbleFace.rectTransform, 0f, 0f, 0f, 0f);
            _bubbleText = UiFactory.Label("Message", _bubble, string.Empty, 30, MontessoriPalette.WalnutDeep, TextAnchor.MiddleCenter);
            UiFactory.Stretch(_bubbleText.rectTransform, 36f, 28f, 28f, 22f);
            _bubbleText.font = UiFont.Builtin;
            _bubble.gameObject.SetActive(false);

            _veil = new GameObject("VoileCacahuete", typeof(RectTransform), typeof(CanvasGroup), typeof(Image));
            _veil.transform.SetParent(home, false);
            var veilRect = (RectTransform)_veil.transform;
            UiFactory.Stretch(veilRect, 0f, 0f, 0f, 0f);
            var veilImage = _veil.GetComponent<Image>();
            veilImage.sprite = PeanutArt.Solid;
            veilImage.color = new Color(0.22f, 0.12f, 0.06f, 1f);
            veilImage.raycastTarget = true;
            _veilGroup = _veil.GetComponent<CanvasGroup>();
            _veilGroup.alpha = 0f;
            _veilGroup.blocksRaycasts = true;
            _veil.SetActive(false);

            _sign = UiFactory.Rect("PanneauPrenom", home);
            UiFactory.AnchorCenter(_sign, SignHidden, new Vector2(680f, 440f));
            var board = Art("Planche", _sign, PeanutArt.Board, Vector2.zero, new Vector2(680f, 440f), new Vector2(0.5f, 0.5f));
            UiFactory.Stretch(board.rectTransform, 0f, 0f, 0f, 0f);
            board.raycastTarget = true;

            var question = UiFactory.Label("Question", _sign, "Comment tu t'appelles ?", 40, MontessoriPalette.WalnutDeep, TextAnchor.MiddleCenter);
            UiFactory.AnchorCenter(question.rectTransform, new Vector2(0f, 118f), new Vector2(580f, 72f));
            question.font = UiFont.Builtin;

            _hint = UiFactory.Label("Indice", _sign, "Ton prénom", 24, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            UiFactory.AnchorCenter(_hint.rectTransform, new Vector2(0f, 64f), new Vector2(520f, 40f));
            _hint.font = UiFont.Builtin;

            var fieldImage = Art("Champ", _sign, PeanutArt.Field, new Vector2(0f, -4f), new Vector2(520f, 92f), new Vector2(0.5f, 0.5f));
            fieldImage.raycastTarget = true;
            _field = fieldImage.rectTransform;
            var typed = UiFactory.Label("Saisie", fieldImage.transform, string.Empty, 40, MontessoriPalette.Ink, TextAnchor.MiddleCenter);
            UiFactory.Stretch(typed.rectTransform, 28f, 10f, 28f, 10f);
            typed.font = UiFont.Builtin;
            typed.supportRichText = false;
            var placeholder = UiFactory.Label("Invitation", fieldImage.transform, "Écris ici", 34, new Color(0.62f, 0.50f, 0.40f, 0.85f), TextAnchor.MiddleCenter);
            UiFactory.Stretch(placeholder.rectTransform, 28f, 10f, 28f, 10f);
            placeholder.font = UiFont.Builtin;
            placeholder.fontStyle = FontStyle.Italic;

            _input = fieldImage.gameObject.AddComponent<InputField>();
            _input.textComponent = typed;
            _input.placeholder = placeholder;
            _input.targetGraphic = fieldImage;
            _input.characterLimit = 16;
            _input.lineType = InputField.LineType.SingleLine;
            _input.contentType = InputField.ContentType.Standard;
            _input.keyboardType = TouchScreenKeyboardType.Default;
            _input.customCaretColor = true;
            _input.caretColor = MontessoriPalette.Walnut;
            _input.caretWidth = 3;
            _input.selectionColor = new Color(0.86f, 0.62f, 0.32f, 0.35f);
            var navigation = _input.navigation;
            navigation.mode = Navigation.Mode.None;
            _input.navigation = navigation;
            _input.onSubmit.AddListener(delegate { OnConfirm(); });

            var confirm = Art("Valider", _sign, PeanutArt.ButtonFace, new Vector2(0f, -132f), new Vector2(300f, 88f), new Vector2(0.5f, 0.5f));
            confirm.raycastTarget = true;
            confirm.gameObject.AddComponent<Pressable>();
            var confirmButton = UiFactory.CreateButton(confirm.gameObject);
            confirmButton.onClick.AddListener(OnConfirm);
            var confirmLabel = UiFactory.Label("Action", confirm.transform, "C'est moi !", 32, MontessoriPalette.Cream, TextAnchor.MiddleCenter);
            UiFactory.Stretch(confirmLabel.rectTransform, 12f, 8f, 12f, 8f);
            confirmLabel.font = UiFont.Builtin;

            _sign.gameObject.SetActive(false);
            BringForward();
        }

        IEnumerator FirstMeeting()
        {
            SetHomeEnabled(false);
            _mouth.sprite = PeanutArt.Smile;
            _root.anchoredPosition = Entrance;
            _root.localScale = Vector3.one * StageScale;
            RestLimbs();
            BringForward();
            yield return WalkTo(Stage, 1.65f, StageScale);
            yield return Hop();
            yield return Wave();
            Say("Coucou !");
            yield return new WaitForSecondsRealtime(0.85f);
            Say("Je m'appelle Cacahuète.");
            yield return new WaitForSecondsRealtime(1.15f);
            HideBubble();
            yield return new WaitForSecondsRealtime(0.2f);
            yield return ShowForm(string.Empty);
            Say("Enchantée, " + ChildProfile.Name + " !");
            _mouth.sprite = PeanutArt.Grin;
            yield return new WaitForSecondsRealtime(1.2f);
            HideBubble();
            _mouth.sprite = PeanutArt.Smile;
            yield return WalkTo(Corner, 0.95f, CornerScale);
            FinishVisit();
        }

        IEnumerator ReturnHello()
        {
            SetHomeEnabled(true);
            _mouth.sprite = PeanutArt.Smile;
            _root.anchoredPosition = Entrance;
            _root.localScale = Vector3.one * StageScale;
            RestLimbs();
            BringForward();
            yield return WalkTo(Corner, 1.35f, CornerScale);
            Say("Bonjour, " + ChildProfile.Name + " !");
            _mouth.sprite = PeanutArt.Grin;
            yield return new WaitForSecondsRealtime(1.5f);
            HideBubble();
            _mouth.sprite = PeanutArt.Smile;
            FinishVisit();
        }

        IEnumerator EditName()
        {
            _busy = true;
            SetPettable(false);
            SetHomeEnabled(false);
            HideBubble();
            BringForward();
            if (Vector2.Distance(_root.anchoredPosition, Stage) > 80f)
                yield return WalkTo(Stage, 0.75f, StageScale);
            Say("On change de prénom ?");
            yield return new WaitForSecondsRealtime(0.7f);
            HideBubble();
            yield return ShowForm(ChildProfile.Name);
            Say("D'accord, " + ChildProfile.Name + " !");
            _mouth.sprite = PeanutArt.Grin;
            yield return new WaitForSecondsRealtime(1.05f);
            HideBubble();
            _mouth.sprite = PeanutArt.Smile;
            yield return WalkTo(Corner, 0.85f, CornerScale);
            FinishVisit();
        }

        IEnumerator ShowForm(string prefill)
        {
            _committed = false;
            _input.text = prefill ?? string.Empty;
            _hint.text = "Ton prénom";
            _hint.color = MontessoriPalette.InkSoft;
            _sign.gameObject.SetActive(true);
            _veil.SetActive(true);
            _veilGroup.alpha = 0f;
            _sign.anchoredPosition = SignHidden;
            _sign.localScale = Vector3.one * 0.2f;
            BringForward();

            float left = SignedAngle(_armL);
            float right = SignedAngle(_armR);
            float t = 0f;
            const float duration = 0.62f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float u = EaseCurve.Evaluate(Ease.OutBack, Mathf.Clamp01(t / duration));
                _veilGroup.alpha = Mathf.Clamp01(t / 0.28f) * 0.42f;
                _armL.localRotation = Quaternion.Euler(0f, 0f, Mathf.LerpUnclamped(left, 148f, u));
                _armR.localRotation = Quaternion.Euler(0f, 0f, Mathf.LerpUnclamped(right, -132f, u));
                _sign.anchoredPosition = Vector2.LerpUnclamped(SignHidden, SignShown, u);
                _sign.localScale = Vector3.one * Mathf.LerpUnclamped(0.2f, 1f, u);
                yield return null;
            }
            _sign.anchoredPosition = SignShown;
            _sign.localScale = Vector3.one;
            _input.ActivateInputField();
            while (!_committed)
                yield return null;
            yield return HideForm();
        }

        IEnumerator HideForm()
        {
            float left = SignedAngle(_armL);
            float right = SignedAngle(_armR);
            Vector2 from = _sign.anchoredPosition;
            float t = 0f;
            const float duration = 0.32f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float u = EaseCurve.Evaluate(Ease.InQuad, Mathf.Clamp01(t / duration));
                _veilGroup.alpha = 0.42f * (1f - u);
                _armL.localRotation = Quaternion.Euler(0f, 0f, Mathf.LerpUnclamped(left, 22f, u));
                _armR.localRotation = Quaternion.Euler(0f, 0f, Mathf.LerpUnclamped(right, -20f, u));
                _sign.anchoredPosition = Vector2.LerpUnclamped(from, SignHidden, u);
                _sign.localScale = Vector3.one * Mathf.LerpUnclamped(1f, 0.2f, u);
                yield return null;
            }
            HideGate();
            RestLimbs();
        }

        IEnumerator WalkTo(Vector2 target, float duration, float endScale)
        {
            Vector2 from = _root.anchoredPosition;
            float fromScale = _root.localScale.x;
            float t = 0f;
            _idleMotion = false;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float u = EaseCurve.Evaluate(Ease.InOutSine, Mathf.Clamp01(t / duration));
                _root.anchoredPosition = Vector2.LerpUnclamped(from, target, u);
                float scale = Mathf.LerpUnclamped(fromScale, endScale, u);
                _root.localScale = new Vector3(scale, scale, 1f);
                float step = Mathf.Sin(t * 14f);
                _corps.anchoredPosition = new Vector2(0f, Mathf.Abs(step) * 14f);
                _corps.localRotation = Quaternion.Euler(0f, 0f, step * 3.2f);
                _legL.localRotation = Quaternion.Euler(0f, 0f, step * 26f);
                _legR.localRotation = Quaternion.Euler(0f, 0f, -step * 26f);
                _shadow.localScale = new Vector3(1f - Mathf.Abs(step) * 0.08f, 1f, 1f);
                Look(new Vector2(3.5f, 0f));
                yield return null;
            }
            _root.anchoredPosition = target;
            _root.localScale = new Vector3(endScale, endScale, 1f);
            _corps.anchoredPosition = Vector2.zero;
            _corps.localRotation = Quaternion.identity;
            _shadow.localScale = Vector3.one;
            Look(Vector2.zero);
            RestLimbs();
            _idleMotion = true;
        }

        IEnumerator Hop()
        {
            _idleMotion = false;
            float t = 0f;
            const float duration = 0.38f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float wave = Mathf.Sin(Mathf.Clamp01(t / duration) * Mathf.PI);
                _corps.anchoredPosition = new Vector2(0f, wave * 26f);
                float squash = 1f + wave * 0.07f;
                _corps.localScale = new Vector3(1f / squash, squash, 1f);
                yield return null;
            }
            _corps.anchoredPosition = Vector2.zero;
            _corps.localScale = Vector3.one;
            _idleMotion = true;
        }

        IEnumerator Wave()
        {
            for (int i = 0; i < 3; i++)
            {
                yield return Swing(_armL, 22f, 158f, 0.18f);
                yield return Swing(_armL, 158f, 22f, 0.18f);
            }
        }

        IEnumerator Swing(Transform arm, float from, float to, float duration)
        {
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float u = EaseCurve.Evaluate(Ease.InOutSine, Mathf.Clamp01(t / duration));
                arm.localRotation = Quaternion.Euler(0f, 0f, Mathf.LerpUnclamped(from, to, u));
                yield return null;
            }
        }

        void Update()
        {
            if (!Application.isPlaying || _sprout == null)
                return;
            _sprout.localRotation = Quaternion.Euler(0f, 0f, -12f + Mathf.Sin(Time.unscaledTime * 1.7f) * 7f);
            if (_idleMotion && _corps != null)
            {
                float breath = Mathf.Sin(Time.unscaledTime * 2.05f);
                _corps.localScale = new Vector3(1f + breath * 0.012f, 1f + breath * 0.022f, 1f);
            }
            if (_blinking || _eyeL == null)
                return;
            _blinkTimer -= Time.unscaledDeltaTime;
            if (_blinkTimer <= 0f)
                StartCoroutine(Blink());
        }

        IEnumerator Blink()
        {
            _blinking = true;
            _eyeL.sprite = PeanutArt.EyeShut;
            _eyeR.sprite = PeanutArt.EyeShut;
            yield return new WaitForSecondsRealtime(0.09f);
            if (_eyeL != null)
                _eyeL.sprite = PeanutArt.EyeOpen;
            if (_eyeR != null)
                _eyeR.sprite = PeanutArt.EyeOpen;
            _blinking = false;
            _blinkTimer = Random.Range(2.1f, 4.6f);
        }

        void OnTapped()
        {
            if (_busy || !ChildProfile.HasName)
                return;
            if (_ritual != null)
                StopCoroutine(_ritual);
            _ritual = StartCoroutine(EditName());
        }

        void OnConfirm()
        {
            if (_committed || _input == null)
                return;
            if (!ChildProfile.TryAccept(_input.text, out var name))
            {
                _hint.text = string.IsNullOrWhiteSpace(_input.text) ? "Écris ton prénom." : "Juste des lettres.";
                _hint.color = MontessoriPalette.Coral;
                Motion.Shake(_field, 16f, 0.34f);
                WoodenAudio.PlayTock();
                return;
            }
            ChildProfile.Save(name);
            _input.text = name;
            _committed = true;
            WoodenAudio.PlaySuccess();
            if (MontessoriApp.Instance != null && _sign != null)
                MontessoriApp.Instance.Sparkle(_sign.position, MontessoriPalette.Sun);
        }

        void Say(string message)
        {
            if (_bubble == null)
                return;
            _bubbleText.text = message;
            _bubble.gameObject.SetActive(true);
            _bubble.localScale = Vector3.one * 0.2f;
            Motion.Scale(_bubble, Vector3.one, 0.34f, Ease.OutBack);
        }

        void HideBubble()
        {
            if (_bubble == null || !_bubble.gameObject.activeSelf)
                return;
            Motion.Scale(_bubble, Vector3.zero, 0.18f, Ease.InQuad).OnComplete(() =>
            {
                if (_bubble != null)
                    _bubble.gameObject.SetActive(false);
            });
        }

        void FinishVisit()
        {
            _busy = false;
            _ritual = null;
            SetPettable(true);
            SetHomeEnabled(true);
            _idleMotion = true;
        }

        void HideGate()
        {
            if (_sign != null)
                _sign.gameObject.SetActive(false);
            if (_veil != null)
                _veil.SetActive(false);
        }

        void RestLimbs()
        {
            if (_armL != null)
                _armL.localRotation = Quaternion.Euler(0f, 0f, 22f);
            if (_armR != null)
                _armR.localRotation = Quaternion.Euler(0f, 0f, -20f);
            if (_legL != null)
                _legL.localRotation = Quaternion.Euler(0f, 0f, 10f);
            if (_legR != null)
                _legR.localRotation = Quaternion.Euler(0f, 0f, -12f);
        }

        void Look(Vector2 offset)
        {
            if (_eyeL != null)
                _eyeL.rectTransform.anchoredPosition = new Vector2(-38f, 72f) + offset;
            if (_eyeR != null)
                _eyeR.rectTransform.anchoredPosition = new Vector2(36f, 84f) + offset;
        }

        void BringForward()
        {
            if (_veil != null)
                _veil.transform.SetAsLastSibling();
            if (_root != null)
                _root.SetAsLastSibling();
            if (_sign != null)
                _sign.SetAsLastSibling();
        }

        void SetPettable(bool enabled)
        {
            if (_pet != null)
                _pet.interactable = enabled;
        }

        void SetHomeEnabled(bool enabled)
        {
            if (_homeButton != null)
                _homeButton.interactable = enabled;
        }

        void OnEnable()
        {
            HideGate();
            _blinking = false;
            if (_eyeL != null)
                _eyeL.sprite = PeanutArt.EyeOpen;
            if (_eyeR != null)
                _eyeR.sprite = PeanutArt.EyeOpen;
            if (_mouth != null && !_busy)
                _mouth.sprite = PeanutArt.Smile;
        }

        void OnDisable()
        {
            _idleMotion = false;
            _blinking = false;
            if (_ritual != null)
            {
                StopCoroutine(_ritual);
                _ritual = null;
            }
        }

        static Image Art(string name, Transform parent, Sprite sprite, Vector2 position, Vector2 size, Vector2 pivot)
        {
            var image = UiFactory.Picture(name, parent, sprite, Color.white, false, false);
            var rect = image.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            image.preserveAspect = true;
            image.raycastTarget = false;
            return image;
        }

        static RectTransform Limb(string name, Transform parent, Vector2 shoulder, float angle, bool leg, bool flip)
        {
            var pivot = new Vector2(0.5f, 0.9f);
            var size = leg ? new Vector2(78f, 108f) : new Vector2(58f, 124f);
            var image = Art(name, parent, leg ? PeanutArt.Leg : PeanutArt.Arm, shoulder, size, pivot);
            image.rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
            if (flip)
                image.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
            return image.rectTransform;
        }

        static float SignedAngle(Transform target)
        {
            if (target == null)
                return 0f;
            float angle = target.localEulerAngles.z;
            if (angle > 180f)
                angle -= 360f;
            return angle;
        }
    }
}
