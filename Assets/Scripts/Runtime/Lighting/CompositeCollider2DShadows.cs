using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace TheCursedBroom.Lighting {
    [RequireComponent(typeof(CompositeCollider2D), typeof(ShadowCaster2D))]
    public class CompositeCollider2DShadows : MonoBehaviour {
        CompositeCollider2D compositeCollider;
        ShadowCaster2D shadowCaster;

        Vector3[] m_ShapePath {
            get => m_ShapwPathInfo.GetValue(shadowCaster) as Vector3[];
            set => m_ShapwPathInfo.SetValue(shadowCaster, value);
        }
        FieldInfo m_ShapwPathInfo = typeof(ShadowCaster2D)
            .GetField(nameof(m_ShapePath), BindingFlags.NonPublic | BindingFlags.Instance);

        int pointCount = 0;
        Vector2[] path = new Vector2[1000];

        void Awake() {
            compositeCollider = GetComponent<CompositeCollider2D>();
            shadowCaster = GetComponent<ShadowCaster2D>();
        }

        void Update() {
            pointCount = compositeCollider.GetPath(0, path);
            m_ShapePath = path
                .Take(pointCount)
                .Select(position => (Vector3)(position + compositeCollider.offset))
                .ToArray();
        }
    }
}