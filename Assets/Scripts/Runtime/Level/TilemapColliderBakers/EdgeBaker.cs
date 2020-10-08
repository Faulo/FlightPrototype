using System;
using System.Collections.Generic;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace TheCursedBroom.Level.TilemapColliderBakers {
    public class EdgeBaker : TilemapColliderBaker {
        [SerializeField, Expandable]
        EdgeCollider2D edgeCollider = default;

        protected override void OnValidate() {
            base.OnValidate();
            if (!edgeCollider) {
                edgeCollider = GetComponent<EdgeCollider2D>();
            }
        }

        protected override void SetupCollider() {
            Assert.IsNotNull(edgeCollider);
        }
        protected override void RegenerateCollider(ISet<Vector3Int> positions) {
            throw new NotImplementedException();
        }
    }
}