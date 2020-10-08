using System.Collections.Generic;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level.TilemapColliderBakers {
    public class CompositeBaker : TilemapColliderBaker {
        [SerializeField, Expandable]
        TilemapCollider2D tilemapCollider = default;
        [SerializeField, Expandable]
        CompositeCollider2D compositeCollider = default;
        protected override void OnValidate() {
            base.OnValidate();
            if (!tilemapCollider) {
                tilemapCollider = GetComponent<TilemapCollider2D>();
            }
            if (!compositeCollider) {
                compositeCollider = GetComponent<CompositeCollider2D>();
            }
        }

        protected override void SetupCollider() {
            Assert.IsNotNull(tilemapCollider);
            Assert.IsNotNull(compositeCollider);
        }
        protected override void RegenerateCollider(ISet<Vector3Int> positions) {
            compositeCollider.GenerateGeometry();
        }
    }
}