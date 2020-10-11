using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    [Serializable]
    public class TileGroup {
        [SerializeField]
        TileBase[] tiles = new TileBase[0];

        public IEnumerable<(TileBase, TileBase)> allTileCombinations {
            get {
                for (int i = 0; i < tiles.Length; i++) {
                    for (int j = i; j < tiles.Length; j++) {
                        yield return (tiles[i], tiles[j]);
                    }
                }
            }
        }
    }
}