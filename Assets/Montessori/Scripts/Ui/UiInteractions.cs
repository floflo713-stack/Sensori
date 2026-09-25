using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sensori.Montessori
{
    public enum NavigationTarget
    {
        Home,
        Category
    }

    public sealed class Pressable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        bool _pressed;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;
            _pressed = true;
            Motion.Scale(transform, Vector3.one * 0.96f, 0.08f, Ease.OutQuad);
            WoodenAudio.PlayTap();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_pressed)
                return;
            _pressed = false;
            Motion.Scale(transform, Vector3.one, 0.18f, Ease.OutBack);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_pressed)
                return;
            _pressed = false;
            Motion.Scale(transform, Vector3.one, 0.16f, Ease.OutQuad);
        }
    }

    public sealed class SimpleClick : MonoBehaviour, IPointerDownHandler
    {
        public event Action Clicked;

        void OnEnable()
        {
            var image = GetComponent<Image>();
            if (image != null)
                image.raycastTarget = true;
            if (!Application.isPlaying)
                return;
            var button = GetComponent<Button>();
            if (button != null)
                Destroy(button);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData != null && eventData.button != PointerEventData.InputButton.Left)
                return;
            Clicked?.Invoke();
        }
    }

    public sealed class NavigationButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] NavigationTarget _target = NavigationTarget.Home;
        Button _button;
        int _actedFrame = -1;

        public void Configure(NavigationTarget target)
        {
            _target = target;
        }

        void OnEnable()
        {
            var image = GetComponent<Image>();
            if (image != null)
                image.raycastTarget = true;
            _button = UiFactory.CreateButton(gameObject);
            UiClick.Wire(_button, Navigate);
        }

        void OnDisable()
        {
            if (_button != null)
                _button.onClick.RemoveListener(Navigate);
        }

        void Update()
        {
            if (!isActiveAndEnabled || !Application.isPlaying)
                return;
            var pointer = UnityEngine.InputSystem.Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame)
                return;
            var rect = transform as RectTransform;
            if (rect == null)
                return;
            var canvas = GetComponentInParent<Canvas>();
            Camera cam = canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;
            if (!RectTransformUtility.RectangleContainsScreenPoint(rect, pointer.position.ReadValue(), cam))
                return;
            Navigate();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData != null && eventData.button != PointerEventData.InputButton.Left)
                return;
            Navigate();
        }

        public void Navigate()
        {
            if (_actedFrame == Time.frameCount)
                return;
            _actedFrame = Time.frameCount;
            var app = MontessoriApp.Instance;
            if (app == null)
                return;
            if (_target == NavigationTarget.Home)
                app.ShowHome();
            else
                app.ShowCategory();
        }
    }

    public sealed class CategoryCard : MonoBehaviour, IPointerClickHandler
    {
        static readonly Color Apple = new Color(0.435f, 0.859f, 0.184f, 1f);
        [SerializeField] LearningCategory _category;
        [SerializeField] TMP_Text _title;
        [SerializeField] TMP_Text _count;
        [SerializeField] RectTransform _root;
        [SerializeField] RectTransform _icon;
        [SerializeField] CanvasGroup _group;

        public LearningCategory Category => _category;
        public RectTransform Root => _root != null ? _root : (RectTransform)transform;
        public RectTransform Icon => _icon;
        public CanvasGroup Group => _group;

        public void Bind(LearningCategory category, TMP_Text title, TMP_Text count, RectTransform root, RectTransform icon, CanvasGroup group)
        {
            _category = category;
            _title = title;
            _count = count;
            _root = root;
            _icon = icon;
            _group = group;
        }

        public void Refresh()
        {
            if (_category == null)
                return;
            if (_title != null)
                _title.text = _category.Title;
            if (_count == null)
                return;
            int total = _category.ItemCount;
            if (_category.CategoryId == WordThemes.CategoryId && MontessoriApp.Instance != null)
                total = MontessoriApp.Instance.ImagierCardCount;
            int done = LearningProgress.CountDiscovered(_category);
            _count.fontStyle = FontStyles.Bold;
            if (done <= 0)
            {
                _count.text = total + " " + _category.CountLabel;
                _count.color = MontessoriPalette.Ink;
            }
            else if (done >= total && total > 0)
            {
                _count.text = "Terminé";
                _count.color = Apple;
            }
            else
            {
                _count.text = done + " / " + total;
                _count.color = Apple;
            }
        }

        int _openedFrame = -1;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData == null || eventData.button != PointerEventData.InputButton.Left)
                return;
            Open();
        }

        void OnEnable()
        {
            var button = GetComponent<Button>();
            if (button != null)
                button.onClick.RemoveListener(Open);
            var face = transform.Find("Face");
            var image = face != null ? face.GetComponent<Image>() : GetComponent<Image>();
            if (image != null)
                image.raycastTarget = true;
        }

        public void Open()
        {
            if (_openedFrame == Time.frameCount)
                return;
            if (_category == null || MontessoriApp.Instance == null)
                return;
            _openedFrame = Time.frameCount;
            MontessoriApp.Instance.OpenCategory(_category);
        }
    }

    public sealed class GameCard : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] MiniGameDefinition _definition;
        [SerializeField] TMP_Text _title;
        [SerializeField] TMP_Text _description;
        [SerializeField] RectTransform _root;
        [SerializeField] CanvasGroup _group;

        public MiniGameDefinition Definition => _definition;
        public RectTransform Root => _root != null ? _root : (RectTransform)transform;
        public CanvasGroup Group => _group;
        public string GameId => _definition != null ? _definition.GameId : string.Empty;

        public void Bind(MiniGameDefinition definition, TMP_Text title, TMP_Text description, RectTransform root, CanvasGroup group)
        {
            _definition = definition;
            _title = title;
            _description = description;
            _root = root;
            _group = group;
            Refresh();
        }

        public void Refresh()
        {
            if (_definition == null)
                return;
            if (_title != null)
                _title.text = _definition.Title;
            if (_description != null)
                _description.text = _definition.Description;
        }

        int _launchedFrame = -1;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData == null || eventData.button != PointerEventData.InputButton.Left)
                return;
            Launch();
        }

        void OnEnable()
        {
            var button = GetComponent<Button>();
            if (button != null)
                button.onClick.RemoveListener(Launch);
            var face = transform.Find("Face");
            var image = face != null ? face.GetComponent<Image>() : GetComponent<Image>();
            if (image != null)
                image.raycastTarget = true;
        }

        public void Launch()
        {
            if (_launchedFrame == Time.frameCount)
                return;
            if (_definition == null || MontessoriApp.Instance == null)
                return;
            _launchedFrame = Time.frameCount;
            MontessoriApp.Instance.StartGame(_definition);
        }
    }

    public static class UiClick
    {
        public static void Wire(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button == null || action == null)
                return;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                int count = button.onClick.GetPersistentEventCount();
                string method = action.Method.Name;
                for (int i = 0; i < count; i++)
                {
                    if (button.onClick.GetPersistentMethodName(i) == method)
                        return;
                }
                UnityEditor.Events.UnityEventTools.AddPersistentListener(button.onClick, action);
                return;
            }
#endif
            button.onClick.RemoveListener(action);
            button.onClick.AddListener(action);
        }
    }
}
