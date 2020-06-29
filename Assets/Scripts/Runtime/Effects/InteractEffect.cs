using TheCursedBroom.Level;
using UnityEngine;

namespace TheCursedBroom.Effects {
    [CreateAssetMenu(fileName = "Interact_New", menuName = "Effects/Interact")]
    public class InteractEffect : Effect {
        public override void Invoke(GameObject context) {
            if (context.TryGetComponent(out Interactable interactable)) {
                interactable.Interact();
            }
        }
    }
}
