using Slothsoft.UnityExtensions;
using UnityEngine;


namespace TheCursedBroom.Player.AvatarStates {
    public class Dash : AvatarState {
        enum DashDirection {
            CurrentIntention,
            CurrentVelocity
        }
        enum VelocityMode {
            SetVelocity,
            AddVelocity
        }
        [Header("Dash")]
        [SerializeField, Range(0, 100)]
        int dashFrameCount = 1;
        [SerializeField]
        VelocityMode initialMode = default;
        [SerializeField, Range(-100, 100)]
        float initialSpeed = 15;
        [SerializeField]
        VelocityMode exitMode = default;
        [SerializeField, Range(-100, 100)]
        float exitSpeed = 1;
        [SerializeField]
        DashDirection direction = default;
        [SerializeField, Range(1, 360)]
        int dashDirections = 8;
        [SerializeField, Range(0, 360)]
        int rotationOffset = 0;


        int dashTimer;
        float rotation;
        Vector2 velocity;
        public override void EnterState() {
            base.EnterState();

            dashTimer = 0;

            avatar.AlignFaceToIntend();
            avatar.UseGlideCharge();

            velocity = avatar.attachedRigidbody.velocity;

            switch (direction) {
                case DashDirection.CurrentIntention:
                    rotation = avatar.intendedRotation.eulerAngles.z;
                    break;
                case DashDirection.CurrentVelocity:
                    rotation = velocity.magnitude > 0
                        ? Vector2.SignedAngle(Vector2.up, velocity.normalized)
                        : Vector2.SignedAngle(Vector2.up, Vector2.right * avatar.facingSign);
                    break;
                default:
                    break;
            }
            rotation += rotationOffset * avatar.facingSign;
            rotation = Mathf.RoundToInt(rotation * dashDirections / 360) * 360 / dashDirections;

            velocity = Quaternion.Euler(0, 0, rotation) * Vector2.right * initialSpeed * avatar.facingSign;

            switch (initialMode) {
                case VelocityMode.SetVelocity:
                    break;
                case VelocityMode.AddVelocity:
                    velocity += avatar.attachedRigidbody.velocity;
                    break;
            }

            avatar.attachedRigidbody.constraints = RigidbodyConstraints2D.None;
            avatar.attachedRigidbody.velocity = velocity;
            avatar.attachedRigidbody.rotation = rotation;
            //avatar.attachedSprite.transform.rotation = avatar.transform.rotation * Quaternion.Euler(0, 0, 90 * avatar.facingSign);
            avatar.attachedAnimator.Play(AvatarAnimations.Mounting);
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            dashTimer++;
            avatar.attachedRigidbody.velocity = velocity;
        }

        public override void ExitState() {
            base.ExitState();

            //avatar.attachedSprite.transform.rotation = avatar.transform.rotation;
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState glidingState = default;
        [SerializeField, Expandable]
        AvatarState airborneState = default;
        public override AvatarState CalculateNextState() {
            if (dashTimer < dashFrameCount) {
                return this;
            }
            if (avatar.intendsGlide) {
                return glidingState;
            } else {
                velocity = Quaternion.Euler(0, 0, rotation) * Vector2.up * exitSpeed;
                velocity += Physics2D.gravity * Time.deltaTime;
                switch (exitMode) {
                    case VelocityMode.SetVelocity:
                        break;
                    case VelocityMode.AddVelocity:
                        velocity += avatar.attachedRigidbody.velocity;
                        break;
                }
                avatar.attachedRigidbody.rotation = 0;
                avatar.attachedRigidbody.velocity = velocity;
                avatar.attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                return airborneState;
            }
        }
    }
}