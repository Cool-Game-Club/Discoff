using CoolGameClub.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub.Map
{
    public class LevelManager : Singleton<LevelManager>
    {
        [Header("Level")]
        [SerializeField] private Tilemap _levelRoomTilemap;
        [Range(1, 10)]
        [SerializeField] private int _amountOfRooms;

        [Header("Room Prefabs")]
        [Tooltip("The first room on each level")]
        [SerializeField] private Room _spawnRoom;
        [Tooltip("Will be loaded between any horizontally-connecting rooms")]
        [SerializeField] private Room _horizontalConnector;
        [Tooltip("Will be loaded between any vertically-connecting rooms")]
        [SerializeField] private Room _verticalConnector;

        [Header("Dance Floor")]
        [SerializeField] private Tile _blankTile;
        [SerializeField] private DanceFloorTile _redTile;
        [SerializeField] private DanceFloorTile _greenTile;
        [SerializeField] private DanceFloorTile _blueTile;
        [SerializeField] private DanceFloorTile _purpleTile;

        [Header("Extras")]
        [SerializeField] private Tile _barTile;


        private Dictionary<Vector3Int, TileBase> _levelRoomTiles = new();

        private List<Room> _choosableRooms;

        private Dictionary<Vector3Int, DoorMarkerTile> _unselectedDoors = new();

        private int _addedRooms = 0;


        public void Start() {
            _choosableRooms = Resources.LoadAll<Room>("Rooms").ToList();

            LoadRoom(_spawnRoom, Vector3Int.zero, Vector3Int.zero);
            foreach (KeyValuePair<Vector3Int, DoorMarkerTile> pair in _spawnRoom.GetDoorTiles()) {
                _unselectedDoors.Add(pair.Key, pair.Value);
            }

            AddRandomRoomAtRandomDoor();
        }

        private void AddRandomRoomAtRandomDoor() {

            List<Room> choosableRoomsCopy = new(_choosableRooms);

            do {
                Room room = choosableRoomsCopy[Random.Range(0, choosableRoomsCopy.Count)];
                choosableRoomsCopy.Remove(room);

                Vector3Int roomOrigin = Vector3Int.zero;
                Vector3Int levelPos = Vector3Int.zero;

                Dictionary<Vector3Int, DoorMarkerTile> unselectedDoorsCopy = new(_unselectedDoors);

                KeyValuePair<Vector3Int, DoorMarkerTile> randomDoor = new();

                do {
                    if (unselectedDoorsCopy.Count == 0) {
                        break;
                    }

                    randomDoor = _unselectedDoors.ElementAt(Random.Range(0, _unselectedDoors.Count));
                    unselectedDoorsCopy.Remove(randomDoor.Key);

                    roomOrigin = room.GetDoorPos(randomDoor.Value.OppositeDirection);
                    levelPos = randomDoor.Key + randomDoor.Value.DirectionVector;

                } while (!CanLoadRoom(room, roomOrigin, levelPos));

                foreach (KeyValuePair<Vector3Int, DoorMarkerTile> pair in room.GetDoorTiles()) {
                    if (pair.Value.Direction != randomDoor.Value.OppositeDirection) {
                        _unselectedDoors.Add(pair.Key, pair.Value);
                    }
                }

                LoadRoom(room, roomOrigin, levelPos);
                return;

            } while (choosableRoomsCopy.Count > 0);

            Debug.LogError("No more rooms to place.");
            return;
        }

        /*private void AddRandomRoomAtRandomDoor() {
            KeyValuePair<Vector3Int, DoorMarkerTile> randomDoor = _unselectedDoors.ElementAt(Random.Range(0, _unselectedDoors.Count - 1));
            _unselectedDoors.Remove(randomDoor.Key);

            // Find the correct connecting room
            Room connector = null;
            if (randomDoor.Value.Direction == (DoorMarkerTile.DoorDirection.Left | DoorMarkerTile.DoorDirection.Right)) {
                connector = _horizontalConnector;
            } else if (randomDoor.Value.Direction == (DoorMarkerTile.DoorDirection.Up | DoorMarkerTile.DoorDirection.Down)) {
                connector = _verticalConnector;
            }

            // Choose a random room to add
            Room randomRoom = _choosableRooms[Random.Range(0, _choosableRooms.Length - 1)];

            // Check if corridor can be loaded
            Vector3Int corridorRoomOrigin = connector.GetDoorPos(randomDoor.Value.OppositeDirection);
            Vector3Int corridorLevelPos = randomDoor.Key + randomDoor.Value.DirectionVector;
            bool canLoadCorridor = CanLoadRoom(connector, corridorRoomOrigin, corridorLevelPos);

            // Check if room can be loaded
            Vector3Int roomRoomOrigin = connector.GetDoorPos(randomDoor.Value.Direction);
            Vector3Int roomLevelPos = randomDoor.Key + randomDoor.Value.DirectionVector;
            bool canLoadRoom = CanLoadRoom(randomRoom, roomRoomOrigin, roomLevelPos);

            if (canLoadCorridor && canLoadRoom) {

            } else {

            }
        }*/

        /// <summary>
        /// Loads a room to the Level tilemap at the given position from the given room origin.
        /// </summary>
        /// <param name="room">The room to load.</param>
        /// <param name="roomOrigin">The room position to originate the loading from.</param>
        /// <param name="levelPos">The level position to load to.</param>
        private void LoadRoom(Room room, Vector3Int roomOrigin, Vector3Int levelPos) {
            foreach (KeyValuePair<Vector3Int, TileBase> pair in room.GetRoomTiles()) {
                Vector3Int pos = pair.Key + levelPos - roomOrigin;
                _levelRoomTiles[pos] = pair.Value;
                _levelRoomTilemap.SetTile(pos, pair.Value);
            }
        }

        /// <summary>
        /// Checks whether a room can be loaded at the given position without overlapping other rooms.
        /// </summary>
        /// <param name="room">The room to test.</param>
        /// <param name="roomOrigin">The room position to originate the loading from.</param>
        /// <param name="levelPos">The level position to load to.</param>
        /// <returns>Whether the room can be loaded the the position.</returns>
        private bool CanLoadRoom(Room room, Vector3Int roomOrigin, Vector3Int levelPos) {
            foreach (KeyValuePair<Vector3Int, TileBase> pair in room.GetRoomTiles()) {
                if (_levelRoomTiles.ContainsKey(pair.Key + levelPos - roomOrigin)) {
                    return false;
                }
            }
            return true;
        }

        private DanceFloorTile GetDanceFloorTile(Colors.Color type) {
            return type switch {
                Colors.Color.Red => _redTile,
                Colors.Color.Green => _greenTile,
                Colors.Color.Blue => _blueTile,
                Colors.Color.Purple => _purpleTile,
                _ => _redTile,
            };
        }
    }
}