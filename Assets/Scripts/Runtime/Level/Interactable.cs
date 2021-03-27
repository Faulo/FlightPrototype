using UnityEngine;

namespace TheCursedBroom.Level {
    public class Interactable : MonoBehaviour {
        [SerializeField]
        bool onlyInteractOnce = false;
        [SerializeField]
        GameObjectEvent onInteract = default;

        public void Interact() {
            onInteract.Invoke(gameObject);
            if (onlyInteractOnce) {
                Destroy(this);
            }
        }
    }
}