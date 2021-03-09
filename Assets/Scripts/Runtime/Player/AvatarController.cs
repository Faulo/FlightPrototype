using System;
using Slothsoft.UnityExtensions;
using TheCursedBroom.Level;
using UnityEngine;

namespace TheCursedBroom.Player {
    public class AvatarController : MonoBehaviour {
        public event Action<GameObject> onSpawn;
        public event Action<GameObject> onReset;
        public event Action<GameObject, Vector3> onTeleport;

        public static AvatarController instance;
        public readonly BroomData broom = new BroomData();

        [Header("MonoBehaviour configuration")]
        [SerializeField, Expandable]
        Transform horizontalFlipTransform = default;
        [SerializeField, Expandable]
        public Rigidbody2D attachedRigidbody = default;
        [SerializeField, Expandable]
        AvatarAnimator visualsAnimator = default;
        [SerializeField, Expandable]
        AvatarAnimator physicsAnimator = default;
        [SerializeField, Expandable]
        public PhysicsEventEmitter physics = default;
        [SerializeField, Expandable]
        public AudioReverbFilter audioReverbFilter = default;

        public AvatarAnimations currentAnimation {
            set {
                visualsAnimator.Play(value);
                physicsAnimator.Play(value);
            }
        }

        [SerializeField, Expandable]
        AvatarState currentState = default;
        [SerializeField, Expandable]
        AvatarState saveState = default;
        [SerializeField, Expandable]
        AvatarState loadState = default;
        [SerializeField]
        Vector2 loadPosition = Vector2.zero;
        [SerializeField]
        Vector2 loadVelocity = Vector2.up;

        [SerializeField, Expandable]
        GroundedCheck groundedCheck = default;

        [Header("Current Input")]
        public int intendedFacing = 1;
        public float intendedMovement = 0;
        public Vector2 intendedFlight = Vector2.zero;

        public bool intendsJumpStart = false;
        public bool intendsJump = false;
        public bool intendsGlide = false;
        public bool intendsCrouch = false;
        public bool intendsSave = false;
        public bool intendsLoad = false;
        public bool intendsReset = false;

        public bool isFacingRight = true;
        public int facing {
            get => isFacingRight ? 1 : -1;
        }
        public int wallFacing;
        public float rotationAngle {
            get {
                return isFacingRight
                    ? attachedRigidbody.rotation
                    : attachedRigidbody.rotation + 180;
            }
            private set {
                value = AngleUtil.NormalizeAngle(value);
                int facing = AngleUtil.HorizontalSign(value);
                switch (facing) {
                    case 1:
                        isFacingRight = true;
                        break;
                    case -1:
                        isFacingRight = false;
                        value -= 180;
                        break;
                }
                attachedRigidbody.rotation = value;
                horizontalFlipTransform.rotation = isFacingRight
                    ? transform.rotation
                    : transform.rotation * Quaternion.Euler(0, 180, 0);
            }
        }

        public Vector2 forward => horizontalFlipTransform.right;
        public Vector2 velocity {
            get => attachedRigidbody.velocity;
            set => attachedRigidbody.velocity = value;
        }

        public float gravityScale = 0;
        public Vector2 gravityStep => gravityScale * Physics2D.gravity * Time.deltaTime;
        public float drag {
            get => attachedRigidbody.drag;
            set => attachedRigidbody.drag = value;
        }

        public float dissolveAmount = 0;

        public MovementCalculator movementCalculator;
        public void UpdateMovement() {
            (velocity, rotationAngle) = movementCalculator();
        }

        void Start() {
            instance = this;
            currentState.EnterState();

            LevelController.instance.observedObjects.Add(state);

            onSpawn?.Invoke(gameObject);
        }

        void Update() {
            currentState.UpdateState();
            if (intendsReset) {
                intendsReset = false;
                onReset?.Invoke(gameObject);
            }
        }

        void FixedUpdate() {
            UpdateRumbling();

            var newState = currentState.CalculateNextState();

            if (currentState == newState) {
                currentState.FixedUpdateState();
            } else {
                SetState(newState);
            }
        }
        void SetState(AvatarState newState) {
            currentState.ExitState();
            currentState = newState;
            currentState.EnterState();
        }

        public bool isGrounded => groundedCheck.isGrounded;
        public float groundKinematicFriction => groundedCheck.kinematicFriction;
        public float groundStaticFriction => groundedCheck.staticFriction;

        public void CastSave() {
            if (currentState != saveState) {
                SetState(saveState);
            }
        }
        public void CastLoad() {
            if (currentState != loadState) {
                SetState(loadState);
            }
        }

        class AvatarSaveState : ILevelObject {
            public Vector3 position;
            public float rotationAngle;

            Vector3 ILevelObject.position => position;
            bool ILevelObject.requireLevel => false;

            public void TranslateX(float x) {
                position.x += x;
            }
        }

        readonly AvatarSaveState state = new AvatarSaveState();

        public void StateSave() {
            state.position = transform.position;
            state.rotationAngle = rotationAngle;
        }
        public void StateLoad() {
            var delta = state.position - transform.position;
            transform.position = state.position + (Vector3)loadPosition;
            rotationAngle = state.rotationAngle;

            attachedRigidbody.velocity = loadVelocity;
            attachedRigidbody.angularVelocity = 0;

            onTeleport?.Invoke(gameObject, delta);
        }

        public bool isRumbling => rumblingDuration > 0;
        public float rumblingLowIntensity;
        public float rumblingHighIntensity;
        public float rumblingDuration;

        void UpdateRumbling() {
            if (rumblingDuration >= 0) {
                rumblingDuration -= Time.deltaTime;
            } else {
                rumblingLowIntensity = 0;
                rumblingHighIntensity = 0;
            }
        }

        public int collectibleCount;
    }
}