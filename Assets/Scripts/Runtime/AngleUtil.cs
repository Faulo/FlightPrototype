using UnityEngine;
using UnityEngine.Assertions;

namespace TheCursedBroom {
    public static class AngleUtil {
        public static float NormalizeAngle(float angle) {
            angle = Quaternion.Euler(0, 0, angle).eulerAngles.z;
            while (angle > 360) {
                angle -= 360;
            }
            while (angle < 0) {
                angle += 360;
            }
            return angle;
        }
        public static float RoundAngle(float angle, int numberOfDirections) {
            return Mathf.RoundToInt(angle * numberOfDirections / 360) * 360 / numberOfDirections;
        }
        public static float DirectionalAngle(Vector2 direction) => DirectionalRotation(direction).eulerAngles.z;
        public static Quaternion DirectionalRotation(Vector2 direction) {
            return Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, direction));
        }
        public static int HorizontalSign(Vector2 direction) => HorizontalSign(DirectionalAngle(direction));
        public static int HorizontalSign(float angle) {
            if (Mathf.Approximately(angle, 90) || Mathf.Approximately(angle, 270)) {
                return 0;
            }
            if (angle < 90 || angle > 270) {
                return 1;
            }
            return -1;
        }
        public static float HorizontalAngle(int sign) {
            Assert.AreNotEqual(0, sign);
            return sign > 0
                ? 0
                : 180;
        }

        public static float Alignment(Quaternion firstRotation, Quaternion secondRotation) {
            var alignmentRotation = firstRotation * Quaternion.Inverse(secondRotation);
            return Mathf.Cos(alignmentRotation.eulerAngles.z * Mathf.Deg2Rad);
        }
    }
}