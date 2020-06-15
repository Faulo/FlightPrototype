using System;
using UnityEngine;

namespace TheCursedBroom.Extensions {
    public static class Vector2Extensions {
        public static float As2DRotation(this Vector2 direction, int facing) {
            return Vector2.SignedAngle(Vector2.right, new Vector2(Math.Abs(direction.x), facing * direction.y));
        }
    }
}