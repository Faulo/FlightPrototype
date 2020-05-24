using Slothsoft.UnityExtensions;
using UnityEngine;


namespace AvatarStateMachine {
    public class Dash : AvatarState {
        [Header("Dash")]
        [SerializeField, Range(0.0001f, 100)]
        float dashDistance = 1;
        [SerializeField, Range(0.0001f, 100)]
        float dashSpeed = 1;
        [SerializeField, Range(0.0001f, 100)]
        float exitSpeed = 1;
        [SerializeField, Range(2, 360)]
        int dashDirections = 8;

        [Header("Sub-components")]
        [SerializeField, Expandable]
        ParticleSystem particles = default;
        bool particlesEnabled {
            set {
                if (value) {
                    particles.Play();
                } else {
                    particles.Stop();
                }
            }
        }

        float dashDuration => dashDistance / dashSpeed;

        float dashTimer;
        float rotation;
        Vector2 velocity;
        public override void EnterState() {
            base.EnterState();

            dashTimer = 0;
            particlesEnabled = true;

            avatar.UseDashCharge();

            rotation = Mathf.RoundToInt(avatar.intendedRotation.eulerAngles.z * dashDirections / 360) * 360 / dashDirections;
            velocity = Quaternion.Euler(0, 0, rotation) * Vector2.up * dashSpeed;

            avatar.attachedRigidbody.constraints = RigidbodyConstraints2D.None;
            avatar.attachedRigidbody.velocity = velocity;
            avatar.attachedRigidbody.rotation = rotation;
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            dashTimer += Time.deltaTime;
            avatar.attachedRigidbody.velocity = velocity;
        }

        public override void ExitState() {
            base.ExitState();
            particlesEnabled = false;
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState glidingState = default;
        [SerializeField, Expandable]
        AvatarState airborneState = default;
        public override AvatarState CalculateNextState() {
            if (dashTimer > dashDuration) {
                if (avatar.intendsGlide) {
                    return glidingState;
                } else {
                    avatar.attachedRigidbody.rotation = 0;
                    avatar.attachedRigidbody.velocity = Quaternion.Euler(0, 0, rotation) * Vector2.up * exitSpeed;
                    avatar.attachedRigidbody.velocity += Physics2D.gravity * Time.deltaTime;
                    avatar.attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                    return airborneState;
                }
            }
            return base.CalculateNextState();
        }
    }
}