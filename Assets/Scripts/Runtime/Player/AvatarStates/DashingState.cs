using Slothsoft.UnityExtensions;
using UnityEngine;


namespace TheCursedBroom.Player.AvatarStates {
    public class DashingState : AvatarState {
        [Header("Dashing")]
        [SerializeField, Range(0, 1)]
        float minimumDashDuration = 0;

        float dashDuration;
        public override void EnterState() {
            base.EnterState();

            dashDuration = 0;

            avatar.broom.isDashing = true;
            avatar.broom.isFlying = true;
            avatar.broom.canBoost = false;

            avatar.UpdateMovement();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            dashDuration += Time.deltaTime;

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
            if (dashDuration < minimumDashDuration) {
                return this;
            }
            if (avatar.intendsGlide) {
                return intendsGlideState;
            }
            return rejectsGlideState;
        }
    }
}