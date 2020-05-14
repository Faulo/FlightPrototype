using UnityEngine;

public class AvatarInput : MonoBehaviour {
    [SerializeField]
    Avatar avatar = default;
    void Update() {
        avatar.intendedMovement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        avatar.intendsJump = Input.anyKey;
        if (avatar.isAirborne) {
            avatar.intendsGlide = Input.anyKeyDown;
        }
        if (avatar.isGliding) {
            avatar.intendsGlide = Input.anyKey;
        }
    }
}
