using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class Crouching : AvatarState {
        [Header("Crouching")]
        [SerializeField, Range(0, 100)]
        int minimumCrouchFrameCount = 1;
        [SerializeField, Range(0, 1)]
        float breakingSpeedLerp = 1;

        bool intendsJump;

        int crouchDuration;
        public override void EnterState() {
            base.EnterState();

            crouchDuration = 0;
            avatar.attachedAnimator.Play(AvatarAnimations.Crouching);
            avatar.RechargeGlide();
            intendsJump = avatar.intendsJump;
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();
            crouchDuration++;

            var velocity = avatar.attachedRigidbody.velocity;

            velocity.x = Mathf.Lerp(velocity.x, 0, breakingSpeedLerp);

            velocity.x = Mathf.Clamp(velocity.x, -avatar.maximumRunningSpeed, avatar.maximumRunningSpeed);

            avatar.attachedRigidbody.velocity = velocity;
        }

        public override void ExitState() {
            base.ExitState();
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState jumpingState = default;
        [SerializeField, Expandable]
        AvatarState idleState = default;
        [SerializeField, Expandable]
        AvatarState airborneState = default;
        public override AvatarState CalculateNextState() {
            if (crouchDuration < minimumCrouchFrameCount) {
                return this;
            }
            if (intendsJump || avatar.intendsJump) {
                return jumpingState;
            }
            if (!avatar.CalculateGrounded()) {
                return airborneState;
            }
            if (!avatar.intendsCrouch) {
                return idleState;
            }
            return this;
        }
    }
}