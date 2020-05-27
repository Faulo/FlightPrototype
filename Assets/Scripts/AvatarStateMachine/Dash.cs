using Slothsoft.UnityExtensions;
using UnityEngine;


namespace AvatarStateMachine {
    public class Dash : AvatarState {
        enum VelocityMode {
            SetVelocity,
            AddVelocity
        }
        [Header("Dash")]
        [SerializeField, Range(0, 10)]
        float dashDuration = 1;
        [SerializeField]
        VelocityMode initialMode = default;
        [SerializeField, Range(-100, 100)]
        float initialSpeed = 15;
        [SerializeField]
        VelocityMode exitMode = default;
        [SerializeField, Range(-100, 100)]
        float exitSpeed = 1;
        [SerializeField, Range(1, 360)]
        int dashDirections = 8;


        float dashTimer;
        float rotation;
        Vector2 velocity;
        public override void EnterState() {
            base.EnterState();

            dashTimer = 0;

            //avatar.AlignFaceToIntend();
            avatar.UseDashCharge();

            rotation = Mathf.RoundToInt(avatar.intendedRotation.eulerAngles.z * dashDirections / 360) * 360 / dashDirections;
            velocity = Quaternion.Euler(0, 0, rotation) * Vector2.right * initialSpeed * avatar.facingSign;

            switch (initialMode) {
                case VelocityMode.SetVelocity:
                    break;
                case VelocityMode.AddVelocity:
                    velocity += avatar.attachedRigidbody.velocity;
                    break;
            }

            avatar.attachedRigidbody.constraints = RigidbodyConstraints2D.None;
            avatar.attachedRigidbody.velocity = velocity;
            avatar.attachedRigidbody.rotation = rotation;
            //avatar.attachedSprite.transform.rotation = avatar.transform.rotation * Quaternion.Euler(0, 0, 90 * avatar.facingSign);
        }
        public override void FixedUpdateState() {
            base.FixedUpdateState();

            dashTimer += Time.deltaTime;
            avatar.attachedRigidbody.velocity = velocity;
        }

        public override void ExitState() {
            base.ExitState();

            //avatar.attachedSprite.transform.rotation = avatar.transform.rotation;
        }

        [Header("Transitions")]
        [SerializeField, Expandable]
        AvatarState glidingState = default;
        [SerializeField, Expandable]
        AvatarState airborneState = default;
        public override AvatarState CalculateNextState() {
            if (dashTimer >= dashDuration) {
                if (avatar.intendsGlide) {
                    return glidingState;
                } else {
                    velocity = Quaternion.Euler(0, 0, rotation) * Vector2.up * exitSpeed;
                    velocity += Physics2D.gravity * Time.deltaTime;
                    switch (exitMode) {
                        case VelocityMode.SetVelocity:
                            break;
                        case VelocityMode.AddVelocity:
                            velocity += avatar.attachedRigidbody.velocity;
                            break;
                    }

                    avatar.attachedRigidbody.rotation = 0;
                    avatar.attachedRigidbody.velocity = velocity;
                    avatar.attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                    return airborneState;
                }
            }
            return base.CalculateNextState();
        }
    }
}