using System.Collections.Generic;
using System.Linq;
using Slothsoft.UnityExtensions;
using TheCursedBroom.Level;
using UnityEngine;

namespace TheCursedBroom.Player {
    public class AvatarController : MonoBehaviour {
        [Header("MonoBehaviour configuration")]
        [SerializeField, Expandable]
        Transform horizontalFlipTransform = default;
        [SerializeField, Expandable]
        public Rigidbody2D attachedRigidbody = default;

        [SerializeField, Expandable]
        AvatarAnimator visualsAnimator = default;

        [SerializeField, Expandable]
        AvatarAnimator physicsAnimator = default;

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

        public bool isFacingRight = true;
        public int facing {
            get => isFacingRight ? 1 : -1;
        }
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

        public bool canGlide => true;
        public Vector2 velocity {
            get => attachedRigidbody.velocity;
            set => attachedRigidbody.velocity = value;
        }

        public float gravityScale = 0;
        public float drag {
            get => attachedRigidbody.drag;
            set => attachedRigidbody.drag = value;
        }

        public MovementCalculator movementCalculator;
        public void UpdateMovement() {
            (velocity, rotationAngle) = movementCalculator();
        }

        void Update() {
            currentState.UpdateState();
        }

        void FixedUpdate() {
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


        void OnTriggerEnter2D(Collider2D collider) {
            if (collider.TryGetComponent(out Interactable interactable)) {
                interactable.Interact();
            }
        }
    }
}