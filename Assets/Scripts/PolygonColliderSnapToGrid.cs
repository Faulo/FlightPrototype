using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheCursedBroom {
    [ExecuteAlways()]
    public class PolygonColliderSnapToGrid : MonoBehaviour {
#if UNITY_EDITOR
        const int GRID_SIZE = 16;
        void Update() {
            foreach (var polygon in GetComponentsInChildren<PolygonCollider2D>()) {
                polygon.offset = SnapToGrid(polygon.offset);
                for (int i = 0; i < polygon.pathCount; i++) {
                    var path = polygon.GetPath(i);
                    for (int j = 0; j < path.Length; j++) {
                        path[j] = SnapToGrid(path[j]);
                    }
                    polygon.SetPath(i, CleanPath(path).ToList());
                }
            }
        }
        Vector2 SnapToGrid(Vector2 position) {
            position *= GRID_SIZE;
            position = new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            position /= GRID_SIZE;
            return position;
        }
        IEnumerable<Vector2> CleanPath(IEnumerable<Vector2> path) {
            var lastPosition = Vector2.zero;
            foreach (var position in path) {
                if (lastPosition != position) {
                    lastPosition = position;
                    yield return position;
                }
            }
        }
#endif
    }
}