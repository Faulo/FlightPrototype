using System;
using UnityEngine;

namespace TheCursedBroom.Level {
    [Serializable]
    public class TilemapBounds {
        [SerializeField, Range(1, 100)]
        int width = 10;
        [SerializeField, Range(1, 100)]
        int height = 10;

        public BoundsInt.PositionEnumerator allPositionsWithin => bounds.allPositionsWithin;
        public bool Contains(Vector3Int position) => bounds.Contains(position);
        public Vector3Int center {
            get => m_center;
            set {
                if (m_center != value) {
                    m_center = value;

                    extends.x = width;
                    extends.y = height;

                    bounds.position = value - extends;
                    bounds.size = (2 * extends) + Vector3Int.one;
                }
            }
        }
        Vector3Int m_center;
        public Vector3 worldCenter => bounds.center;
        public Vector3 worldSize => bounds.size;

        Vector3Int extends;
        BoundsInt bounds;
    }
}