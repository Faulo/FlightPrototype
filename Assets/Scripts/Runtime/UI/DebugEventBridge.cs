using TheCursedBroom.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheCursedBroom.UI {
    public class DebugEventBridge : MonoBehaviour, Controls.IDebugActions {
        [SerializeField]
        GameObjectEvent onF1 = default;
        [SerializeField]
        GameObjectEvent onF2 = default;
        [SerializeField]
        GameObjectEvent onF3 = default;
        [SerializeField]
        GameObjectEvent onF4 = default;
        [SerializeField]
        GameObjectEvent onF5 = default;
        [SerializeField]
        GameObjectEvent onF6 = default;
        [SerializeField]
        GameObjectEvent onF7 = default;
        [SerializeField]
        GameObjectEvent onF8 = default;
        [SerializeField]
        GameObjectEvent onF9 = default;
        [SerializeField]
        GameObjectEvent onF10 = default;
        [SerializeField]
        GameObjectEvent onF11 = default;
        [SerializeField]
        GameObjectEvent onF12 = default;

        Controls controls;
        void Awake() {
            controls = new Controls();
            controls.Debug.SetCallbacks(this);
        }
        public void OnEnable() {
            controls.Debug.Enable();
        }
        public void OnDisable() {
            controls.Debug.Disable();
        }

        public void OnF1(InputAction.CallbackContext context) {
            if (context.performed) {
                onF1.Invoke(gameObject);
            }
        }

        public void OnF2(InputAction.CallbackContext context) {
            if (context.performed) {
                onF2.Invoke(gameObject);
            }
        }

        public void OnF3(InputAction.CallbackContext context) {
            if (context.performed) {
                onF3.Invoke(gameObject);
            }
        }

        public void OnF4(InputAction.CallbackContext context) {
            if (context.performed) {
                onF4.Invoke(gameObject);
            }
        }

        public void OnF5(InputAction.CallbackContext context) {
            if (context.performed) {
                onF5.Invoke(gameObject);
            }
        }

        public void OnF6(InputAction.CallbackContext context) {
            if (context.performed) {
                onF6.Invoke(gameObject);
            }
        }

        public void OnF7(InputAction.CallbackContext context) {
            if (context.performed) {
                onF7.Invoke(gameObject);
            }
        }

        public void OnF8(InputAction.CallbackContext context) {
            if (context.performed) {
                onF8.Invoke(gameObject);
            }
        }

        public void OnF9(InputAction.CallbackContext context) {
            if (context.performed) {
                onF9.Invoke(gameObject);
            }
        }

        public void OnF10(InputAction.CallbackContext context) {
            if (context.performed) {
                onF10.Invoke(gameObject);
            }
        }

        public void OnF11(InputAction.CallbackContext context) {
            if (context.performed) {
                onF11.Invoke(gameObject);
            }
        }

        public void OnF12(InputAction.CallbackContext context) {
            if (context.performed) {
                onF12.Invoke(gameObject);
            }
        }
    }
}