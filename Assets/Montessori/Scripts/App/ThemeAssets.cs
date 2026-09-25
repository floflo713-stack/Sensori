using UnityEngine;

namespace Sensori.Montessori
{
    public sealed class ThemeAssets
    {
        public Sprite Panel;
        public Sprite Inset;
        public Sprite Piece;
        public Sprite Shadow;
        public Sprite Pearl;
        public Sprite Chip;
        public Sprite Solid;
        public Sprite Background;
        public Sprite IconAlphabet;
        public Sprite IconDigits;
        public Sprite IconShapes;
        public Sprite IconColors;
        public Sprite IconParlant;
        public Sprite IconPuzzle;
        public Sprite IconImagier;
        public Sprite IconTrace;

        public Sprite IconForCategory(string categoryId)
        {
            switch (categoryId)
            {
                case "alphabet": return IconAlphabet;
                case "chiffres": return IconDigits;
                case "formes": return IconShapes;
                case "couleurs": return IconColors;
                case "imagier-parlant": return IconParlant != null ? IconParlant : IconImagier;
                default: return Pearl;
            }
        }

        public Sprite IconForGame(string gameId)
        {
            if (gameId == GameIds.Puzzle) return IconPuzzle;
            if (gameId == GameIds.Imagier) return IconImagier;
            if (gameId == GameIds.Tracing) return IconTrace;
            return Pearl;
        }
    }
}
