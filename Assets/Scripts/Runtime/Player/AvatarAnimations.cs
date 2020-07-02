using System;

namespace TheCursedBroom.Player {
    [Flags]
    public enum AvatarAnimations {
        Idling = 1 << 0,
        Jumping = 1 << 1,
        Hanging = 1 << 2,
        Falling = 1 << 3,
        Mounting = 1 << 4,
        Flying = 1 << 5,
        Dismounting = 1 << 6,
        Crouching = 1 << 7,
        WallCollision = 1 << 8,
        WallTurn = 1 << 9,
        WallJump = 1 << 10,
        WallPlummet = 1 << 11,
    }
}