using UnityEngine;

public class AvatarInput : MonoBehaviour {
    [SerializeField]
    Avatar avatar = default;

    [SerializeField]
    string horizontalAxis = "Horizontal";
    [SerializeField]
    string verticalAxis = "Vertical";
    [SerializeField]
    string jumpButton = "Fire1";

    void Update() {
        avatar.intendedMovement = new Vector2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis));
        if (avatar.isGrounded) {
            avatar.intendsGlide = false;
            if (Input.GetButtonDown(jumpButton)) {
                avatar.intendsJump = true;
            }
        }
        if (avatar.isJumping) {
            avatar.intendsJump = Input.GetButton(jumpButton);
        }
        if (avatar.isAirborne) {
            avatar.intendsJump = false;
            if (Input.GetButtonDown(jumpButton)) {
                avatar.intendsGlide = true;
            }
        }
        if (avatar.isGliding) {
            avatar.intendsGlide = Input.GetButton(jumpButton);
        }
    }
}
