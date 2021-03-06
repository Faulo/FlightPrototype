﻿using UnityEngine;

namespace TheCursedBroom.Player {
    public abstract class AvatarMovement : ScriptableObject {
        public abstract MovementCalculator CreateMovementCalculator(AvatarController avatar);
    }
}