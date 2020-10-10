using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace TheCursedBroom.Lighting {
    [RequireComponent(typeof(PolygonCollider2D), typeof(ShadowCaster2D))]
    [ExecuteInEditMode]
    public class PolygonCollider2DShadows : MonoBehaviour {
        [SerializeField, Range(0, 10)]
        int pathIndex = 0;

        PolygonCollider2D polygonCollider;
        ShadowCaster2D shadowCaster;

        Vector3[] m_ShapePath {
            get => m_ShapwPathInfo.GetValue(shadowCaster) as Vector3[];
            set => m_ShapwPathInfo.SetValue(shadowCaster, value);
        }
        FieldInfo m_ShapwPathInfo = typeof(ShadowCaster2D)
            .GetField(nameof(m_ShapePath), BindingFlags.NonPublic | BindingFlags.Instance);

        void Awake() {
            polygonCollider = GetComponent<PolygonCollider2D>();
            shadowCaster = GetComponent<ShadowCaster2D>();
        }

        void Update() {
            m_ShapePath = polygonCollider.GetPath(pathIndex)
                .Select(position => (Vector3)(position + polygonCollider.offset))
                .ToArray();
        }
    }
}