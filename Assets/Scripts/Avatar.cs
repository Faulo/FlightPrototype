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
    public SpriteRenderer attachedSprite = default;

    [SerializeField, Expandable]
    GroundedCheck groundedCheck = default;

    [SerializeField, Expandable]
    TrailRenderer glidingTrail = default;

    [SerializeField, Expandable]
    ParticleSystem dashParticles = default;

    [SerializeField]
    AvatarState currentState = default;

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

    [Header("Current Input")]
    public Vector2 intendedMovement = Vector2.zero;
    public float intendedRotation => intendedMovement.magnitude > 0
        ? Vector2.SignedAngle(Vector2.up, intendedMovement.normalized)
        : transform.rotation.eulerAngles.z;
    public bool intendsJump = false;
    public bool intendsGlide = false;

    [Header("Debug Info")]
    public bool isFacingRight = true;
    public int facingSign => isFacingRight
        ? 1
        : -1;


    int currentGlideCharges = 0;
    public bool canGlide => currentGlideCharges > 0;

    float glidingTimer = 0;

    public bool isGrounded;
    public bool isJumping;
    public bool isAirborne;
    public bool isGliding;

    void Start() {
        glidingParticlesEnabled = false;
    }

    void Update() {
        currentState.UpdateState();
    }

    void FixedUpdate() {
        var newState = currentState.CalculateNextState();

        if (currentState == newState) {
            currentState.FixedUpdateState();
        } else {
            currentState.ExitState();
            currentState = newState;
            currentState.EnterState();
        }
    }

    public bool CalculateGrounded() => groundedCheck.IsGrounded(gameObject);
}