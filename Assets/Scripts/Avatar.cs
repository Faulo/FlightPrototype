using System;
using AvatarStateMachine;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;

public class Avatar : MonoBehaviour {
    public event Action<Avatar> onJump;
    public event Action<Avatar> onDash;

    [SerializeField, Expandable]
    public Rigidbody2D attachedRigidbody = default;

    [SerializeField, Expandable]
    SpriteRenderer attachedSprite = default;

    [SerializeField, Expandable]
    GroundedCheck groundedCheck = default;

    [SerializeField, Expandable]
    TrailRenderer glidingTrail = default;

    [SerializeField, Expandable]
    ParticleSystem dashParticles = default;

    public bool glidingParticlesEnabled {
        set {
            var emission = dashParticles.emission;
            emission.enabled = value;
        }
    }

    [Header("Movement")]
    [SerializeField, Range(0, 100)]
    public float maximumRunningSpeed = 10;
    [SerializeField, Range(0, 10)]
    int maximumGlideCharges = 1;

    public void RechargeDashes() {
        currentGlideCharges = maximumGlideCharges;
    }
    public void UseDashCharge() {
        Assert.IsTrue(currentGlideCharges > 0);
        currentGlideCharges--;
    }

    [Header("State configuration")]
    [SerializeField, Expandable]
    Grounded groundedState = default;
    [SerializeField, Expandable]
    Jumping jumpingState = default;
    [SerializeField, Expandable]
    Airborne airborneState = default;
    [SerializeField, Expandable]
    Gliding glidingState = default;

    [SerializeField]
    AvatarStates currentState;
    AvatarStateBehaviour currentStateBehaviour {
        get {
            switch (currentState) {
                case AvatarStates.Grounded:
                    return groundedState;
                case AvatarStates.Jumping:
                    return jumpingState;
                case AvatarStates.Airborne:
                    return airborneState;
                case AvatarStates.Gliding:
                    return glidingState;
                default:
                    throw new NotImplementedException(currentState.ToString());
            }
        }
    }

    [Header("Current Input")]
    public Vector2 intendedMovement = Vector2.zero;
    public float intendedRotation => intendedMovement.magnitude > 0
        ? Vector2.SignedAngle(Vector2.up, intendedMovement.normalized)
        : transform.rotation.eulerAngles.z;
    public bool intendsJump = false;
    public bool intendsGlide = false;


    public bool isFacingRight = true;
    public int facingSign => isFacingRight
        ? 1
        : -1;


    int currentGlideCharges = 0;
    public bool canGlide => currentGlideCharges > 0;

    float glidingTimer = 0;

    public bool isGrounded => currentState == AvatarStates.Grounded;
    public bool isJumping => currentState == AvatarStates.Jumping;
    public bool isAirborne => currentState == AvatarStates.Airborne;
    public bool isGliding => currentState == AvatarStates.Gliding;

    void Start() {
        currentState = AvatarStates.Grounded;
        glidingParticlesEnabled = false;
    }

    void Update() {
        attachedSprite.color = currentStateBehaviour.stateColor;
        attachedSprite.flipX = !isFacingRight;
    }

    void FixedUpdate() {
        UpdateStateMachine();
    }

    void UpdateStateMachine() {
        var newState = CalculateState();

        if (currentState == newState) {
            currentStateBehaviour.FixedUpdateState(this);
        } else {
            currentStateBehaviour.ExitState(this);
            currentState = newState;
            currentStateBehaviour.EnterState(this);
        }
    }

    AvatarStates CalculateState() {
        if (shouldBeGliding) {
            return AvatarStates.Gliding;
        }
        if (shouldBeJumping) {
            return AvatarStates.Jumping;
        }
        if (shouldBeAirborne) {
            return AvatarStates.Airborne;
        }
        if (shouldBeGrounded) {
            return AvatarStates.Grounded;
        }
        return currentState;
    }

    bool shouldBeGliding => currentStateBehaviour.ShouldTransitionToGliding(this);
    bool shouldBeJumping => currentStateBehaviour.ShouldTransitionToJumping(this);
    bool shouldBeAirborne => currentStateBehaviour.ShouldTransitionToAirborne(this);
    bool shouldBeGrounded => currentStateBehaviour.ShouldTransitionToGrounded(this);

    public bool CalculateGrounded() => groundedCheck.IsGrounded(gameObject);
}