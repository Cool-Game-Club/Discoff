using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub.Map
{
    public class LevelManager : Singleton<LevelManager>
    {
        [SerializeField] private Tilemap _levelRoomTilemap;

        private Dictionary<Vector3Int, TileBase> _levelRoomTiles = new();

        private List<Room> _choosableRooms;

        [Header("Prefabs")]
        [SerializeField] private Room _spawnRoom;
        [SerializeField] private Room _HorizontalCorridor;
        [SerializeField] private Room _VerticalCorridor;

        public void Start() {
            _choosableRooms = Resources.LoadAll<Room>("Rooms").ToList();

            LoadRoom(_spawnRoom, Vector3Int.zero, Vector3Int.zero);
            LoadRoom(_spawnRoom, Vector3Int.zero, new Vector3Int(-9, 0, 0));
        }

        private void LoadRoom(Room room, Vector3Int roomOrigin, Vector3Int levelPos) {
            Vector3Int offset = levelPos - roomOrigin;

            foreach (KeyValuePair<Vector3Int, TileBase> pair in room.GetRoomTiles()) {
                SetTileInLevel(pair.Key + offset, pair.Value);
            }
        }

        private void SetTileInLevel(Vector3Int pos, TileBase tile) {
            _levelRoomTiles[pos] = tile;
            _levelRoomTilemap.SetTile(pos, tile);
        }

        private struct RoomNode
        {
            public Room Room;

            public Room RoomLeft;
            public Room RoomRight;
            public Room RoomUp;
            public Room RoomDown;
        }
    }


    enum Direction { Left, Right, Up, Down }
}