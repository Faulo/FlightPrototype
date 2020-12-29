using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class HangTimeState : AvatarState {
        [Header("Hang Time")]
        [SerializeField, Range(0, 100)]
        int minimumHangFrameCount = 1;
        [SerializeField, Range(0, 100)]
        int maximumHangFrameCount = 1;
        [SerializeField, Range(0, 10)]
        float rejectJumpGravity = 1;

        int hangTimer;
        public override void EnterState() {
            base.EnterState();

            hangTimer = 0;

            avatar.broom.isFlying = false;

            avatar.UpdateMovement();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            hangTimer++;
            if (!avatar.intendsJump) {
                avatar.gravityScale = rejectJumpGravity;
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
        AvatarState rejectsGlideState = default;
        public override AvatarState CalculateNextState() {
            if (avatar.intendsGlide && avatar.broom.canBoost) {
                return intendsGlideState;
            }
            if (hangTimer < minimumHangFrameCount) {
                return this;
            }
            if (!avatar.intendsJump) {
                return rejectsGlideState;
            }
            if (hangTimer < maximumHangFrameCount) {
                return this;
            }
            return rejectsGlideState;
        }
    }
}