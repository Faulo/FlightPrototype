using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class HangTimeState : AvatarState {
        [Header("Hang Time")]
        [SerializeField, Range(0, 1)]
        float minimumHangDuration = 0;
        [SerializeField, Range(0, 1)]
        float maximumHangDuration = 0;
        [SerializeField, Range(0, 10)]
        float rejectJumpGravity = 1;

        float hangDuration;
        public override void EnterState() {
            base.EnterState();

            hangDuration = 0;

            avatar.broom.isFlying = false;

            avatar.UpdateMovement();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            hangDuration += Time.deltaTime;
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
            if (hangDuration < minimumHangDuration) {
                return this;
            }
            if (!avatar.intendsJump) {
                return rejectsGlideState;
            }
            if (hangDuration < maximumHangDuration) {
                return this;
            }
            return rejectsGlideState;
        }
    }
}