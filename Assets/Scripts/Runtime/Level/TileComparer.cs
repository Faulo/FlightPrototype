using System.Collections.Generic;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public class TileComparer {
        Dictionary<TileBase, HashSet<TileBase>> synonymousTiles = new Dictionary<TileBase, HashSet<TileBase>>();

        public void AddSynonym(TileBase one, TileBase two) {
            if (!synonymousTiles.ContainsKey(one)) {
                synonymousTiles[one] = new HashSet<TileBase>();
            }
            if (!synonymousTiles.ContainsKey(two)) {
                synonymousTiles[two] = new HashSet<TileBase>();
            }
            if (!synonymousTiles[one].Contains(two)) {
                synonymousTiles[one].Add(two);
            }
            if (!synonymousTiles[two].Contains(one)) {
                synonymousTiles[two].Add(one);
            }
        }

        public bool IsSynonym(TileBase one, TileBase two) {
            return synonymousTiles.ContainsKey(one)
                ? synonymousTiles[one].Contains(two)
                : false;
        }
    }
}