using System.Linq;
using TheCursedBroom.Extensions;
using UnityEngine;
using UnityEngine.Assertions;

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
        [SerializeField, Range(0, 100)]
        float rotationDuration = 1;

        [Header("Drag")]
        [SerializeField]
        AnimationCurve dragOverAlignment = default;
        [SerializeField, Range(0, 100)]
        float minimumDrag = 0;
        [SerializeField, Range(0, 100)]
        float maximumDrag = 1;

        [Header("Velocity Conversion")]
        [SerializeField]
        AnimationCurve alignDurationOverAlignment = default;
        [SerializeField, Range(0, 100)]
        float minimumAlignDuration = 0;
        [SerializeField, Range(0, 100)]
        float maximumAlignDuration = 1;

        [Header("Boost")]
        [SerializeField]
        AnimationCurve boostOverAlignment = default;
        [SerializeField, Range(0, 100)]
        float minimumBoost = 0;
        [SerializeField, Range(0, 100)]
        float maximumBoost = 1;

        [Header("Visualization")]
        [SerializeField]
        string particlesName = "ParticleTrail_Glide";
        [SerializeField]
        Gradient colorOverAlignment = default;
        [SerializeField]
        AnimationCurve particleCountOverAlignment = default;
        [SerializeField, Range(0, 1000)]
        int minimumParticleCount = 10;
        [SerializeField, Range(0, 1000)]
        int maximumParticleCount = 100;

        public override MovementCalculator CreateMovementCalculator(AvatarController avatar) {
            var particles = avatar
                .GetComponentsInChildren<ParticleSystem>()
                .FirstOrDefault(p => p.name == particlesName);
            Assert.IsNotNull(particles, $"Couldn't find particles '{particlesName}'!");
            float angularVelocity = 0;
            var acceleration = Vector2.zero;
            return () => {
                var direction = avatar.intendedFlight == Vector2.zero
                    ? avatar.forward
                    : avatar.intendedFlight;

                float rotation = AngleUtil.DirectionalAngle(direction);

                if (directionsNormalized) {
                    rotation = Mathf.RoundToInt(rotation * directionRange / 360) * 360 / directionRange;
                }

                rotation = Mathf.SmoothDampAngle(avatar.rotationAngle, rotation, ref angularVelocity, rotationDuration);

                var glideRotation = Quaternion.Euler(0, 0, rotation);

                var velocity = avatar.velocity;
                var velocityRotation = AngleUtil.DirectionalRotation(velocity);
                var gravity = avatar.gravityScale * Physics2D.gravity * Time.deltaTime;

                var alignmentRotation = glideRotation * Quaternion.Inverse(velocityRotation);
                float alignment = Mathf.Cos(alignmentRotation.eulerAngles.z * Mathf.Deg2Rad);

                alignment = Mathf.Clamp01((alignment + 1) / 2);

                avatar.drag = minimumDrag + ((maximumDrag - minimumDrag) * dragOverAlignment.Evaluate(alignment));

                float alignDuration = minimumAlignDuration + (maximumAlignDuration - minimumAlignDuration) * alignDurationOverAlignment.Evaluate(alignment);
                var targetVelocity = glideRotation * Vector2.right * velocity.magnitude;

                float boostAcceleration = minimumBoost + ((maximumBoost - minimumBoost) * boostOverAlignment.Evaluate(alignment));
                var boost = (Vector2)(glideRotation * Vector2.right * boostAcceleration * Time.deltaTime);


                velocity = Vector2.SmoothDamp(velocity, targetVelocity, ref acceleration, alignDuration);
                velocity += boost;
                velocity += gravity;

                particles.SetParticleColor(colorOverAlignment.Evaluate(alignment));
                particles.SetParticleCount(minimumParticleCount + Mathf.RoundToInt((maximumParticleCount - minimumParticleCount) * particleCountOverAlignment.Evaluate(alignment)));

                return (velocity, rotation);
            };
        }
    }
}