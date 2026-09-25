using UnityEngine;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public static class MontessoriUiBuilder
    {
        public static void Build(GameObject root, ContentCatalog catalog, ThemeAssets theme, MiniGameDefinition[] games, WordCardDeck deck)
        {
            if (root.GetComponent<MontessoriApp>() == null)
                root.AddComponent<MontessoriApp>();
            if (root.GetComponent<TweenHost>() == null)
                root.AddComponent<TweenHost>();
            if (root.GetComponent<WoodenAudio>() == null)
                root.AddComponent<WoodenAudio>();

            var canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasObject.transform.SetParent(root.transform, false);
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;
            if (canvasObject.GetComponent<GraphicRaycaster>() == null)
                canvasObject.AddComponent<GraphicRaycaster>();
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            scaler.referencePixelsPerUnit = 100f;

            var background = UiFactory.Picture("Fond", canvasObject.transform, theme.Background, Color.white, false, false);
            UiFactory.Stretch(background.rectTransform, 0f, 0f, 0f, 0f);
            background.preserveAspect = false;
            background.raycastTarget = false;

            var safe = UiFactory.Rect("ZoneSure", canvasObject.transform);
            UiFactory.Stretch(safe, 0f, 0f, 0f, 0f);
            safe.gameObject.AddComponent<SafeAreaFitter>();

            var homeObject = UiFactory.Rect("Accueil", safe).gameObject;
            UiFactory.Stretch(homeObject.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);
            var home = homeObject.AddComponent<HomePresenter>();
            UiFactory.AddGroup(homeObject);
            home.Construct(theme, catalog);
            home.SealWelcome();

            var categoryObject = UiFactory.Rect("Categories", safe).gameObject;
            UiFactory.Stretch(categoryObject.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);
            var category = categoryObject.AddComponent<CategoryPresenter>();
            UiFactory.AddGroup(categoryObject);
            category.Construct(theme, catalog, games);

            var gameRoot = UiFactory.Rect("Jeux", safe).gameObject;
            UiFactory.Stretch(gameRoot.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);

            var puzzleObject = UiFactory.Rect("PuzzleBois", gameRoot.transform).gameObject;
            UiFactory.Stretch(puzzleObject.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);
            var puzzle = puzzleObject.AddComponent<WoodenPuzzleGame>();
            puzzle.Construct(theme);

            var imagierObject = UiFactory.Rect("Imagier", gameRoot.transform).gameObject;
            UiFactory.Stretch(imagierObject.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);
            var imagier = imagierObject.AddComponent<ImagierGame>();
            imagier.Construct(theme);

            var tracingObject = UiFactory.Rect("Trace", gameRoot.transform).gameObject;
            UiFactory.Stretch(tracingObject.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);
            var tracing = tracingObject.AddComponent<TracingGame>();
            tracing.Construct(theme);

            var imagierParlantObject = UiFactory.Rect("ImagierParlant", safe).gameObject;
            UiFactory.Stretch(imagierParlantObject.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);
            var imagierParlant = imagierParlantObject.AddComponent<FlashcardPresenter>();
            imagierParlant.Construct(theme, deck);
            UiFactory.AddGroup(imagierParlantObject);

            var sparkleObject = UiFactory.Rect("Etincelles", canvasObject.transform).gameObject;
            var sparkles = sparkleObject.AddComponent<SparkleBurst>();
            sparkles.Construct(theme);
            var sparkleGroup = sparkleObject.AddComponent<CanvasGroup>();
            sparkleGroup.blocksRaycasts = false;
            sparkleGroup.interactable = false;

            var celebrationObject = UiFactory.Rect("Celebration", canvasObject.transform).gameObject;
            UiFactory.Stretch(celebrationObject.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);
            var celebration = celebrationObject.AddComponent<CelebrationView>();
            celebration.Construct(theme);

            categoryObject.SetActive(false);
            puzzleObject.SetActive(false);
            imagierObject.SetActive(false);
            tracingObject.SetActive(false);
            imagierParlantObject.SetActive(false);
            gameRoot.SetActive(false);
            celebrationObject.SetActive(false);
            homeObject.SetActive(true);

            var app = root.GetComponent<MontessoriApp>();
            app.Configure(catalog, home, category, gameRoot, celebration, sparkles, imagierParlant);
            app.PrepareCanvas();
        }
    }
}
