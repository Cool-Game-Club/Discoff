using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub.Map
{
    public class LevelManager : Singleton<LevelManager>
    {
        [SerializeField] private Tilemap _levelTilemap;

        private List<Room> _roomSelection;

        [Header("Prefabs")]
        [SerializeField] private Room _spawnRoom;
        [SerializeField] private Room _HorizontalCorridor;
        [SerializeField] private Room _VerticalCorridor;

        public void Start() {
            _roomSelection = Resources.LoadAll<Room>("Rooms").ToList();

            LoadRoom(_spawnRoom, Vector3Int.zero, Direction.Up);
        }

        private void LoadRoom(Room room, Vector3Int origin, Direction dir) {
            Dictionary<Vector3Int, TileBase> roomTiles = room.GetRoomTiles();
            //Dictionary<Vector3Int, TileBase> POITiles = room.GetPOITiles();
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