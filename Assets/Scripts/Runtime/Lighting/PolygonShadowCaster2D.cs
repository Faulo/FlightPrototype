using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace TheCursedBroom.Lighting {
    public class PolygonShadowCaster2D : ShadowCaster2D {
        public Vector3[] shapePath {
            get {
                if (m_shapePath == default) {
                    m_shapePath = m_shapePathInfo.GetValue(this) as Vector3[];
                }
                return m_shapePath;
            }
            set {
                if (m_shapePath != value) {
                    m_shapePath = value;
                    m_shapePathInfo.SetValue(this, value);
                }
            }
        }
        Vector3[] m_shapePath;
        static readonly FieldInfo m_shapePathInfo = typeof(ShadowCaster2D)
            .GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);

        public int shapePathHash {
            get {
                if (m_shapePathHash == default) {
                    m_shapePathHash = (int)m_shapePathHashInfo.GetValue(this);
                }
                return m_shapePathHash;
            }
            set {
                if (m_shapePathHash != value) {
                    m_shapePathHash = value;
                    m_shapePathHashInfo.SetValue(this, value);
                }
            }
        }
        int m_shapePathHash;
        static readonly FieldInfo m_shapePathHashInfo = typeof(ShadowCaster2D)
            .GetField("m_ShapePathHash", BindingFlags.NonPublic | BindingFlags.Instance);

        int shapeHash;
        public void SetShapePath(List<Vector2> path) {
            int newHash = GetShapePathHash(path);
            if (shapeHash != newHash) {
                shapeHash = newHash;
                shapePath = path
                    .Select(position => (Vector3)position)
                    .ToArray();
                shapePathHash++;
            }
        }
        static int GetShapePathHash(List<Vector2> path) {
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