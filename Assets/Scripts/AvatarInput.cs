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
        avatar.intendsJump = Input.GetButton(jumpButton);
        if (avatar.isAirborne) {
            if (Input.GetButtonDown(jumpButton)) {
                avatar.intendsGlide = true;
            }
        }
        if (avatar.isGliding) {
            avatar.intendsGlide = Input.GetButton(jumpButton);
        }
    }
}
