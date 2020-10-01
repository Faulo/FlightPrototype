using System;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace TheCursedBroom.Level {
    public class TilemapController : MonoBehaviour {
        public event Action<TilemapController> onRegenerateCollider;

        [SerializeField, Expandable]
        public TilemapLayerAsset type = default;

        public void RegenerateCollider() {
            onRegenerateCollider?.Invoke(this);
        }
    }
}