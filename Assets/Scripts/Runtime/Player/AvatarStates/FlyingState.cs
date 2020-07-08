using System;
using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;


namespace TheCursedBroom.Player.AvatarStates {
    public class FlyingState : AvatarState {
        [Header("Flying")]
        [SerializeField, Expandable]
        ParticleSystem updraftParticles = default;
        [SerializeField]
        bool wallMustBeAirborne = true;
        [SerializeField, Range(0, 1)]
        float wallMinimumX = 0.5f;
        [SerializeField, Range(0, 1)]
        float wallMaximumY = 0.5f;

        bool hasCollided;
        public override void EnterState() {
            base.EnterState();

            updraftParticles.Play();

            hasCollided = false;
            avatar.physics.onCollisionEnter += CollisionListener;

            avatar.UpdateMovement();
        }

        public override void FixedUpdateState() {
            base.FixedUpdateState();

            //updraftParticles.transform.rotation = avatar.intendedLookRotation;

            var main = updraftParticles.main;
            //main.startSpeed = new ParticleSystem.MinMaxCurve(avatar.intendedFlight.y, avatar.intendedFlight.y * 5);

            avatar.UpdateMovement();
        }

        public override void ExitState() {
            base.ExitState();

            avatar.physics.onCollisionEnter -= CollisionListener;

            updraftParticles.Stop();
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
            if (!avatar.intendsGlide || !avatar.canGlide) {
                return rejectsGlideState;
            }
            return this;
        }
    }
}