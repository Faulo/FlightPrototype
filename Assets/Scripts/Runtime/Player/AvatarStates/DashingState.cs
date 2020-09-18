using Slothsoft.UnityExtensions;
using UnityEngine;


namespace TheCursedBroom.Player.AvatarStates {
    public class DashingState : AvatarState {
        [Header("Dashing")]
        [SerializeField, Range(0, 100)]
        int minimumDashFrameCount = 1;

        int dashTimer;
        public override void EnterState() {
            base.EnterState();

            dashTimer = 0;

            avatar.broom.isDashing = true;
            avatar.broom.isFlying = true;
            avatar.broom.canDash = false;

            avatar.UpdateMovement();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            dashTimer++;

            avatar.UpdateMovement();
        }

        public override void ExitState() {
            base.ExitState();

            avatar.broom.isDashing = false;
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState intendsGlideState = default;
        [SerializeField, Expandable]
        AvatarState rejectsGlideState = default;
        public override AvatarState CalculateNextState() {
            if (dashTimer < minimumDashFrameCount) {
                return this;
            }
            if (avatar.intendsGlide) {
                return intendsGlideState;
            }
            return rejectsGlideState;
        }
    }
}