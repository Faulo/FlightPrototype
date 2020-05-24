using System;
using System.Runtime.Remoting.Messaging;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace AvatarStateMachine {
    public class Airborne : AvatarState {

        [Header("Airborne")]
        [Header("Movement")]
        [SerializeField, Range(0, 1)]
        float forwardSpeedLerp = 1;
        public override void EnterState() {
            base.EnterState();

            avatar.isAirborne = true;
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            switch (Math.Sign(avatar.intendedMovement.x)) {
                case -1:
                    avatar.isFacingRight = false;
                    break;
                case 1:
                    avatar.isFacingRight = true;
                    break;
            }
            var velocity = avatar.attachedRigidbody.velocity;
            velocity.x = Mathf.Lerp(velocity.x, avatar.intendedMovement.x * avatar.maximumRunningSpeed, forwardSpeedLerp);
            avatar.attachedRigidbody.velocity = velocity;
        }
        public override void ExitState() {
            avatar.isAirborne = false;
            base.ExitState();
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState groundedState = default;
        [SerializeField, Expandable]
        AvatarState glidingState = default;
        public override AvatarState CalculateNextState() {
            if (avatar.CalculateGrounded()) {
                return groundedState;
            }
            if (avatar.intendsGlide && avatar.canGlide) {
                return glidingState;
            }
            return base.CalculateNextState();
        }
    }
}