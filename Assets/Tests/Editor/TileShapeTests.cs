using NUnit.Framework;
using TheCursedBroom.Level;
using UnityEngine;

namespace Tests {
    public class TileShapeTests {
        Vector3Int bottomLeft = new Vector3Int(0, 0, 0);
        Vector3Int topLeft = new Vector3Int(0, 2, 0);
        Vector3Int topRight = new Vector3Int(2, 2, 0);
        Vector3Int bottomRight = new Vector3Int(2, 0, 0);
        Vector3Int center = new Vector3Int(1, 1, 0);

        TileShape square;
        TileShape cross;
        TileShape weird;

        [OneTimeSetUp]
        public void SetupShape() {
            square = new TileShape();
            square.positions.AddRange(new[] { bottomLeft, topLeft, topRight, bottomRight });

            cross = new TileShape();
            cross.positions.AddRange(new[] { bottomLeft, center, topLeft, center, topRight, center, bottomRight, center });

            weird = new TileShape();
            weird.positions.AddRange(new[] { new Vector3Int(-18, 34, 0), new Vector3Int(28, 34, 0), new Vector3Int(28, 26, 0), new Vector3Int(24, 25, 0), new Vector3Int(21, 24, 0), new Vector3Int(6, 23, 0), new Vector3Int(5, 22, 0), new Vector3Int(3, 21, 0), new Vector3Int(2, 20, 0), new Vector3Int(2, 19, 0), new Vector3Int(1, 18, 0), new Vector3Int(2, 14, 0), new Vector3Int(3, 13, 0), new Vector3Int(8, 14, 0), new Vector3Int(12, 15, 0), new Vector3Int(13, 15, 0), new Vector3Int(14, 12, 0), new Vector3Int(16, 13, 0), new Vector3Int(17, 14, 0), new Vector3Int(17, 16, 0), new Vector3Int(18, 16, 0), new Vector3Int(19, 14, 0), new Vector3Int(20, 12, 0), new Vector3Int(22, 13, 0), new Vector3Int(22, 19, 0), new Vector3Int(23, 19, 0), new Vector3Int(24, 17, 0), new Vector3Int(25, 15, 0), new Vector3Int(26, 15, 0), new Vector3Int(27, 14, 0), new Vector3Int(28, 13, 0), new Vector3Int(28, 2, 0), new Vector3Int(-18, 2, 0) });
        }

        [Test]
        public void TestSquareDoesContainCorners() {
            foreach (var position in square.positions) {
                AssertInside(square, position);
            }
        }
        [Test]
        public void TestSquareDoesContainBorders() {
            AssertInside(square, new Vector3Int(1, 0, 0));
            AssertInside(square, new Vector3Int(1, 2, 0));
            AssertInside(square, new Vector3Int(0, 1, 0));
            AssertInside(square, new Vector3Int(2, 1, 0));
        }
        [Test]
        public void TestSquareDoesContainInside() {
            AssertInside(square, center);
        }
        [Test]
        public void TestSquareDoesNotContainOutside() {
            AssertOutside(square, new Vector3Int(-1, 0, 0));
            AssertOutside(square, new Vector3Int(0, -1, 0));
            AssertOutside(square, new Vector3Int(1, 3, 0));
            AssertOutside(square, new Vector3Int(3, 1, 0));
            AssertOutside(square, new Vector3Int(3, 3, 0));
        }

        [Test]
        public void TestCrossDoesContainCorners() {
            foreach (var position in cross.positions) {
                AssertInside(cross, position);
            }
        }
        [Test]
        public void TestCrossDoesNotContainBorders() {
            AssertOutside(cross, new Vector3Int(1, 0, 0));
            AssertOutside(cross, new Vector3Int(1, 2, 0));
            AssertOutside(cross, new Vector3Int(0, 1, 0));
            AssertOutside(cross, new Vector3Int(2, 1, 0));
        }
        [Test]
        public void TestCrossDoesNotContainOutside() {
            AssertOutside(cross, new Vector3Int(-1, 0, 0));
            AssertOutside(cross, new Vector3Int(0, -1, 0));
            AssertOutside(cross, new Vector3Int(1, 3, 0));
            AssertOutside(cross, new Vector3Int(3, 1, 0));
            AssertOutside(cross, new Vector3Int(3, 3, 0));
        }
        [Test]
        public void TestWeirdDoesContainInside() {
            AssertInside(weird, new Vector3Int(13, 13, 0));
        }

        void AssertInside(TileShape shape, Vector3Int position) {
            Assert.IsTrue(shape.ContainsPosition(position), $"{position} must be considered inside!");
        }

        void AssertOutside(TileShape shape, Vector3Int position) {
            Assert.IsFalse(shape.ContainsPosition(position), $"{position} must be considered outside!");
        }
    }
}