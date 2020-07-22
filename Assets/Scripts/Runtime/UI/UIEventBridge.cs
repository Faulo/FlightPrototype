using TheCursedBroom.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheCursedBroom.UI {
    public class UIEventBridge : MonoBehaviour, Controls.IUIActions {
        [SerializeField]
        GameObjectEvent onLeftClick = default;
        [SerializeField]
        GameObjectEvent onMiddleClick = default;
        [SerializeField]
        GameObjectEvent onRightClick = default;
        [SerializeField]
        GameObjectEvent onScrollUp = default;
        [SerializeField]
        GameObjectEvent onScrollDown = default;

        Controls controls;
        void Awake() {
            controls = new Controls();
            controls.UI.SetCallbacks(this);
        }
        public void OnEnable() {
            controls.UI.Enable();
        }
        public void OnDisable() {
            controls.UI.Disable();
        }

        public void OnNavigate(InputAction.CallbackContext context) {
        }

        public void OnSubmit(InputAction.CallbackContext context) {
        }

        public void OnCancel(InputAction.CallbackContext context) {
        }

        public void OnPoint(InputAction.CallbackContext context) {
        }

        public void OnScrollWheel(InputAction.CallbackContext context) {
            var input = context.ReadValue<Vector2>();
            if (input.y > 0) {
                onScrollUp.Invoke(gameObject);
            }
            if (input.y < 0) {
                onScrollDown.Invoke(gameObject);
            }
        }

        public void OnLeftClick(InputAction.CallbackContext context) {
            if (context.performed) {
                onLeftClick.Invoke(gameObject);
            }
        }

        public void OnMiddleClick(InputAction.CallbackContext context) {
            if (context.performed) {
                onMiddleClick.Invoke(gameObject);
            }
        }

        public void OnRightClick(InputAction.CallbackContext context) {
            if (context.performed) {
                onRightClick.Invoke(gameObject);
            }
        }
    }
}