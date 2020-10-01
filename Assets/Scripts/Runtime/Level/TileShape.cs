using System.Collections.Generic;
using UnityEngine;

namespace TheCursedBroom.Level {
    public class TileShape {
        public readonly List<Vector3Int> positions = new List<Vector3Int>();
        public readonly List<Vector2> vertices = new List<Vector2>();

        public bool ContainsPosition(Vector3Int point) {
            bool inside = false;
            for (int i = 0, j = positions.Count - 1; i < positions.Count; j = i++) {
                var a = positions[i];
                var b = positions[j];
                // check borders
                if ((b.y == a.y) && (point.y == a.y) && (a.x <= point.x) && (point.x <= b.x)) {
                    return true;
                }
                if ((b.x == a.x) && (point.x == a.x) && (a.y <= point.y) && (point.y <= b.y)) {
                    return true;
                }
                // check inside
                if (((b.y < point.y) && (a.y >= point.y)) || ((a.y < point.y) && (b.y >= point.y))) {
                    if ((point.y - b.y) * (a.x - b.x) / (a.y - b.y) <= (point.x - b.x)) {
                        inside = !inside;
                    }
                }
            }
            return inside;
        }
        public bool ContainsVertex(Vector2 p) {
            int j = vertices.Count - 1;
            bool inside = false;
            for (int i = 0; i < vertices.Count; j = i++) {
                var pi = vertices[i];
                var pj = vertices[j];
                if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
                    (p.x < ((pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y)) + pi.x)) {
                    inside = !inside;
                }
            }
            return inside;
        }
    }
}