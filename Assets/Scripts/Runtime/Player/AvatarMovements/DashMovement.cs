using System;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarMovements {
    [CreateAssetMenu()]
    public class DashMovement : AvatarMovement {
        enum DashDirection {
            CurrentIntention,
            CurrentVelocity
        }
        enum VelocityMode {
            SetVelocity,
            AddVelocity
        }
        [Header("Dash")]
        [SerializeField]
        VelocityMode initialMode = default;
        [SerializeField, Range(-100, 100)]
        float initialSpeed = 15;
        [SerializeField]
        DashDirection direction = default;
        [SerializeField, Range(1, 360)]
        int dashDirections = 8;

        public override MovementCalculator CreateMovementCalculator(Avatar avatar) {
            var velocity = avatar.attachedRigidbody.velocity;
            float rotation = 0;

            switch (direction) {
                case DashDirection.CurrentIntention:
                    rotation = avatar.intendedRotation.eulerAngles.z;
                    break;
                case DashDirection.CurrentVelocity:
                    rotation = velocity.magnitude > 0
                        ? Vector2.SignedAngle(Vector2.up, velocity.normalized)
                        : Vector2.SignedAngle(Vector2.up, Vector2.right * avatar.facingSign);
                    break;
                default:
                    break;
            }
            rotation = Mathf.RoundToInt(rotation * dashDirections / 360) * 360 / dashDirections;

            velocity = Quaternion.Euler(0, 0, rotation) * Vector2.right * initialSpeed * avatar.facingSign;

            switch (initialMode) {
                case VelocityMode.SetVelocity:
                    break;
                case VelocityMode.AddVelocity:
                    velocity += avatar.attachedRigidbody.velocity;
                    break;
            }

            return () => {
                return (velocity, rotation);
            };
        }
    }
}