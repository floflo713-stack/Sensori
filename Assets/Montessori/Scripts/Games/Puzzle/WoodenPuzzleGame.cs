using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public sealed class WoodenPuzzleGame : MiniGameController
    {
        [SerializeField] RectTransform _playfield;
        [SerializeField] Text _title;
        [SerializeField] Text _progress;
        [SerializeField] Text _instruction;

        readonly List<PuzzleSlot> _slots = new List<PuzzleSlot>();
        readonly List<WoodenPiece> _pieces = new List<WoodenPiece>();
        GameRequest _request;
        bool _categoryWasComplete;
        bool _roundResolved;
        Sprite _panel;
        Sprite _inset;
        Sprite _piece;
        Sprite _shadow;
        Sprite _pearl;
        Sprite _chip;

        public override string GameId => GameIds.Puzzle;

        public void Construct(ThemeAssets theme)
        {
            Remember(theme);
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);

            UiFactory.BuildGameHeader(transform, theme, out _title, out _progress);
            var stage = UiFactory.Rect("Plateau", transform);
            UiFactory.Stretch(stage, 36f, 28f, 36f, 132f);
            var inset = UiFactory.Picture("Bois", stage, theme.Inset, Color.white, true, false);
            UiFactory.Stretch(inset.rectTransform, 0f, 0f, 0f, 0f);
            inset.preserveAspect = false;
            _instruction = UiFactory.Label("Consigne", stage, "Pose chaque pièce dans son empreinte.", 26, MontessoriPalette.InkSoft, TextAnchor.UpperCenter);
            UiFactory.AnchorTop(_instruction.rectTransform, 48f, 24f, 24f);
            _playfield = UiFactory.Rect("Pieces", stage);
            UiFactory.Stretch(_playfield, 12f, 12f, 12f, 56f);
        }

        void Remember(ThemeAssets theme)
        {
            if (theme == null)
                return;
            _panel = theme.Panel;
            _inset = theme.Inset;
            _piece = theme.Piece;
            _shadow = theme.Shadow;
            _pearl = theme.Pearl;
            _chip = theme.Chip;
        }

        ThemeAssets Theme()
        {
            return new ThemeAssets
            {
                Panel = _panel,
                Inset = _inset,
                Piece = _piece,
                Shadow = _shadow,
                Pearl = _pearl,
                Chip = _chip
            };
        }

        public override void Begin(GameRequest request)
        {
            _request = request;
            if (_title != null)
            {
                string gameTitle = request != null && request.Definition != null ? request.Definition.Title : "Puzzle en bois";
                string category = request != null && request.Category != null ? request.Category.Title : string.Empty;
                _title.text = string.IsNullOrEmpty(category) ? gameTitle : gameTitle + " · " + category;
            }
            if (_instruction != null)
                _instruction.text = "Pose chaque pièce dans son empreinte.";
            _categoryWasComplete = IsCategoryComplete();
            SpawnRound();
        }

        public override void Close()
        {
            ClearSpawned();
        }

        void OnDisable()
        {
            ClearSpawned();
            Motion.KillOwner(this);
        }

        public void HighlightFor(WoodenPiece piece)
        {
            PuzzleSlot nearest = null;
            float best = float.MaxValue;
            for (int i = 0; i < _slots.Count; i++)
            {
                var slot = _slots[i];
                if (slot == null || slot.Filled || piece == null || slot.Item != piece.Item)
                {
                    if (slot != null)
                        slot.SetHot(false);
                    continue;
                }
                float distance = Vector2.Distance(piece.AnchoredPosition, slot.AnchoredPosition);
                bool hot = distance <= slot.Magnet * 1.35f;
                slot.SetHot(hot && (nearest == null || distance < best));
                if (distance < best)
                {
                    best = distance;
                    nearest = slot;
                }
            }
            for (int i = 0; i < _slots.Count; i++)
            {
                var slot = _slots[i];
                if (slot == null)
                    continue;
                slot.SetHot(slot == nearest && !slot.Filled && piece != null && slot.Item == piece.Item && best <= slot.Magnet * 1.35f);
            }
        }

        public void Release(WoodenPiece piece)
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i] != null)
                    _slots[i].SetHot(false);
            }
            if (piece == null || _roundResolved)
                return;
            PuzzleSlot best = null;
            float bestDistance = float.MaxValue;
            for (int i = 0; i < _slots.Count; i++)
            {
                var slot = _slots[i];
                if (slot == null || slot.Filled || slot.Item != piece.Item)
                    continue;
                float distance = Vector2.Distance(piece.AnchoredPosition, slot.AnchoredPosition);
                if (distance <= slot.Magnet && distance < bestDistance)
                {
                    best = slot;
                    bestDistance = distance;
                }
            }
            if (best == null)
            {
                piece.ReturnHome();
                return;
            }
            best.MarkFilled();
            piece.MoveTo(best.AnchoredPosition, true);
            WoodenAudio.PlayTap();
            if (_request != null && _request.Category != null && piece.Item != null)
                LearningProgress.Mark(_request.Category.CategoryId, GameId, piece.Item.ItemId);
            RefreshProgress();
            var app = MontessoriApp.Instance;
            if (app != null)
                app.Sparkle(piece.transform.position, piece.Item != null ? piece.Item.SymbolColor : MontessoriPalette.Sun);
            if (AllFilled())
                FinishRound();
        }

        void SpawnRound()
        {
            _roundResolved = false;
            ClearSpawned();
            Canvas.ForceUpdateCanvases();
            var category = _request != null ? _request.Category : null;
            if (category == null || _playfield == null)
                return;
            var chosen = ChooseItems(category);
            if (chosen.Count == 0)
                return;
            float width = _playfield.rect.width;
            float height = _playfield.rect.height;
            if (width < 100f)
                width = 1400f;
            if (height < 100f)
                height = 640f;
            int count = chosen.Count;
            float gap = 26f;
            float size = Mathf.Min(190f, (width - gap * (count + 1)) / count);
            size = Mathf.Max(120f, size);
            float ySlot = height * 0.18f;
            float yTray = -height * 0.22f;
            var order = new List<int>(count);
            for (int i = 0; i < count; i++)
                order.Add(i);
            for (int i = order.Count - 1; i > 0; i--)
            {
                int swap = Random.Range(0, i + 1);
                int tmp = order[i];
                order[i] = order[swap];
                order[swap] = tmp;
            }
            float total = count * size + (count - 1) * gap;
            float start = -total * 0.5f + size * 0.5f;
            var theme = Theme();
            WoodenPiece first = null;
            for (int i = 0; i < count; i++)
            {
                var item = chosen[i];
                float x = start + i * (size + gap);
                var slotRect = UiFactory.Rect("Emplacement", _playfield);
                UiFactory.AnchorCenter(slotRect, new Vector2(x, ySlot), new Vector2(size, size));
                var slot = slotRect.gameObject.AddComponent<PuzzleSlot>();
                slot.Build(slotRect, item, size, theme);
                _slots.Add(slot);

                int trayIndex = order[i];
                float trayX = start + trayIndex * (size + gap);
                var pieceRoot = UiFactory.Rect("Piece", _playfield);
                UiFactory.AnchorCenter(pieceRoot, new Vector2(trayX, yTray), new Vector2(size * 0.92f, size * 0.92f));
                var hit = UiFactory.Picture("Touche", pieceRoot, theme.Chip, new Color(1f, 1f, 1f, 0f), false, true);
                UiFactory.Stretch(hit.rectTransform, 0f, 0f, 0f, 0f);
                hit.preserveAspect = false;
                var visual = UiFactory.Rect("Visuel", pieceRoot);
                UiFactory.Stretch(visual, 0f, 0f, 0f, 0f);
                ItemPlate.Paint(visual, item, theme, false);
                var piece = pieceRoot.gameObject.AddComponent<WoodenPiece>();
                piece.Build(pieceRoot, hit, item, this, new Vector2(trayX, yTray));
                _pieces.Add(piece);
                if (first == null)
                    first = piece;
            }
            RefreshProgress();
            if (first != null)
                first.Nudge();
        }

        List<LearningItem> ChooseItems(LearningCategory category)
        {
            var fresh = new List<LearningItem>();
            var done = new List<LearningItem>();
            var items = category.Items;
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                if (item == null)
                    continue;
                if (LearningProgress.IsDone(category.CategoryId, GameId, item.ItemId))
                    done.Add(item);
                else
                    fresh.Add(item);
            }
            var source = fresh.Count > 0 ? fresh : done;
            int amount = Mathf.Min(category.PuzzleGroupSize, source.Count);
            var chosen = new List<LearningItem>(amount);
            if (fresh.Count > 0)
            {
                for (int i = 0; i < amount; i++)
                    chosen.Add(source[i]);
            }
            else
            {
                var pool = new List<LearningItem>(source);
                for (int i = 0; i < amount && pool.Count > 0; i++)
                {
                    int index = Random.Range(0, pool.Count);
                    chosen.Add(pool[index]);
                    pool.RemoveAt(index);
                }
            }
            return chosen;
        }

        void FinishRound()
        {
            if (_roundResolved)
                return;
            _roundResolved = true;
            bool completeNow = IsCategoryComplete();
            var app = MontessoriApp.Instance;
            if (app == null)
                return;
            bool finishedCategory = completeNow && !_categoryWasComplete;
            string subtitle = finishedCategory
                ? "Tu as tout rangé dans l'atelier."
                : "Les pièces ont trouvé leur place.";
            string button = finishedCategory ? "Continuer" : "Encore";
            Motion.Float(0f, 1f, 0.05f, _ => { }, Ease.Linear, this, "finale")
                .SetDelay(0.35f)
                .OnComplete(() =>
                {
                    if (!isActiveAndEnabled || MontessoriApp.Instance == null)
                        return;
                    MontessoriApp.Instance.Celebrate("Bravo !", subtitle, "", MontessoriPalette.Honey, () =>
                    {
                        if (!isActiveAndEnabled || MontessoriApp.Instance == null)
                            return;
                        if (completeNow && !_categoryWasComplete)
                            MontessoriApp.Instance.ShowCategory();
                        else
                        {
                            _categoryWasComplete = IsCategoryComplete();
                            SpawnRound();
                        }
                    }, button);
                });
        }

        bool AllFilled()
        {
            if (_slots.Count == 0)
                return false;
            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i] == null || !_slots[i].Filled)
                    return false;
            }
            return true;
        }

        bool IsCategoryComplete()
        {
            var category = _request != null ? _request.Category : null;
            if (category == null)
                return false;
            return LearningProgress.CountDone(category, GameId) >= category.ItemCount && category.ItemCount > 0;
        }

        void RefreshProgress()
        {
            var category = _request != null ? _request.Category : null;
            if (_progress == null || category == null)
                return;
            _progress.text = LearningProgress.CountDone(category, GameId) + " / " + category.ItemCount;
        }

        void ClearSpawned()
        {
            for (int i = 0; i < _pieces.Count; i++)
            {
                if (_pieces[i] == null)
                    continue;
                Motion.KillOwner(_pieces[i].transform);
                Destroy(_pieces[i].gameObject);
            }
            _pieces.Clear();
            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i] != null)
                    Destroy(_slots[i].gameObject);
            }
            _slots.Clear();
        }
    }
}
