using System;
using System.Runtime.Remoting.Messaging;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace AvatarStateMachine {
    public class HangTime : AvatarState {

        [Header("Hang Time")]
        [SerializeField, Range(0, 1)]
        float hangDuration = 1;
        [Header("Movement")]
        [SerializeField, Range(0, 1)]
        float forwardSpeedLerp = 1;

        float hangTimer;
        public override void EnterState() {
            base.EnterState();

            hangTimer = 0;
            avatar.isAirborne = true;
            avatar.attachedRigidbody.rotation = 0;
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            hangTimer += Time.deltaTime;

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
        [SerializeField, Expandable]
        AvatarState airborneState = default;
        public override AvatarState CalculateNextState() {
            if (avatar.CalculateGrounded()) {
                return groundedState;
            }
            if (avatar.intendsGlide && avatar.canGlide) {
                return glidingState;
            }
            if (hangTimer > hangDuration) {
                return airborneState;
            }
            return base.CalculateNextState();
        }
    }
}