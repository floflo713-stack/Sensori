using UnityEngine;

namespace Sensori.Montessori
{
    [CreateAssetMenu(fileName = "Categorie", menuName = "Montessori/Catégorie")]
    public sealed class LearningCategory : ScriptableObject
    {
        [SerializeField] string _categoryId = "categorie";
        [SerializeField] string _title = "Catégorie";
        [SerializeField] string _subtitle = "";
        [SerializeField] string _countLabel = "éléments";
        [SerializeField] Color _accent = new Color(0.855f, 0.322f, 0.420f, 1f);
        [SerializeField, Range(2, 8)] int _puzzleGroupSize = 4;
        [SerializeField] LearningItem[] _items = new LearningItem[0];
        [SerializeField] MiniGameDefinition[] _games = new MiniGameDefinition[0];

        public string CategoryId => _categoryId;
        public string Title => _title;
        public string Subtitle => _subtitle ?? string.Empty;
        public string CountLabel => string.IsNullOrEmpty(_countLabel) ? "éléments" : _countLabel;
        public Color Accent => _accent;
        public int PuzzleGroupSize => Mathf.Clamp(_puzzleGroupSize, 2, 8);
        public LearningItem[] Items => _items ?? new LearningItem[0];
        public MiniGameDefinition[] Games => _games ?? new MiniGameDefinition[0];

        public int ItemCount
        {
            get
            {
                if (_items == null)
                    return 0;
                int count = 0;
                for (int i = 0; i < _items.Length; i++)
                {
                    if (_items[i] != null)
                        count++;
                }
                return count;
            }
        }

        public bool HasGame(string gameId)
        {
            if (_games == null || string.IsNullOrEmpty(gameId))
                return false;
            for (int i = 0; i < _games.Length; i++)
            {
                if (_games[i] != null && _games[i].GameId == gameId)
                    return true;
            }
            return false;
        }

        public void Define(
            string categoryId,
            string title,
            string subtitle,
            string countLabel,
            Color accent,
            int puzzleGroupSize,
            LearningItem[] items,
            MiniGameDefinition[] games)
        {
            _categoryId = categoryId;
            _title = title;
            _subtitle = subtitle;
            _countLabel = countLabel;
            _accent = accent;
            _puzzleGroupSize = Mathf.Clamp(puzzleGroupSize, 2, 8);
            _items = items ?? new LearningItem[0];
            _games = games ?? new MiniGameDefinition[0];
        }

        void OnValidate()
        {
            if (_puzzleGroupSize < 2)
                _puzzleGroupSize = 2;
            if (_puzzleGroupSize > 8)
                _puzzleGroupSize = 8;
        }
    }
}
