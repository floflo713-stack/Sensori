using UnityEngine;

namespace Sensori.Montessori
{
    public enum WordTheme
    {
        Vehicules,
        Animaux,
        ActionsQuotidien,
        Nourriture,
        Emotions,
        Saisons
    }

    [CreateAssetMenu(fileName = "CarteMot", menuName = "Montessori/Carte mot")]
    public sealed class WordCardData : ScriptableObject
    {
        [SerializeField] string _id = "mot";
        [SerializeField] string _motAffiche = "Mot";
        [SerializeField] WordTheme _theme = WordTheme.Animaux;
        [SerializeField] Sprite _illustration;
        [SerializeField] string _pictogramme = "";
        [SerializeField] AudioClip _voixPrononciation;
        [SerializeField] AudioClip _effetSonore;
        [SerializeField] string _aide = "";

        public string Id => _id ?? string.Empty;
        public string MotAffiche => string.IsNullOrEmpty(_motAffiche) ? "Mot" : _motAffiche;
        public WordTheme Theme => _theme;
        public Sprite Illustration => _illustration;
        public string Pictogramme => _pictogramme ?? string.Empty;
        public AudioClip VoixPrononciation => _voixPrononciation;
        public AudioClip EffetSonore => _effetSonore;
        public string Aide => _aide ?? string.Empty;
        public bool HasAide => !string.IsNullOrEmpty(_aide);

        public Sprite Visuel
        {
            get
            {
                if (_illustration != null)
                    return _illustration;
                string key = string.IsNullOrEmpty(_pictogramme) ? "badge" : _pictogramme;
                return PictogramPainter.Get(key);
            }
        }

        public void Define(
            string id,
            string motAffiche,
            WordTheme theme,
            Sprite illustration,
            string pictogramme,
            AudioClip voixPrononciation,
            AudioClip effetSonore,
            string aide)
        {
            _id = string.IsNullOrEmpty(id) ? "mot" : id;
            _motAffiche = string.IsNullOrEmpty(motAffiche) ? "Mot" : motAffiche;
            _theme = theme;
            _illustration = illustration;
            _pictogramme = pictogramme ?? string.Empty;
            _voixPrononciation = voixPrononciation;
            _effetSonore = effetSonore;
            _aide = aide ?? string.Empty;
        }
    }
}
