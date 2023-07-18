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

    public class TilemapController
    {
        private List<TilemapLayer> _layers;

        public TilemapController(List<TilemapLayer> layers) {
            _layers = layers;
            LoadTilemaps();
        }

        public void SetTile(Vector3Int pos, TileBase tile, int layerIndex = 0) {
            TilemapLayer layer = _layers[layerIndex];
            if (layer.TileDict.ContainsKey(pos)) {
                layer.TileDict[pos] = tile;
            } else {
                layer.TileDict.Add(pos, tile);
            }
            layer.Tilemap.SetTile(pos, tile);
        }

        public bool ContainsTile(Vector3Int pos, int layerIndex = 0) {
            return _layers[layerIndex].TileDict.ContainsKey(pos);
        }

        public TileBase GetTile(Vector3Int pos, int layerIndex = 0) {
            return _layers[layerIndex].TileDict[pos];
        }

        public List<TileInfo<TileBase>> GetTiles(int layerIndex = 0) {
            return GetTiles<TileBase>(layerIndex);
        }

        public List<TileInfo<gT>> GetTiles<gT>(int layerIndex = 0) where gT : TileBase {
            List<TileInfo<gT>> tiles = new();
            foreach (KeyValuePair<Vector3Int, TileBase> pair in _layers[layerIndex].TileDict) {
                if (pair.Value is gT gTile) {
                    tiles.Add(new TileInfo<gT>(pair.Key, gTile));
                }
            }
            return tiles;
        }

        public void ClearAllLayers() {
            for (int i = 0; i < _layers.Count; i++) {
                Clear(i);
            }
        }

        public void Clear(int layerIndex = 0) {
            TilemapLayer layer = _layers[layerIndex];
            layer.TileDict.Clear();
            layer.Tilemap.ClearAllTiles();
        }

        private void LoadTilemaps() {
            foreach (TilemapLayer layer in _layers) {
                layer.Tilemap.CompressBounds();
                foreach (Vector3Int pos in layer.Tilemap.cellBounds.allPositionsWithin) {
                    TileBase tile = layer.Tilemap.GetTile(pos);
                    if (tile != null && tile is TileBase tTile) {
                        layer.TileDict.Add(pos, tTile);
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class TilemapLayer
    {
        public Tilemap Tilemap;
        public Dictionary<Vector3Int, TileBase> TileDict = new();
    }

    public class TileInfo : TileInfo<TileBase>
    {
        public TileInfo(TileInfo<TileBase> tileInfo) : base(tileInfo) { }

        public TileInfo(Vector3Int pos, TileBase tile) : base(pos, tile) { }
    }

    public class TileInfo<T> where T : TileBase
    {
        private Vector3Int _pos;
        public Vector3Int Pos => _pos;

        private T _tile;
        public T Tile => _tile;

        public TileInfo(TileInfo<T> tileInfo) {
            _pos = tileInfo._pos;
            _tile = tileInfo._tile;
        }

        public TileInfo(Vector3Int pos, T tile) {
            _pos = pos;
            _tile = tile;
        }
    }
}
