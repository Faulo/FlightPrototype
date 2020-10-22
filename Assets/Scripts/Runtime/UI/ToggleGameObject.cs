using UnityEngine;

namespace TheCursedBroom.UI {
    public class ToggleGameObject : MonoBehaviour {
        public void ToggleActive() {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}