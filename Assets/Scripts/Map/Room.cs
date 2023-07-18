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

        public Tilemap RoomTileMap => _roomTilemap;
        public Tilemap MarkerTilemap => _markerTilemap;
        public Tilemap ExtrasTilemap => _extrasTilemap;

        public Dictionary<Vector3Int, TileBase> GetRoomTiles() {
            return _roomTilemap.GetTileDict();
        }
    }
}
