using UnityEngine;

namespace TheCursedBroom.Player {
    public delegate Vector2 VelocityCalculator(Vector2 currentVelocity, bool isFacingRight, Vector2 input);
}