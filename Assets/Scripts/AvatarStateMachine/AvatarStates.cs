using System;

namespace AvatarStateMachine {
    [Flags]
    public enum AvatarStates {
        Grounded = 1 << 0,
        Jumping = 1 << 1,
        Airborne = 1 << 2,
        Gliding = 1 << 3
    }
}