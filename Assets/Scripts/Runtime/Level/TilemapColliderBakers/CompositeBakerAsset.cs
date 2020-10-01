using TheCursedBroom.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level.TilemapColliderBakers {
    [CreateAssetMenu(menuName = "Tilemap Collider Baker/Composite", fileName = "TCB_Composite_New")]
    public class CompositeBakerAsset : TilemapColliderBakerAsset {
        [Header("TilemapCollider2D")]
        [SerializeField, Range(0, 10)]
        float extrusionFactor = 1f / 32;

        [Header("CompositeCollider2D")]
        [SerializeField]
        bool isTrigger = false;
        [SerializeField]
        CompositeCollider2D.GeometryType geometryType = CompositeCollider2D.GeometryType.Polygons;
        [SerializeField, Range(0, 10)]
        float vertexDistance = 1f / 2;
        [SerializeField, Range(0, 10)]
        float offsetDistance = 1f / 32;
        [SerializeField, Range(0, 10)]
        float edgeRadius = 0;

        public override void SetupBaker(TilemapController tilemap) {
            var collider = tilemap.gameObject.GetOrAddComponent<TilemapCollider2D>();
            collider.usedByComposite = true;
            collider.maximumTileChangeCount = uint.MaxValue;
            collider.extrusionFactor = extrusionFactor;

            var composite = tilemap.gameObject.GetOrAddComponent<CompositeCollider2D>();
            composite.generationType = CompositeCollider2D.GenerationType.Manual;
            composite.vertexDistance = vertexDistance;
            composite.offsetDistance = offsetDistance;
            composite.edgeRadius = edgeRadius;
            composite.isTrigger = isTrigger;
            composite.geometryType = geometryType;

            tilemap.onRegenerateCollider += controller => composite.GenerateGeometry();
        }
    }
}