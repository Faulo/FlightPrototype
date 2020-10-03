using UnityEngine;
using UnityEngine.Serialization;

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
        [SerializeField, Range(-100, 0)]
        float requiredYSpeed = -1;
        [SerializeField, Range(1, 100)]
        int boostGatheringFrameCount = 10;
        [SerializeField, Range(0, 10)]
        float boostRotationDuration = 0;
        [SerializeField, Range(0, 10)]
        float boostExecutionFrameMultiplier = 1;
        [SerializeField, Range(0, 100)]
        float boostExecutionSpeed = 1;
        [SerializeField, Range(0, 10)]
        float boostDrag = 0;

        [Header("Events")]
        [SerializeField]
        GameObjectEvent onBoost = default;

        public override MovementCalculator CreateMovementCalculator(AvatarController avatar) {
            float angularVelocity = 0;
            var acceleration = Vector2.zero;
            int boostGatheringTimer = 0;
            int boostExecutionTimer = 0;
            int boostExecutionFrameCount = 0;
            return () => {
                float currentAngle = avatar.rotationAngle;

                var targetDirection = avatar.intendedFlight == Vector2.zero
                    ? avatar.forward
                    : avatar.intendedFlight;
                float targetAngle = AngleUtil.DirectionalAngle(targetDirection);
                if (directionsNormalized) {
                    targetAngle = AngleUtil.RoundAngle(targetAngle, directionRange);
                }
                var targetRotation = Quaternion.Euler(0, 0, targetAngle);

                var currentVelocity = avatar.velocity;

                (Vector2, float) boost() {
                    avatar.drag = boostDrag;

                    boostExecutionTimer++;
                    if (boostExecutionTimer >= boostExecutionFrameCount) {
                        boostExecutionTimer = 0;
                        avatar.broom.isBoosting = false;
                        angularVelocity = 0;
                        return glide();
                    }

                    float angle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref angularVelocity, boostRotationDuration);
                    var rotation = Quaternion.Euler(0, 0, angle);
                    var targetVelocity = (Vector2)(rotation * Vector2.right * currentVelocity.magnitude);

                    var velocity = targetVelocity;

                    return (velocity, angle);
                }

                (Vector2, float) glide() {
                    float angle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref angularVelocity, rotationDuration);
                    var rotation = Quaternion.Euler(0, 0, angle);

                    var velocityRotation = AngleUtil.DirectionalRotation(currentVelocity);

                    float alignment = AngleUtil.Alignment(rotation, velocityRotation);

                    float alignDuration = GetAlignDuration(alignment);
                    var targetVelocity = rotation * Vector2.right * currentVelocity.magnitude;

                    avatar.drag = GetDrag(alignment);

                    avatar.broom.isAligned = alignment > requiredAlignment;
                    avatar.broom.isDiving = currentVelocity.y < requiredYSpeed;
                    bool intendsBoost = targetAngle <= 180;

                    if (intendsBoost) {
                        if (avatar.broom.canBoost) {
                            avatar.broom.canBoost = false;
                            avatar.broom.isBoosting = true;
                            float boostDuration = Mathf.Abs(currentVelocity.y);
                            boostDuration *= boostExecutionFrameMultiplier;
                            boostExecutionFrameCount = Mathf.RoundToInt(boostDuration);
                            onBoost.Invoke(avatar.gameObject);
                            currentVelocity += currentVelocity.normalized * boostExecutionSpeed;
                            angularVelocity = 0;
                            return boost();
                        }
                    } else {
                        if (avatar.broom.isAligned && avatar.broom.isDiving) {
                            if (!avatar.broom.canBoost && boostGatheringTimer < boostGatheringFrameCount) {
                                boostGatheringTimer++;
                                if (boostGatheringTimer >= boostGatheringFrameCount) {
                                    boostGatheringTimer = 0;
                                    avatar.broom.canBoost = true;
                                }
                            }
                        }
                    }

                    var velocity = Vector2.SmoothDamp(currentVelocity, targetVelocity, ref acceleration, alignDuration);
                    velocity += (Vector2)(rotation * Vector2.right * GetAcceleration(alignment) * Time.deltaTime);
                    velocity += avatar.gravityStep;

                    return (velocity, angle);
                }
                return avatar.broom.isBoosting
                    ? boost()
                    : glide();
            };
        }
    }
}