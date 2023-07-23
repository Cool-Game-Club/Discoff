using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub.Map
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private List<TilemapLayer> _tilemapLayers;
        private TilemapController _tilemapController;

        private enum RoomLayer { WallsAndFloor, Markers }

        public void Init() {
            _tilemapController = new(_tilemapLayers);
        }

        public List<TileInfo<TileBase>> GetWallsAndFloorTiles() {
            return _tilemapController.GetTiles((int)RoomLayer.WallsAndFloor);
        }

        public List<TileInfo<DoorMarkerTile>> GetDoorMarkerTiles() {
            return _tilemapController.GetTiles<DoorMarkerTile>((int)RoomLayer.Markers);
        }

        public TileInfo<DoorMarkerTile> GetDoorMarkerTile(DoorDirection doorDirection) {
            foreach (TileInfo<DoorMarkerTile> doorMarkerTile in _tilemapController.GetTiles<DoorMarkerTile>((int)RoomLayer.Markers)) {
                if (doorMarkerTile.Tile.Direction == doorDirection) return doorMarkerTile;
            }
            return null;
        }

        public TileInfo<MarkerTile> GetBarMarkerTile() {
            foreach (TileInfo<MarkerTile> markerTile in _tilemapController.GetTiles<MarkerTile>((int)RoomLayer.Markers)) {
                if (markerTile.Tile.Type == MarkerTile.MarkerType.Bar) return markerTile;
            }
            return null;
        }
    }
}
