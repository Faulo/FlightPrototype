using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class WallCollisionState : AvatarState {
        [SerializeField, Range(0, 100)]
        int minimumCollisionFrameCount = 1;
        [SerializeField, Range(0, 100)]
        int maximumCollisionFrameCount = 1;

        int collisionTimer;
        public override void EnterState() {
            base.EnterState();

            collisionTimer = 0;

            avatar.broom.isFlying = false;
            avatar.broom.canBoost = false;

            avatar.UpdateMovement();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            collisionTimer++;

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
            if (collisionTimer < minimumCollisionFrameCount) {
                return this;
            }
            if (avatar.intendedFacing != avatar.facing) {
                return intendsJumpState;
            }
            if (collisionTimer < maximumCollisionFrameCount) {
                return this;
            }
            return rejectsJumpState;
        }
    }
}