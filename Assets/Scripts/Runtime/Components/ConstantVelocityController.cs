using UnityEngine;

namespace TheCursedBroom.Components {
    public class ConstantVelocityController : MonoBehaviour {
        [SerializeField]
        Vector2 velocity = Vector2.right;

        void FixedUpdate() {
            transform.Translate(velocity * Time.deltaTime);
        }
    }
}