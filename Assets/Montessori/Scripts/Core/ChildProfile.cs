using System.Globalization;
using System.Text;
using UnityEngine;

namespace Sensori.Montessori
{
    public static class ChildProfile
    {
        const string Key = "sensori.child.name";

        public static bool HasName
        {
            get { return !string.IsNullOrEmpty(Name); }
        }

        public static string Name
        {
            get { return PlayerPrefs.GetString(Key, string.Empty); }
        }

        public static bool TryAccept(string raw, out string cleaned)
        {
            cleaned = string.Empty;
            if (string.IsNullOrWhiteSpace(raw))
                return false;

            var builder = new StringBuilder(raw.Length);
            bool spaced = false;
            string trimmed = raw.Trim();
            for (int i = 0; i < trimmed.Length; i++)
            {
                char c = trimmed[i];
                if (c == '\u2019' || c == '\u2018')
                    c = '\'';
                if (char.IsLetter(c) || c == '-' || c == '\'')
                {
                    builder.Append(c);
                    spaced = false;
                    continue;
                }
                if (char.IsWhiteSpace(c))
                {
                    if (builder.Length > 0 && !spaced)
                    {
                        builder.Append(' ');
                        spaced = true;
                    }
                    continue;
                }
                return false;
            }

            cleaned = Prettify(builder.ToString().Trim());
            return cleaned.Length >= 1 && cleaned.Length <= 16;
        }

        public static void Save(string name)
        {
            PlayerPrefs.SetString(Key, name ?? string.Empty);
            PlayerPrefs.Save();
        }

        static string Prettify(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            var chars = value.ToCharArray();
            bool cap = true;
            var culture = CultureInfo.CurrentCulture;
            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];
                if (c == ' ' || c == '-' || c == '\'')
                {
                    cap = true;
                    continue;
                }
                if (cap && char.IsLower(c))
                    chars[i] = char.ToUpper(c, culture);
                cap = false;
            }
            return new string(chars);
        }
    }
}
