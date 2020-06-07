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

        [Header("Movement")]
        [SerializeField, Range(0, 10000)]
        int maximumGlideFrameCount = 1000;

        public void RechargeGlide() {
            currentGlideCharges = maximumGlideFrameCount;
        }
        public void UseGlideCharge() {
            Assert.IsTrue(currentGlideCharges > 0);
            currentGlideCharges--;
        }

        [Header("Current Input")]
        public Vector2 intendedMovement = Vector2.zero;
        public Quaternion intendedRotation => intendedMovement.magnitude > 0
            ? Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, intendedMovement.normalized))
            : Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, Vector2.right * facingSign));
        public Quaternion currentRotation => transform.rotation;

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
        Quaternion flipRotation;

        public void AlignFaceToIntend() {
            switch (Math.Sign(intendedMovement.x)) {
                case -1:
                    isFacingRight = false;
                    break;
                case 1:
                    isFacingRight = true;
                    break;
            }
        }


        int currentGlideCharges = 0;
        public bool canGlide => currentGlideCharges > 0;
        public Vector2 velocity {
            get => attachedRigidbody.velocity;
            set => attachedRigidbody.velocity = value;
        }
        public float gravityScale {
            get => attachedRigidbody.gravityScale;
            set => attachedRigidbody.gravityScale = value;
        }
        public float drag {
            get => attachedRigidbody.drag;
            set => attachedRigidbody.drag = value;
        }

        public Func<Vector2> velocityCalculator;
        public void UpdateVelocity() {
            velocity = velocityCalculator();
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