using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub.Map
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private Tilemap _roomTilemap;
        [SerializeField] private Tilemap _markerTilemap;
        [SerializeField] private Tilemap _extrasTilemap;

        public Dictionary<Vector3Int, TileBase> GetRoomTiles() {
            return _roomTilemap.GetTileDict();
        }

        public Vector3Int GetDoorPos(DoorMarkerTile.DoorDirection direction) {
            foreach (KeyValuePair<Vector3Int, DoorMarkerTile> pair in _markerTilemap.GetTileDict<DoorMarkerTile>()) {
                if (pair.Value.Direction == direction) {
                    return pair.Key;
                }
            }
            Debug.LogError("Door with corresponding direction not found.");
            return Vector3Int.zero;
        }

        public Dictionary<Vector3Int, DoorMarkerTile> GetDoorTiles() {
            return _markerTilemap.GetTileDict<DoorMarkerTile>();
        }

        /*public Dictionary<Vector3Int, MarkerTile> GetDoorTiles() {
            Dictionary<Vector3Int, MarkerTile> doors = new();
            foreach (KeyValuePair<Vector3Int, MarkerTile> pair in _markerTilemap.GetTileDict<MarkerTile>()) {
                switch (pair.Value.Type) {
                    case MarkerTile.MarkerType.DoorLeft:
                    case MarkerTile.MarkerType.DoorRight:
                    case MarkerTile.MarkerType.DoorUp:
                    case MarkerTile.MarkerType.DoorDown:
                        doors.Add(pair.Key, pair.Value);
                        break;
                    default: break;
                }
            }
            return doors;
        }*/
    }
}
