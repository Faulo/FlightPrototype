using System;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarMovements {
    [CreateAssetMenu()]
    public class FlightMovement : AvatarMovement {
        [SerializeField, Range(0, 1000)]
        float upDraft = 10;
        [SerializeField, Range(0, 1000)]
        float downDraft = 10;
        [SerializeField, Range(0, 1000)]
        float acceleration = 10;
        [SerializeField, Range(0, 1000)]
        float deceleration = 10;


        public override MovementCalculator CreateMovementCalculator(Avatar avatar) {
            return () => {
                var velocity = avatar.velocity;

                if (avatar.intendedLook.y > 0) {
                    velocity.y += upDraft * avatar.intendedLook.y * Time.deltaTime;
                }
                if (avatar.intendedLook.y < 0) {
                    velocity.y += downDraft * avatar.intendedLook.y * Time.deltaTime;
                }

                if (avatar.intendedLook.x > 0) {
                    velocity.x += acceleration * avatar.intendedLook.x * Time.deltaTime;
                }
                if (avatar.intendedLook.x < 0) {
                    velocity.x += deceleration * avatar.intendedLook.x * Time.deltaTime;
                }

                return (velocity, avatar.rotation);
            };
        }
    }
}