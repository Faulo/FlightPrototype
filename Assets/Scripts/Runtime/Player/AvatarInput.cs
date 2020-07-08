using System;
using System.Linq;
using TheCursedBroom.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Haptics;

namespace TheCursedBroom.Player {
    public class AvatarInput : MonoBehaviour, Controls.IPlayerActions {
        [SerializeField]
        AvatarController avatar = default;

        [Header("Input Refinement")]
        [SerializeField, Range(0, 10)]
        float jumpBufferDuration = 0;
        [SerializeField, Range(0, 1)]
        float turningDeadZone = 0;
        [Header("Directional Input, grounded")]
        [SerializeField, Range(0, 1)]
        float movementDeadZone = 0;
        [SerializeField, Tooltip("Normalize to input range [0, 1] instead of [deadzone, 1]")]
        bool movementNormalized = true;
        [SerializeField, Tooltip("Use Ceil/Floor instead of Round, to round away from 0.")]
        bool movementAlwaysRoundUp = true;
        [SerializeField, Range(1, 360), Tooltip("How many different input values are possible")]
        int movementRange = 360;
        [Header("Directional Input, flying")]
        [SerializeField, Range(0, 1)]
        float flightDeadZone = 0;
        [SerializeField, Tooltip("Normalize to input range [0, 1] instead of [deadzone, 1]")]
        bool flightNormalized = true;

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
            rumbleMotor?.ResetHaptics();
        }
        void Start() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.pauseStateChanged += state => ApplicationPauseListener(state == UnityEditor.PauseState.Paused);
#endif
        }

        public void OnMove(InputAction.CallbackContext context) {
            var input = context.ReadValue<Vector2>();
            avatar.intendedFacing = ProcessFacing(input);
            avatar.intendedMovement = ProcessMovement(input);
            avatar.intendedFlight = ProcessFlight(input);
        }
        public void OnLook(InputAction.CallbackContext context) {
        }
        public void OnJump(InputAction.CallbackContext context) {
            SetRumble(context.control.device as IDualMotorRumble);
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
            SetRumble(context.control.device as IDualMotorRumble);
            avatar.intendsCrouch = context.performed;
        }
        public void OnGlide(InputAction.CallbackContext context) {
            SetRumble(context.control.device as IDualMotorRumble);
            avatar.intendsGlide = context.performed;
        }
        public void OnSave(InputAction.CallbackContext context) {
            if (context.started) {
                avatar.intendsSave = true;
            }
            if (context.canceled) {
                avatar.intendsSave = false;
            }
        }
        public void OnLoad(InputAction.CallbackContext context) {
            if (context.started) {
                avatar.intendsLoad = true;
            }
            if (context.canceled) {
                avatar.intendsLoad = false;
            }
        }

        void FixedUpdate() {
            if (avatar.intendsJumpStart) {
                if (jumpBufferTimer >= 0) {
                    jumpBufferTimer -= Time.deltaTime;
                } else {
                    avatar.intendsJumpStart = false;
                }
            }
            rumbleMotor?.SetMotorSpeeds(avatar.rumblingLowIntensity, avatar.rumblingHighIntensity);
        }

        int ProcessFacing(Vector2 input) {
            return Math.Abs(input.x) > turningDeadZone
                ? Math.Sign(input.x)
                : avatar.intendedFacing;
        }

        float ProcessMovement(Vector2 intention) {
            float x = intention.x;
            if (Math.Abs(x) <= movementDeadZone) {
                x = 0;
            }

            if (movementNormalized) {
                x = (Math.Abs(x) - movementDeadZone) * Math.Sign(x) / (1 - movementDeadZone);
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
            if (Math.Abs(intention.x) <= flightDeadZone) {
                intention.x = 0;
            }
            if (Math.Abs(intention.y) <= flightDeadZone) {
                intention.y = 0;
            }

            if (flightNormalized) {
                intention.x = (Math.Abs(intention.x) - flightDeadZone) * Math.Sign(intention.x) / (1 - flightDeadZone);
                intention.y = (Math.Abs(intention.y) - flightDeadZone) * Math.Sign(intention.y) / (1 - flightDeadZone);
            }

            return intention;
        }

        public void OnStart(InputAction.CallbackContext context) {
            avatar.intendsReset = context.performed;
        }

        #region Rumble
        IDualMotorRumble rumbleMotor;
        void ApplicationPauseListener(bool isPaused) {
            if (isPaused) {
                rumbleMotor?.PauseHaptics();
            } else {
                rumbleMotor?.ResumeHaptics();
            }
        }

        void SetRumble(IDualMotorRumble newMotor) {
            if (rumbleMotor != newMotor) {
                rumbleMotor?.ResetHaptics();
                rumbleMotor = newMotor;
                rumbleMotor?.ResetHaptics();
            }
        }
        #endregion
    }
}