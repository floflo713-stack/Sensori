using UnityEngine;

namespace Sensori.Montessori
{
    public sealed class WoodenAudio : MonoBehaviour
    {
        public static WoodenAudio Instance { get; private set; }

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
            _source.playOnAwake = false;
            _source.spatialBlend = 0f;
            _source.volume = 0.85f;
            _voice = gameObject.AddComponent<AudioSource>();
            _voice.playOnAwake = false;
            _voice.spatialBlend = 0f;
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

        public void Say(AudioClip clip)
        {
            if (_voice == null || clip == null)
                return;
            _voice.Stop();
            _voice.clip = clip;
            _voice.volume = 1f;
            _voice.Play();
        }

        void Play(AudioClip clip, float volume)
        {
            if (_source == null || clip == null)
                return;
            _source.PlayOneShot(clip, volume);
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
