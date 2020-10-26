using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace TheCursedBroom.Lighting {
    public class PolygonShadowCaster2D : ShadowCaster2D {
        public Vector3[] shapePath {
            get {
                if (m_shapePath == default) {
                    m_shapePath = m_ShapwPathInfo.GetValue(this) as Vector3[];
                }
                return m_shapePath;
            }
            set {
                if (m_shapePath != value) {
                    m_shapePath = value;
                    m_ShapwPathInfo.SetValue(this, value);
                }
            }
        }
        Vector3[] m_shapePath;
        static readonly FieldInfo m_ShapwPathInfo = typeof(ShadowCaster2D)
            .GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);

    }
}