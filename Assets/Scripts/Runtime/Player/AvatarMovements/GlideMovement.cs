using System;
using TheCursedBroom.Extensions;
using UnityEngine;

namespace TheCursedBroom.Player.AvatarMovements {
    [CreateAssetMenu()]
    public class GlideMovement : AvatarMovement {
        enum GlideMode {
            RotationControl,
            AngularVelocityControl
        }
        [Header("Gliding movement")]
        [SerializeField, Tooltip("Round input to rotationDirections")]
        bool directionsNormalized = true;
        [SerializeField, Range(1, 360)]
        int directionRange = 8;
        [SerializeField]
        bool instantRotation = true;
        [SerializeField, Range(0, 720)]
        float rotationSpeed = 360;

        [Header("Drag")]
        [SerializeField]
        AnimationCurve dragOverAlignment = default;
        [SerializeField, Range(0, 100)]
        float maximumDrag = 10;

        [Header("Boost")]
        [SerializeField]
        AnimationCurve alignDurationOverAlignment = default;
        [SerializeField, Range(0, 100)]
        float maximumAlignDuration = 10;

        public override MovementCalculator CreateMovementCalculator(Avatar avatar) {
            var angularVelocity = Quaternion.identity;
            var acceleration = Vector2.zero;
            return () => {
                var direction = avatar.intendedFlight == Vector2.zero
                    ? avatar.currentForward
                    : avatar.intendedFlight;

                float rotation = AngleUtil.DirectionalAngle(direction);

                if (directionsNormalized) {
                    rotation = Mathf.RoundToInt(rotation * directionRange / 360) * 360 / directionRange;
                }

                Quaternion glideRotation;
                if (instantRotation) {
                    glideRotation = Quaternion.Euler(0, 0, rotation);
                } else { 
                    var currentRotation = Quaternion.Euler(0, 0, avatar.rotation);
                    var targetRotation = Quaternion.Euler(0, 0, rotation);
                    glideRotation = QuaternionUtil.SmoothDamp(currentRotation, targetRotation, ref angularVelocity, Time.deltaTime, rotationSpeed);
                    rotation = glideRotation.eulerAngles.z;
                }

                var velocity = avatar.velocity;
                var velocityRotation = AngleUtil.DirectionalRotation(velocity);
                var gravity = avatar.gravityScale * Physics2D.gravity * Time.deltaTime;

                var alignmentRotation = glideRotation * Quaternion.Inverse(velocityRotation);
                float alignment = Mathf.Cos(alignmentRotation.eulerAngles.z * Mathf.Deg2Rad);

                avatar.drag = maximumDrag * dragOverAlignment.Evaluate(alignment);

                float alignDuration = maximumAlignDuration * alignDurationOverAlignment.Evaluate(alignment);
                var targetVelocity = glideRotation * Vector2.right * (velocity - gravity).magnitude;
                velocity = Vector2.SmoothDamp(velocity, targetVelocity, ref acceleration, alignDuration);

                velocity += gravity;

                return (velocity, rotation);
            };
        }
    }
}