using System;
using System.Collections.Generic;
using System.Linq;
using Slothsoft.UnityExtensions;
using TheCursedBroom.Extensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    [CreateAssetMenu()]
    public class TilemapLayerAsset : ScriptableObject {
        enum RenderMode {
            None,
            Chunk,
            Individual,
        }
        enum CollisionMode {
            None,
            Collision,
            Trigger,
        }
        const int TILE_MAX_COUNT = 100;

        public static TilemapLayerAsset[] all => Resources
            .LoadAll<TilemapLayerAsset>("TilemapLayers")
            .OrderBy(layer => layer.name)
            .ToArray();

        [Header("Tile settings")]
        [SerializeField, Expandable]
        Grid tilemapPalette = default;
        [SerializeField]
        Color tilemapColor = Color.white;
        [SerializeField, Layer]
        int tilemapLayer = default;
        public IEnumerable<TileBase> allowedTiles {
            get {
                var tilemap = tilemapPalette.GetComponentInChildren<Tilemap>();
                var tiles = new TileBase[TILE_MAX_COUNT];
                int count = tilemap.GetUsedTilesNonAlloc(tiles);
                return new HashSet<TileBase>(tiles.Take(count));
            }
        }

        [Header("Renderer settings")]
        [SerializeField]
        RenderMode renderMode = RenderMode.None;
        [SerializeField, SortingLayer]
        int renderSortingLayer = default;
        [SerializeField, Range(0, 100)]
        int renderOrderInLayer = 0;
        [SerializeField]
        Material renderMaterial = default;

        [Header("Collision settings")]
        [SerializeField]
        CollisionMode collisionMode = CollisionMode.None;
        [SerializeField]
        PhysicsMaterial2D collisionMaterial = default;

        public Tilemap InstallTilemap(GameObject obj) {
            if (obj.name != name) {
                obj.name = name;
            }
            if (obj.layer != tilemapLayer) {
                obj.layer = tilemapLayer;
            }
            if (obj.transform.position != Vector3.zero) {
                obj.transform.position = Vector3.zero;
            }

            // Tilemap
            var tilemap = obj.GetOrAddComponent<Tilemap>();
            tilemap.color = tilemapColor;

            // Renderer
            if (renderMode == RenderMode.None) {
                obj.DestroyComponent<TilemapRenderer>();
            } else {
                var renderer = obj.GetOrAddComponent<TilemapRenderer>();
                renderer.material = renderMaterial;
                renderer.sortingLayerID = renderSortingLayer;
                renderer.sortingOrder = renderOrderInLayer;
                switch (renderMode) {
                    case RenderMode.Chunk:
                        renderer.mode = TilemapRenderer.Mode.Chunk;
                        break;
                    case RenderMode.Individual:
                        renderer.mode = TilemapRenderer.Mode.Individual;
                        break;
                    default:
                        throw new NotImplementedException(collisionMode.ToString());
                }
            }

            // Collider
            if (collisionMode == CollisionMode.None) {
                obj.DestroyComponent<CompositeCollider2D>();
                obj.DestroyComponent<TilemapCollider2D>();
                obj.DestroyComponent<Rigidbody2D>();
            } else {
                var rigidbody = obj.GetOrAddComponent<Rigidbody2D>();
                rigidbody.bodyType = RigidbodyType2D.Static;
                rigidbody.sharedMaterial = collisionMaterial;

                var collider = obj.GetOrAddComponent<TilemapCollider2D>();
                collider.usedByComposite = true;
                collider.maximumTileChangeCount = uint.MaxValue;
                collider.extrusionFactor = 1f / 32;

                var composite = obj.GetOrAddComponent<CompositeCollider2D>();
                composite.generationType = CompositeCollider2D.GenerationType.Manual;
                composite.vertexDistance = 1f / 2;
                composite.offsetDistance = 1f / 32;
                composite.edgeRadius = 0;
                switch (collisionMode) {
                    case CollisionMode.Collision:
                        composite.isTrigger = false;
                        composite.geometryType = CompositeCollider2D.GeometryType.Outlines;
                        break;
                    case CollisionMode.Trigger:
                        composite.isTrigger = true;
                        composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
                        break;
                    default:
                        throw new NotImplementedException(collisionMode.ToString());
                }
            }

            return tilemap;
        }
    }
}