using TheCursedBroom.Extensions;
using UnityEngine;

namespace TheCursedBroom.Level.TilemapColliderBakers {
    [CreateAssetMenu(menuName = "Tilemap Collider Baker/Edge", fileName = "TCB_Edge_New")]
    public class EdgeBakerAsset : TilemapColliderBakerAsset {
        [Header("EdgeCollider2D")]
        [SerializeField]
        bool isTrigger = false;
        public override void SetupBaker(TilemapController tilemap) {
            var collider = tilemap.gameObject.GetOrAddComponent<EdgeCollider2D>();
            collider.isTrigger = isTrigger;
        }
    }
}