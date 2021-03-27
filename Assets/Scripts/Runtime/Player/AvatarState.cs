using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace TheCursedBroom.Player {
    public abstract class AvatarState : MonoBehaviour {
        protected AvatarController avatar;

        [Header("Avatar parameters")]
        [SerializeField]
        AvatarAnimations animatorState = default;
        [SerializeField, Range(0, 2)]
        float gravity = 1;
        [SerializeField, Range(0, 10)]
        float drag = 0;
        [SerializeField]
        bool allowPhysicsRotation = false;
        [SerializeField]
        bool allowSave = false;
        [SerializeField]
        bool allowLoad = true;
        [SerializeField, Expandable]
        AvatarMovement movement = default;
        [SerializeField, Expandable]
        GameObject stateObject = default;
        [SerializeField, Expandable]
        ParticleSystem stateParticles = default;
        [Header("Events")]
        [SerializeField]
        GameObjectEvent onStateEnter = new GameObjectEvent();
        [SerializeField]
        GameObjectEvent onStateExit = new GameObjectEvent();

        void Awake() {
            avatar = GetComponentInParent<AvatarController>();
            Assert.IsTrue(avatar);
            if (stateObject) {
                stateObject.SetActive(false);
            }
            if (stateParticles) {
                stateParticles.Stop();
            }
        }

        #region State
        public virtual void EnterState() {
            avatar.currentAnimation = animatorState;
            avatar.gravityScale = gravity;
            avatar.drag = drag;
            avatar.attachedRigidbody.constraints = allowPhysicsRotation
                ? RigidbodyConstraints2D.None
                : RigidbodyConstraints2D.FreezeRotation;
            avatar.movementCalculator = movement
                ? movement.CreateMovementCalculator(avatar)
                : () => (avatar.velocity, avatar.rotationAngle);

            onStateEnter.Invoke(avatar.gameObject);
            if (stateObject) {
                stateObject.SetActive(true);
            }
            if (stateParticles) {
                stateParticles.Play();
            }
        }
        public virtual void UpdateState() {
            if (allowSave && avatar.intendsSaveStart) {
                avatar.intendsSaveStart = false;
                avatar.CastSave();
            }
            if (allowLoad && avatar.intendsLoadStart) {
                avatar.intendsLoadStart = false;
                avatar.CastLoad();
            }
        }
        public virtual void FixedUpdateState() {
        }
        public virtual void ExitState() {
            onStateExit.Invoke(avatar.gameObject);
            if (stateObject) {
                stateObject.SetActive(false);
            }
            if (stateParticles) {
                stateParticles.Stop();
            }
        }
        public abstract AvatarState CalculateNextState();
        #endregion
    }
}