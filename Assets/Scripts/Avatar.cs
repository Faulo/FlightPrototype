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

    [SerializeField, Expandable]
    Rigidbody2D attachedRigidbody = default;

    [SerializeField, Expandable]
    SpriteRenderer attachedSprite = default;

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

    [Header("Gliding movement")]
    [SerializeField, Range(0, 10)]
    float glidingSpeedMultiplier = 1;
    [SerializeField, Range(0, 2)]
    float glidingGravity = 1;
    [SerializeField, Range(0, 10)]
    float glidingDrag = 0;
    [SerializeField, Range(0, 100)]
    float glidingUpdrift = 10;

    [Header("Current Input")]
    public Vector2 intendedMovement = Vector2.zero;
    public bool intendsJump = false;
    public bool intendsGlide = false;

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
                    return airborneColor;
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
    Color glidingColor = default;


    void Update() {
        attachedSprite.color = stateColor;
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

        switch (state) {
            case MoveState.Grounded:
                velocity.x = Mathf.Lerp(velocity.x, intendedMovement.x * defaultSpeed, defaultSpeedLerp);
                if (intendsJump) {
                    velocity.y = liftoffSpeed;
                }
                gravity = defaultGravity;
                drag = defaultDrag;
                rotation = 0;
                break;
            case MoveState.Airborne:
                if (intendsGlide) {
                    state = MoveState.Gliding;
                    velocity.x *= glidingSpeedMultiplier;
                    velocity.y += glidingUpdrift;
                    goto case MoveState.Gliding;
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
                if (!intendsGlide) {
                    state = MoveState.Grounded;
                    FixedUpdate();
                    return;
                }
                gravity = glidingGravity;
                drag = glidingDrag;
                rotation = 90;
                break;
            default:
                break;
        }

        attachedRigidbody.velocity = velocity;
        attachedRigidbody.rotation = rotation;
        attachedRigidbody.gravityScale = gravity;
        attachedRigidbody.drag = drag;
    }

    [Header("Grounded Check")]
    [SerializeField, Range(0, 1)]
    float groundedRadius = 1;
    [SerializeField]
    LayerMask groundedLayers = default;
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