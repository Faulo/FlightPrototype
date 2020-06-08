using System;
using UnityEngine;

namespace TheCursedBroom.Player {
    public class AvatarInput : MonoBehaviour {
        [SerializeField]
        Avatar avatar = default;

        [SerializeField]
        string horizontalAxis = "Horizontal";
        [SerializeField]
        string verticalAxis = "Vertical";
        [SerializeField]
        string jumpButton = "Fire1";
        [SerializeField]
        string glideButton = "Fire2";
        [SerializeField]
        string crouchButton = "Fire3";

        [Header("Input Refinement")]
        [SerializeField, Range(0, 10)]
        float jumpBufferDuration = 0;
        [SerializeField, Range(0, 1)]
        float turningDeadZone = 0;
        [SerializeField, Range(0, 1)]
        float movementDeadZone = 0;
        [SerializeField, Tooltip("Normalize to input range [0, 1] instead of [deadzone, 1]")]
        bool movementNormalized = true;
        [SerializeField, Tooltip("Use Ceil/Floor instead of Round, to round away from 0.")]
        bool movementAlwaysRoundUp = true;
        [SerializeField, Range(1, 360), Tooltip("How many different input values are possible")]
        int movementRange = 360;

        float jumpBufferTimer;

        void Update() {
            var input = new Vector2(Input.GetAxisRaw(horizontalAxis), Input.GetAxisRaw(verticalAxis));

            avatar.intendedFacing = ProcessFacing(input);
            avatar.intendedMovement = ProcessMovement(input);
            avatar.intendedFlight = ProcessFlight(input);
            avatar.intendedRotation = ProcessRotation(input);

            if (jumpBufferTimer >= 0) {
                jumpBufferTimer -= Time.deltaTime;
            }
            if (Input.GetButtonDown(jumpButton)) {
                avatar.intendsJumpStart = true;
                jumpBufferTimer = jumpBufferDuration;
            }
            avatar.intendsJump = Input.GetButton(jumpButton);
            if (Input.GetButtonUp(jumpButton) || jumpBufferTimer < 0) {
                avatar.intendsJumpStart = false;
            }

            avatar.intendsGlide = Input.GetButton(glideButton);
            avatar.intendsCrouch = Input.GetButton(crouchButton);
        }

        int ProcessFacing(Vector2 input) {
            return Mathf.Abs(input.x) > turningDeadZone
                ? Math.Sign(input.x)
                : 0;
        }

        float ProcessMovement(Vector2 intention) {
            float x = intention.x;
            if (Mathf.Abs(x) <= movementDeadZone) {
                x = 0;
            }

            if (movementNormalized) {
                x = (Mathf.Abs(x) - movementDeadZone) * Math.Sign(x) / (1 - movementDeadZone);
            }

            if (movementAlwaysRoundUp) {
                x = x > 0
                    ? Mathf.Ceil(x * movementRange) / movementRange
                    : Mathf.Floor(x * movementRange) / movementRange;
            } else {
                x = Mathf.Round(x * movementRange) / movementRange;
            }

            return x;
        }
        Vector2 ProcessFlight(Vector2 intention) {
            return intention;
        }
        Quaternion ProcessRotation(Vector2 input) {
            return input.magnitude > 0
                ? Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, input.normalized))
                : Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, Vector2.right * avatar.facingSign));
        }
    }
}