using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class AirborneState : AvatarState {
        [Header("Airborne")]
        [SerializeField]
        bool resetVelocityOnGround = true;
        [SerializeField]
        bool rechargeBoost = false;
        [SerializeField, Range(0, 100)]
        float rechargeBoostSpeed = 0;

        public override void EnterState() {
            base.EnterState();

            avatar.broom.isFlying = false;

            avatar.UpdateMovement();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            if (rechargeBoost && avatar.velocity.magnitude > rechargeBoostSpeed) {
                avatar.broom.canBoost = true;
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
        AvatarState isGroundedAndMovingState = default;
        [SerializeField, Expandable]
        AvatarState isGroundedState = default;
        public override AvatarState CalculateNextState() {
            if (avatar.intendsGlide && avatar.broom.canBoost) {
                return intendsGlideState;
            }
            if (avatar.isGrounded) {
                if (avatar.intendedMovement == 0) {
                    if (resetVelocityOnGround) {
                        avatar.velocity = new Vector2(0, avatar.velocity.y);
                    }
                    return isGroundedState;
                } else {
                    return isGroundedAndMovingState;
                }
            }
            return this;
        }
    }
}