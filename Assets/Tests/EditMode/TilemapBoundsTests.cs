using NUnit.Framework;
using TheCursedBroom.Level;
using UnityEngine;

namespace TheCursedBroom.Tests.EditMode {
    public class TilemapBoundsTests {
        TilemapBounds bounds;

        [SetUp]
        public void SetupShape() {
            bounds = new TilemapBounds();
        }

        [Test]
        public void TestPrepareCenter() {
            int loadCount = 0;
            int discardCount = 0;
            bounds.onLoadTiles += (position) => loadCount++;
            bounds.onDiscardTiles += (position) => discardCount++;

            bounds.PrepareTiles(Vector3Int.zero);

            Assert.AreEqual(bounds.tileCount, loadCount);
            Assert.AreEqual(0, discardCount);
        }
        [Test]
        public void TestUpdateSameCenter() {
            int loadCount = 0;
            int discardCount = 0;
            int changeCount = 0;
            bounds.onLoadTiles += (position) => loadCount++;
            bounds.onDiscardTiles += (position) => discardCount++;

            bounds.PrepareTiles(Vector3Int.zero);
            changeCount += bounds.UpdateTiles(Vector3Int.zero);
            changeCount += bounds.UpdateTiles(Vector3Int.zero);
            changeCount += bounds.UpdateTiles(Vector3Int.zero);

            Assert.AreEqual(0, changeCount);
            Assert.AreEqual(bounds.tileCount, loadCount);
            Assert.AreEqual(0, discardCount);
        }
        [Test]
        public void TestDisableCenter() {
            int loadCount = 0;
            int discardCount = 0;
            int changeCount = 0;
            bounds.onLoadTiles += (position) => loadCount++;
            bounds.onDiscardTiles += (position) => discardCount++;

            bounds.enabled = false;
            bounds.PrepareTiles(Vector3Int.zero);
            changeCount += bounds.UpdateTiles(Vector3Int.zero);
            changeCount += bounds.UpdateTiles(Vector3Int.zero);
            changeCount += bounds.UpdateTiles(Vector3Int.zero);

            Assert.AreEqual(0, changeCount);
            Assert.AreEqual(0, loadCount);
            Assert.AreEqual(0, discardCount);
        }
        [Test]
        public void TestDisableEnableCenter() {
            int loadCount = 0;
            int discardCount = 0;
            int changeCount = 0;
            bounds.onLoadTiles += (position) => loadCount++;
            bounds.onDiscardTiles += (position) => discardCount++;

            bounds.enabled = false;
            bounds.PrepareTiles(Vector3Int.zero);
            bounds.enabled = true;
            changeCount += bounds.UpdateTiles(Vector3Int.zero);
            bounds.enabled = false;
            changeCount += bounds.UpdateTiles(Vector3Int.zero);
            bounds.enabled = true;
            changeCount += bounds.UpdateTiles(Vector3Int.zero);

            Assert.AreEqual(3 * bounds.tileCount, changeCount);
            Assert.AreEqual(2 * bounds.tileCount, loadCount);
            Assert.AreEqual(1 * bounds.tileCount, discardCount);
        }
        [Test]
        public void TestMoveCenterFully() {
            int loadCount = 0;
            int discardCount = 0;
            int changeCount = 0;
            bounds.onLoadTiles += (position) => loadCount++;
            bounds.onDiscardTiles += (position) => discardCount++;

            bounds.PrepareTiles(Vector3Int.zero);
            changeCount += bounds.UpdateTiles(new Vector3Int(100, 0, 0));
            changeCount += bounds.UpdateTiles(new Vector3Int(200, 0, 0));
            changeCount += bounds.UpdateTiles(new Vector3Int(300, 0, 0));

            Assert.AreEqual(6 * bounds.tileCount, changeCount);
            Assert.AreEqual(4 * bounds.tileCount, loadCount);
            Assert.AreEqual(3 * bounds.tileCount, discardCount);
        }
        [Test]
        public void TestMoveCenterPartially() {
            int loadCount = 0;
            int discardCount = 0;
            int changeCount = 0;
            bounds.onLoadTiles += (position) => loadCount++;
            bounds.onDiscardTiles += (position) => discardCount++;

            var center = bounds.extends;
            bounds.PrepareTiles(center);
            center += bounds.extends;
            changeCount += bounds.UpdateTiles(center);

            Assert.AreEqual(6 * bounds.tileCount / 4, changeCount);
            Assert.AreEqual(7 * bounds.tileCount / 4, loadCount);
            Assert.AreEqual(3 * bounds.tileCount / 4, discardCount);
        }
    }
}