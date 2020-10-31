using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheCursedBroom {
    [ExecuteAlways()]
    public class PolygonColliderSnapToGrid : MonoBehaviour {
#if UNITY_EDITOR
        const int GRID_SIZE = 16;
        PolygonCollider2D[] polygons {
            get {
                if (m_polygons == null) {
                    m_polygons = GetComponentsInChildren<PolygonCollider2D>();
                }
                return m_polygons;
            }
        }
        PolygonCollider2D[] m_polygons;

        void Update() {
            if (!Application.isPlaying) {
                foreach (var polygon in polygons) {
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
        }
        Vector2 SnapToGrid(Vector2 position) {
            position *= GRID_SIZE;
            position = new Vector2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            position /= GRID_SIZE;
            return position;
        }
        IEnumerable<Vector2> CleanPath(IEnumerable<Vector2> path) {
            var lastPosition = path.LastOrDefault();
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