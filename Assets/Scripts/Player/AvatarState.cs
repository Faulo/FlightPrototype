using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace TheCursedBroom.Player {
    public abstract class AvatarState : MonoBehaviour {
        protected Avatar avatar;

        [Header("Avatar parameters")]
        [SerializeField]
        AvatarAnimations animatorState = default;
        [SerializeField, Range(0, 2)]
        protected float gravity = 1;
        [SerializeField, Range(0, 10)]
        protected float drag = 0;
        [SerializeField, Expandable]
        AvatarMovement movement = default;

        void Awake() {
            avatar = GetComponentInParent<Avatar>();
            Assert.IsNotNull(avatar);
        }
        void OnDisable() {
        }

        #region State
        public virtual void EnterState() {
            avatar.currentAnimation = animatorState;
            avatar.gravityScale = gravity;
            avatar.drag = drag;
            avatar.velocityCalculator = movement
                ? movement.CreateVelocityCalculator(avatar)
                : () => avatar.velocity;
        }
        public virtual void UpdateState() {
        }
        public virtual void FixedUpdateState() {
        }
        public virtual void ExitState() {
        }
        public abstract AvatarState CalculateNextState();
        #endregion
    }
}