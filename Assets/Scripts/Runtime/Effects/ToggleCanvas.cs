using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "ToggleCanvas_New", menuName = "Effects/Toggle Canvas")]
    public class ToggleCanvas : Effect {
        [SerializeField, Expandable]
        GameObject canvasPrefab = default;
        [SerializeField]
        bool autoSelectFirst = true;

        GameObject canvas;
        public override void Invoke(GameObject context) {
            if (canvas) {
                Destroy(canvas);
            } else {
                canvas = Instantiate(canvasPrefab);

                if (autoSelectFirst) {
                    var firstElement = canvas
                        .GetComponentsInChildren<Selectable>()
                        .FirstOrDefault(selectable => selectable.IsInteractable());
                    var eventSystem = FindObjectOfType<EventSystem>();
                    if (firstElement && eventSystem) {
                        eventSystem.SetSelectedGameObject(firstElement.gameObject);
                    }
                }
            }
        }
    }
}