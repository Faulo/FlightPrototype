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

    }
}