using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
            if (!isActiveAndEnabled || _success || _advancing || _pointer >= 0)
                return;
            _idle += Time.unscaledDeltaTime;
            if (_idle > 7f)
            {
                _idle = 0f;
                PlayHint();
            }
        }

        public void PointerDown(PointerEventData eventData)
        {
            if (_success || _advancing || eventData.button != PointerEventData.InputButton.Left)
                return;
            if (_pointer >= 0)
                return;
            _pointer = eventData.pointerId;
            _idle = 0f;
            StopHint();
            var gesture = new List<Vector2>(64);
            _gestures.Add(gesture);
            AddPoint(eventData, gesture);
        }

        public void PointerDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != _pointer || _gestures.Count == 0)
                return;
            AddPoint(eventData, _gestures[_gestures.Count - 1]);
        }

        public void PointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != _pointer)
                return;
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
            _idle = 0f;
            _gestures.Clear();
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
            if (_boardFace != null && item != null)
                _boardFace.color = Color.Lerp(Color.white, item.SymbolColor, item.Visual == ItemVisual.Swatch ? 0.55f : 0.12f);
            BuildGuide(item);
            RenderGuide();
            PlaceDots(item);
            PlayHint();
        }

        void BuildGuide(LearningItem item)
        {
            _guideStrokes.Clear();
            _flatGuide.Clear();
            StrokePath[] strokes = item != null ? item.Strokes : null;
            bool missing = strokes == null || strokes.Length == 0;
            if (!missing)
            {
                missing = true;
                for (int i = 0; i < strokes.Length; i++)
                {
                    if (strokes[i] != null && strokes[i].Points != null && strokes[i].Points.Length >= 2)
                    {
                        missing = false;
                        break;
                    }
                }
            }
            if (missing)
                strokes = StrokeLibrary.For("shape-circle");

            for (int i = 0; i < strokes.Length; i++)
            {
                var stroke = strokes[i];
                if (stroke == null || stroke.Points == null || stroke.Points.Length < 2)
                    continue;
                _smooth.Clear();
                PolylineMath.Smooth(stroke.Points, _smooth, 4);
                var sampled = new List<Vector2>();
                PolylineMath.Resample(_smooth, 0.02f, sampled);
                if (sampled.Count < 2)
                    continue;
                _guideStrokes.Add(sampled);
                for (int p = 0; p < sampled.Count; p++)
                    _flatGuide.Add(sampled[p]);
            }
        }

        void RenderGuide()
        {
            if (_guide == null)
                return;
            _mesh.Clear();
            for (int i = 0; i < _guideStrokes.Count; i++)
                _mesh.Add(ToLocal(_guideStrokes[i]));
            float width = BoardWidth() * 0.065f;
            _guide.SetPaths(_mesh, width, MontessoriPalette.WithAlpha(MontessoriPalette.WalnutDeep, 0.55f));
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
            for (int i = 0; i < _guideStrokes.Count; i++)
            {
                if (_guideStrokes[i].Count == 0)
                    continue;
                var dot = UiFactory.Picture("Depart", _dots, _pearl, color, false, false);
                UiFactory.AnchorCenter(dot.rectTransform, NormToLocal(_guideStrokes[i][0]), new Vector2(i == 0 ? 28f : 18f, i == 0 ? 28f : 18f));
            }
        }

        void AddPoint(PointerEventData eventData, List<Vector2> gesture)
        {
            if (!TryNorm(eventData, out var point))
                return;
            if (gesture.Count > 0 && (gesture[gesture.Count - 1] - point).sqrMagnitude < 0.00008f)
                return;
            if (gesture.Count > 700)
                return;
            gesture.Add(point);
            var item = CurrentItem();
            Color ink = item != null ? item.SymbolColor : MontessoriPalette.Ink;
            RenderInk(ink);
        }

        void Evaluate()
        {
            if (_success || _flatGuide.Count == 0)
                return;
            _flatUser.Clear();
            int count = 0;
            for (int i = 0; i < _gestures.Count; i++)
            {
                count += _gestures[i].Count;
                for (int p = 0; p < _gestures[i].Count; p++)
                    _flatUser.Add(_gestures[i][p]);
            }
            if (count < 8)
                return;
            float recall = PolylineMath.Recall(_flatGuide, _flatUser, 0.085f);
            float precision = PolylineMath.Precision(_flatGuide, _flatUser, 0.1f);
            if (recall < 0.74f || precision < 0.4f)
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
            if (_ink != null)
                _ink.ClearPaths();
            _idle = 0f;
        }

        void PlayHint()
        {
            if (_guideStrokes.Count == 0 || _hint == null || _success)
                return;
            StopHint();
            _hint.gameObject.SetActive(true);
            var color = _hint.color;
            color.a = 1f;
            _hint.color = color;
            float duration = Mathf.Clamp(1.3f + _flatGuide.Count * 0.012f, 1.6f, 3.6f);
            Motion.Float(0f, 1f, duration, PlaceHint, Ease.InOutSine, this, "hint").OnComplete(HideHint);
        }

        void PlaceHint(float u)
        {
            if (_hint == null || _guideStrokes.Count == 0)
                return;
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

        bool TryNorm(PointerEventData eventData, out Vector2 normalized)
        {
            normalized = Vector2.zero;
            if (_board == null)
                return false;
            var area = GuideRect();
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(area, eventData.position, eventData.pressEventCamera, out var local))
                return false;
            var rect = area.rect;
            if (rect.width <= 1f || rect.height <= 1f)
                return false;
            normalized = new Vector2(
                Mathf.InverseLerp(rect.xMin, rect.xMax, local.x),
                Mathf.InverseLerp(rect.yMin, rect.yMax, local.y));
            return true;
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
