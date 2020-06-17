using TheCursedBroom.Player;
using UnityEngine;

namespace TheCursedBroom.Level {
    public class Interactable : MonoBehaviour {
        [SerializeField]
        GameObjectEvent onInteract = default;

        public void Interact() {
            onInteract.Invoke(gameObject);
        }
    }
}