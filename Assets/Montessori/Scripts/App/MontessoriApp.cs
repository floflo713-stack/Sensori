using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class MontessoriApp : MonoBehaviour
    {
        public static MontessoriApp Instance { get; private set; }

        static readonly string[] DecorativeNames =
        {
            "Fond", "ZoneSure", "Cadre", "Ombre", "Accent", "Icone",
            "Enseigne", "Legende", "Entete", "ZoneCartes", "ZoneJeux",
            "ZoneChoix", "SousTitre", "Compte", "Description", "Progression",
            "Glyph", "Libelle", "Embleme", "Titre", "Etiquette", "Etincelles",
            "Jeton", "Jetons", "Forme", "Lettre"
        };

        [SerializeField] ContentCatalog _catalog;
        [SerializeField] HomePresenter _home;
        [SerializeField] CategoryPresenter _category;
        [SerializeField] GameObject _gameRoot;
        [SerializeField] CelebrationView _celebration;
        [SerializeField] SparkleBurst _sparkles;

        LearningCategory _current;
        Button _sensoriButton;

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
        }

        void Start()
        {
            RepairPointerActions();
            if (_home != null)
            {
                _sensoriButton = _home.EnsureButton();
                _home.PresentAsWelcome();
            }
            if (_category != null)
            {
                Sprite panel = _home != null ? _home.PlateSprite() : null;
                Sprite shadow = _home != null ? _home.ShadowSprite() : null;
                _category.EnsureMenus(_catalog, panel, shadow);
            }
            WireCategoryBack();
            PrepareCanvas();
            WireHomeButton(_sensoriButton);
            if (_celebration != null)
                _celebration.gameObject.SetActive(false);
            if (_catalog == null)
            {
                Debug.LogError("Catalogue introuvable. Utilise le menu Montessori > Auto-Setup Scene.");
                return;
            }
            ShowHome();
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void OnSensoriClicked()
        {
            Debug.Log("[Montessori] Clic détecté sur le bouton Sensori !");
            if (_home != null)
            {
                _home.BeginDeparture(OpenCategories);
                return;
            }
            OpenCategories();
        }

        public void OpenCategories()
        {
            SetScreen(_category != null ? _category.gameObject : null, true);
            if (_category != null)
                _category.ShowChoices();
        }

        public void ShowHome()
        {
            SetScreen(_home != null ? _home.gameObject : null, false);
            if (_home != null)
            {
                var group = _home.GetComponent<CanvasGroup>();
                if (group != null)
                {
                    group.alpha = 1f;
                    group.interactable = true;
                    group.blocksRaycasts = true;
                }
                _home.Refresh();
                _home.PlayIntro();
            }
        }

        public void OpenCategory(LearningCategory category)
        {
            if (category == null)
                return;
            _current = category;
            SetScreen(_category != null ? _category.gameObject : null, false);
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

        public void BackFromCategories()
        {
            if (_category != null && _category.ShowingGames)
                OpenCategories();
            else
                ShowHome();
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

        public void Celebrate(string title, string subtitle, string emblem, Color emblemColor, System.Action onContinue, string buttonLabel = null)
        {
            if (_celebration == null)
            {
                onContinue?.Invoke();
                return;
            }
            _celebration.transform.SetAsLastSibling();
            _celebration.Show(title, subtitle, emblem, emblemColor, onContinue, buttonLabel);
        }

        public void Sparkle(Vector3 worldPosition, Color color)
        {
            if (_sparkles != null)
                _sparkles.Play(worldPosition, color);
        }

        public void WireHomeButton(Button button)
        {
            if (button == null)
            {
                Debug.LogError("[Montessori] Bouton Sensori absent : le clic d'accueil ne peut pas être câblé.");
                return;
            }

            _sensoriButton = button;
            var image = button.GetComponent<Image>();
            if (image != null)
                image.raycastTarget = true;
            if (button.targetGraphic != null)
                button.targetGraphic.raycastTarget = true;
            button.interactable = true;

            button.onClick.RemoveListener(OnSensoriClicked);
            if (!HasPersistent(button, nameof(OnSensoriClicked)))
                button.onClick.AddListener(OnSensoriClicked);
        }

        public static bool HasPersistent(Button button, string method)
        {
            if (button == null || string.IsNullOrEmpty(method))
                return false;
            int count = button.onClick.GetPersistentEventCount();
            for (int i = 0; i < count; i++)
            {
                if (button.onClick.GetPersistentMethodName(i) == method)
                    return true;
            }
            return false;
        }

        void WireCategoryBack()
        {
            if (_category == null)
                return;
            var retour = _category.transform.Find("Entete/Retour");
            if (retour == null)
                return;
            var face = retour.Find("Face");
            var target = face != null ? face.gameObject : retour.gameObject;
            var navigation = target.GetComponent<NavigationButton>();
            if (navigation != null)
                navigation.enabled = false;
            var button = UiFactory.CreateButton(target);
            button.onClick.RemoveListener(BackFromCategories);
            button.onClick.AddListener(BackFromCategories);
        }

        void SetScreen(GameObject screen, bool animateIn)
        {
            if (_home != null && _home.gameObject != screen)
            {
                var homeGroup = _home.GetComponent<CanvasGroup>();
                if (homeGroup != null)
                {
                    homeGroup.interactable = false;
                    homeGroup.blocksRaycasts = false;
                }
                _home.gameObject.SetActive(false);
            }
            if (_category != null && _category.gameObject != screen)
                _category.gameObject.SetActive(false);
            if (_gameRoot != null)
                _gameRoot.SetActive(false);
            if (_celebration != null && _celebration.gameObject.activeSelf)
                _celebration.gameObject.SetActive(false);

            if (screen == null)
                return;

            bool wasActive = screen.activeSelf;
            screen.SetActive(true);
            var group = screen.GetComponent<CanvasGroup>();
            if (group != null)
            {
                group.alpha = 1f;
                group.interactable = true;
                group.blocksRaycasts = true;
            }

            if (!animateIn || wasActive || !Application.isPlaying)
                return;

            screen.transform.localScale = Vector3.one * 0.975f;
            Motion.Scale(screen.transform, Vector3.one, 0.3f, Ease.OutBack);
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
                if (canvas == null)
                    continue;
                canvas.enabled = true;
                var raycaster = canvas.GetComponent<GraphicRaycaster>();
                if (raycaster == null)
                    raycaster = canvas.gameObject.AddComponent<GraphicRaycaster>();
                raycaster.enabled = true;
            }

            var graphics = GetComponentsInChildren<Graphic>(true);
            for (int i = 0; i < graphics.Length; i++)
            {
                var graphic = graphics[i];
                if (graphic != null && IsDecorative(graphic.gameObject.name))
                    graphic.raycastTarget = false;
            }

            var texts = GetComponentsInChildren<Text>(true);
            var font = UiFont.Builtin;
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i] == null)
                    continue;
                if (texts[i].GetComponent<Button>() == null)
                    texts[i].raycastTarget = false;
                if (font != null && texts[i].font == null)
                    texts[i].font = font;
            }

            var buttons = GetComponentsInChildren<Button>(true);
            for (int i = 0; i < buttons.Length; i++)
            {
                var button = buttons[i];
                if (button == null)
                    continue;
                if (button.targetGraphic != null)
                    button.targetGraphic.raycastTarget = true;
                var image = button.GetComponent<Image>();
                if (image != null)
                    image.raycastTarget = true;
            }

            RepairPointerActions();
        }

        public static bool IsDecorative(string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
                return false;
            for (int i = 0; i < DecorativeNames.Length; i++)
            {
                if (DecorativeNames[i] == objectName)
                    return true;
            }
            return false;
        }

        static void RepairPointerActions()
        {
            var eventSystem = FindAnyObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                var eventObject = new GameObject("EventSystem");
                eventSystem = eventObject.AddComponent<EventSystem>();
            }
            eventSystem.enabled = true;
            eventSystem.sendNavigationEvents = false;

            var legacy = eventSystem.GetComponent<StandaloneInputModule>();
            if (legacy != null)
            {
                if (Application.isPlaying)
                    UnityEngine.Object.Destroy(legacy);
                else
                    UnityEngine.Object.DestroyImmediate(legacy);
            }

            var module = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (module == null)
                module = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();

            bool pointMissing = module.point == null || module.point.action == null;
            bool clickMissing = module.leftClick == null || module.leftClick.action == null;
            if (pointMissing || clickMissing)
            {
                module.actionsAsset = null;
                module.AssignDefaultActions();
            }
            module.enabled = true;
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
