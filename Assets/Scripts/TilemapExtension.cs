using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub
{
    public static class TilemapExtension
    {
        public static Dictionary<Vector3Int, T> GetTileDict<T>(this Tilemap tilemap) {
            Dictionary<Vector3Int, T> dict = new();
            foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin) {
                TileBase tile = tilemap.GetTile(pos);
                if (tile != null && tile is T tTile) {
                    dict.Add(pos, tTile);
                }
            }
            return dict;
        }

        public static Dictionary<Vector3Int, TileBase> GetTileDict(this Tilemap tilemap) {
            return GetTileDict<TileBase>(tilemap);
        }
    }
}
