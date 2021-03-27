using System.Linq;
using Slothsoft.UnityExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheCursedBroom.Player {
    public class ControlProvider : MonoBehaviour {
        [Header("MonoBehaviour configuration")]
        [SerializeField, Expandable]
        Canvas canvas = default;
        [SerializeField, Expandable]
        TextMeshProUGUI textContainer = default;

        [Header("MonoBehaviour configuration")]
        [SerializeField]
        SerializableKeyValuePairs<string, Sprite> gamepadSprites = new SerializableKeyValuePairs<string, Sprite>();
        [SerializeField]
        SerializableKeyValuePairs<string, Sprite> joystickSprites = new SerializableKeyValuePairs<string, Sprite>();
        [SerializeField]
        SerializableKeyValuePairs<string, string> fallbackNames = new SerializableKeyValuePairs<string, string>();

        [Header("Runtime")]
        [SerializeField]
        public InputActionReference action = default;

        void Start() {
            var control = LookupControl(action);
            if (TryLookupSprite(control, out var sprite)) {
                // sprites are rendered via TilemapRenderer
                textContainer.text = control.path;
                canvas.gameObject.SetActive(false);
            } else {
                // Fallback: Render Text
                textContainer.text = LookupPath(fallbackNames, control, out string name)
                    ? name
                    : control.displayName;
                canvas.gameObject.SetActive(true);
            }
        }

        public InputControl LookupControl(InputActionReference action) {
            return action.action.activeControl ?? action.action.controls
                .OrderByDescending(control => control.device.lastUpdateTime)
                .First();
        }

        public bool TryLookupSprite(InputControl control, out Sprite sprite) {
            if (control.device is Gamepad) {
                if (LookupPath(gamepadSprites, control, out sprite)) {
                    return true;
                }
            }
            if (control.device is Joystick) {
                if (LookupPath(joystickSprites, control, out sprite)) {
                    return true;
                }
            }
            sprite = null;
            return false;
        }

        bool LookupPath<T>(SerializableKeyValuePairs<string, T> paths, InputControl control, out T value) {
            foreach (string path in paths.Keys) {
                if (InputControlPath.Matches(path, control)) {
                    value = paths[path];
                    return true;
                }
            }
            foreach (string path in paths.Keys) {
                if (InputControlPath.Matches($"/*{path}", control)) {
                    value = paths[path];
                    return true;
                }
            }
            value = default;
            return false;
        }
    }
}