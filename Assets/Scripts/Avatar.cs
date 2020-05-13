using System;
using System.Collections;
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

    [Header("Movement")]
    [SerializeField, Range(0, 10)]
    float maxSpeed = 1;
    [SerializeField, Range(0, 10)]
    float jumpHeight = 1;
    float jumpForce => Mathf.Sqrt(jumpHeight * -20f * Physics2D.gravity.y);

    Vector2 input = Vector2.zero;
    bool isJumping = false;

    MoveState state = MoveState.Airborne;
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

    [SerializeField, ColorUsage(true, true)]
    Color groundedColor = default;
    [SerializeField, ColorUsage(true, true)]
    Color airborneColor = default;
    [SerializeField, ColorUsage(true, true)]
    Color glidingColor = default;

    void Start() {
    }

    void Update() {
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.anyKeyDown) {
            switch (state) {
                case MoveState.Grounded:
                    Jump();
                    break;
                case MoveState.Airborne:
                    Glide();
                    break;
                case MoveState.Gliding:
                    break;
                default:
                    break;
            }
        }
        attachedSprite.color = stateColor;
    }

    void Jump() {
        isJumping = true;
    }
    void Glide() {
        state = MoveState.Gliding;
    }

    void FixedUpdate() {
        var velocity = attachedRigidbody.velocity;
        float rotation = attachedRigidbody.rotation;
        switch (state) {
            case MoveState.Grounded:
                velocity.x = input.x * maxSpeed;
                velocity.y = isJumping
                    ? jumpForce
                    : 0;
                isJumping = false;
                rotation = 0;
                break;
            case MoveState.Airborne:
                rotation = 0;
                break;
            case MoveState.Gliding:
                rotation = 90;
                break;
            default:
                break;
        }
        attachedRigidbody.velocity = velocity;
        attachedRigidbody.rotation = rotation;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        state = MoveState.Grounded;
    }
    void OnCollisionExit2D(Collision2D collision) {
        state = MoveState.Airborne;
    }
}