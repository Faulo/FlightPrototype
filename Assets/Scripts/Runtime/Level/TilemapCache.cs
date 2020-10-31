using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    public class TilemapCache {
        public TilemapController this[Component tilemap] {
            get {
                if (tilemapOverComponent.TryGetValue(tilemap, out var controller) && controller) {
                    return controller;
                }
                controller = tilemap.GetComponent<TilemapController>();
                return tilemapOverComponent[tilemap] = controller;
            }
        }

        readonly Dictionary<Component, TilemapController> tilemapOverComponent = new Dictionary<Component, TilemapController>();

        public TilemapController this[ITilemap tilemap] {
            get {
                if (tilemapOverInterface.TryGetValue(tilemap, out var controller) && controller) {
                    return controller;
                }
                controller = tilemap.GetComponent<TilemapController>();
                return tilemapOverInterface[tilemap] = controller;
            }
        }

        readonly Dictionary<ITilemap, TilemapController> tilemapOverInterface = new Dictionary<ITilemap, TilemapController>();
    }
}