using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class WallCollisionState : AvatarState {
        [SerializeField, Range(0, 1)]
        float minimumCollisionDuration = 0;
        [SerializeField, Range(0, 1)]
        float maximumCollisionDuration = 0;

        float collisionDuration;
        public override void EnterState() {
            base.EnterState();

            collisionDuration = 0;

            avatar.broom.isFlying = false;
            avatar.broom.canBoost = false;

            avatar.UpdateMovement();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            collisionDuration += Time.deltaTime;

            avatar.UpdateMovement();
        }
        public override void ExitState() {
            base.ExitState();
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState intendsJumpState = default;
        [SerializeField, Expandable]
        AvatarState rejectsJumpState = default;
        public override AvatarState CalculateNextState() {
            if (collisionDuration < minimumCollisionDuration) {
                return this;
            }
            if (!avatar.intendsGlide) {
                return rejectsJumpState;
            }
            if (avatar.intendedFacing != avatar.facing) {
                return intendsJumpState;
            }
            if (collisionDuration < maximumCollisionDuration) {
                return this;
            }
            return rejectsJumpState;
        }
    }
}