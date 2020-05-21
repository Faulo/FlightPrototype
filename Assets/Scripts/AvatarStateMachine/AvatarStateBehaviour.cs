using UnityEngine;

namespace AvatarStateMachine {
    public abstract class AvatarStateBehaviour : MonoBehaviour {
        [Header("Rigidbody configuration")]
        [SerializeField, Range(0, 2)]
        float gravity = 1;
        [SerializeField, Range(0, 10)]
        float drag = 0;

        [Header("State visualization")]
        [SerializeField, ColorUsage(true, true)]
        public Color stateColor = default;

        public virtual void EnterState(Avatar avatar) {
            avatar.attachedRigidbody.gravityScale = gravity;
            avatar.attachedRigidbody.drag = drag;
        }
        public virtual void UpdateState(Avatar avatar) {
        }
        public virtual void FixedUpdateState(Avatar avatar) {
        }
        public virtual void ExitState(Avatar avatar) {

        }

        public abstract bool ShouldTransitionToGliding(Avatar avatar);
        public abstract bool ShouldTransitionToJumping(Avatar avatar);
        public abstract bool ShouldTransitionToAirborne(Avatar avatar);
        public abstract bool ShouldTransitionToGrounded(Avatar avatar);
    }
}