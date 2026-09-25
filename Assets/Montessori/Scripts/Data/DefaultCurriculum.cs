using UnityEngine;

namespace Sensori.Montessori
{
    public sealed class ItemSeed
    {
        public string Id;
        public string Symbol;
        public string DisplayName;
        public string Word;
        public ItemKind Kind;
        public PhoneticRole Role;
        public ItemVisual Visual;
        public Color Accent;
        public string PictogramId;
        public string StrokeKey;
    }

    public sealed class CategorySeed
    {
        public string Id;
        public string Title;
        public string Subtitle;
        public string CountLabel;
        public Color Accent;
        public int PuzzleGroupSize;
        public ItemSeed[] Items;
    }

    public sealed class MiniGameSeed
    {
        public string Id;
        public string Title;
        public string Description;
        public Color Accent;
    }

    public static class DefaultCurriculum
    {
        public static MiniGameSeed[] Games()
        {
            return new[]
            {
                new MiniGameSeed
                {
                    Id = GameIds.Puzzle,
                    Title = "Puzzle en bois",
                    Description = "Glisse chaque pièce dans son empreinte.",
                    Accent = MontessoriPalette.Honey
                },
                new MiniGameSeed
                {
                    Id = GameIds.Imagier,
                    Title = "Imagier",
                    Description = "Retourne la carte pour découvrir.",
                    Accent = MontessoriPalette.Moss
                },
                new MiniGameSeed
                {
                    Id = GameIds.Tracing,
                    Title = "Tracé",
                    Description = "Suis le chemin avec le doigt.",
                    Accent = MontessoriPalette.Sky
                }
            };
        }

        public static CategorySeed[] Categories()
        {
            return new[]
            {
                Alphabet(),
                Digits(),
                Shapes(),
                Colors(),
                ImagierParlant()
            };
        }

        static CategorySeed Alphabet()
        {
            return new CategorySeed
            {
                Id = "alphabet",
                Title = "Alphabet",
                Subtitle = "Voyelles et consonnes",
                CountLabel = "lettres",
                Accent = MontessoriPalette.ConsonantRose,
                PuzzleGroupSize = 4,
                Items = new[]
                {
                    Letter('A', PhoneticRole.Vowel, "Avion"),
                    Letter('B', PhoneticRole.Consonant, "Bateau"),
                    Letter('C', PhoneticRole.Consonant, "Chat"),
                    Letter('D', PhoneticRole.Consonant, "Dauphin"),
                    Letter('E', PhoneticRole.Vowel, "Éléphant"),
                    Letter('F', PhoneticRole.Consonant, "Fleur"),
                    Letter('G', PhoneticRole.Consonant, "Gâteau"),
                    Letter('H', PhoneticRole.Consonant, "Hibou"),
                    Letter('I', PhoneticRole.Vowel, "Igloo"),
                    Letter('J', PhoneticRole.Consonant, "Jus"),
                    Letter('K', PhoneticRole.Consonant, "Koala"),
                    Letter('L', PhoneticRole.Consonant, "Lune"),
                    Letter('M', PhoneticRole.Consonant, "Maison"),
                    Letter('N', PhoneticRole.Consonant, "Nuage"),
                    Letter('O', PhoneticRole.Vowel, "Oiseau"),
                    Letter('P', PhoneticRole.Consonant, "Poisson"),
                    Letter('Q', PhoneticRole.Consonant, "Quille"),
                    Letter('R', PhoneticRole.Consonant, "Robot"),
                    Letter('S', PhoneticRole.Consonant, "Soleil"),
                    Letter('T', PhoneticRole.Consonant, "Train"),
                    Letter('U', PhoneticRole.Vowel, "Uniforme"),
                    Letter('V', PhoneticRole.Consonant, "Voiture"),
                    Letter('W', PhoneticRole.Consonant, "Wagon"),
                    Letter('X', PhoneticRole.Consonant, "Xylophone"),
                    Letter('Y', PhoneticRole.Consonant, "Yacht"),
                    Letter('Z', PhoneticRole.Consonant, "Zèbre")
                }
            };
        }

        static CategorySeed Digits()
        {
            return new CategorySeed
            {
                Id = "chiffres",
                Title = "Chiffres",
                Subtitle = "De zéro à neuf",
                CountLabel = "chiffres",
                Accent = MontessoriPalette.Honey,
                PuzzleGroupSize = 5,
                Items = new[]
                {
                    Digit(0, "Zéro", "Oeuf"),
                    Digit(1, "Un", "Bougie"),
                    Digit(2, "Deux", "Ballons"),
                    Digit(3, "Trois", "Fleurs"),
                    Digit(4, "Quatre", "Trèfle"),
                    Digit(5, "Cinq", "Main"),
                    Digit(6, "Six", "Dé"),
                    Digit(7, "Sept", "Étoiles"),
                    Digit(8, "Huit", "Bonhomme"),
                    Digit(9, "Neuf", "Bouquet")
                }
            };
        }

        static CategorySeed Shapes()
        {
            return new CategorySeed
            {
                Id = "formes",
                Title = "Formes",
                Subtitle = "Ronds, carrés et amis",
                CountLabel = "formes",
                Accent = MontessoriPalette.Moss,
                PuzzleGroupSize = 4,
                Items = new[]
                {
                    Shape("shape-circle", "Cercle", "Ballon", MontessoriPalette.Sky),
                    Shape("shape-square", "Carré", "Cadeau", MontessoriPalette.Honey),
                    Shape("shape-triangle", "Triangle", "Montagne", MontessoriPalette.Moss),
                    Shape("shape-rectangle", "Rectangle", "Livre", MontessoriPalette.VowelBlue),
                    Shape("shape-star", "Étoile", "Étoile", MontessoriPalette.Sun),
                    Shape("shape-heart", "Cœur", "Cœur", MontessoriPalette.ConsonantRose),
                    Shape("shape-oval", "Ovale", "Oeuf", MontessoriPalette.Coral),
                    Shape("shape-diamond", "Losange", "Cerf-volant", MontessoriPalette.Sky)
                }
            };
        }

        static CategorySeed ImagierParlant()
        {
            return new CategorySeed
            {
                Id = WordThemes.CategoryId,
                Title = "Imagier Parlant",
                Subtitle = "Vocabulaire et actions",
                CountLabel = "mots",
                Accent = MontessoriPalette.Moss,
                PuzzleGroupSize = 4,
                Items = new ItemSeed[0]
            };
        }

        static CategorySeed Colors()
        {
            return new CategorySeed
            {
                Id = "couleurs",
                Title = "Couleurs",
                Subtitle = "Les tablettes de couleur",
                CountLabel = "couleurs",
                Accent = MontessoriPalette.Sky,
                PuzzleGroupSize = 4,
                Items = new[]
                {
                    Swatch("color-red", "Rouge", "Pomme", MontessoriPalette.Hex("#E24B4B")),
                    Swatch("color-blue", "Bleu", "Mer", MontessoriPalette.Hex("#2F74D0")),
                    Swatch("color-yellow", "Jaune", "Soleil", MontessoriPalette.Hex("#F2C14E")),
                    Swatch("color-green", "Vert", "Feuille", MontessoriPalette.Hex("#3E9A56")),
                    Swatch("color-orange", "Orange", "Orange", MontessoriPalette.Hex("#F08A2C")),
                    Swatch("color-purple", "Violet", "Raisin", MontessoriPalette.Hex("#7B5EA7")),
                    Swatch("color-pink", "Rose", "Fleur", MontessoriPalette.Hex("#F2A0B5")),
                    Swatch("color-brown", "Marron", "Ours", MontessoriPalette.Hex("#8A5A3B")),
                    Swatch("color-black", "Noir", "Nuit", MontessoriPalette.Hex("#2C2A28")),
                    Swatch("color-white", "Blanc", "Nuage", MontessoriPalette.Hex("#F7F4EE"))
                }
            };
        }

        static ItemSeed Letter(char symbol, PhoneticRole role, string word)
        {
            string id = "letter-" + char.ToLowerInvariant(symbol);
            return new ItemSeed
            {
                Id = id,
                Symbol = symbol.ToString(),
                DisplayName = symbol.ToString(),
                Word = word,
                Kind = ItemKind.Letter,
                Role = role,
                Visual = ItemVisual.Glyph,
                Accent = role == PhoneticRole.Vowel ? MontessoriPalette.VowelBlue : MontessoriPalette.ConsonantRose,
                PictogramId = id,
                StrokeKey = id
            };
        }

        static ItemSeed Digit(int value, string name, string word)
        {
            string id = "digit-" + value.ToString();
            return new ItemSeed
            {
                Id = id,
                Symbol = value.ToString(),
                DisplayName = name,
                Word = word,
                Kind = ItemKind.Digit,
                Role = PhoneticRole.Neutral,
                Visual = ItemVisual.Glyph,
                Accent = MontessoriPalette.Honey,
                PictogramId = id,
                StrokeKey = id
            };
        }

        static ItemSeed Shape(string id, string name, string word, Color accent)
        {
            return new ItemSeed
            {
                Id = id,
                Symbol = name,
                DisplayName = name,
                Word = word,
                Kind = ItemKind.Shape,
                Role = PhoneticRole.Neutral,
                Visual = ItemVisual.Pictogram,
                Accent = accent,
                PictogramId = id,
                StrokeKey = id
            };
        }

        static ItemSeed Swatch(string id, string name, string word, Color accent)
        {
            return new ItemSeed
            {
                Id = id,
                Symbol = name,
                DisplayName = name,
                Word = word,
                Kind = ItemKind.Color,
                Role = PhoneticRole.Neutral,
                Visual = ItemVisual.Swatch,
                Accent = accent,
                PictogramId = id,
                StrokeKey = "shape-circle"
            };
        }
    }
}
