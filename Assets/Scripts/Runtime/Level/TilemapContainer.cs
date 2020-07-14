using System;
using System.Collections.Generic;
using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TheCursedBroom.Level {
    [Serializable]
    public class TilemapContainer {
        [Header("Level size")]
        [SerializeField, Range(1, 1000)]
        public int width = 300;
        [SerializeField, Range(1, 1000)]
        public int height = 150;

        [Header("Tilemaps")]
        [SerializeField, Expandable]
        public Tilemap background = default;
        [SerializeField, Expandable]
        public Tilemap ground = default;
        [SerializeField, Expandable]
        public Tilemap objects = default;
        [SerializeField, Expandable]
        public Tilemap decorations = default;

        public IEnumerable<(TilemapType, Tilemap)> all {
            get {
                if (background) {
                    yield return (TilemapType.Background, background);
                }
                if (ground) {
                    yield return (TilemapType.Ground, ground);
                }
                if (objects) {
                    yield return (TilemapType.Objects, objects);
                }
                if (decorations) {
                    yield return (TilemapType.Decorations, decorations);
                }
            }
        }

        public Tilemap GetTilemapByType(TilemapType type) {
            switch (type) {
                case TilemapType.Background:
                    return background;
                case TilemapType.Ground:
                    return ground;
                case TilemapType.Objects:
                    return objects;
                case TilemapType.Decorations:
                    return decorations;
                default:
                    throw new NotImplementedException(type.ToString());
            }
        }

        public void OnValidate(Transform context) {
            if (!background) {
                background = context.GetComponentsInChildren<Tilemap>()[0];
            }
            if (!ground) {
                ground = context.GetComponentsInChildren<Tilemap>()[1];
            }
            if (!objects) {
                objects = context.GetComponentsInChildren<Tilemap>()[2];
            }
            if (!decorations) {
                decorations = context.GetComponentsInChildren<Tilemap>()[3];
            }
        }

        public Vector3Int WorldToCell(Vector3 position) => background.WorldToCell(position);
        public Vector3 CellToWorld(Vector3Int position) => background.CellToWorld(position);
        public Vector3 worldBottomLeft => CellToWorld(Vector3Int.zero);
        public IEnumerable<Vector3Int> tilePositions {
            get {
                for (int x = 0; x < width; x++) {
                    for (int y = 0; y < height; y++) {
                        yield return new Vector3Int(x, y, 0);
                    }
                }
            }
        }
    }
}