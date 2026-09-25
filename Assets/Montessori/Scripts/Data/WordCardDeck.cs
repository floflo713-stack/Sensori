using System.Collections.Generic;
using UnityEngine;

namespace Sensori.Montessori
{
    public static class WordThemes
    {
        public const string CategoryId = "imagier-parlant";

        public static WordTheme[] All()
        {
            return new[]
            {
                WordTheme.Vehicules,
                WordTheme.Animaux,
                WordTheme.ActionsQuotidien,
                WordTheme.Nourriture,
                WordTheme.Emotions,
                WordTheme.Saisons
            };
        }

        public static string Title(WordTheme theme)
        {
            switch (theme)
            {
                case WordTheme.Vehicules: return "Véhicules";
                case WordTheme.Animaux: return "Animaux";
                case WordTheme.ActionsQuotidien: return "Actions";
                case WordTheme.Nourriture: return "Nourriture";
                case WordTheme.Emotions: return "Émotions";
                case WordTheme.Saisons: return "Saisons";
                default: return "Mots";
            }
        }

        public static string Subtitle(WordTheme theme)
        {
            switch (theme)
            {
                case WordTheme.Vehicules: return "Ce qui roule et qui vole";
                case WordTheme.Animaux: return "Les petits cris";
                case WordTheme.ActionsQuotidien: return "Ce que l'on fait";
                case WordTheme.Nourriture: return "Ce que l'on croque";
                case WordTheme.Emotions: return "Ce que l'on ressent";
                case WordTheme.Saisons: return "Le temps qui passe";
                default: return string.Empty;
            }
        }

        public static Color Accent(WordTheme theme)
        {
            switch (theme)
            {
                case WordTheme.Vehicules: return MontessoriPalette.Sky;
                case WordTheme.Animaux: return MontessoriPalette.Honey;
                case WordTheme.ActionsQuotidien: return MontessoriPalette.Moss;
                case WordTheme.Nourriture: return MontessoriPalette.Coral;
                case WordTheme.Emotions: return MontessoriPalette.ConsonantRose;
                case WordTheme.Saisons: return MontessoriPalette.Sun;
                default: return MontessoriPalette.Honey;
            }
        }
    }

    [CreateAssetMenu(fileName = "PaquetMots", menuName = "Montessori/Paquet de cartes")]
    public sealed class WordCardDeck : ScriptableObject
    {
        [SerializeField] WordCardData[] _cards = new WordCardData[0];

        public WordCardData[] Cards => _cards ?? new WordCardData[0];

        public int CardCount
        {
            get
            {
                var cards = Cards;
                int count = 0;
                for (int i = 0; i < cards.Length; i++)
                {
                    if (cards[i] != null)
                        count++;
                }
                return count;
            }
        }

        public void Define(WordCardData[] cards)
        {
            _cards = cards ?? new WordCardData[0];
        }

        public WordCardData[] Of(WordTheme theme)
        {
            var cards = Cards;
            var found = new List<WordCardData>();
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i] != null && cards[i].Theme == theme)
                    found.Add(cards[i]);
            }
            return found.ToArray();
        }

        public int Count(WordTheme theme)
        {
            return Of(theme).Length;
        }

        public bool Has(WordTheme theme)
        {
            return Count(theme) > 0;
        }

        public int Next(WordTheme theme, int index)
        {
            int count = Count(theme);
            if (count == 0)
                return 0;
            return (index + 1) % count;
        }

        public int Previous(WordTheme theme, int index)
        {
            int count = Count(theme);
            if (count == 0)
                return 0;
            return (index - 1 + count) % count;
        }

        public WordCardData At(WordTheme theme, int index)
        {
            var cards = Of(theme);
            if (cards.Length == 0)
                return null;
            int safe = ((index % cards.Length) + cards.Length) % cards.Length;
            return cards[safe];
        }

        public WordCardData Draw(WordTheme theme)
        {
            var cards = Of(theme);
            if (cards.Length == 0)
                return null;
            return cards[Random.Range(0, cards.Length)];
        }
    }
}
