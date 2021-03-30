using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace TheCursedBroom.Tests.EditMode {
    public class BoundsIntTest {
        [Test]
        public void TestAllPositionsWithin() {
            var observedCenter = new Vector3Int(3, 5, 0);
            var colliderRange = new Vector3Int(2, 4, 0);

            IEnumerable<Vector3Int> positionsWithinBounds() {
                var bounds = new BoundsInt(observedCenter - colliderRange, colliderRange + colliderRange + Vector3Int.one);
                foreach (var position in bounds.allPositionsWithin) {
                    yield return position;
                }
            }

            IEnumerable<Vector3Int> positionsWithinFor() {
                for (int y = observedCenter.y - colliderRange.y; y <= observedCenter.y + colliderRange.y; y++) {
                    for (int x = observedCenter.x - colliderRange.x; x <= observedCenter.x + colliderRange.x; x++) {
                        yield return new Vector3Int(x, y, 0);
                    }
                }
            }

            CollectionAssert.AreEquivalent(positionsWithinFor().ToList(), positionsWithinBounds().ToList());
            CollectionAssert.AreEqual(positionsWithinFor().ToList(), positionsWithinBounds().ToList());
        }
    }
}