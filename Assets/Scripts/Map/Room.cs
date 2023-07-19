using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub.Map
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private List<TilemapLayer> _tilemapLayers;
        private TilemapController _tilemapController;

        public void Init() {
            _tilemapController = new(_tilemapLayers);
        }

        public List<TileInfo<TileBase>> GetEnvironmentTiles() {
            return _tilemapController.GetTiles((int)LevelLayers.Environment);
        }

        public List<TileInfo<DoorMarkerTile>> GetDoorMarkerTiles() {
            return _tilemapController.GetTiles<DoorMarkerTile>((int)LevelLayers.Markers);
        }

        public TileInfo<DoorMarkerTile> GetDoorMarkerTile(DoorDirection doorDirection) {
            foreach (TileInfo<DoorMarkerTile> doorMarkerTile in _tilemapController.GetTiles<DoorMarkerTile>((int)LevelLayers.Markers)) {
                if (doorMarkerTile.Tile.Direction == doorDirection) return doorMarkerTile;
            }
            return null;
        }

        public TileInfo<MarkerTile> GetBarMarkerTile() {
            foreach (TileInfo<MarkerTile> markerTile in _tilemapController.GetTiles<MarkerTile>((int)LevelLayers.Markers)) {
                if (markerTile.Tile.Type == MarkerTile.MarkerType.Bar) return markerTile;
            }
            return null;
        }
    }
}
