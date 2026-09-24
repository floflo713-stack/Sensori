using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class TracingSurface : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] TracingGame _game;

        public void Bind(TracingGame game)
        {
            _game = game;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_game != null)
                _game.PointerDown(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_game != null)
                _game.PointerDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_game != null)
                _game.PointerUp(eventData);
        }
    }

    public sealed class TracingGame : MiniGameController
    {
        [SerializeField] Text _title;
        [SerializeField] Text _progress;
        [SerializeField] Text _instruction;
        [SerializeField] Text _symbol;
        [SerializeField] Text _ghost;
        RectTransform _trail;
        readonly List<Image> _marks = new List<Image>();
        readonly List<Vector2> _shown = new List<Vector2>();
        [SerializeField] RectTransform _board;
        [SerializeField] Image _boardFace;
        [SerializeField] RibbonGraphic _guide;
        [SerializeField] RibbonGraphic _ink;
        [SerializeField] Image _hint;
        [SerializeField] RectTransform _dots;
        [SerializeField] SimpleClick _clear;
        [SerializeField] SimpleClick _watch;
        [SerializeField] Sprite _pearl;

        readonly List<List<Vector2>> _guideStrokes = new List<List<Vector2>>();
        readonly List<List<Vector2>> _gestures = new List<List<Vector2>>();
        readonly List<Vector2> _flatGuide = new List<Vector2>();
        readonly List<Vector2> _flatUser = new List<Vector2>();
        readonly List<Vector2> _smooth = new List<Vector2>();
        readonly List<Vector2[]> _mesh = new List<Vector2[]>();

        GameRequest _request;
        int _index = -1;
        int _pointer = -1;
        bool _success;
        bool _advancing;
        bool _drawing;
        float _idle;

        public override string GameId => GameIds.Tracing;

        public void Construct(ThemeAssets theme)
        {
            _pearl = theme.Pearl;
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);

            UiFactory.BuildGameHeader(transform, theme, out _title, out _progress);

            _board = UiFactory.Rect("Planche", transform);
            UiFactory.AnchorCenter(_board, new Vector2(0f, -20f), new Vector2(760f, 760f));
            var shadow = UiFactory.Picture("Ombre", _board, theme.Shadow, MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.28f), false, false);
            UiFactory.Stretch(shadow.rectTransform, -22f, -34f, -22f, -6f);
            _boardFace = UiFactory.Picture("Bois", _board, theme.Inset, Color.white, true, true);
            UiFactory.Stretch(_boardFace.rectTransform, 0f, 0f, 0f, 0f);
            _boardFace.preserveAspect = false;
            var surface = _boardFace.gameObject.AddComponent<TracingSurface>();
            surface.Bind(this);

            var guideGo = UiFactory.Rect("Guide", _board);
            UiFactory.Stretch(guideGo, 36f, 36f, 36f, 36f);
            _guide = guideGo.gameObject.AddComponent<RibbonGraphic>();
            var inkGo = UiFactory.Rect("Encre", _board);
            UiFactory.Stretch(inkGo, 36f, 36f, 36f, 36f);
            _ink = inkGo.gameObject.AddComponent<RibbonGraphic>();

            _dots = UiFactory.Rect("Departs", guideGo);
            UiFactory.Stretch(_dots, 0f, 0f, 0f, 0f);

            _hint = UiFactory.Picture("Doigt", guideGo, theme.Pearl, MontessoriPalette.Sun, false, false);
            UiFactory.AnchorCenter(_hint.rectTransform, Vector2.zero, new Vector2(42f, 42f));
            _hint.gameObject.SetActive(false);

            _symbol = UiFactory.Label("Symbole", _board, "", 42, MontessoriPalette.InkSoft, TextAnchor.UpperLeft);
            _symbol.rectTransform.anchorMin = new Vector2(0f, 1f);
            _symbol.rectTransform.anchorMax = new Vector2(0f, 1f);
            _symbol.rectTransform.pivot = new Vector2(0f, 1f);
            _symbol.rectTransform.sizeDelta = new Vector2(120f, 64f);
            _symbol.rectTransform.anchoredPosition = new Vector2(18f, -8f);

            _instruction = UiFactory.Label("Consigne", transform, "Suis le chemin avec le doigt.", 28, MontessoriPalette.InkSoft, TextAnchor.MiddleCenter);
            var instructionRect = _instruction.rectTransform;
            UiFactory.AnchorBottom(instructionRect, 48f, 80f, 80f);
            instructionRect.offsetMin = new Vector2(80f, 162f);
            instructionRect.offsetMax = new Vector2(-80f, 210f);

            _clear = MakeButton("Effacer", "Effacer", theme, new Vector2(-150f, 46f));
            _watch = MakeButton("Geste", "Voir le geste", theme, new Vector2(170f, 46f));
        }

        SimpleClick MakeButton(string name, string label, ThemeAssets theme, Vector2 position)
        {
            var root = UiFactory.Rect(name, transform);
            root.anchorMin = new Vector2(0.5f, 0f);
            root.anchorMax = new Vector2(0.5f, 0f);
            root.pivot = new Vector2(0.5f, 0f);
            root.sizeDelta = new Vector2(300f, 104f);
            root.anchoredPosition = position;
            var face = UiFactory.Picture("Face", root, theme.Panel, Color.white, true, true);
            UiFactory.Stretch(face.rectTransform, 0f, 0f, 0f, 0f);
            face.preserveAspect = false;
            face.gameObject.AddComponent<Pressable>();
            var click = face.gameObject.AddComponent<SimpleClick>();
            var text = UiFactory.Label("Libelle", face.transform, label, 28, MontessoriPalette.Ink, TextAnchor.MiddleCenter);
            UiFactory.Stretch(text.rectTransform, 8f, 8f, 8f, 8f);
            return click;
        }

        public override void Begin(GameRequest request)
        {
            ScreenBackdrop.Ensure(transform, "trace");
            _request = request;
            if (_title != null)
            {
                string name = request != null && request.Definition != null ? request.Definition.Title : "Tracé";
                string category = request != null && request.Category != null ? request.Category.Title : string.Empty;
                _title.text = string.IsNullOrEmpty(category) ? name : name + " · " + category;
            }
            _index = FirstIncomplete();
            ShowItem();
        }

        void OnEnable()
        {
            if (_clear != null)
                _clear.Clicked += ClearInk;
            if (_watch != null)
                _watch.Clicked += PlayHint;
        }

        void OnDisable()
        {
            if (_clear != null)
                _clear.Clicked -= ClearInk;
            if (_watch != null)
                _watch.Clicked -= PlayHint;
            Motion.KillOwner(this);
            _pointer = -1;
        }

        void Update()
        {
            PollFinger();
            if (!isActiveAndEnabled || _success || _advancing || _drawing)
                return;
            _idle += Time.unscaledDeltaTime;
            if (_idle > 7f)
            {
                _idle = 0f;
                PlayHint();
            }
        }

        void PollFinger()
        {
            if (!isActiveAndEnabled || _success || _advancing)
                return;
            var pointer = Pointer.current;
            if (pointer == null)
                return;
            Vector2 screen = pointer.position.ReadValue();
            Camera cam = EventCamera();
            if (pointer.press.wasPressedThisFrame)
            {
                if (_wiping)
                    return;
                if (HitButton("Effacer", _clear, screen))
                {
                    ClearInk();
                    return;
                }
                if (HitButton("Geste", _watch, screen))
                {
                    PlayHint();
                    return;
                }
                if (HitEraser(screen))
                {
                    Wipe();
                    return;
                }
                var back = transform.Find("Entete/Retour") as RectTransform;
                if (Hit(back, screen, 8f))
                    return;
                if (!Contains(_board, screen, cam))
                    return;
                BeginStroke(screen, cam);
                return;
            }
            if (_drawing && pointer.press.isPressed)
                ExtendStroke(screen, cam);
            if (_drawing && pointer.press.wasReleasedThisFrame)
            {
                _drawing = false;
                _pointer = -1;
                Evaluate();
            }
        }

        bool _offLane;

        void BeginStroke(Vector2 screen, Camera cam)
        {
            if (_drawing)
                return;
            if (!TryNormScreen(screen, cam, out var point) || !InsideLane(point, out point))
                return;
            StopHint();
            _drawing = true;
            _offLane = false;
            _pointer = 0;
            _idle = 0f;
            var gesture = new List<Vector2>(64);
            gesture.Add(point);
            _gestures.Add(gesture);
            PaintInk();
        }

        void ExtendStroke(Vector2 screen, Camera cam)
        {
            if (_gestures.Count == 0)
                return;
            if (!TryNormScreen(screen, cam, out var point) || !InsideLane(point, out point))
            {
                _offLane = true;
                return;
            }
            if (_offLane)
            {
                _offLane = false;
                var fresh = new List<Vector2>(32);
                fresh.Add(point);
                _gestures.Add(fresh);
                PaintInk();
                return;
            }
            var gesture = _gestures[_gestures.Count - 1];
            if (gesture.Count > 0 && (gesture[gesture.Count - 1] - point).sqrMagnitude < 0.00008f)
                return;
            if (gesture.Count > 700)
                return;
            gesture.Add(point);
            PaintInk();
        }

        public void PointerDown(PointerEventData eventData)
        {
            if (_wiping || _success || _advancing || _drawing || eventData == null || eventData.button != PointerEventData.InputButton.Left)
                return;
            if (HitEraser(eventData.position))
            {
                Wipe();
                return;
            }
            BeginStroke(eventData.position, eventData.pressEventCamera);
        }

        public void PointerDrag(PointerEventData eventData)
        {
            if (!_drawing || eventData == null)
                return;
            ExtendStroke(eventData.position, eventData.pressEventCamera);
        }

        public void PointerUp(PointerEventData eventData)
        {
            if (!_drawing)
                return;
            _drawing = false;
            _pointer = -1;
            Evaluate();
        }

        void ShowItem()
        {
            var items = Items();
            if (items.Length == 0)
                return;
            if (_index < 0 || _index >= items.Length)
                _index = 0;
            Canvas.ForceUpdateCanvases();
            var item = items[_index];
            _success = false;
            _advancing = false;
            _pointer = -1;
            _drawing = false;
            _idle = 0f;
            _gestures.Clear();
            ClearTrail();
            if (_ink != null)
                _ink.ClearPaths();
            if (_progress != null)
                _progress.text = (_index + 1) + " / " + items.Length;
            if (_symbol != null && item != null)
            {
                _symbol.text = string.IsNullOrEmpty(item.Symbol) ? item.DisplayName : item.Symbol;
                _symbol.color = item.SymbolColor;
            }
            if (_instruction != null)
                _instruction.text = "Suis le chemin avec le doigt.";
            DressBoard();
            BuildGuide(item);
            Color ink = item != null ? item.SymbolColor : MontessoriPalette.ConsonantRose;
            EnsureDrawn(_guide);
            EnsureDrawn(_ink);
            RenderGuide(ink);
            PlaceDots(item);
            HideGhost();
            ClearTrail();
            if (_guide != null)
                _guide.transform.SetAsLastSibling();
            if (_trail != null)
                _trail.SetAsLastSibling();
            if (_symbol != null)
                _symbol.transform.SetAsLastSibling();
            if (_instruction != null)
                _instruction.transform.SetAsLastSibling();
            RaiseButtons();
            PlayHint();
        }

        void RaiseButtons()
        {
            var clear = transform.Find("Effacer");
            var watch = transform.Find("Geste");
            if (clear != null)
                clear.SetAsLastSibling();
            if (watch != null)
                watch.SetAsLastSibling();
        }

        bool HitButton(string name, SimpleClick click, Vector2 screen)
        {
            var rect = transform.Find(name) as RectTransform;
            if (rect == null && click != null)
                rect = click.transform.parent as RectTransform;
            return Hit(rect, screen, 18f);
        }

        static bool Hit(RectTransform rect, Vector2 screen, float pad)
        {
            if (rect == null)
                return false;
            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            float minX = Mathf.Min(corners[0].x, corners[2].x) - pad;
            float maxX = Mathf.Max(corners[0].x, corners[2].x) + pad;
            float minY = Mathf.Min(corners[0].y, corners[2].y) - pad;
            float maxY = Mathf.Max(corners[0].y, corners[2].y) + pad;
            return screen.x >= minX && screen.x <= maxX && screen.y >= minY && screen.y <= maxY;
        }

        static readonly Color Slate = new Color(0.12f, 0.38f, 0.30f, 1f);
        static readonly Color SlateFrame = new Color(0.90f, 0.78f, 0.60f, 1f);
        static Sprite _flat;

        static Sprite FlatSprite()
        {
            if (_flat != null)
                return _flat;
            var texture = WoodTextures.CreateSolid(8);
            _flat = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
            _flat.name = "ardoise";
            return _flat;
        }

        void DressBoard()
        {
            if (_board != null)
                UiFactory.AnchorCenter(_board, new Vector2(0f, 36f), new Vector2(720f, 640f));
            if (_boardFace != null)
            {
                _boardFace.color = SlateFrame;
                _boardFace.raycastTarget = true;
            }
            var slate = _board != null ? _board.Find("Ardoise") as RectTransform : null;
            Image slateImage;
            if (slate == null && _board != null)
            {
                slateImage = UiFactory.Picture("Ardoise", _board, FlatSprite(), Slate, false, false);
                UiFactory.Stretch(slateImage.rectTransform, 46f, 78f, 46f, 46f);
                slateImage.preserveAspect = false;
                slate = slateImage.rectTransform;
            }
            else
                slateImage = slate != null ? slate.GetComponent<Image>() : null;
            if (slateImage != null)
            {
                slateImage.sprite = FlatSprite();
                slateImage.color = Slate;
                slateImage.raycastTarget = false;
                int frame = _boardFace != null ? _boardFace.transform.GetSiblingIndex() : 0;
                slate.SetSiblingIndex(frame + 1);
            }
            if (slate != null)
                UiFactory.Stretch(slate, 52f, 108f, 52f, 64f);
            if (slateImage != null)
                slateImage.preserveAspect = false;
            BuildChalkboard(slate);
            FitChalkArea();
            if (_symbol != null)
            {
                _symbol.fontSize = 32;
                _symbol.fontStyle = FontStyle.Bold;
                _symbol.color = new Color(0.95f, 0.96f, 0.91f, 1f);
                _symbol.alignment = TextAnchor.MiddleLeft;
                _symbol.horizontalOverflow = HorizontalWrapMode.Overflow;
                _symbol.verticalOverflow = VerticalWrapMode.Overflow;
                var label = _symbol.rectTransform;
                label.anchorMin = new Vector2(0f, 1f);
                label.anchorMax = new Vector2(1f, 1f);
                label.pivot = new Vector2(0f, 1f);
                label.offsetMin = new Vector2(68f, -108f);
                label.offsetMax = new Vector2(-70f, -62f);
            }
            if (_instruction == null)
                return;
            var line = _instruction.rectTransform;
            line.anchorMin = new Vector2(0f, 0f);
            line.anchorMax = new Vector2(1f, 0f);
            line.pivot = new Vector2(0.5f, 0f);
            line.sizeDelta = new Vector2(-120f, 46f);
            line.anchoredPosition = new Vector2(0f, 168f);
        }

        void FitChalkArea()
        {
            FitNamed("Guide");
            FitNamed("Encre");
            FitNamed("Trait");
        }

        void FitNamed(string childName)
        {
            if (_board == null)
                return;
            var rect = _board.Find(childName) as RectTransform;
            if (rect != null)
                UiFactory.Stretch(rect, 68f, 124f, 68f, 112f);
        }

        void BuildChalkboard(RectTransform slate)
        {
            if (_board == null)
                return;
            var stale = FindNamed("Rebord");
            if (stale != null)
                stale.gameObject.SetActive(false);
            var lip = Flat("Lèvre", new Color(0.28f, 0.16f, 0.09f, 1f));
            UiFactory.Stretch(lip.rectTransform, 28f, 28f, 28f, 28f);
            if (slate != null)
                lip.transform.SetSiblingIndex(slate.GetSiblingIndex());

            var shelf = Flat("Tablette", new Color(0.55f, 0.36f, 0.20f, 1f));
            PinBottom(shelf.rectTransform, 22f, 36f, 58f);
            var shelfTop = Flat("TabletteClaire", new Color(0.78f, 0.58f, 0.36f, 1f));
            PinBottom(shelfTop.rectTransform, 50f, 10f, 58f);

            var eraser = Flat("Eponge", new Color(0.86f, 0.74f, 0.52f, 1f));
            PinBottomSize(eraser.rectTransform, 168f, 52f, 130f, 26f);
            var felt = Flat("Feutre", new Color(0.16f, 0.24f, 0.20f, 1f));
            felt.transform.SetParent(eraser.transform, false);
            UiFactory.Stretch(felt.rectTransform, 7f, 8f, 7f, 8f);
            var feltShine = Flat("FeutreClair", new Color(0.45f, 0.58f, 0.50f, 1f));
            feltShine.transform.SetParent(felt.transform, false);
            feltShine.rectTransform.anchorMin = new Vector2(0f, 1f);
            feltShine.rectTransform.anchorMax = new Vector2(1f, 1f);
            feltShine.rectTransform.pivot = new Vector2(0.5f, 1f);
            feltShine.rectTransform.sizeDelta = new Vector2(-16f, 10f);
            feltShine.rectTransform.anchoredPosition = new Vector2(0f, -6f);

            var chalk = Flat("Craie", new Color(0.96f, 0.96f, 0.93f, 1f));
            PinBottomSize(chalk.rectTransform, 78f, 16f, -150f, 40f);
            var chalkTip = Flat("CraieBout", new Color(0.82f, 0.84f, 0.80f, 1f));
            chalkTip.transform.SetParent(chalk.transform, false);
            chalkTip.rectTransform.anchorMin = new Vector2(1f, 0f);
            chalkTip.rectTransform.anchorMax = new Vector2(1f, 1f);
            chalkTip.rectTransform.pivot = new Vector2(1f, 0.5f);
            chalkTip.rectTransform.sizeDelta = new Vector2(14f, 0f);
            chalkTip.rectTransform.anchoredPosition = Vector2.zero;

            if (slate != null)
            {
                shelf.transform.SetSiblingIndex(slate.GetSiblingIndex() + 1);
                shelfTop.transform.SetSiblingIndex(shelf.transform.GetSiblingIndex() + 1);
                eraser.transform.SetSiblingIndex(shelfTop.transform.GetSiblingIndex() + 1);
                chalk.transform.SetSiblingIndex(eraser.transform.GetSiblingIndex() + 1);
            }
            _eraser = eraser.rectTransform;
            _eraserRest = _eraser.anchoredPosition;
            eraser.raycastTarget = true;
        }

        bool _wiping;
        RectTransform _eraser;
        Vector2 _eraserRest;

        bool HitEraser(Vector2 screen)
        {
            return _eraser != null && Hit(_eraser, screen, 16f);
        }

        void Wipe()
        {
            if (_wiping || _eraser == null || _success)
                return;
            _wiping = true;
            _drawing = false;
            _pointer = -1;
            StopHint();
            ClearInk();
            Motion.Kill(_eraser, "pos");
            var high = _eraserRest + new Vector2(-220f, 250f);
            var far = _eraserRest + new Vector2(220f, 250f);
            Motion.Anchored(_eraser, high, 0.22f, Ease.OutQuad).OnComplete(() =>
            {
                Motion.Anchored(_eraser, far, 0.38f, Ease.InOutQuad).OnComplete(() =>
                {
                    Motion.Anchored(_eraser, _eraserRest, 0.24f, Ease.OutQuad).OnComplete(() => _wiping = false);
                });
            });
        }

        Image Flat(string name, Color color)
        {
            var found = FindNamed(name);
            Image image = found != null ? found.GetComponent<Image>() : null;
            if (image == null)
                image = UiFactory.Picture(name, _board, FlatSprite(), color, false, false);
            image.sprite = FlatSprite();
            image.color = color;
            image.type = Image.Type.Simple;
            image.preserveAspect = false;
            image.raycastTarget = false;
            return image;
        }

        Transform FindNamed(string name)
        {
            if (_board == null)
                return null;
            var all = _board.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i].name == name)
                    return all[i];
            }
            return null;
        }

        static void PinBottom(RectTransform rect, float y, float height, float inset)
        {
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.offsetMin = new Vector2(inset, y);
            rect.offsetMax = new Vector2(-inset, y + height);
        }

        static void PinBottomSize(RectTransform rect, float width, float height, float x, float y)
        {
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = new Vector2(x, y);
        }

        void EnsureTray()
        {
        }

        bool InsideLane(Vector2 point, out Vector2 anchored)
        {
            anchored = point;
            float limit = LaneLimit();
            float best = limit * limit;
            bool hit = false;
            Vector2 nearest = point;
            for (int s = 0; s < _guideStrokes.Count; s++)
            {
                var path = _guideStrokes[s];
                for (int i = 1; i < path.Count; i++)
                {
                    Vector2 on = Closest(path[i - 1], path[i], point);
                    float dist = (on - point).sqrMagnitude;
                    if (dist > best)
                        continue;
                    best = dist;
                    nearest = on;
                    hit = true;
                }
            }
            if (!hit)
                return false;
            anchored = Vector2.Lerp(point, nearest, 0.4f);
            return true;
        }

        float LaneLimit()
        {
            float half = Mathf.Max(96f, BoardWidth() * 0.16f) * 0.5f;
            return half / Mathf.Max(1f, BoardWidth());
        }

        static Vector2 Closest(Vector2 a, Vector2 b, Vector2 point)
        {
            Vector2 ab = b - a;
            float denom = ab.sqrMagnitude;
            if (denom < 0.0000001f)
                return a;
            float t = Mathf.Clamp01(Vector2.Dot(point - a, ab) / denom);
            return a + ab * t;
        }

        void HideGhost()
        {
            if (_ghost != null)
                _ghost.gameObject.SetActive(false);
        }

        void EnsureTrail()
        {
            if (_trail != null || _board == null)
                return;
            _trail = UiFactory.Rect("Trait", _board);
            UiFactory.Stretch(_trail, 36f, 36f, 36f, 36f);
        }

        void PaintInk()
        {
            EnsureTrail();
            if (_trail == null)
                return;
            _trail.SetAsLastSibling();
            if (_symbol != null)
                _symbol.transform.SetAsLastSibling();
            var item = CurrentItem();
            Color ink = item != null ? item.SymbolColor : MontessoriPalette.ConsonantRose;
            _shown.Clear();
            for (int g = 0; g < _gestures.Count; g++)
            {
                var gesture = _gestures[g];
                if (gesture.Count == 0)
                    continue;
                _shown.Add(gesture[0]);
                float walked = 0f;
                for (int i = 1; i < gesture.Count; i++)
                {
                    walked += Vector2.Distance(gesture[i - 1], gesture[i]);
                    if (walked < 0.012f)
                        continue;
                    walked = 0f;
                    _shown.Add(gesture[i]);
                }
                if (_shown.Count == 0 || _shown[_shown.Count - 1] != gesture[gesture.Count - 1])
                    _shown.Add(gesture[gesture.Count - 1]);
            }
            while (_marks.Count < _shown.Count)
            {
                var mark = UiFactory.Picture("Trait", _trail, _pearl, ink, false, false);
                _marks.Add(mark);
            }
            for (int i = 0; i < _marks.Count; i++)
            {
                bool on = i < _shown.Count;
                _marks[i].gameObject.SetActive(on);
                if (!on)
                    continue;
                _marks[i].color = ink;
                UiFactory.AnchorCenter(_marks[i].rectTransform, NormToLocal(_shown[i]), new Vector2(54f, 54f));
            }
        }

        void ClearTrail()
        {
            for (int i = 0; i < _marks.Count; i++)
            {
                if (_marks[i] != null)
                    _marks[i].gameObject.SetActive(false);
            }
        }

        static void EnsureDrawn(Graphic graphic)
        {
            if (graphic == null)
                return;
            if (graphic.GetComponent<CanvasRenderer>() == null)
                graphic.gameObject.AddComponent<CanvasRenderer>();
            graphic.enabled = true;
            graphic.raycastTarget = false;
        }

        void BuildGuide(LearningItem item)
        {
            _guideStrokes.Clear();
            _flatGuide.Clear();
            StrokePath[] strokes = item != null && !string.IsNullOrEmpty(item.ItemId)
                ? StrokeLibrary.For(item.ItemId)
                : null;
            if (strokes == null || strokes.Length == 0)
                strokes = StrokeLibrary.For("shape-circle");

            for (int i = 0; i < strokes.Length; i++)
            {
                var stroke = strokes[i];
                if (stroke == null || stroke.Points == null || stroke.Points.Length < 2)
                    continue;
                _smooth.Clear();
                for (int p = 0; p < stroke.Points.Length; p++)
                    _smooth.Add(stroke.Points[p]);
                var sampled = new List<Vector2>();
                PolylineMath.Resample(_smooth, 0.012f, sampled);
                if (sampled.Count < 2)
                    continue;
                _guideStrokes.Add(sampled);
                for (int p = 0; p < sampled.Count; p++)
                    _flatGuide.Add(sampled[p]);
            }
        }

        void RenderGuide(Color ink)
        {
            if (_guide == null)
                return;
            EnsureDrawn(_guide);
            _mesh.Clear();
            for (int i = 0; i < _guideStrokes.Count; i++)
                _mesh.Add(ToLocal(_guideStrokes[i]));
            var lane = new Color(0.93f, 0.96f, 0.91f, 0.55f);
            float width = Mathf.Max(96f, BoardWidth() * 0.16f);
            _guide.SetPaths(_mesh, width, lane);
            var spine = EnsureSpine();
            if (spine == null)
                return;
            EnsureDrawn(spine);
            var chalk = Color.Lerp(new Color(0.97f, 0.98f, 0.94f, 1f), ink, 0.35f);
            chalk.a = 0.92f;
            spine.SetPaths(_mesh, Mathf.Max(18f, width * 0.2f), chalk);
        }

        RibbonGraphic EnsureSpine()
        {
            if (_guide == null)
                return null;
            var found = _guide.transform.Find("Milieu");
            if (found != null)
                return found.GetComponent<RibbonGraphic>();
            var spineGo = UiFactory.Rect("Milieu", _guide.transform);
            UiFactory.Stretch(spineGo, 0f, 0f, 0f, 0f);
            return spineGo.gameObject.AddComponent<RibbonGraphic>();
        }

        void RenderInk(Color color)
        {
            if (_ink == null)
                return;
            _mesh.Clear();
            for (int i = 0; i < _gestures.Count; i++)
            {
                if (_gestures[i].Count < 2)
                    continue;
                _smooth.Clear();
                PolylineMath.Smooth(_gestures[i], _smooth, 3);
                _mesh.Add(ToLocal(_smooth));
            }
            _ink.SetPaths(_mesh, BoardWidth() * 0.05f, color);
        }

        void PlaceDots(LearningItem item)
        {
            if (_dots == null)
                return;
            for (int i = _dots.childCount - 1; i >= 0; i--)
                Destroy(_dots.GetChild(i).gameObject);
            Color color = item != null ? item.SymbolColor : MontessoriPalette.Sun;
            Color chalk = new Color(1f, 0.97f, 0.93f, 0.96f);
            for (int s = 0; s < _guideStrokes.Count; s++)
            {
                var path = _guideStrokes[s];
                if (path.Count == 0)
                    continue;
                Vector2 at = NormToLocal(path[0]);
                var halo = UiFactory.Picture("Depart", _dots, _pearl, chalk, false, false);
                UiFactory.AnchorCenter(halo.rectTransform, at, new Vector2(40f, 40f));
                var dot = UiFactory.Picture("Depart", _dots, _pearl, color, false, false);
                UiFactory.AnchorCenter(dot.rectTransform, at, new Vector2(s == 0 ? 24f : 18f, s == 0 ? 24f : 18f));
            }
        }

        bool TryNormScreen(Vector2 screen, Camera cam, out Vector2 normalized)
        {
            normalized = Vector2.zero;
            var area = GuideRect();
            if (area == null)
                return false;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(area, screen, cam, out var local))
                return false;
            var rect = area.rect;
            if (rect.width <= 1f || rect.height <= 1f)
                return false;
            normalized = new Vector2(
                Mathf.InverseLerp(rect.xMin, rect.xMax, local.x),
                Mathf.InverseLerp(rect.yMin, rect.yMax, local.y));
            return true;
        }

        Camera EventCamera()
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                return canvas.worldCamera;
            return null;
        }

        static bool Contains(RectTransform rect, Vector2 screen, Camera cam)
        {
            return rect != null && RectTransformUtility.RectangleContainsScreenPoint(rect, screen, cam);
        }

        static bool Pressed(SimpleClick click, Vector2 screen, Camera cam)
        {
            if (click == null)
                return false;
            var own = click.transform as RectTransform;
            var parent = click.transform.parent as RectTransform;
            if (Contains(parent, screen, cam) || Contains(own, screen, cam))
                return true;
            return false;
        }

        void Evaluate()
        {
            if (_success || _guideStrokes.Count == 0 || _flatGuide.Count == 0)
                return;
            _flatUser.Clear();
            for (int i = 0; i < _gestures.Count; i++)
            {
                for (int p = 0; p < _gestures[i].Count; p++)
                    _flatUser.Add(_gestures[i][p]);
            }
            if (_flatUser.Count < 16)
                return;
            float guideLength = 0f;
            for (int i = 0; i < _guideStrokes.Count; i++)
                guideLength += PolylineMath.Length(_guideStrokes[i]);
            if (PolylineMath.Length(_flatUser) < guideLength * 0.7f)
                return;
            for (int i = 0; i < _guideStrokes.Count; i++)
            {
                if (PolylineMath.Recall(_guideStrokes[i], _flatUser, 0.11f) < 0.78f)
                    return;
            }
            if (PolylineMath.Precision(_flatGuide, _flatUser, 0.12f) < 0.5f)
                return;
            Succeed();
        }

        void Succeed()
        {
            _success = true;
            _advancing = true;
            var item = CurrentItem();
            var category = _request != null ? _request.Category : null;
            bool wasDone = item != null && category != null && LearningProgress.IsDone(category.CategoryId, GameId, item.ItemId);
            if (item != null && category != null)
                LearningProgress.Mark(category.CategoryId, GameId, item.ItemId);
            bool allNow = category != null && category.ItemCount > 0 && LearningProgress.CountDone(category, GameId) >= category.ItemCount;
            Color ink = item != null ? item.SymbolColor : MontessoriPalette.Success;
            RenderInk(ink);
            if (_instruction != null)
                _instruction.text = "Magnifique";
            WoodenAudio.PlaySuccess();
            FrenchVoice.SayItem(item);
            Motion.Delayed(0.85f, () => FrenchVoice.SayWord(item));
            if (MontessoriApp.Instance != null && _board != null)
                MontessoriApp.Instance.Sparkle(_board.position, ink);
            Motion.PunchScale(_board, 0.035f, 0.35f);

            if (!wasDone && allNow)
            {
                Motion.Float(0f, 1f, 0.05f, _ => { }, Ease.Linear, this, "next").SetDelay(0.7f).OnComplete(() =>
                {
                    if (!isActiveAndEnabled || MontessoriApp.Instance == null)
                        return;
                    MontessoriApp.Instance.Celebrate("Magnifique", "Tu as suivi tous les chemins.", item != null ? item.Symbol : string.Empty, ink, () =>
                    {
                        if (MontessoriApp.Instance != null)
                            MontessoriApp.Instance.ShowCategory();
                    });
                });
                return;
            }

            Motion.Float(0f, 1f, 0.05f, _ => { }, Ease.Linear, this, "next").SetDelay(1.05f).OnComplete(() =>
            {
                if (!isActiveAndEnabled)
                    return;
                var items = Items();
                if (items.Length == 0)
                    return;
                _index = (_index + 1) % items.Length;
                ShowItem();
            });
        }

        void ClearInk()
        {
            if (_success)
                return;
            _gestures.Clear();
            ClearTrail();
            if (_ink != null)
                _ink.ClearPaths();
            _idle = 0f;
        }

        void PlayHint()
        {
            if (_guideStrokes.Count == 0 || _hint == null || _success)
                return;
            StopHint();
            _hint.transform.SetAsLastSibling();
            _hint.rectTransform.sizeDelta = new Vector2(58f, 58f);
            _hint.gameObject.SetActive(true);
            var color = CurrentItem() != null ? CurrentItem().SymbolColor : MontessoriPalette.Sun;
            color.a = 1f;
            _hint.color = color;
            float duration = Mathf.Clamp(1.3f + _flatGuide.Count * 0.012f, 1.6f, 3.6f);
            Motion.Float(0f, 1f, duration, PlaceHint, Ease.InOutSine, this, "hint").OnComplete(HideHint);
        }

        void PlaceHint(float u)
        {
            if (_hint == null || _guideStrokes.Count == 0)
                return;
            _hint.transform.SetAsLastSibling();
            float scaled = Mathf.Clamp01(u) * _guideStrokes.Count;
            int stroke = Mathf.Clamp(Mathf.FloorToInt(scaled), 0, _guideStrokes.Count - 1);
            float along = scaled - stroke;
            var path = _guideStrokes[stroke];
            var point = PolylineMath.PointAt(path, PolylineMath.Length(path) * along);
            _hint.rectTransform.anchoredPosition = NormToLocal(point);
        }

        void StopHint()
        {
            Motion.Kill(this, "hint");
            HideHint();
        }

        void HideHint()
        {
            if (_hint != null)
                _hint.gameObject.SetActive(false);
        }

        Vector2 NormToLocal(Vector2 normalized)
        {
            var rect = GuideRect().rect;
            return new Vector2(
                Mathf.Lerp(rect.xMin, rect.xMax, normalized.x),
                Mathf.Lerp(rect.yMin, rect.yMax, normalized.y));
        }

        Vector2[] ToLocal(List<Vector2> normalized)
        {
            var local = new Vector2[normalized.Count];
            for (int i = 0; i < normalized.Count; i++)
                local[i] = NormToLocal(normalized[i]);
            return local;
        }

        RectTransform GuideRect()
        {
            if (_guide != null)
                return _guide.rectTransform;
            return _board;
        }

        float BoardWidth()
        {
            var rect = GuideRect() != null ? GuideRect().rect : new Rect(0f, 0f, 680f, 680f);
            return Mathf.Max(200f, rect.width);
        }

        int FirstIncomplete()
        {
            var items = Items();
            var category = _request != null ? _request.Category : null;
            if (category == null)
                return 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null && !LearningProgress.IsDone(category.CategoryId, GameId, items[i].ItemId))
                    return i;
            }
            return 0;
        }

        LearningItem CurrentItem()
        {
            var items = Items();
            if (_index < 0 || _index >= items.Length)
                return null;
            return items[_index];
        }

        LearningItem[] Items()
        {
            if (_request == null || _request.Category == null)
                return new LearningItem[0];
            return _request.Category.Items;
        }
    }
}
