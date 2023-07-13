using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub.Map
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private Tilemap _RoomTilemap;
        [SerializeField] private Tilemap _MarkerTilemap;
        [SerializeField] private Tilemap _ExtrasTilemap;

        public Tilemap RoomTileMap => _RoomTilemap;
        public Tilemap MarkerTilemap => _MarkerTilemap;
        public Tilemap ExtrasTilemap => _ExtrasTilemap;

        public Dictionary<Vector3Int, TileBase> GetRoomTiles() {
            return _RoomTilemap.GetTileDict();
        }
    }
}
