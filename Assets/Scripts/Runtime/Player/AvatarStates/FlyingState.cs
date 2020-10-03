using System;
using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;


namespace TheCursedBroom.Player.AvatarStates {
    public class FlyingState : AvatarState {
        [Header("Flying")]
        [SerializeField]
        bool wallMustBeAirborne = true;
        [SerializeField, Range(0, 1)]
        float wallMinimumX = 0.5f;
        [SerializeField, Range(0, 1)]
        float wallMaximumY = 0.5f;

        bool hasCollided;
        public override void EnterState() {
            base.EnterState();

            avatar.broom.isFlying = true;

            hasCollided = false;
            avatar.physics.onCollisionEnter += CollisionListener;

            avatar.UpdateMovement();
        }

        public override void FixedUpdateState() {
            base.FixedUpdateState();

            avatar.UpdateMovement();
        }

        public override void ExitState() {
            base.ExitState();

            avatar.broom.isFlying = false;
            avatar.physics.onCollisionEnter -= CollisionListener;
        }


        void CollisionListener(Collision2D collision) {
            if (collision.contacts.Any(IsWallCollision)) {
                hasCollided = true;
            }
        }
        bool IsWallCollision(ContactPoint2D contact) {
            if (wallMustBeAirborne && avatar.isGrounded) {
                return false;
            }
            if (Mathf.Abs(contact.normal.x) < wallMinimumX) {
                return false;
            }
            if (Mathf.Abs(contact.normal.y) > wallMaximumY) {
                return false;
            }
            if (Math.Sign((contact.point - avatar.attachedRigidbody.position).x) != avatar.facing) {
                return false;
            }
            avatar.wallFacing = Math.Sign((contact.point - avatar.attachedRigidbody.position).x);
            return true;
        }


        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState rejectsGlideState = default;
        [SerializeField, Expandable]
        AvatarState wallCollisionState = default;
        public override AvatarState CalculateNextState() {
            if (hasCollided) {
                return wallCollisionState;
            }
            if (!avatar.intendsGlide) {
                return rejectsGlideState;
            }
            return this;
        }
    }
}