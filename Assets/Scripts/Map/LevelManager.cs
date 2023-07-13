using CoolGameClub.Core;
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

        /// <summary>
        /// Loads a room to the Level tilemap at the given position from the given room origin.
        /// </summary>
        /// <param name="room">The room to load.</param>
        /// <param name="roomOrigin">The room position to originate the loading from.</param>
        /// <param name="levelPos">The level position to load to.</param>
        private void LoadRoom(Room room, Vector3Int roomOrigin, Vector3Int levelPos) {
            LoadRoom(room, roomOrigin, levelPos, false);
        }

        /// <summary>
        /// Attempts to load a room to the Level tilemap at the given position from the given room origin.
        /// </summary>
        /// <param name="room">The room to load.</param>
        /// <param name="roomOrigin">The room position to originate the loading from.</param>
        /// <param name="levelPos">The level position to load to.</param>
        /// <param name="preventClipping">Whether clipping should be prevented.</param>
        /// <returns>Whether the room was succesfully loaded.</returns>
        private bool LoadRoom(Room room, Vector3Int roomOrigin, Vector3Int levelPos, bool preventClipping) {
            Dictionary<Vector3Int, TileBase> roomDict = room.GetRoomTiles();

            // If clipping should be prevented, check whether any of the positions are already occupied
            if (preventClipping) {
                foreach (KeyValuePair<Vector3Int, TileBase> pair in roomDict) {
                    if (_levelRoomTiles.ContainsKey(pair.Key + levelPos - roomOrigin)) {
                        return false;
                    }
                }
            }

            foreach (KeyValuePair<Vector3Int, TileBase> pair in roomDict) {
                SetTileInLevel(pair.Key + levelPos - roomOrigin, pair.Value);
            }

            return true;
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