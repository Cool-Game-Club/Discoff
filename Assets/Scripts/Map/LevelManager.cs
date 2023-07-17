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
        [SerializeField] private List<TilemapLayer> _tilemapLayers;

        [Header("Room Prefabs")]
        [Tooltip("The first room on each level")]
        [SerializeField] private Room _spawnRoom;
        [Tooltip("Will be loaded between any horizontally-connecting rooms")]
        [SerializeField] private Room _horizontalConnector;
        [Tooltip("Will be loaded between any vertically-connecting rooms")]
        [SerializeField] private Room _verticalConnector;

        [Header("Door Covers")]
        [SerializeField] private Room _doorCoverLeft;
        [SerializeField] private Room _doorCoverRight;
        [SerializeField] private Room _doorCoverUp;
        [SerializeField] private Room _doorCoverDown;

        [Header("Dance Floor")]
        [SerializeField] private Tile _blankTile;
        [SerializeField] private DanceFloorTile _redTile;
        [SerializeField] private DanceFloorTile _greenTile;
        [SerializeField] private DanceFloorTile _blueTile;
        [SerializeField] private DanceFloorTile _purpleTile;

        [Header("Extras")]
        [SerializeField] private Tile _barTile;


        private List<Room> _choosableRooms;

        private List<TileInfo<DoorMarkerTile>> _unusedDoors = new();

        private TilemapController _tilemapController;


        public void Start() {
            _tilemapController = new(_tilemapLayers);

            _spawnRoom.Init();
            _horizontalConnector.Init();
            _verticalConnector.Init();
            _doorCoverLeft.Init();
            _doorCoverRight.Init();
            _doorCoverUp.Init();
            _doorCoverDown.Init();
            _choosableRooms = Resources.LoadAll<Room>("Rooms").ToList();
            foreach (Room room in _choosableRooms) {
                room.Init();
            }

            LoadRoom(_spawnRoom, Vector3Int.zero, Vector3Int.zero);
            AddDoorsToUnused(_spawnRoom, Vector3Int.zero, Vector3Int.zero);

            AddRandomRoom(_choosableRooms, _unusedDoors);
            AddRandomRoom(_choosableRooms, _unusedDoors);
            AddRandomRoom(_choosableRooms, _unusedDoors);
            AddRandomRoom(_choosableRooms, _unusedDoors);
            AddRandomRoom(_choosableRooms, _unusedDoors);
            AddRandomRoom(_choosableRooms, _unusedDoors);
            AddRandomRoom(_choosableRooms, _unusedDoors);
            AddRandomRoom(_choosableRooms, _unusedDoors);
            AddRandomRoom(_choosableRooms, _unusedDoors);
            AddRandomRoom(_choosableRooms, _unusedDoors);
            AddRandomRoom(_choosableRooms, _unusedDoors);

            CoverUnusedDoors();
        }

        private void AddRandomRoom(List<Room> possibleRooms, List<TileInfo<DoorMarkerTile>> possibleDoors) {


            if (possibleRooms.Count == 0 || possibleDoors.Count == 0) {
                Debug.LogError("Darn");
            }

            List<Room> possibleRoomCopy = new(possibleRooms);
            List<TileInfo<DoorMarkerTile>> possibleDoorsCopy = new(possibleDoors);

            // Choose a random room
            Room randomRoom = possibleRoomCopy[Random.Range(0, possibleRoomCopy.Count)];

            // Find possible doors
            HashSet<DoorDirection> validDoorDirections = new();
            foreach (TileInfo<DoorMarkerTile> door in randomRoom.GetDoorMarkerTiles()) {
                validDoorDirections.Add(door.Tile.OppositeDirection);
            }

            List<TileInfo<DoorMarkerTile>> validDoors = new();
            foreach (TileInfo<DoorMarkerTile> door in possibleDoorsCopy) {
                if (validDoorDirections.Contains(door.Tile.Direction)) {
                    validDoors.Add(door);
                }
            }

            // If there are no doors where the room can be placed, try again without that room
            if (validDoors.Count == 0) {
                possibleRoomCopy.Remove(randomRoom);
                AddRandomRoom(possibleRoomCopy, possibleDoorsCopy);
                return;
            }

            // Choose a random door
            TileInfo<DoorMarkerTile> randomDoor = validDoors[Random.Range(0, validDoors.Count)];

            // Find the correct connector for the random door
            Room connector = null;
            if (randomDoor.Tile.Direction == DoorDirection.Left || randomDoor.Tile.Direction == DoorDirection.Right) {
                connector = _horizontalConnector;
            } else if (randomDoor.Tile.Direction == DoorDirection.Up || randomDoor.Tile.Direction == DoorDirection.Down) {
                connector = _verticalConnector;
            }

            // Check if connector can be placed at door
            Vector3Int connectorRoomOrigin = connector.GetDoorMarkerTile(randomDoor.Tile.OppositeDirection).Pos;
            Vector3Int connectorLevelPos = randomDoor.Pos + randomDoor.Tile.DirectionVector;

            if (!CanLoadRoom(connector, connectorRoomOrigin, connectorLevelPos)) {
                possibleDoorsCopy.Remove(randomDoor);
                AddRandomRoom(possibleRoomCopy, possibleDoorsCopy);
                return;
            }

            // Check if room can be placed at door
            Vector3Int randomRoomRoomOrigin = randomRoom.GetDoorMarkerTile(randomDoor.Tile.OppositeDirection).Pos;
            Vector3Int randomRoomLevelPos = connector.GetDoorMarkerTile(randomDoor.Tile.Direction).Pos + connectorLevelPos - connectorRoomOrigin + randomDoor.Tile.DirectionVector;

            if (!CanLoadRoom(randomRoom, randomRoomRoomOrigin, randomRoomLevelPos)) {
                possibleDoorsCopy.Remove(randomDoor);
                AddRandomRoom(possibleRoomCopy, possibleDoorsCopy);
                return;
            }

            _unusedDoors.Remove(randomDoor);

            LoadRoom(connector, connectorRoomOrigin, connectorLevelPos);
            LoadRoom(randomRoom, randomRoomRoomOrigin, randomRoomLevelPos);
            AddDoorsToUnused(randomRoom, randomRoomRoomOrigin, randomRoomLevelPos, randomDoor.Tile.OppositeDirection);
        }

        private void AddDoorsToUnused(Room room, Vector3Int roomOrigin, Vector3Int levelPos, DoorDirection directionException = DoorDirection.None) {
            foreach (TileInfo<DoorMarkerTile> doorMarkerTile in room.GetDoorMarkerTiles()) {
                if (doorMarkerTile.Tile.Direction != directionException) {
                    TileInfo<DoorMarkerTile> door = new(doorMarkerTile);
                    door.SetPos(door.Pos + levelPos - roomOrigin);
                    _unusedDoors.Add(door);
                }
            }
        }

        private void CoverUnusedDoors() {
            foreach (TileInfo<DoorMarkerTile> tileInfo in _unusedDoors) {
                Room cover = tileInfo.Tile.Direction switch {
                    DoorDirection.Left => _doorCoverLeft,
                    DoorDirection.Right => _doorCoverRight,
                    DoorDirection.Up => _doorCoverUp,
                    DoorDirection.Down => _doorCoverDown,
                    _ => null
                };

                LoadRoom(cover, cover.GetDoorMarkerTile(tileInfo.Tile.Direction).Pos, tileInfo.Pos);
            }
        }

        /// <summary>
        /// Loads a room to the Level tilemap at the given position from the given room origin.
        /// </summary>
        /// <param name="room">The room to load.</param>
        /// <param name="roomOrigin">The room position to originate the loading from.</param>
        /// <param name="levelPos">The level position to load to.</param>
        private void LoadRoom(Room room, Vector3Int roomOrigin, Vector3Int levelPos) {
            foreach (TileInfo<TileBase> tileInfo in room.GetEnvironmentTiles()) {
                _tilemapController.SetTile(tileInfo.Pos + levelPos - roomOrigin, tileInfo.Tile, (int)LevelLayers.Environment);
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
            foreach (TileInfo<TileBase> tileInfo in room.GetEnvironmentTiles()) {
                if (_tilemapController.ContainsTile(tileInfo.Pos + levelPos - roomOrigin, (int)LevelLayers.Environment)) {
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

    public enum LevelLayers { Environment, Markers, Extras }
}