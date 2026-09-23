using UnityEngine;

namespace Sensori.Montessori
{
    public static class MontessoriPalette
    {
        public static readonly Color Paper = Hex("#F6F0E6");
        public static readonly Color PaperDeep = Hex("#E6D5C3");
        public static readonly Color Cream = Hex("#FFF9F2");
        public static readonly Color Ink = Hex("#3C2E24");
        public static readonly Color InkSoft = Hex("#7A6556");
        public static readonly Color VowelBlue = Hex("#1F6FBF");
        public static readonly Color ConsonantRose = Hex("#E24B6A");
        public static readonly Color Maple = Hex("#E2C09A");
        public static readonly Color Honey = Hex("#C4925C");
        public static readonly Color Walnut = Hex("#6B4630");
        public static readonly Color WalnutDeep = Hex("#3E2918");
        public static readonly Color Moss = Hex("#6E9A72");
        public static readonly Color Sky = Hex("#6A8FBF");
        public static readonly Color Success = Hex("#3E8F5A");
        public static readonly Color Sun = Hex("#E2B043");
        public static readonly Color Coral = Hex("#E07A5F");

        public static Color Hex(string html)
        {
            if (string.IsNullOrEmpty(html))
                return Color.white;
            if (html[0] != '#')
                html = "#" + html;
            return ColorUtility.TryParseHtmlString(html, out var color) ? color : Color.white;
        }

        public static Color WithAlpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }
    }
}
