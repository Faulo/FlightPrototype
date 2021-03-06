﻿using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarStates {
    public class GroundedState : AvatarState {
        public override void EnterState() {
            base.EnterState();

            avatar.broom.isFlying = false;
            avatar.broom.canBoost = true;

            avatar.UpdateMovement();
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            avatar.UpdateMovement();
        }
        public override void ExitState() {
            base.ExitState();
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState intendsJumpState = default;
        [SerializeField, Expandable]
        AvatarState intendsCrouchState = default;
        [SerializeField, Expandable]
        AvatarState notGroundedState = default;
        public override AvatarState CalculateNextState() {
            if (avatar.intendsJumpStart) {
                return intendsJumpState;
            }
            if (avatar.intendsCrouch) {
                return intendsCrouchState;
            }
            if (!avatar.isGrounded) {
                return notGroundedState;
            }
            return this;
        }
    }
}