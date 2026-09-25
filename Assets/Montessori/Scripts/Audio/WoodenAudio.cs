using System.Collections.Generic;
using UnityEngine;

namespace Sensori.Montessori
{
    public sealed class WoodenAudio : MonoBehaviour
    {
        public static WoodenAudio Instance { get; private set; }

        readonly Dictionary<string, AudioClip> _tones = new Dictionary<string, AudioClip>();
        AudioSource _source;
        AudioSource _voice;
        AudioClip _tap;
        AudioClip _tock;
        AudioClip _bellC;
        AudioClip _bellE;
        AudioClip _bellG;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            _source = gameObject.GetComponent<AudioSource>();
            if (_source == null)
                _source = gameObject.AddComponent<AudioSource>();
            Prepare(_source, 0.85f);
            _voice = gameObject.AddComponent<AudioSource>();
            Prepare(_voice, 1f);
            _tap = CreateBell("tap", 210f, 0.09f, 0.22f, 0.4f);
            _tock = CreateBell("tock", 150f, 0.12f, 0.18f, 0.2f);
            _bellC = CreateBell("bell-c", 523.25f, 0.55f, 0.22f, 0.15f);
            _bellE = CreateBell("bell-e", 659.25f, 0.6f, 0.2f, 0.12f);
            _bellG = CreateBell("bell-g", 783.99f, 0.7f, 0.18f, 0.1f);
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public static void PlayTap()
        {
            if (Instance != null)
                Instance.Play(Instance._tap, 0.55f);
        }

        public static void PlayTock()
        {
            if (Instance != null)
                Instance.Play(Instance._tock, 0.4f);
        }

        public static void PlaySuccess()
        {
            if (Instance == null)
                return;
            Instance.Play(Instance._bellC, 0.7f);
            Motion.Delayed(0.09f, () =>
            {
                if (Instance != null)
                    Instance.Play(Instance._bellE, 0.65f);
            });
            Motion.Delayed(0.18f, () =>
            {
                if (Instance != null)
                    Instance.Play(Instance._bellG, 0.6f);
            });
        }

        static void Prepare(AudioSource source, float volume)
        {
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.mute = false;
            source.volume = volume;
            source.pitch = 1f;
            source.loop = false;
        }

        public void Say(AudioClip clip)
        {
            if (_voice == null || clip == null)
            {
                Debug.Log("[Voix] AudioSource voix indisponible ou clip null. source=" + (_voice != null) + " clip=" + (clip != null ? clip.name : "null"));
                return;
            }
            _voice.Stop();
            _voice.clip = clip;
            _voice.mute = false;
            _voice.volume = 1f;
            _voice.pitch = 1f;
            _voice.Play();
            Debug.Log("[Voix] AudioSource voix clip=" + clip.name
                + " isPlaying=" + _voice.isPlaying
                + " volume=" + _voice.volume
                + " mute=" + _voice.mute
                + " pitch=" + _voice.pitch);
        }

        public void SpeakWord(AudioClip clip, string phrase)
        {
            if (clip != null)
            {
                Say(clip);
                return;
            }
            if (FrenchVoice.TrySay(phrase))
                return;
            Debug.Log("[Imagier Parlant] Pas de voix pour « " + phrase + " ». Tonalité douce de secours.");
            Play(ToneFor(phrase, false), 0.42f);
        }

        public void PlayEffect(AudioClip clip, string phrase)
        {
            if (clip != null)
            {
                Play(clip, 0.85f);
                return;
            }
            Debug.Log("[Imagier Parlant] Pas d'effet pour « " + phrase + " ». Son synthétique doux.");
            Play(ToneFor(string.IsNullOrEmpty(phrase) ? "effet" : phrase, true), 0.38f);
        }

        AudioClip ToneFor(string phrase, bool effect)
        {
            string key = (effect ? "fx:" : "voix:") + (phrase ?? "mot");
            if (_tones.TryGetValue(key, out var cached) && cached != null)
                return cached;
            int hash = key.GetHashCode();
            float root = 392f + (Mathf.Abs(hash) % 7) * 32f;
            float second = effect ? root * 1.5f : root * 1.25f;
            var clip = CreatePhrase(key, root, second, effect ? 0.34f : 0.55f);
            _tones[key] = clip;
            return clip;
        }

        static AudioClip CreatePhrase(string clipName, float first, float second, float duration)
        {
            const int sampleRate = 44100;
            int samples = Mathf.Max(8, Mathf.RoundToInt(sampleRate * duration));
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float time = i / (float)sampleRate;
                float local = time < duration * 0.48f ? time : time - duration * 0.52f;
                float frequency = time < duration * 0.48f ? first : second;
                float envelope = Mathf.Exp(-4.2f * local) * Mathf.Clamp01(local / 0.01f);
                data[i] = Mathf.Sin(2f * Mathf.PI * frequency * time) * envelope * 0.22f;
            }
            var clip = AudioClip.Create(clipName, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        void Play(AudioClip clip, float volume)
        {
            if (_source == null || clip == null)
            {
                Debug.Log("[Voix] AudioSource effets indisponible ou clip null. source=" + (_source != null) + " clip=" + (clip != null ? clip.name : "null"));
                return;
            }
            _source.mute = false;
            _source.pitch = 1f;
            _source.PlayOneShot(clip, volume);
            Debug.Log("[Voix] AudioSource effets clip=" + clip.name
                + " isPlaying=" + _source.isPlaying
                + " volume=" + _source.volume
                + " mute=" + _source.mute
                + " pitch=" + _source.pitch
                + " oneShot=" + volume);
        }

        static AudioClip CreateBell(string clipName, float frequency, float duration, float volume, float brightness)
        {
            const int sampleRate = 44100;
            int samples = Mathf.Max(8, Mathf.RoundToInt(sampleRate * duration));
            var data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float time = i / (float)sampleRate;
                float envelope = Mathf.Exp(-3.4f * time);
                float attack = Mathf.Clamp01(time / 0.008f);
                float fundamental = Mathf.Sin(2f * Mathf.PI * frequency * time);
                float overtone = Mathf.Sin(2f * Mathf.PI * frequency * 2.01f * time) * brightness * Mathf.Exp(-6f * time);
                data[i] = (fundamental + overtone) * envelope * attack * volume;
            }
            var clip = AudioClip.Create(clipName, samples, 1, sampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
