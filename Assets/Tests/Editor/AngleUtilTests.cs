using NUnit.Framework;
using TheCursedBroom;
using UnityEngine;

namespace Tests {
    public class AngleUtilTests {
        [Test]
        public void TestDirectionalAngle() {
            Assert.AreEqual(90, AngleUtil.DirectionalAngle(Vector2.up), Mathf.Epsilon);
            Assert.AreEqual(0, AngleUtil.DirectionalAngle(Vector2.right), Mathf.Epsilon);
            Assert.AreEqual(270, AngleUtil.DirectionalAngle(Vector2.down), Mathf.Epsilon);
            Assert.AreEqual(180, AngleUtil.DirectionalAngle(Vector2.left), Mathf.Epsilon);
        }

        [Test]
        public void TestHorizontalSign() {
            Assert.AreEqual(0, AngleUtil.HorizontalSign(Vector2.up));
            Assert.AreEqual(1, AngleUtil.HorizontalSign(Vector2.up + Vector2.right));
            Assert.AreEqual(1, AngleUtil.HorizontalSign(Vector2.right));
            Assert.AreEqual(1, AngleUtil.HorizontalSign(Vector2.down + Vector2.right));
            Assert.AreEqual(0, AngleUtil.HorizontalSign(Vector2.down));
            Assert.AreEqual(-1, AngleUtil.HorizontalSign(Vector2.down + Vector2.left));
            Assert.AreEqual(-1, AngleUtil.HorizontalSign(Vector2.left));
            Assert.AreEqual(-1, AngleUtil.HorizontalSign(Vector2.up + Vector2.left));
        }
    }
}
