using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub.Map
{
    public class RoomTemplate : MonoBehaviour
    {
        private enum RoomTemplateLayer { Environment, Markers }
        [SerializeField] private List<TilemapLayer> _tilemapLayers;

        private TilemapController _tilemapController;

        public void Init() {
            _tilemapController = new(_tilemapLayers);
        }

        //public List<TileInfo<TileBase>> EnvironmentTiles { get => _tilemapController.GetTiles<TileBase>((int)RoomTemplateLayer.Environment); }
        //public List<TileInfo<DoorMarkerTile>> DoorMarkerTiles { get => _tilemapController.GetTiles<DoorMarkerTile>((int)RoomTemplateLayer.Markers); }

        public List<TileInfo<TileBase>> GetEnvironmentTiles() {
            return _tilemapController.GetTiles<TileBase>((int)RoomTemplateLayer.Environment);
        }

        public List<TileInfo<DoorMarkerTile>> GetDoorMarkerTiles() {
            return _tilemapController.GetTiles<DoorMarkerTile>((int)RoomTemplateLayer.Markers);
        }

        public TileInfo<DoorMarkerTile> GetDoorMarkerTileAtPos(Vector3Int pos) {
            int layerIndex = (int)RoomTemplateLayer.Markers;
            if (!_tilemapController.ContainsTile(pos, layerIndex)) return null;

            TileInfo<DoorMarkerTile> doorMarkerTile = _tilemapController.GetTile<DoorMarkerTile>(pos, layerIndex);

            if (doorMarkerTile == null) return null;

            return doorMarkerTile;
        }
    }
}
