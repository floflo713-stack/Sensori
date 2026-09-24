using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace Sensori.Montessori
{
    public static class FrenchVoice
    {
        static readonly Dictionary<string, AudioClip> Cache = new Dictionary<string, AudioClip>();

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
            if (string.IsNullOrEmpty(phrase) || WoodenAudio.Instance == null)
                return;
            string key = Slug(phrase);
            if (string.IsNullOrEmpty(key))
                return;
            if (!Cache.TryGetValue(key, out var clip) || clip == null)
            {
                clip = Load(key);
                if (clip == null)
                    return;
                Cache[key] = clip;
            }
            WoodenAudio.Instance.Say(clip);
        }

        static AudioClip Load(string key)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Voix", key + ".wav");
            if (!File.Exists(path))
                return null;
            byte[] bytes = File.ReadAllBytes(path);
            return FromWav(key, bytes);
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
