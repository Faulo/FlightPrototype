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

        void Update() {
            avatar.intendedMovement = new Vector2(Input.GetAxisRaw(horizontalAxis), Input.GetAxisRaw(verticalAxis));
            if (avatar.intendsJump || Input.GetButtonDown(jumpButton)) {
                avatar.intendsJump = Input.GetButton(jumpButton);
            }
            if (avatar.intendsGlide || Input.GetButtonDown(glideButton)) {
                avatar.intendsGlide = Input.GetButton(glideButton);
            }
            if (avatar.intendsCrouch || Input.GetButtonDown(crouchButton)) {
                avatar.intendsCrouch = Input.GetButton(crouchButton);
            }
        }
    }
}