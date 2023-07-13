using CoolGameClub.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub.Map
{
    public class LevelManager : Singleton<LevelManager> {
        [SerializeField] private Tilemap _levelRoomTilemap;

        private Dictionary<Vector3Int, TileBase> _levelRoomTiles = new();

        private List<Room> _choosableRooms;

        [Header("Room Prefabs")]
        
        [Tooltip("The first room on each level")]
        [SerializeField] private Room _spawnRoom;

        [Tooltip("Will be loaded between any horizontally-connecting rooms")]
        [SerializeField] private Room _HorizontalConnector;

        [Tooltip("Will be loaded between any vertically-connecting rooms")]
        [SerializeField] private Room _VerticalConnector;

        [Header("Dance Floor")]
        [SerializeField] private Tile _blankTile;
        [SerializeField] private DanceFloorTile _redTile;
        [SerializeField] private DanceFloorTile _greenTile;
        [SerializeField] private DanceFloorTile _blueTile;
        [SerializeField] private DanceFloorTile _purpleTile;

        [Header("Extras")]
        [SerializeField] private Tile _barTile;

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

        public DanceFloorTile GetDanceFloorTile(Colors.Type type) {
            return type switch {
                Colors.Type.Red => _redTile,
                Colors.Type.Green => _greenTile,
                Colors.Type.Blue => _blueTile,
                Colors.Type.Purple => _purpleTile,
                _ => _redTile,
            };
        }
    }
}