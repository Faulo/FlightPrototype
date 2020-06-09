using System;
using System.Collections.Generic;
using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace TheCursedBroom.Player {
    public class Avatar : MonoBehaviour {
        [Header("MonoBehaviour configuration")]
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
        public int intendedFacing = 0;
        public float intendedMovement = 0;
        public Vector2 intendedFlight = Vector2.zero;
        public Quaternion intendedRotation = Quaternion.identity;

        public bool intendsJumpStart = false;
        public bool intendsJump = false;
        public bool intendsGlide = false;
        public bool intendsCrouch = false;

        [Header("Debug Info")]
        public bool isFacingRight = true;
        public int facingSign => isFacingRight
            ? 1
            : -1;
        public Quaternion facingRotation => isFacingRight
            ? transform.rotation
            : transform.rotation * flipRotation;

        public Quaternion currentRotation => transform.rotation;
        public float rotation {
            get => attachedRigidbody.rotation;
            set => attachedRigidbody.rotation = value;
        }
        public Vector2 currentForward => new Vector2(transform.right.x, transform.right.y) * facingSign;

        Quaternion flipRotation;

        public void AlignFaceToIntend() {
            switch (intendedFacing) {
                case -1:
                    isFacingRight = false;
                    break;
                case 1:
                    isFacingRight = true;
                    break;
            }
        }

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
            (velocity, rotation) = movementCalculator();
        }
        public float walkSpeed => velocity.x;

        void Awake() {
            flipRotation = Quaternion.Euler(0, 180, 0);
        }
        void Start() {
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
    }
}