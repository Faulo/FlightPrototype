using System.Linq;
using UnityEngine;

namespace TheCursedBroom.Lighting {
    [RequireComponent(typeof(PolygonCollider2D), typeof(PolygonShadowCaster2D))]
    [ExecuteInEditMode]
    public class PolygonCollider2DShadows : MonoBehaviour {
        [SerializeField, Range(0, 10)]
        int pathIndex = 0;

        PolygonCollider2D polygonCollider;
        PolygonShadowCaster2D shadowCaster;

        void Awake() {
            polygonCollider = GetComponent<PolygonCollider2D>();
            shadowCaster = GetComponent<PolygonShadowCaster2D>();
        }

        void Update() {
            shadowCaster.shapePath = polygonCollider.GetPath(pathIndex)
                .Select(position => (Vector3)(position + polygonCollider.offset))
                .ToArray();
        }
    }
}