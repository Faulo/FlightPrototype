using UnityEngine;
using UnityEngine.Assertions;

namespace TheCursedBroom.Player {
    public abstract class AvatarState : MonoBehaviour {
        protected Avatar avatar { get; private set; }

        [Header("Rigidbody configuration")]
        [SerializeField, Range(0, 2)]
        protected float gravity = 1;
        [SerializeField, Range(0, 10)]
        protected float drag = 0;
        [SerializeField]
        AvatarHitBox colliderMode = default;

        void Awake() {
            avatar = GetComponentInParent<Avatar>();
            Assert.IsNotNull(avatar);
        }
        void OnDisable() {
        }

        #region State
        public virtual void EnterState() {
            avatar.attachedRigidbody.gravityScale = gravity;
            avatar.attachedRigidbody.drag = drag;
            avatar.colliderMode = colliderMode;
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