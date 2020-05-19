using System;
using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;

public class Avatar : MonoBehaviour {
    enum MoveState {
        Grounded,
        Airborne,
        Gliding
    }
    enum GlidingMode {
        Dash,
        Sail
    }

    [SerializeField, Expandable]
    Rigidbody2D attachedRigidbody = default;

    [SerializeField, Expandable]
    SpriteRenderer attachedSprite = default;

    [SerializeField, Expandable]
    TrailRenderer glidingTrail = default;

    [SerializeField, Expandable]
    ParticleSystem dashParticles = default;

    [Header("Grounded/Falling movement")]
    [SerializeField, Range(0, 100)]
    float defaultSpeed = 10;
    [SerializeField, Range(0, 1)]
    float defaultSpeedLerp = 1;
    [SerializeField, Range(0, 2)]
    float defaultGravity = 1;
    [SerializeField, Range(0, 10)]
    float defaultDrag = 0;
    [SerializeField, Range(0, 100)]
    float liftoffSpeed = 10;

    [Header("Jumping/Rising movement")]
    [SerializeField, Range(0, 100)]
    float jumpingSpeed = 10;
    [SerializeField, Range(0, 1)]
    float jumpingSpeedLerp = 1;
    [SerializeField, Range(0, 2)]
    float jumpingGravity = 1;
    [SerializeField, Range(0, 10)]
    float jumpingDrag = 0;
    [SerializeField, Range(0, 100)]
    float risingSpeed = 10;
    [SerializeField, Range(-10, 10)]
    float cutoffSpeed = 0;
    [SerializeField, Range(0, 10)]
    int maximumGlideCharges = 1;

    [Header("Gliding movement")]
    [SerializeField]
    GlidingMode glidingMode = default;
    [SerializeField, Range(0, 100)]
    float glidingSpeed = 1;
    [SerializeField, Range(0, 2)]
    float glidingGravity = 1;
    [SerializeField, Range(0, 10)]
    float glidingDrag = 0;
    [SerializeField, Range(0, 100)]
    float glidingUpdrift = 10;
    [SerializeField, Range(0, 100)]
    float glidingRotation = 10;
    [SerializeField, Range(0, 10)]
    float glidingSailBoost = 1;
    [SerializeField, Range(0, 10)]
    float glidingDuration = 1;
    [SerializeField, Range(0, 100)]
    int glidingDashParticleCount = 10;

    [Header("Current Input")]
    public Vector2 intendedMovement = Vector2.zero;
    public bool intendsJump = false;
    public bool intendsGlide = false;

    [Header("Grounded Check")]
    [SerializeField, Range(0, 1)]
    float groundedRadius = 1;
    [SerializeField]
    LayerMask groundedLayers = default;

    bool isFacingRight = true;
    int facingSign => isFacingRight
        ? 1
        : -1;

    int currentGlideCharges = 0;
    bool canGlide => currentGlideCharges > 0;

    float glidingTimer = 0;

    MoveState state = MoveState.Airborne;
    public bool isGrounded => state == MoveState.Grounded;
    public bool isAirborne => state == MoveState.Airborne;
    public bool isGliding => state == MoveState.Gliding;

    Color stateColor {
        get {
            switch (state) {
                case MoveState.Grounded:
                    return groundedColor;
                case MoveState.Airborne:
                    return canGlide
                        ? airborneColor
                        : airborneNoChargeColor;
                case MoveState.Gliding:
                    return glidingColor;
                default:
                    throw new NotImplementedException(state.ToString());
            }
        }
    }

    [Header("State visualization")]
    [SerializeField, ColorUsage(true, true)]
    Color groundedColor = default;
    [SerializeField, ColorUsage(true, true)]
    Color airborneColor = default;
    [SerializeField, ColorUsage(true, true)]
    Color airborneNoChargeColor = default;
    [SerializeField, ColorUsage(true, true)]
    Color glidingColor = default;


    void Update() {
        attachedSprite.color = stateColor;
        attachedSprite.flipX = !isFacingRight;

        var emission = dashParticles.emission;
        emission.enabled = isGliding;
    }

    public void Glide() {
        state = MoveState.Gliding;
    }

    void FixedUpdate() {
        CalculateGrounded();

        var velocity = attachedRigidbody.velocity;
        float rotation = attachedRigidbody.rotation;
        float gravity = attachedRigidbody.gravityScale;
        float drag = attachedRigidbody.drag;

        if (velocity.x != 0) {
            isFacingRight = Mathf.Sign(velocity.x) == 1;
        }
        switch (state) {
            case MoveState.Grounded:
                currentGlideCharges = maximumGlideCharges;
                velocity.x = Mathf.Lerp(velocity.x, intendedMovement.x * defaultSpeed, defaultSpeedLerp);
                if (intendsJump) {
                    intendsJump = false;
                    velocity.y = liftoffSpeed;
                }
                gravity = defaultGravity;
                drag = defaultDrag;
                rotation = 0;
                break;
            case MoveState.Airborne:
                if (intendsGlide) {
                    if (canGlide) {
                        currentGlideCharges--;
                        glidingTimer = glidingDuration;
                        state = MoveState.Gliding;
                        switch (glidingMode) {
                            case GlidingMode.Dash:
                                var direction = intendedMovement.magnitude > 0
                                    ? intendedMovement.normalized
                                    : Vector2.right * facingSign;
                                velocity += glidingSpeed * direction;
                                rotation = Vector2.SignedAngle(Vector2.up, direction);
                                break;
                            case GlidingMode.Sail:
                                velocity += glidingSailBoost * Vector2.right * velocity.x;
                                rotation = -90 * facingSign;
                                break;
                        }
                        velocity.y += glidingUpdrift;
                        goto case MoveState.Gliding;
                    } else {
                        intendsGlide = false;
                    }
                }
                velocity.x = Mathf.Lerp(velocity.x, intendedMovement.x * jumpingSpeed, jumpingSpeedLerp);
                if (intendsJump && velocity.y > cutoffSpeed) {
                    velocity.y += risingSpeed * Time.deltaTime;
                    gravity = jumpingGravity;
                    drag = jumpingDrag;
                } else {
                    gravity = defaultGravity;
                    drag = defaultDrag;
                }
                rotation = 0;
                break;
            case MoveState.Gliding:
                if (glidingTimer > 0) {
                    glidingTimer -= Time.deltaTime;
                } else {
                    if (!intendsGlide) {
                        state = MoveState.Grounded;
                        FixedUpdate();
                        return;
                    }
                    switch (glidingMode) {
                        case GlidingMode.Dash:
                            break;
                        case GlidingMode.Sail:
                            if (intendedMovement.x != 0) {
                                float delta = -Math.Sign(intendedMovement.x) * glidingRotation;
                                rotation = isFacingRight
                                    ? Mathf.Clamp(rotation + delta, -179, -1)
                                    : Mathf.Clamp(rotation + delta, 1, 179);
                                velocity = Quaternion.Euler(0, 0, rotation) * Vector2.up * velocity.magnitude;
                            }
                            break;
                    }
                }
                gravity = glidingGravity;
                drag = glidingDrag;
                break;
            default:
                break;
        }

        attachedRigidbody.velocity = velocity;
        attachedRigidbody.rotation = Mathf.RoundToInt(rotation);
        attachedRigidbody.gravityScale = gravity;
        attachedRigidbody.drag = drag;
    }
    void CalculateGrounded() {
        if (isGliding) {
            return;
        }
        bool isGroundedNow = Physics2D
            .OverlapCircleAll(transform.position, groundedRadius, groundedLayers)
            .Any(collider => collider.gameObject != gameObject);
        if (isGroundedNow) {
            state = MoveState.Grounded;
        } else {
            state = MoveState.Airborne;
        }
    }
}