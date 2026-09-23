using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class MontessoriApp : MonoBehaviour
    {
        public static MontessoriApp Instance { get; private set; }

        [SerializeField] ContentCatalog _catalog;
        [SerializeField] HomePresenter _home;
        [SerializeField] CategoryPresenter _category;
        [SerializeField] GameObject _gameRoot;
        [SerializeField] CelebrationView _celebration;
        [SerializeField] SparkleBurst _sparkles;

        LearningCategory _current;

        public ContentCatalog Catalog => _catalog;

        public void Configure(
            ContentCatalog catalog,
            HomePresenter home,
            CategoryPresenter category,
            GameObject gameRoot,
            CelebrationView celebration,
            SparkleBurst sparkles)
        {
            _catalog = catalog;
            _home = home;
            _category = category;
            _gameRoot = gameRoot;
            _celebration = celebration;
            _sparkles = sparkles;
        }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
            EnsureFonts();
            PrepareCanvas();
        }

        void Start()
        {
            if (_catalog == null)
            {
                Debug.LogError("Catalogue introuvable. Utilise le menu Montessori > Auto-Setup Scene.");
                return;
            }
            if (_celebration != null)
                _celebration.gameObject.SetActive(false);
            ShowHome();
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void ShowHome()
        {
            SetScreen(_home != null ? _home.gameObject : null);
            if (_home != null)
            {
                _home.Refresh();
                _home.PlayIntro();
            }
        }

        public void OpenCategory(LearningCategory category)
        {
            if (category == null)
                return;
            _current = category;
            SetScreen(_category != null ? _category.gameObject : null);
            if (_category != null)
                _category.Show(category);
        }

        public void ShowCategory()
        {
            if (_current == null)
            {
                ShowHome();
                return;
            }
            OpenCategory(_current);
        }

        public void StartGame(MiniGameDefinition definition)
        {
            if (definition == null || _current == null)
                return;
            var controller = FindGame(definition.GameId);
            if (controller == null)
            {
                Celebrate("Bientôt", "Ce jeu rejoint l'atelier très vite.", "", MontessoriPalette.Honey, null);
                return;
            }

            if (_gameRoot != null)
                _gameRoot.SetActive(true);
            if (_home != null)
                _home.gameObject.SetActive(false);
            if (_category != null)
                _category.gameObject.SetActive(false);

            var games = GetComponentsInChildren<MiniGameController>(true);
            for (int i = 0; i < games.Length; i++)
            {
                if (games[i] != null && games[i] != controller)
                    games[i].gameObject.SetActive(false);
            }
            controller.gameObject.SetActive(true);
            controller.Begin(new GameRequest
            {
                Category = _current,
                Definition = definition
            });
        }

        public void Celebrate(string title, string subtitle, string emblem, Color emblemColor, System.Action onContinue)
        {
            if (_celebration == null)
            {
                onContinue?.Invoke();
                return;
            }
            _celebration.Show(title, subtitle, emblem, emblemColor, onContinue);
        }

        public void Sparkle(Vector3 worldPosition, Color color)
        {
            if (_sparkles != null)
                _sparkles.Play(worldPosition, color);
        }

        void SetScreen(GameObject screen)
        {
            if (_home != null)
                _home.gameObject.SetActive(_home.gameObject == screen);
            if (_category != null)
                _category.gameObject.SetActive(_category.gameObject == screen);
            if (_gameRoot != null)
                _gameRoot.SetActive(false);
            if (_celebration != null && _celebration.gameObject.activeSelf)
                _celebration.gameObject.SetActive(false);
        }

        MiniGameController FindGame(string gameId)
        {
            var games = GetComponentsInChildren<MiniGameController>(true);
            for (int i = 0; i < games.Length; i++)
            {
                if (games[i] != null && games[i].GameId == gameId)
                    return games[i];
            }
            return null;
        }

        public void PrepareCanvas()
        {
            var canvases = GetComponentsInChildren<Canvas>(true);
            for (int i = 0; i < canvases.Length; i++)
            {
                var canvas = canvases[i];
                if (canvas != null && canvas.GetComponent<GraphicRaycaster>() == null)
                    canvas.gameObject.AddComponent<GraphicRaycaster>();
            }

            var images = GetComponentsInChildren<Image>(true);
            for (int i = 0; i < images.Length; i++)
            {
                var image = images[i];
                if (image == null)
                    continue;
                switch (image.gameObject.name)
                {
                    case "Fond":
                    case "Ombre":
                    case "Plaque":
                    case "Accent":
                    case "Icone":
                        image.raycastTarget = false;
                        break;
                }
            }

            RepairPointerActions();
        }

        static void RepairPointerActions()
        {
            var module = FindAnyObjectByType<InputSystemUIInputModule>();
            if (module == null)
                return;
            bool pointMissing = module.point == null || module.point.action == null;
            bool clickMissing = module.leftClick == null || module.leftClick.action == null;
            if (pointMissing || clickMissing)
                module.AssignDefaultActions();
        }

        void EnsureFonts()
        {
            var font = UiFont.Builtin;
            if (font == null)
                return;
            var texts = GetComponentsInChildren<Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i] != null && texts[i].font == null)
                    texts[i].font = font;
            }
        }
    }
}
