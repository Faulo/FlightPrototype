using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level.TilemapFeatures {
    public class TilemapLoader : ComponentFeature<Tilemap> {
        [SerializeField, Expandable]
        TilemapController tilemap = default;

        protected override void OnValidate() {
            base.OnValidate();
            if (!tilemap) {
                tilemap = GetComponent<TilemapController>();
            }
        }

        void OnEnable() {
            tilemap.onRendererChange += TilemapChangeListener;
        }
        void OnDisable() {
            tilemap.onRendererChange -= TilemapChangeListener;
        }

        void TilemapChangeListener(TilemapChangeData data) {
            observedComponent.SetTiles(data.discardPositions.ToArray(), new TileBase[data.discardTiles.Count]);
            observedComponent.SetTiles(data.loadPositions.ToArray(), data.loadTiles.ToArray());
        }
    }
}