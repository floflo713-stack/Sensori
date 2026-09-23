using UnityEngine;

namespace Sensori.Montessori
{
    public static class GameIds
    {
        public const string Puzzle = "wooden-puzzle";
        public const string Imagier = "imagier";
        public const string Tracing = "tracing";
    }

    [CreateAssetMenu(fileName = "MiniJeu", menuName = "Montessori/Mini-jeu")]
    public sealed class MiniGameDefinition : ScriptableObject
    {
        [Tooltip("Doit correspondre à MiniGameController.GameId. Exemple : wooden-puzzle, imagier, tracing.")]
        [SerializeField] string _gameId = GameIds.Puzzle;
        [SerializeField] string _title = "Mini-jeu";
        [SerializeField] string _description = "";
        [SerializeField] Color _accent = new Color(0.769f, 0.573f, 0.361f, 1f);

        public string GameId => _gameId;
        public string Title => _title;
        public string Description => _description ?? string.Empty;
        public Color Accent => _accent;

        public void Define(string gameId, string title, string description, Color accent)
        {
            _gameId = gameId;
            _title = title;
            _description = description;
            _accent = accent;
        }
    }
}
