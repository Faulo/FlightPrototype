using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class CrouchingState : AvatarState {
        [Header("Crouching")]
        [SerializeField, Range(0, 1)]
        float minimumCrouchDuration = 0;
        [SerializeField, Range(0, 1)]
        float startJumpDuration = 0;
        [SerializeField, Range(0, 1)]
        float coyoteTimeDuration = 0;

        bool intendsJump;
        float crouchDuration;

        public override void EnterState() {
            base.EnterState();

            crouchDuration = 0;
            intendsJump = avatar.intendsJumpStart;

            avatar.broom.isFlying = false;
            avatar.broom.canBoost = true;

            avatar.UpdateMovement();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            crouchDuration += Time.deltaTime;
            if (avatar.intendsJumpStart) {
                intendsJump = true;
            }

            avatar.UpdateMovement();
        }

        public override void ExitState() {
            base.ExitState();
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState intendsGlideState = default;
        [SerializeField, Expandable]
        AvatarState intendsJumpState = default;
        [SerializeField, Expandable]
        AvatarState notGroundedState = default;
        [SerializeField, Expandable]
        AvatarState rejectsCrouchState = default;
        public override AvatarState CalculateNextState() {
            if (!avatar.isGrounded && avatar.intendsGlide) {
                return intendsGlideState;
            }
            if (crouchDuration >= startJumpDuration && intendsJump) {
                return intendsJumpState;
            }
            if (crouchDuration >= coyoteTimeDuration && !avatar.isGrounded) {
                return notGroundedState;
            }
            if (crouchDuration >= minimumCrouchDuration && !avatar.intendsCrouch) {
                return rejectsCrouchState;
            }
            return this;
        }
    }
}