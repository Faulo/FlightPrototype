using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public class TilemapCache {
        public TilemapController this[Component tilemap] {
            get {
                if (!tilemapOverComponent.ContainsKey(tilemap) || !tilemapOverComponent[tilemap]) {
                    tilemapOverComponent[tilemap] = tilemap.GetComponent<TilemapController>();
                }
                return tilemapOverComponent[tilemap];
            }
        }
        Dictionary<Component, TilemapController> tilemapOverComponent = new Dictionary<Component, TilemapController>();

        public TilemapController this[ITilemap tilemap] {
            get {
                if (!tilemapOverInterface.ContainsKey(tilemap) || !tilemapOverInterface[tilemap]) {
                    tilemapOverInterface[tilemap] = tilemap.GetComponent<TilemapController>();
                }
                return tilemapOverInterface[tilemap];
            }
        }
        Dictionary<ITilemap, TilemapController> tilemapOverInterface = new Dictionary<ITilemap, TilemapController>();
    }
}