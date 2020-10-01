using NUnit.Framework;
using TheCursedBroom.Level;
using UnityEngine;

namespace Tests {
    public class TileShapeTests {
        TileShape shape;

        [OneTimeSetUp]
        public void SetupShape() {
            shape = new TileShape();
            shape.positions.AddRange(new[] { Vector3Int.zero, new Vector3Int(0, 2, 0), new Vector3Int(2, 2, 0), new Vector3Int(2, 0, 0) });
        }

        [Test]
        public void TestDoesContainCorners() {
            foreach (var position in shape.positions) {
                AssertInside(position);
            }
        }
        [Test]
        public void TestDoesContainBorders() {
            AssertInside(new Vector3Int(1, 0, 0));
            AssertInside(new Vector3Int(1, 2, 0));
            AssertInside(new Vector3Int(0, 1, 0));
            AssertInside(new Vector3Int(2, 1, 0));
        }
        [Test]
        public void TestDoesContainInside() {
            AssertInside(new Vector3Int(1, 1, 0));
        }
        [Test]
        public void TestDoesNotContainOutside() {
            AssertOutside(new Vector3Int(-1, 0, 0));
            AssertOutside(new Vector3Int(0, -1, 0));
            AssertOutside(new Vector3Int(1, 3, 0));
            AssertOutside(new Vector3Int(3, 1, 0));
            AssertOutside(new Vector3Int(3, 3, 0));
        }

        void AssertInside(Vector3Int position) {
            Assert.IsTrue(shape.ContainsPosition(position), $"{position} must be considered inside!");
        }

        void AssertOutside(Vector3Int position) {
            Assert.IsFalse(shape.ContainsPosition(position), $"{position} must be considered outside!");
        }
    }
}