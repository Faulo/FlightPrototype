using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace TheCursedBroom.Lighting {
    public class PolygonShadowCaster2D : ShadowCaster2D {
        public Vector3[] shapePath {
            get => m_shapePathInfo.GetValue(this) as Vector3[];
            private set => m_shapePathInfo.SetValue(this, value);
        }
        static readonly FieldInfo m_shapePathInfo = typeof(ShadowCaster2D)
            .GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);

        public int shapePathHash {
            get => (int)m_shapePathHashInfo.GetValue(this);
            private set => m_shapePathHashInfo.SetValue(this, value);
        }
        static readonly FieldInfo m_shapePathHashInfo = typeof(ShadowCaster2D)
            .GetField("m_ShapePathHash", BindingFlags.NonPublic | BindingFlags.Instance);

        int shapeHash;
        public void SetShapePath(IList<Vector2> path) {
            int newHash = GetShapePathHash(path);
            if (shapeHash != newHash) {
                shapeHash = newHash;
                shapePathHash = newHash;
                shapePath = path
                    .Select(position => (Vector3)position)
                    .ToArray();
            }
        }
        static int GetShapePathHash(IList<Vector2> path) {
            if (path == null) {
                return 0;
            }
            unchecked {
                int hashCode = (int)2166136261;
                for (int i = 0; i < path.Count; i++) {
                    hashCode = (hashCode * 16777619) ^ path[i].GetHashCode();
                }
                return hashCode;
            }
        }

    }
}