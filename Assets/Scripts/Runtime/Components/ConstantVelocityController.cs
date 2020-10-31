using UnityEngine;

namespace TheCursedBroom.Components {
    public class ConstantVelocityController : MonoBehaviour {
        [SerializeField, Range(0, 10)]
        float startOffset = 1;
        [SerializeField]
        Vector2 velocity = Vector2.right;

        float timer = 0;

        void FixedUpdate() {
            if (timer < startOffset) {
                timer += Time.deltaTime;
            } else {
                transform.Translate(velocity * Time.deltaTime);
            }
        }
    }
}