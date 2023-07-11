using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub
{
    public static class TilemapExtension
    {
        public static List<T> FindTiles<T>(this Tilemap tilemap) where T : Tile {
            List<T> tiles = new();

            tilemap.CompressBounds();
            for (int x = tilemap.cellBounds.min.x; x < tilemap.cellBounds.max.x; x++) {
                for (int y = tilemap.cellBounds.min.y; y < tilemap.cellBounds.max.y; y++) {
                    for (int z = tilemap.cellBounds.min.z; z < tilemap.cellBounds.max.z; z++) {
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, z));
                        if (tile != null && tile is T tTile) {
                            tiles.Add(tTile);
                        }
                    }
                }
            }

            return tiles;
        }

        public static List<Vector3Int> FindTilePositions<T>(this Tilemap tilemap) where T : Tile {
            List<Vector3Int> positions = new();

            tilemap.CompressBounds();
            for (int x = tilemap.cellBounds.min.x; x < tilemap.cellBounds.max.x; x++) {
                for (int y = tilemap.cellBounds.min.y; y < tilemap.cellBounds.max.y; y++) {
                    for (int z = tilemap.cellBounds.min.z; z < tilemap.cellBounds.max.z; z++) {
                        TileBase tile = tilemap.GetTile(new Vector3Int(x, y, z));
                        if (tile != null && tile is T) {
                            positions.Add(new Vector3Int(x, y, z));
                        }
                    }
                }
            }

            return positions;
        }

        public static Dictionary<Vector3Int, TileBase> GetTileDict(this Tilemap tilemap) {
            Dictionary<Vector3Int, TileBase> dict = new();
            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin) {
                TileBase tile = tilemap.GetTile(pos);
                if (tile != null) {
                    dict.Add(pos, tile);
                }
            }
            return dict;
        }
    }
}
