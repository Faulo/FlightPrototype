using System.Linq;
using TheCursedBroom.Extensions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace TheCursedBroom.Player.AvatarMovements {
    [CreateAssetMenu()]
    public class GlideMovement : AvatarMovement {
        enum BoostState {
            Inactive,
            Gathering,
            Start,
            Executing
        }
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
        float GetDrag(float alignment) {
            return minimumDrag + ((maximumDrag - minimumDrag) * dragOverAlignment.Evaluate(alignment));
        }

        [Header("Velocity Conversion")]
        [SerializeField]
        AnimationCurve alignDurationOverAlignment = default;
        [SerializeField, Range(0, 10)]
        float minimumAlignDuration = 0;
        [SerializeField, Range(0, 10)]
        float maximumAlignDuration = 1;
        float GetAlignDuration(float alignment) {
            return minimumAlignDuration + ((maximumAlignDuration - minimumAlignDuration) * alignDurationOverAlignment.Evaluate(alignment));
        }

        [Header("Acceleration")]
        [SerializeField, FormerlySerializedAs("boostOverAlignment")]
        AnimationCurve accelerationOverAlignment = default;
        [SerializeField, Range(0, 100)]
        float minimumAcceleration = 0;
        [SerializeField, Range(0, 100)]
        float maximumAcceleration = 1;
        float GetAcceleration(float alignment) {
            return minimumAcceleration + ((maximumAcceleration - minimumAcceleration) * accelerationOverAlignment.Evaluate(alignment));
        }

        [Header("Boost")]
        [SerializeField, Range(0, 1)]
        float requiredAlignment = 1;
        [SerializeField, Range(0, 100)]
        float requiredSpeed = 10;
        [SerializeField, Range(1, 100)]
        int boostGatheringFrameCount = 10;
        [SerializeField, Range(0, 10)]
        float boostAlignDuration = 0;
        [SerializeField, Range(1, 100)]
        int boostExecutionFrameCount = 10;
        [SerializeField, Range(0, 100)]
        float boostAcceleration = 1;
        [SerializeField, Range(0, 1000)]
        int boostParticleCount = 100;
        [SerializeField, Range(0, 10)]
        float boostParticleSpeed = 2;
        [SerializeField, Range(0, 10)]
        float boostDrag = 0;

        [Header("Visualization")]
        [SerializeField]
        string particlesName = "ParticleTrail_Glide";
        [SerializeField]
        Gradient colorOverAlignment = default;
        [SerializeField]
        Gradient boostGatheringColor = default;
        [SerializeField]
        Gradient boostExecutionColor = default;
        [SerializeField]
        AnimationCurve particleCountOverAlignment = default;
        [SerializeField, Range(0, 1000)]
        int minimumParticleCount = 10;
        [SerializeField, Range(0, 1000)]
        int maximumParticleCount = 100;

        [Header("Events")]
        [SerializeField]
        GameObjectEvent onBoost = default;

        public override MovementCalculator CreateMovementCalculator(AvatarController avatar) {
            var particles = avatar
                .GetComponentsInChildren<ParticleSystem>()
                .FirstOrDefault(p => p.name == particlesName);
            Assert.IsNotNull(particles, $"Couldn't find particles '{particlesName}'!");
            float angularVelocity = 0;
            var acceleration = Vector2.zero;
            var boostState = BoostState.Inactive;
            int boostGatheringTimer = 0;
            int boostExecutionTimer = 0;
            return () => {
                var targetDirection = avatar.intendedFlight == Vector2.zero
                    ? avatar.forward
                    : avatar.intendedFlight;
                float targetAngle = AngleUtil.DirectionalAngle(targetDirection);

                if (directionsNormalized) {
                    targetAngle = AngleUtil.RoundAngle(targetAngle, directionRange);
                }
                var targetRotation = Quaternion.Euler(0, 0, targetAngle);

                float currentAngle = Mathf.SmoothDampAngle(avatar.rotationAngle, targetAngle, ref angularVelocity, rotationDuration);
                var currentRotation = Quaternion.Euler(0, 0, currentAngle);

                var velocity = avatar.velocity;
                var velocityRotation = AngleUtil.DirectionalRotation(velocity);

                float alignment = AngleUtil.Alignment(currentRotation, velocityRotation);

                float alignDuration = GetAlignDuration(alignment);
                var targetVelocity = currentRotation * Vector2.right * velocity.magnitude;

                var particleColor = colorOverAlignment.Evaluate(alignment);
                int particleCount = minimumParticleCount + Mathf.RoundToInt((maximumParticleCount - minimumParticleCount) * particleCountOverAlignment.Evaluate(alignment));
                float particleSpeed = 1;

                switch (boostState) {
                    case BoostState.Inactive:
                    case BoostState.Gathering:
                        avatar.drag = GetDrag(alignment);

                        if (alignment > requiredAlignment) {
                            if (velocity.magnitude > requiredSpeed) {
                                if (boostGatheringTimer < boostGatheringFrameCount) {
                                    boostGatheringTimer++;
                                }
                                boostState = BoostState.Gathering;
                                particleColor = boostGatheringColor.Evaluate((float)boostGatheringTimer / (boostGatheringFrameCount - 1));
                            }
                        } else {
                            if (boostGatheringTimer == boostGatheringFrameCount) {
                                boostGatheringTimer = 0;
                                boostState = BoostState.Executing;
                                onBoost.Invoke(avatar.gameObject);
                                goto case BoostState.Executing;
                            } else {
                                boostState = BoostState.Inactive;
                                if (boostGatheringTimer > 0) {
                                    boostGatheringTimer--;
                                }
                            }
                        }
                        velocity = Vector2.SmoothDamp(velocity, targetVelocity, ref acceleration, alignDuration);
                        velocity += (Vector2)(currentRotation * Vector2.right * GetAcceleration(alignment) * Time.deltaTime);
                        velocity += avatar.gravityStep;
                        break;
                    case BoostState.Executing:
                        avatar.drag = boostDrag;

                        velocity = Vector2.SmoothDamp(velocity, targetVelocity, ref acceleration, boostAlignDuration);
                        velocity += (Vector2)(targetRotation * Vector2.right * boostAcceleration * Time.deltaTime);

                        particleColor = boostExecutionColor.Evaluate((float)boostExecutionTimer / (boostExecutionFrameCount - 1));
                        particleCount = boostParticleCount;
                        particleSpeed = boostParticleSpeed;

                        boostExecutionTimer++;
                        if (boostExecutionTimer == boostExecutionFrameCount) {
                            boostExecutionTimer = 0;
                            boostState = BoostState.Inactive;
                        }
                        break;
                }
                particles.SetParticleColor(particleColor);
                particles.SetParticleCount(particleCount);
                particles.SetStartSpeedMultiplier(2);

                return (velocity, currentAngle);
            };
        }
    }
}