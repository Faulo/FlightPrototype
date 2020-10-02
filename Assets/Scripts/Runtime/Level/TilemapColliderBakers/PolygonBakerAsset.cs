using TheCursedBroom.Extensions;
using UnityEngine;

namespace TheCursedBroom.Level.TilemapColliderBakers {
    [CreateAssetMenu(menuName = "Tilemap Collider Baker/Polygon", fileName = "TCB_Polygon_New")]
    public class PolygonBakerAsset : TilemapColliderBakerAsset {
        [Header("PolygonCollider2D")]
        [SerializeField]
        bool isTrigger = false;
        public override void SetupBaker(TilemapController tilemap) {
            var collider = tilemap.gameObject.GetOrAddComponent<PolygonCollider2D>();
            collider.usedByComposite = false;
            collider.isTrigger = isTrigger;

            tilemap.onRegenerateCollider += controller => {
                var shapes = LevelController.instance.GetTileShapes(controller.type);
                collider.pathCount = shapes.Count;
                for (int i = 0; i < shapes.Count; i++) {
                    collider.SetPath(i, shapes[i].vertices);
                }
            };
        }
    }
}