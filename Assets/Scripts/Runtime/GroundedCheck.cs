using UnityEngine;

namespace TheCursedBroom {
    public class GroundedCheck : MonoBehaviour {
        [SerializeField, Range(0, 10)]
        float defaultKinematicFriction = 1;
        [SerializeField, Range(0, 10)]
        float defaultStaticFriction = 1;

        Ground ground;
        Ground oldGround;
        public bool isGrounded => ground;

        public float kinematicFriction => ground
            ? ground.kinematicFriction
            : defaultKinematicFriction;
        public float staticFriction => ground
            ? ground.staticFriction
            : defaultStaticFriction;

        void OnTriggerEnter2D(Collider2D collider) => SetGround(collider);
        void OnTriggerStay2D(Collider2D collider) => SetGround(collider);
        void FixedUpdate() => ClearGround();
        void ClearGround() {
            if (oldGround) {
                oldGround = null;
            } else {
                ground = null;
            }
        }
        void SetGround(Component other) {
            if (other.TryGetComponent<Ground>(out var newGround)) {
                ground = oldGround = newGround;
            }
        }
    }
}