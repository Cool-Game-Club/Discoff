using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub
{
    public class TilemapController
    {
        private List<TilemapLayer> _layers;

        public TilemapController(List<TilemapLayer> layers) {
            _layers = layers;
            LoadTilemaps();
        }

        public Vector3Int WorldToCell(Vector3 worldPosition, int layerIndex = 0) => _layers[layerIndex].Tilemap.WorldToCell(worldPosition);

        public Vector3 CellToWorld(Vector3Int cellPosition, int layerIndex = 0) => _layers[layerIndex].Tilemap.CellToWorld(cellPosition);

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

        public TileInfo<TileBase> GetTile(Vector3Int pos, int layerIndex = 0) {
            return GetTile<TileBase>(pos, layerIndex);
        }

        public TileInfo<T> GetTile<T>(Vector3Int pos, int layerIndex = 0) where T : TileBase {
            if (_layers[layerIndex].TileDict[pos] is T tTile) {
                return new TileInfo<T>(pos, tTile);
            } else {
                return null;
            }
        }

        public List<TileInfo<TileBase>> GetTiles(int layerIndex = 0) {
            return GetTiles<TileBase>(layerIndex);
        }

        public List<TileInfo<T>> GetTiles<T>(int layerIndex = 0) where T : TileBase {
            List<TileInfo<T>> tiles = new();
            foreach (KeyValuePair<Vector3Int, TileBase> pair in _layers[layerIndex].TileDict) {
                if (pair.Value is T gTile) {
                    tiles.Add(new TileInfo<T>(pair.Key, gTile));
                }
            }
            return tiles;
        }

        public void ClearAllLayers() {
            for (int i = 0; i < _layers.Count; i++) {
                ClearLayer(i);
            }
        }

        public void ClearLayer(int layerIndex = 0) {
            TilemapLayer layer = _layers[layerIndex];
            layer.TileDict.Clear();
            layer.Tilemap.ClearAllTiles();
            layer.Tilemap.CompressBounds();
        }

        private void LoadTilemaps() {
            foreach (TilemapLayer layer in _layers) {
                layer.TileDict.Clear();
                layer.Tilemap.CompressBounds();
                foreach (Vector3Int pos in layer.Tilemap.cellBounds.allPositionsWithin) {
                    TileBase tile = layer.Tilemap.GetTile(pos);
                    if (tile != null) {
                        layer.TileDict.Add(pos, tile);
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
