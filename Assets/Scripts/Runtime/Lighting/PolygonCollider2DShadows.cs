using System.Linq;
using UnityEngine;

namespace TheCursedBroom.Lighting {
    [RequireComponent(typeof(PolygonCollider2D), typeof(PolygonShadowCaster2D))]
    [ExecuteInEditMode]
    public class PolygonCollider2DShadows : MonoBehaviour {
        [SerializeField, Range(0, 10)]
        readonly int pathIndex = 0;

        PolygonCollider2D polygonCollider;
        PolygonShadowCaster2D shadowCaster;

        void Awake() {
            polygonCollider = GetComponent<PolygonCollider2D>();
            shadowCaster = GetComponent<PolygonShadowCaster2D>();
        }

        void Update() {
            if (polygonCollider.offset == Vector2.zero) {
                shadowCaster.SetShapePath(polygonCollider.GetPath(pathIndex));
            } else {
                shadowCaster.SetShapePath(polygonCollider.GetPath(pathIndex).Select(pos => pos + polygonCollider.offset).ToList());
            }
        }
    }
}