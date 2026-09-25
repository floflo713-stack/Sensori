using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Sensori.Montessori
{
    public static class FrenchVoice
    {
        static readonly Dictionary<string, AudioClip> Cache = new Dictionary<string, AudioClip>();
        static readonly HashSet<string> Loading = new HashSet<string>();

        public static void SayItem(LearningItem item)
        {
            if (item == null)
                return;
            string phrase = item.Kind == ItemKind.Letter ? item.Symbol : item.DisplayName;
            Say(phrase);
        }

        public static void SayWord(LearningItem item)
        {
            if (item == null || string.IsNullOrEmpty(item.AssociatedWord))
                return;
            if (string.Equals(item.AssociatedWord, item.DisplayName, StringComparison.OrdinalIgnoreCase))
                return;
            if (item.Kind == ItemKind.Letter && string.Equals(item.AssociatedWord, item.Symbol, StringComparison.OrdinalIgnoreCase))
                return;
            Say(item.AssociatedWord);
        }

        public static void Say(string phrase)
        {
            TrySay(phrase);
        }

        public static bool TrySay(string phrase)
        {
            if (string.IsNullOrEmpty(phrase) || WoodenAudio.Instance == null)
            {
                Debug.Log("[Voix] Lecture annulée. phrase=\"" + phrase + "\" WoodenAudio=" + (WoodenAudio.Instance != null));
                return false;
            }
            string key = Slug(phrase);
            if (string.IsNullOrEmpty(key))
            {
                Debug.Log("[Voix] Phrase sans fichier : \"" + phrase + "\"");
                return false;
            }
            string path = VoicePath(key);
            Debug.Log("[Voix] Recherche fichier \"" + key + ".wav\" chemin=" + path);
            if (Cache.TryGetValue(key, out var clip) && clip != null)
            {
                Debug.Log("[Voix] Clip en cache \"" + key + "\" durée=" + clip.length.ToString("0.00", CultureInfo.InvariantCulture) + "s");
                WoodenAudio.Instance.Say(clip);
                return true;
            }
            if (NeedsWebRequest(path))
            {
                if (!Loading.Add(key))
                {
                    Debug.Log("[Voix] Chargement déjà en cours pour \"" + key + ".wav\"");
                    return true;
                }
                WoodenAudio.Instance.StartCoroutine(LoadAndSay(key, path));
                return true;
            }
            if (!File.Exists(path))
            {
                Debug.Log("[Voix] Fichier introuvable : " + path);
                return false;
            }
            clip = FromWav(key, File.ReadAllBytes(path));
            return PlayLoaded(key, clip);
        }

        public static string VoicePath(string key)
        {
            return Application.streamingAssetsPath + "/Voix/" + key + ".wav";
        }

        static bool NeedsWebRequest(string path)
        {
            return Application.platform == RuntimePlatform.Android
                || path.IndexOf("://", StringComparison.Ordinal) >= 0
                || path.IndexOf("jar:", StringComparison.Ordinal) >= 0;
        }

        static IEnumerator LoadAndSay(string key, string path)
        {
            using (var request = UnityWebRequest.Get(path))
            {
                yield return request.SendWebRequest();
                Loading.Remove(key);
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("[Voix] Échec de lecture \"" + key + ".wav\" erreur=" + request.error + " chemin=" + path);
                    yield break;
                }
                byte[] bytes = request.downloadHandler.data;
                Debug.Log("[Voix] Octets reçus pour \"" + key + ".wav\" : " + (bytes != null ? bytes.Length : 0));
                PlayLoaded(key, FromWav(key, bytes));
            }
        }

        static bool PlayLoaded(string key, AudioClip clip)
        {
            if (clip == null)
            {
                Debug.Log("[Voix] Clip null pour \"" + key + ".wav\"");
                return false;
            }
            Cache[key] = clip;
            Debug.Log("[Voix] Clip trouvé \"" + clip.name + "\" durée=" + clip.length.ToString("0.00", CultureInfo.InvariantCulture) + "s");
            if (WoodenAudio.Instance == null)
                return false;
            WoodenAudio.Instance.Say(clip);
            return true;
        }

        static AudioClip FromWav(string clipName, byte[] wav)
        {
            if (wav == null || wav.Length < 44)
                return null;
            int channels = 1;
            int rate = 22050;
            int bits = 16;
            int data = -1;
            int dataSize = 0;
            int cursor = 12;
            while (cursor + 8 <= wav.Length)
            {
                string id = Encoding.ASCII.GetString(wav, cursor, 4);
                int size = BitConverter.ToInt32(wav, cursor + 4);
                int body = cursor + 8;
                if (id == "fmt " && body + 16 <= wav.Length)
                {
                    channels = BitConverter.ToInt16(wav, body + 2);
                    rate = BitConverter.ToInt32(wav, body + 4);
                    bits = BitConverter.ToInt16(wav, body + 14);
                }
                else if (id == "data")
                {
                    data = body;
                    dataSize = size;
                    break;
                }
                cursor = body + size;
            }
            if (data < 0 || bits != 16 || channels < 1)
                return null;
            int sampleCount = dataSize / 2 / channels;
            var samples = new float[sampleCount * channels];
            int offset = data;
            for (int i = 0; i < samples.Length; i++)
            {
                if (offset + 1 >= wav.Length)
                    break;
                short value = BitConverter.ToInt16(wav, offset);
                samples[i] = value / 32768f;
                offset += 2;
            }
            var clip = AudioClip.Create(clipName, sampleCount, channels, rate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        public static string Slug(string phrase)
        {
            if (string.IsNullOrEmpty(phrase))
                return string.Empty;
            string form = phrase.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(form.Length);
            for (int i = 0; i < form.Length; i++)
            {
                char c = form[i];
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                if (category == UnicodeCategory.NonSpacingMark)
                    continue;
                if (c == 'œ')
                {
                    builder.Append("oe");
                    continue;
                }
                if (c == 'æ')
                {
                    builder.Append("ae");
                    continue;
                }
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                    builder.Append(c);
                else if (c == ' ' || c == '-' || c == '\'')
                    builder.Append('-');
            }
            return builder.ToString().Trim('-');
        }
    }
}
