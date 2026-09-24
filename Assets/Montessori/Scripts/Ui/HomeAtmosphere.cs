using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public enum TokenShape
    {
        Disc,
        Triangle
    }

    public sealed class SoftShape : MaskableGraphic
    {
        [SerializeField] TokenShape _shape = TokenShape.Disc;
        bool _meshed;

        public void Configure(TokenShape shape, Color tint)
        {
            _shape = shape;
            color = tint;
            raycastTarget = false;
            SetVerticesDirty();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            raycastTarget = false;
            SetVerticesDirty();
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            SetVerticesDirty();
        }

        void LateUpdate()
        {
            if (_meshed || !Application.isPlaying || rectTransform.rect.width < 2f)
                return;
            _meshed = true;
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect rect = GetPixelAdjustedRect();
            if (_shape == TokenShape.Triangle)
                FillTriangle(vh, rect);
            else
                FillDisc(vh, rect, 32);
        }

        void FillDisc(VertexHelper vh, Rect rect, int segments)
        {
            Vector2 center = rect.center;
            float radius = Mathf.Min(rect.width, rect.height) * 0.46f;
            vh.AddVert(center, color, new Vector2(0.5f, 0.5f));
            float step = Mathf.PI * 2f / segments;
            for (int i = 0; i <= segments; i++)
            {
                float angle = step * i;
                var point = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                float u = point.x / Mathf.Max(1f, rect.width) + 0.5f;
                float v = point.y / Mathf.Max(1f, rect.height) + 0.5f;
                vh.AddVert(point, color, new Vector2(u, v));
            }
            for (int i = 1; i <= segments; i++)
                vh.AddTriangle(0, i, i + 1);
        }

        void FillTriangle(VertexHelper vh, Rect rect)
        {
            Vector2 center = rect.center;
            float halfW = rect.width * 0.42f;
            float halfH = rect.height * 0.44f;
            var top = new Vector2(center.x, center.y + halfH);
            var left = new Vector2(center.x - halfW, center.y - halfH * 0.78f);
            var right = new Vector2(center.x + halfW, center.y - halfH * 0.78f);
            vh.AddVert(top, color, new Vector2(0.5f, 1f));
            vh.AddVert(left, color, new Vector2(0f, 0f));
            vh.AddVert(right, color, new Vector2(1f, 0f));
            vh.AddTriangle(0, 1, 2);
        }
    }

    public sealed class FloatingToken : MonoBehaviour
    {
        const float ArriveDuration = 0.68f;

        RectTransform _rect;
        RectTransform _mark;
        CanvasGroup _group;
        Vector2 _rest;
        Vector2 _from;
        Vector2 _markRest;
        float _phase;
        float _bobSpeed;
        float _fromAngle;
        float _clock;
        float _delay;
        float _settledAt;
        bool _arrived = true;

        public void Boot(float phase, float amplitude, float bobSpeed, float tiltDegrees, float tiltSpeed)
        {
            _rect = (RectTransform)transform;
            _rest = _rect.anchoredPosition;
            _phase = phase;
            _bobSpeed = bobSpeed;
            _mark = FindMark();
            if (_mark != null)
                _markRest = _mark.anchoredPosition;
            _group = GetComponent<CanvasGroup>();
            if (_group == null)
                _group = gameObject.AddComponent<CanvasGroup>();
            _group.interactable = false;
            _group.blocksRaycasts = false;
            if (Application.isPlaying)
                Hide();
        }

        public void PlayArrival(float delay)
        {
            if (_rect == null)
                _rect = (RectTransform)transform;
            if (_mark == null)
                _mark = FindMark();
            if (_group == null)
            {
                _group = GetComponent<CanvasGroup>();
                if (_group == null)
                    _group = gameObject.AddComponent<CanvasGroup>();
                _group.interactable = false;
                _group.blocksRaycasts = false;
            }

            _delay = Mathf.Max(0f, delay);
            _clock = 0f;
            _arrived = false;
            _fromAngle = Mathf.Sin(_phase * 1.7f) * 24f;
            Vector2 away = _rest.sqrMagnitude > 4f ? _rest.normalized : Vector2.up;
            _from = _rest + away * 86f;
            Hide();
        }

        void Update()
        {
            if (!Application.isPlaying || _rect == null)
                return;
            _clock += Time.unscaledDeltaTime;
            if (!_arrived)
            {
                AdvanceArrival();
                return;
            }
            IdleMark();
        }

        void AdvanceArrival()
        {
            if (_clock < _delay)
            {
                Hide();
                return;
            }

            float u = Mathf.Clamp01((_clock - _delay) / ArriveDuration);
            float ease = EaseCurve.Evaluate(Ease.OutBack, u);
            float fade = EaseCurve.Evaluate(Ease.OutQuad, Mathf.Clamp01(u / 0.4f));
            _rect.anchoredPosition = Vector2.LerpUnclamped(_from, _rest, ease);
            _rect.localScale = Vector3.one * Mathf.LerpUnclamped(0.42f, 1f, ease);
            _rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.LerpUnclamped(_fromAngle, 0f, ease));
            _group.alpha = fade;
            if (_mark != null)
            {
                float glyph = Mathf.Clamp01((u - 0.34f) / 0.66f);
                float pop = EaseCurve.Evaluate(Ease.OutBack, glyph);
                _mark.localScale = Vector3.one * Mathf.LerpUnclamped(0.15f, 1f, pop);
                _mark.localRotation = Quaternion.Euler(0f, 0f, Mathf.LerpUnclamped(-_fromAngle * 0.55f, 0f, pop));
            }
            if (u < 1f)
                return;

            _arrived = true;
            _settledAt = Time.unscaledTime;
            _rect.anchoredPosition = _rest;
            _rect.localScale = Vector3.one;
            _rect.localRotation = Quaternion.identity;
            _group.alpha = 1f;
            if (_mark != null)
            {
                _mark.localScale = Vector3.one;
                _mark.localRotation = Quaternion.identity;
                _mark.anchoredPosition = _markRest;
            }
        }

        void IdleMark()
        {
            if (_mark == null || _rect == null)
                return;
            float calm = Mathf.Clamp01((Time.unscaledTime - _settledAt) / 1.7f);
            float near = NearPointer();
            float breath = Mathf.Lerp(0.09f, 0.045f, calm) + near * 0.05f;
            float tilt = Mathf.Lerp(7f, 2.8f, calm) + near * 4f;
            float lift = Mathf.Lerp(7f, 3.2f, calm) + near * 6f;
            float time = Time.unscaledTime;
            float wave = Mathf.Sin(time * _bobSpeed + _phase);
            float scale = 1f + ((wave + 1f) * 0.5f) * breath;
            _mark.localScale = new Vector3(scale, scale, 1f);
            _mark.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(time * _bobSpeed * 0.82f + _phase) * tilt);
            var markPos = _markRest;
            markPos.y += Mathf.Sin(time * _bobSpeed + _phase * 0.6f) * lift;
            _mark.anchoredPosition = markPos;
            var drift = _rest;
            drift.y += Mathf.Sin(time * _bobSpeed * 0.65f + _phase) * (4f + near * 5f);
            drift.x += Mathf.Cos(time * _bobSpeed * 0.45f + _phase) * (3f + near * 4f);
            _rect.anchoredPosition = drift;
            _rect.localScale = Vector3.one * (1f + near * 0.06f);
            _rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(time * 0.7f + _phase) * (1.6f + near * 3f));
        }

        float NearPointer()
        {
            var pointer = UnityEngine.InputSystem.Pointer.current;
            if (pointer == null || _rect == null)
                return 0f;
            var canvas = GetComponentInParent<Canvas>();
            Camera cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(_rect, pointer.position.ReadValue(), cam, out var local))
                return 0f;
            float dist = local.magnitude;
            return Mathf.Clamp01(1f - dist / 150f);
        }

        void Hide()
        {
            _rect.anchoredPosition = _from.sqrMagnitude > 0.01f ? _from : _rest;
            _rect.localScale = Vector3.one * 0.35f;
            _rect.localRotation = Quaternion.Euler(0f, 0f, _fromAngle);
            if (_group != null)
                _group.alpha = 0f;
            if (_mark != null)
                _mark.localScale = Vector3.zero;
        }

        RectTransform FindMark()
        {
            var glyph = transform.Find("Jeton/Glyph") as RectTransform;
            if (glyph != null)
                return glyph;
            return transform.Find("Jeton/Forme") as RectTransform;
        }
    }

    public sealed class WelcomeLetters : MonoBehaviour
    {
        struct Letter
        {
            public RectTransform Rect;
            public Vector2 Rest;
        }

        Letter[] _letters;
        float _clock;
        float _glow;
        bool _playing;
        bool _glowing;
        bool _restKnown;

        public void Play()
        {
            Collect();
            if (_letters == null || _letters.Length == 0)
                return;
            _playing = true;
            _glowing = false;
            _clock = 0f;
            _glow = 0f;
            for (int i = 0; i < _letters.Length; i++)
            {
                if (_letters[i].Rect == null)
                    continue;
                _letters[i].Rect.localScale = Vector3.zero;
                _letters[i].Rect.anchoredPosition = _letters[i].Rest + new Vector2(0f, -26f);
            }
        }

        void Update()
        {
            if (!Application.isPlaying)
                return;
            if (_playing)
                Advance();
            else if (_glowing)
                Glow();
            else if (_letters != null)
                IdleLetters();
        }

        void Advance()
        {
            _clock += Time.unscaledDeltaTime;
            bool done = true;
            for (int i = 0; i < _letters.Length; i++)
            {
                var letter = _letters[i];
                if (letter.Rect == null)
                    continue;
                float delay = 0.05f + i * 0.07f;
                if (_clock < delay)
                {
                    letter.Rect.localScale = Vector3.zero;
                    done = false;
                    continue;
                }
                float u = Mathf.Clamp01((_clock - delay) / 0.52f);
                float ease = EaseCurve.Evaluate(Ease.OutBack, u);
                letter.Rect.localScale = Vector3.one * Mathf.LerpUnclamped(0.4f, 1f, ease);
                letter.Rect.anchoredPosition = Vector2.LerpUnclamped(letter.Rest + new Vector2(0f, -26f), letter.Rest, ease);
                letter.Rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.LerpUnclamped((i % 2 == 0 ? -8f : 8f), 0f, ease));
                if (u < 1f)
                    done = false;
            }
            if (!done)
                return;
            _playing = false;
            _glowing = true;
            _glow = 0f;
        }

        void Glow()
        {
            _glow += Time.unscaledDeltaTime;
            float calm = Mathf.Clamp01(_glow / 1.15f);
            float amount = (1f - calm) * 0.045f;
            for (int i = 0; i < _letters.Length; i++)
            {
                var letter = _letters[i];
                if (letter.Rect == null)
                    continue;
                float wave = (Mathf.Sin(Time.unscaledTime * 3.2f + i * 0.55f) + 1f) * 0.5f;
                float scale = 1f + wave * amount;
                letter.Rect.localScale = new Vector3(scale, scale, 1f);
                letter.Rect.anchoredPosition = letter.Rest;
                letter.Rect.localRotation = Quaternion.identity;
            }
            if (calm < 1f)
                return;
            _glowing = false;
        }

        void IdleLetters()
        {
            if (_letters == null)
                return;
            float time = Time.unscaledTime;
            for (int i = 0; i < _letters.Length; i++)
            {
                var letter = _letters[i];
                if (letter.Rect == null)
                    continue;
                float wave = Mathf.Sin(time * 1.6f + i * 0.7f);
                float near = NearLetter(letter.Rect);
                float scale = 1f + wave * 0.012f + near * 0.06f;
                letter.Rect.localScale = new Vector3(scale, scale, 1f);
                var pos = letter.Rest;
                pos.y += wave * (1.5f + near * 4f);
                letter.Rect.anchoredPosition = pos;
                letter.Rect.localRotation = Quaternion.Euler(0f, 0f, wave * (0.8f + near * 3f));
            }
        }

        static float NearLetter(RectTransform rect)
        {
            var pointer = UnityEngine.InputSystem.Pointer.current;
            if (pointer == null || rect == null)
                return 0f;
            var canvas = rect.GetComponentInParent<Canvas>();
            Camera cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, pointer.position.ReadValue(), cam, out var local))
                return 0f;
            return Mathf.Clamp01(1f - local.magnitude / 80f);
        }

        void Collect()
        {
            if (_restKnown && _letters != null && _letters.Length > 0)
                return;
            int count = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name == "Lettre")
                    count++;
            }
            _letters = new Letter[count];
            int index = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name != "Lettre")
                    continue;
                var rect = (RectTransform)transform.GetChild(i);
                _letters[index] = new Letter
                {
                    Rect = rect,
                    Rest = rect.anchoredPosition
                };
                index++;
            }
            _restKnown = count > 0;
        }
    }

    public sealed class HomePlateMotion : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        const float BreathPeriod = 3.6f;
        const float PressScale = 0.95f;
        const float PressDrop = 8f;
        const float SinkDuration = 0.12f;
        const float HoldAfterSink = 0.16f;

        RectTransform _rect;
        Vector2 _rest;
        float _breathLock;
        float _sinkAt = -1f;
        bool _holding;
        bool _sunk;
        bool _rising;
        bool _departing;

        void Awake()
        {
            _rect = (RectTransform)transform;
            _rest = _rect.anchoredPosition;
        }

        void Update()
        {
            if (!Application.isPlaying || _sunk || _departing || _rising)
                return;
            if (Time.unscaledTime < _breathLock)
                return;
            float wave = (Mathf.Sin(Time.unscaledTime * (Mathf.PI * 2f / BreathPeriod)) + 1f) * 0.5f;
            float scale = Mathf.Lerp(1f, 1.03f, wave);
            transform.localScale = new Vector3(scale, scale, 1f);
        }

        void LateUpdate()
        {
            if (_sunk && !_holding && !_departing)
                Rise();
        }

        public void ResetForWelcome()
        {
            if (_rect == null)
                _rect = (RectTransform)transform;
            _departing = false;
            _sunk = false;
            _rising = false;
            _holding = false;
            _sinkAt = -1f;
            _breathLock = 0f;
            _rest = Vector2.zero;
            _rect.anchoredPosition = _rest;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
            Motion.Kill(transform, "scale");
            Motion.Kill(_rect, "pos");
        }

        public void HoldBreath(float seconds)
        {
            if (_departing)
                return;
            _breathLock = Time.unscaledTime + Mathf.Max(0f, seconds);
            if (!_sunk)
                transform.localScale = Vector3.one;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_departing || eventData.button != PointerEventData.InputButton.Left)
                return;
            _holding = true;
            Sink();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _holding = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_holding || _departing)
                return;
            _holding = false;
            Rise();
        }

        public void Depart(Action then)
        {
            if (_departing)
                return;
            _departing = true;
            _holding = false;
            if (!_sunk)
                Sink();
            float elapsed = _sinkAt < 0f ? 0f : Time.unscaledTime - _sinkAt;
            float wait = Mathf.Max(0.04f, HoldAfterSink - elapsed);
            Motion.Delayed(wait, () =>
            {
                WoodenAudio.PlayTap();
                then?.Invoke();
            });
        }

        void Sink()
        {
            _rising = false;
            _sunk = true;
            _sinkAt = Time.unscaledTime;
            if (_rect == null)
                _rect = (RectTransform)transform;
            Motion.Scale(transform, Vector3.one * PressScale, SinkDuration, Ease.OutQuad);
            Motion.Anchored(_rect, _rest + new Vector2(0f, -PressDrop), SinkDuration, Ease.OutQuad);
        }

        void Rise()
        {
            _sunk = false;
            _rising = true;
            _sinkAt = -1f;
            Motion.Scale(transform, Vector3.one, 0.16f, Ease.OutQuad).OnComplete(() => _rising = false);
            if (_rect != null)
                Motion.Anchored(_rect, _rest, 0.16f, Ease.OutQuad);
        }
    }
}
