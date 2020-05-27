using System;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace TheCursedBroom.Player {
    public class Avatar : MonoBehaviour {
        public event Action<Avatar> onColliderChange;

        [SerializeField, Expandable]
        public Rigidbody2D attachedRigidbody = default;

        [SerializeField, Expandable]
        public SpriteRenderer attachedSprite = default;

        [SerializeField, Expandable]
        GameObject uprightCollider = default;
        [SerializeField, Expandable]
        GameObject flyingCollider = default;

        [SerializeField, Expandable]
        GroundedCheck groundedCheck = default;

        [SerializeField]
        AvatarState currentState = default;

        [Header("Movement")]
        [SerializeField, Range(0, 100)]
        public float maximumRunningSpeed = 10;
        [SerializeField, Range(0, 10)]
        int maximumGlideCharges = 1;

        public void RechargeDashes() {
            currentGlideCharges = maximumGlideCharges;
        }
        public void UseDashCharge() {
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

        [Header("Debug Info")]
        public bool isFacingRight = true;
        public int facingSign => isFacingRight
            ? 1
            : -1;

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

        public AvatarHitBox colliderMode {
            get => colliderModeCache;
            set {
                if (colliderModeCache != value) {
                    currentCollider.SetActive(false);
                    colliderModeCache = value;
                    currentCollider.SetActive(true);
                    onColliderChange?.Invoke(this);
                }
            }
        }
        AvatarHitBox colliderModeCache;

        GameObject currentCollider {
            get {
                switch (colliderMode) {
                    case AvatarHitBox.Upright:
                        return uprightCollider;
                    case AvatarHitBox.Flying:
                        return flyingCollider;
                    default:
                        throw new NotImplementedException(colliderMode.ToString());
                }
            }
        }

        public bool isGrounded;
        public bool isJumping;
        public bool isAirborne;

        public bool isFlying => colliderModeCache == AvatarHitBox.Flying;

        void Start() {
        }

        void Update() {
            currentState.UpdateState();
        }

        void FixedUpdate() {
            var newState = currentState.CalculateNextState();

            if (currentState == newState) {
                currentState.FixedUpdateState();
            } else {
                currentState.ExitState();
                currentState = newState;
                currentState.EnterState();
            }
        }

        public bool CalculateGrounded() => groundedCheck.IsGrounded(gameObject);
    }
}