using System;
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

    public sealed class SimpleClick : MonoBehaviour
    {
        public event Action Clicked;
        Button _button;

        void OnEnable()
        {
            _button = UiFactory.CreateButton(gameObject);
            _button.onClick.RemoveListener(Raise);
            _button.onClick.AddListener(Raise);
        }

        void OnDisable()
        {
            if (_button != null)
                _button.onClick.RemoveListener(Raise);
        }

        void Raise()
        {
            Clicked?.Invoke();
        }
    }

    public sealed class NavigationButton : MonoBehaviour
    {
        [SerializeField] NavigationTarget _target = NavigationTarget.Home;
        Button _button;

        public void Configure(NavigationTarget target)
        {
            _target = target;
        }

        void OnEnable()
        {
            _button = UiFactory.CreateButton(gameObject);
            UiClick.Wire(_button, Navigate);
        }

        void OnDisable()
        {
            if (_button != null)
                _button.onClick.RemoveListener(Navigate);
        }

        public void Navigate()
        {
            var app = MontessoriApp.Instance;
            if (app == null)
                return;
            if (_target == NavigationTarget.Home)
                app.ShowHome();
            else
                app.ShowCategory();
        }
    }

    public sealed class CategoryCard : MonoBehaviour
    {
        [SerializeField] LearningCategory _category;
        [SerializeField] Text _title;
        [SerializeField] Text _count;
        [SerializeField] RectTransform _root;
        [SerializeField] RectTransform _icon;
        [SerializeField] CanvasGroup _group;

        public LearningCategory Category => _category;
        public RectTransform Root => _root != null ? _root : (RectTransform)transform;
        public RectTransform Icon => _icon;
        public CanvasGroup Group => _group;

        public void Bind(LearningCategory category, Text title, Text count, RectTransform root, RectTransform icon, CanvasGroup group)
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
            int done = LearningProgress.CountDiscovered(_category);
            if (done <= 0)
            {
                _count.text = total + " " + _category.CountLabel;
                _count.color = MontessoriPalette.InkSoft;
            }
            else if (done >= total && total > 0)
            {
                _count.text = "Terminé";
                _count.color = MontessoriPalette.Success;
            }
            else
            {
                _count.text = done + " / " + total;
                _count.color = MontessoriPalette.InkSoft;
            }
        }

        Button _button;

        void OnEnable()
        {
            _button = UiFactory.CreateButton(gameObject);
            UiClick.Wire(_button, Open);
        }

        void OnDisable()
        {
            if (_button != null)
                _button.onClick.RemoveListener(Open);
        }

        public void Open()
        {
            if (_category == null || MontessoriApp.Instance == null)
                return;
            MontessoriApp.Instance.OpenCategory(_category);
        }
    }

    public sealed class GameCard : MonoBehaviour
    {
        [SerializeField] MiniGameDefinition _definition;
        [SerializeField] Text _title;
        [SerializeField] Text _description;
        [SerializeField] RectTransform _root;
        [SerializeField] CanvasGroup _group;

        public MiniGameDefinition Definition => _definition;
        public RectTransform Root => _root != null ? _root : (RectTransform)transform;
        public CanvasGroup Group => _group;
        public string GameId => _definition != null ? _definition.GameId : string.Empty;

        public void Bind(MiniGameDefinition definition, Text title, Text description, RectTransform root, CanvasGroup group)
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

        Button _button;

        void OnEnable()
        {
            _button = UiFactory.CreateButton(gameObject);
            UiClick.Wire(_button, Launch);
        }

        void OnDisable()
        {
            if (_button != null)
                _button.onClick.RemoveListener(Launch);
        }

        public void Launch()
        {
            if (_definition == null || MontessoriApp.Instance == null)
                return;
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
            if (button.onClick.GetPersistentEventCount() > 0)
                return;
            button.onClick.RemoveListener(action);
            button.onClick.AddListener(action);
        }
    }
}
