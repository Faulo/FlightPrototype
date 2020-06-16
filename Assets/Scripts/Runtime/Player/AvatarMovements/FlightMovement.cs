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

        public override MovementCalculator CreateMovementCalculator(AvatarController avatar) {
            return () => {
                var velocity = avatar.velocity;

                if (avatar.intendedFlight.y > 0) {
                    velocity.y += upDraft * avatar.intendedFlight.y * Time.deltaTime;
                }
                if (avatar.intendedFlight.y < 0) {
                    velocity.y += downDraft * avatar.intendedFlight.y * Time.deltaTime;
                }

                if (avatar.intendedFlight.x > 0) {
                    velocity.x += acceleration * avatar.intendedFlight.x * Time.deltaTime;
                }
                if (avatar.intendedFlight.x < 0) {
                    velocity.x += deceleration * avatar.intendedFlight.x * Time.deltaTime;
                }

                return (velocity, avatar.rotationAngle);
            };
        }
    }
}