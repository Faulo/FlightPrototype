using System;
using System.Collections.Generic;
using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player {
    public class AvatarController : MonoBehaviour {
        public event Action<GameObject> onSpawn;
        public event Action<GameObject> onSave;
        public event Action<GameObject> onLoad;
        public event Action<GameObject> onReset;
        public event Action<GameObject, Vector3> onTeleport;

        public static AvatarController instance;

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

        public AvatarAnimations currentAnimation {
            set {
                visualsAnimator.Play(value);
                physicsAnimator.Play(value);
            }
        }

        [SerializeField, Expandable]
        AvatarState currentState = default;

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

        public bool canGlide = true;
        public bool isFlying => !attachedRigidbody.freezeRotation;
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

        public MovementCalculator movementCalculator;
        public void UpdateMovement() {
            (velocity, rotationAngle) = movementCalculator();
        }

        void Start() {
            instance = this;
            onSpawn?.Invoke(gameObject);
        }

        void Update() {
            currentState.UpdateState();
            if (intendsReset) {
                onReset?.Invoke(gameObject);
            }
        }

        void FixedUpdate() {
            UpdateRumbling();
            grounds = groundedCheck.GetGrounds();

            var newState = currentState.CalculateNextState();

            if (currentState == newState) {
                currentState.FixedUpdateState();
            } else {
                currentState.ExitState();
                currentState = newState;
                currentState.EnterState();
            }
        }

        IReadOnlyList<Ground> grounds;
        public bool isGrounded => grounds
            .Any();
        public float groundKinematicFriction => grounds
            .Select(ground => ground.kinematicFriction)
            .DefaultIfEmpty(1)
            .Min();
        public float groundStaticFriction => grounds
            .Select(ground => ground.staticFriction)
            .DefaultIfEmpty(1)
            .Min();

        public void CastSave() {
            onSave?.Invoke(gameObject);
        }
        public void CastLoad() {
            onLoad?.Invoke(gameObject);
        }

        struct AvatarSaveState {
            public Vector3 position;
            public float rotationAngle;
        }
        AvatarSaveState state = default;

        public void StateSave() {
            state.position = transform.position;
            state.rotationAngle = rotationAngle;
        }
        public void StateLoad() {
            var delta = state.position - transform.position;
            transform.position = state.position;
            rotationAngle = state.rotationAngle;

            attachedRigidbody.velocity = Vector2.zero;
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
    }
}