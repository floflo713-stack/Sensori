using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public enum Ease
    {
        Linear,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InSine,
        OutSine,
        InOutSine,
        OutBack,
        InOutBack,
        OutElastic,
        OutBounce
    }

    public enum LoopKind
    {
        Restart,
        Yoyo
    }

    public static class EaseCurve
    {
        public static float Evaluate(Ease ease, float t)
        {
            t = Mathf.Clamp01(t);
            switch (ease)
            {
                case Ease.InQuad:
                    return t * t;
                case Ease.OutQuad:
                    return 1f - (1f - t) * (1f - t);
                case Ease.InOutQuad:
                    return t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) * 0.5f;
                case Ease.InCubic:
                    return t * t * t;
                case Ease.OutCubic:
                    return 1f - Mathf.Pow(1f - t, 3f);
                case Ease.InOutCubic:
                    return t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) * 0.5f;
                case Ease.InSine:
                    return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                case Ease.OutSine:
                    return Mathf.Sin(t * Mathf.PI * 0.5f);
                case Ease.InOutSine:
                    return -(Mathf.Cos(Mathf.PI * t) - 1f) * 0.5f;
                case Ease.OutBack:
                    return OutBack(t);
                case Ease.InOutBack:
                    return InOutBack(t);
                case Ease.OutElastic:
                    return OutElastic(t);
                case Ease.OutBounce:
                    return OutBounce(t);
                default:
                    return t;
            }
        }

        static float OutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
        }

        static float InOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c2 = c1 * 1.525f;
            return t < 0.5f
                ? (Mathf.Pow(2f * t, 2f) * ((c2 + 1f) * 2f * t - c2)) * 0.5f
                : (Mathf.Pow(2f * t - 2f, 2f) * ((c2 + 1f) * (t * 2f - 2f) + c2) + 2f) * 0.5f;
        }

        static float OutElastic(float t)
        {
            if (t <= 0f)
                return 0f;
            if (t >= 1f)
                return 1f;
            const float c4 = (2f * Mathf.PI) / 3f;
            return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
        }

        static float OutBounce(float t)
        {
            const float n1 = 7.5625f;
            const float d1 = 2.75f;
            if (t < 1f / d1)
                return n1 * t * t;
            if (t < 2f / d1)
                return n1 * (t -= 1.5f / d1) * t + 0.75f;
            if (t < 2.5f / d1)
                return n1 * (t -= 2.25f / d1) * t + 0.9375f;
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
        }
    }

    public sealed class TweenHandle
    {
        internal TweenInstance Instance;

        public bool IsPlaying => Instance != null && Instance.Alive;

        public TweenHandle SetDelay(float delay)
        {
            if (Instance != null)
                Instance.Delay = Mathf.Max(0f, delay);
            return this;
        }

        public TweenHandle OnComplete(Action callback)
        {
            if (callback == null)
                return this;
            if (Instance == null)
            {
                callback();
                return this;
            }
            Instance.Completed += callback;
            return this;
        }

        public TweenHandle SetLoops(int count, LoopKind kind)
        {
            if (Instance == null)
                return this;
            Instance.Loops = count;
            Instance.LoopKind = kind;
            return this;
        }

        public void Kill()
        {
            if (Instance != null)
                Instance.Alive = false;
        }
    }

    sealed class TweenInstance
    {
        public bool Alive;
        public float Delay;
        public float Duration;
        public float Elapsed;
        public Ease Ease;
        public Action<float> Apply;
        public Action Completed;
        public int Loops = 1;
        public LoopKind LoopKind;
        public int LoopsDone;
        public bool YoyoReverse;
        public object Owner;
        public string Channel;
    }

    public sealed class TweenHost : MonoBehaviour
    {
        public static TweenHost Instance { get; private set; }

        readonly List<TweenInstance> _items = new List<TweenInstance>(64);

        public static TweenHost Ensure()
        {
            if (Instance != null)
                return Instance;
            var found = FindAnyObjectByType<TweenHost>();
            if (found != null)
            {
                Instance = found;
                return found;
            }
            var go = new GameObject("TweenHost");
            return go.AddComponent<TweenHost>();
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
            _items.Clear();
        }

        internal TweenHandle Play(TweenInstance tween)
        {
            if (tween.Owner != null)
                Kill(tween.Owner, tween.Channel);
            _items.Add(tween);
            return new TweenHandle { Instance = tween };
        }

        public void Kill(object owner, string channel)
        {
            if (owner == null)
                return;
            for (int i = 0; i < _items.Count; i++)
            {
                var tween = _items[i];
                if (!tween.Alive || tween.Owner != owner)
                    continue;
                if (channel == null || tween.Channel == channel)
                    tween.Alive = false;
            }
        }

        public void KillOwner(object owner)
        {
            Kill(owner, null);
        }

        void Update()
        {
            float dt = Mathf.Min(Time.unscaledDeltaTime, 0.05f);
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                var tween = _items[i];
                if (!tween.Alive)
                {
                    _items.RemoveAt(i);
                    continue;
                }

                if (tween.Delay > 0f)
                {
                    tween.Delay -= dt;
                    if (tween.Delay > 0f)
                        continue;
                }

                tween.Elapsed += dt;
                float u = tween.Duration <= 0.0001f ? 1f : Mathf.Clamp01(tween.Elapsed / tween.Duration);
                float sample = tween.YoyoReverse ? 1f - u : u;
                float eased = EaseCurve.Evaluate(tween.Ease, sample);
                if (tween.Apply != null)
                    tween.Apply(eased);

                if (u < 1f)
                    continue;

                tween.LoopsDone++;
                bool infinite = tween.Loops < 0;
                if (!infinite && tween.LoopsDone >= Mathf.Max(1, tween.Loops))
                {
                    tween.Alive = false;
                    var completed = tween.Completed;
                    tween.Completed = null;
                    _items.RemoveAt(i);
                    completed?.Invoke();
                    continue;
                }

                tween.Elapsed = 0f;
                if (tween.LoopKind == LoopKind.Yoyo)
                    tween.YoyoReverse = !tween.YoyoReverse;
            }
        }
    }

    public static class Motion
    {
        public static TweenHandle Float(float from, float to, float duration, Action<float> setter, Ease ease, object owner, string channel)
        {
            if (!Application.isPlaying)
            {
                setter?.Invoke(to);
                return new TweenHandle();
            }

            var tween = new TweenInstance
            {
                Alive = true,
                Duration = Mathf.Max(0.0001f, duration),
                Ease = ease,
                Loops = 1,
                Owner = owner,
                Channel = channel,
                Apply = value => setter?.Invoke(Mathf.LerpUnclamped(from, to, value))
            };
            return TweenHost.Ensure().Play(tween);
        }

        public static TweenHandle Anchored(RectTransform rect, Vector2 to, float duration, Ease ease = Ease.OutCubic)
        {
            if (rect == null)
                return new TweenHandle();
            Vector2 from = rect.anchoredPosition;
            return Float(0f, 1f, duration, u =>
            {
                if (rect != null)
                    rect.anchoredPosition = Vector2.LerpUnclamped(from, to, u);
            }, ease, rect, "pos");
        }

        public static TweenHandle Scale(Transform target, Vector3 to, float duration, Ease ease = Ease.OutCubic)
        {
            if (target == null)
                return new TweenHandle();
            Vector3 from = target.localScale;
            return Float(0f, 1f, duration, u =>
            {
                if (target != null)
                    target.localScale = Vector3.LerpUnclamped(from, to, u);
            }, ease, target, "scale");
        }

        public static TweenHandle Fade(CanvasGroup group, float to, float duration, Ease ease = Ease.OutQuad)
        {
            if (group == null)
                return new TweenHandle();
            float from = group.alpha;
            return Float(from, to, duration, value =>
            {
                if (group != null)
                    group.alpha = value;
            }, ease, group, "fade");
        }

        public static TweenHandle GraphicAlpha(Graphic graphic, float to, float duration, Ease ease = Ease.OutQuad)
        {
            if (graphic == null)
                return new TweenHandle();
            Color color = graphic.color;
            float from = color.a;
            return Float(from, to, duration, value =>
            {
                if (graphic == null)
                    return;
                var next = graphic.color;
                next.a = value;
                graphic.color = next;
            }, ease, graphic, "alpha");
        }

        public static TweenHandle PunchScale(Transform target, float amount, float duration)
        {
            if (target == null)
                return new TweenHandle();
            Vector3 baseScale = target.localScale;
            return Float(0f, 1f, duration, u =>
            {
                if (target == null)
                    return;
                float wave = Mathf.Sin(u * Mathf.PI) * amount;
                target.localScale = baseScale * (1f + wave);
            }, Ease.Linear, target, "scale");
        }

        public static TweenHandle Shake(RectTransform rect, float magnitude, float duration)
        {
            if (rect == null)
                return new TweenHandle();
            Vector2 origin = rect.anchoredPosition;
            return Float(0f, 1f, duration, u =>
            {
                if (rect == null)
                    return;
                float damper = 1f - u;
                float x = Mathf.Sin(u * 42f) * magnitude * damper;
                rect.anchoredPosition = origin + new Vector2(x, 0f);
            }, Ease.Linear, rect, "pos").OnComplete(() =>
            {
                if (rect != null)
                    rect.anchoredPosition = origin;
            });
        }

        public static TweenHandle Delayed(float delay, Action action)
        {
            if (!Application.isPlaying)
            {
                action?.Invoke();
                return new TweenHandle();
            }
            return Float(0f, 1f, 0.01f, _ => { }, Ease.Linear, null, null)
                .SetDelay(delay)
                .OnComplete(action);
        }

        public static void Kill(object owner, string channel)
        {
            if (TweenHost.Instance != null)
                TweenHost.Instance.Kill(owner, channel);
        }

        public static void KillOwner(object owner)
        {
            if (TweenHost.Instance != null)
                TweenHost.Instance.KillOwner(owner);
        }
    }
}
