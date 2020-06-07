using System;
using UnityEngine;

namespace TheCursedBroom.Player {
    public abstract class AvatarMovement : ScriptableObject {
        public abstract Func<Vector2> CreateVelocityCalculator(Avatar avatar);
    }
}