using UnityEngine;

namespace TheCursedBroom {
    public class NoRotation : MonoBehaviour {
        [Header("No Rotation")]
        [SerializeField]
        Quaternion targetRotation = Quaternion.identity;

        void LateUpdate() {
            gameObject.transform.rotation = targetRotation;
        }
    }
}
