using UnityEngine;

namespace Sensori.Montessori
{
    public enum ItemKind
    {
        Letter,
        Digit,
        Shape,
        Color
    }

    public enum PhoneticRole
    {
        Neutral,
        Vowel,
        Consonant
    }

    public enum ItemVisual
    {
        Glyph,
        Pictogram,
        Swatch
    }

    [CreateAssetMenu(fileName = "Element", menuName = "Montessori/Élément")]
    public sealed class LearningItem : ScriptableObject
    {
        [SerializeField] string _itemId = "element";
        [SerializeField] string _symbol = "A";
        [SerializeField] string _displayName = "A";
        [SerializeField] string _associatedWord = "";
        [SerializeField] ItemKind _kind = ItemKind.Letter;
        [SerializeField] PhoneticRole _role = PhoneticRole.Neutral;
        [SerializeField] ItemVisual _visual = ItemVisual.Glyph;
        [SerializeField] Color _accent = new Color(0.855f, 0.322f, 0.420f, 1f);
        [Tooltip("Dessin utilisé par l'imagier. Un identifiant inconnu affiche un badge coloré, sans modifier le code.")]
        [SerializeField] string _pictogramId = "";
        [Tooltip("Chemin du doigt, coordonnées 0 à 1, origine en bas à gauche.")]
        [SerializeField] StrokePath[] _strokes = new StrokePath[0];

        public string ItemId => _itemId;
        public string Symbol => _symbol ?? string.Empty;
        public string DisplayName => string.IsNullOrEmpty(_displayName) ? Symbol : _displayName;
        public string AssociatedWord => _associatedWord ?? string.Empty;
        public ItemKind Kind => _kind;
        public PhoneticRole Role => _role;
        public ItemVisual Visual => _visual;
        public Color Accent => _accent;
        public string PictogramId => string.IsNullOrEmpty(_pictogramId) ? _itemId : _pictogramId;
        public StrokePath[] Strokes => _strokes ?? new StrokePath[0];

        public Color SymbolColor
        {
            get
            {
                if (_role == PhoneticRole.Vowel)
                    return MontessoriPalette.VowelBlue;
                if (_role == PhoneticRole.Consonant)
                    return MontessoriPalette.ConsonantRose;
                if (_visual == ItemVisual.Swatch)
                    return _accent;
                return MontessoriPalette.Ink;
            }
        }

        public void Define(
            string itemId,
            string symbol,
            string displayName,
            string associatedWord,
            ItemKind kind,
            PhoneticRole role,
            ItemVisual visual,
            Color accent,
            string pictogramId,
            StrokePath[] strokes)
        {
            _itemId = itemId;
            _symbol = symbol;
            _displayName = displayName;
            _associatedWord = associatedWord;
            _kind = kind;
            _role = role;
            _visual = visual;
            _accent = accent;
            _pictogramId = pictogramId;
            _strokes = strokes ?? new StrokePath[0];
        }
    }
}
