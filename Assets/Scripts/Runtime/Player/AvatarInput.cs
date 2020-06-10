using System;
using TheCursedBroom.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheCursedBroom.Player {
    public class AvatarInput : MonoBehaviour, Controls.IPlayerActions {
        [SerializeField]
        Avatar avatar = default;

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
        [SerializeField, Range(0, 1)]
        float flightDeadZone = 0;
        [SerializeField, Range(-360, 360)]
        int rotationOffset = 0;

        Controls controls;
        float jumpBufferTimer;

        void Awake() {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }
        public void OnEnable() {
            controls.Player.Enable();
        }
        public void OnDisable() {
            controls.Player.Disable();
        }

        public void OnMove(InputAction.CallbackContext context) {
            var input = context.ReadValue<Vector2>();
            avatar.intendedFacing = ProcessFacing(input);
            avatar.intendedMovement = ProcessMovement(input);
        }
        public void OnLook(InputAction.CallbackContext context) {
            var input = context.ReadValue<Vector2>();
            avatar.intendedFacing = ProcessFacing(input);
            avatar.intendedFlight = ProcessFlight(input);
            avatar.intendedRotation = ProcessRotation(input);
            avatar.intendsGlide = input.magnitude > flightDeadZone;
        }
        public void OnJump(InputAction.CallbackContext context) {
            if (context.started) {
                avatar.intendsJumpStart = true;
                jumpBufferTimer = jumpBufferDuration;
            }
            avatar.intendsJump = context.performed;
            if (context.canceled) {
                avatar.intendsJumpStart = false;
            }
        }
        public void OnCrouch(InputAction.CallbackContext context) {
            avatar.intendsCrouch = context.performed;
        }
        public void OnGlide(InputAction.CallbackContext context) {
            avatar.intendsGlide = context.performed;
        }

        void FixedUpdate() {
            if (avatar.intendsJumpStart) {
                if (jumpBufferTimer >= 0) {
                    jumpBufferTimer -= Time.deltaTime;
                } else {
                    avatar.intendsJumpStart = false;
                }
            }
        }

        int ProcessFacing(Vector2 input) {
            return Mathf.Abs(input.x) > turningDeadZone
                ? Math.Sign(input.x)
                : avatar.intendedFacing;
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
            if (input.magnitude <= flightDeadZone) {
                input = avatar.currentForward;
            }
            return Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, input) + rotationOffset * avatar.facingSign);
        }
    }
}