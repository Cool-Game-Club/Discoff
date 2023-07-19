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
        [Range(1, 100)]
        [SerializeField] private int _numberOfRooms;
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
        [SerializeField] private DanceFloorTile _redTile;
        [SerializeField] private DanceFloorTile _greenTile;
        [SerializeField] private DanceFloorTile _blueTile;
        [SerializeField] private DanceFloorTile _purpleTile;

        [Header("Extras")]
        [SerializeField] private Tile _barTile;

        private List<Room> _pickableRooms;

        private List<TileInfo<DoorMarkerTile>> _pickableDoors = new();

        private TilemapController _tilemapController;

        public void Start() {
            _tilemapController = new(_tilemapLayers);

            // Eww
            _spawnRoom.Init();
            _horizontalConnector.Init();
            _verticalConnector.Init();
            _doorCoverLeft.Init();
            _doorCoverRight.Init();
            _doorCoverUp.Init();
            _doorCoverDown.Init();

            _pickableRooms = Resources.LoadAll<Room>("Rooms").ToList();
            foreach (Room room in _pickableRooms) {
                room.Init();
            }

            LoadRoom(_spawnRoom, Vector3Int.zero, Vector3Int.zero);
            AddDoorsToUnused(_spawnRoom, Vector3Int.zero, Vector3Int.zero);

            for (int i = 0; i < _numberOfRooms; i++) {
                AddRandomRoom(i == _numberOfRooms - 1);
            }

            CoverUnusedDoors();
            ReplaceBlankDanceFloor();
        }

        private void AddRandomRoom(bool isBarRoom = false) {

            // Initialize the list of valid rooms to pick from
            // Rooms are removed from the list if they are deemed unvalid
            List<Room> validRooms = new(_pickableRooms);

            // Only execute if there are rooms to pick from
            while (validRooms.Count > 0) {

                // Choose a random room from the list of valid rooms
                Room randomRoom = validRooms[Random.Range(0, validRooms.Count)];

                // Remove the picked room from the list to prevent it being picked again
                validRooms.Remove(randomRoom);

                // Find all the valid door directions that can be picked from in the room
                // Example: If room only has a door going up -> only pick doors that are down in the level
                // A HashSet is a list that only has unique elements, so each direction will only be in the list once even if it is added multiple times
                HashSet<DoorDirection> validDoorDirections = new();
                foreach (TileInfo<DoorMarkerTile> door in randomRoom.GetDoorMarkerTiles()) {
                    validDoorDirections.Add(door.Tile.OppositeDirection);
                }

                // Find all the doors in the level that have a valid direction
                List<TileInfo<DoorMarkerTile>> validDoors = new();
                foreach (TileInfo<DoorMarkerTile> door in _pickableDoors) {
                    if (validDoorDirections.Contains(door.Tile.Direction)) {
                        validDoors.Add(door);
                    }
                }

                // Only execute if there are doors to pick from
                while (validDoors.Count > 0) {

                    // Choose a random door from the list of valid doors
                    // The list of valid doors will only be doors that 
                    TileInfo<DoorMarkerTile> randomDoor = validDoors[Random.Range(0, validDoors.Count)];

                    // Remove the picked door from the list to prevent it being picked again
                    validDoors.Remove(randomDoor);

                    // Find the correct connector for the random door
                    // The connector will be placed between the picked random door and the picked random room
                    Room connector = null;
                    if (randomDoor.Tile.Direction == DoorDirection.Left || randomDoor.Tile.Direction == DoorDirection.Right) {
                        connector = _horizontalConnector;
                    } else if (randomDoor.Tile.Direction == DoorDirection.Up || randomDoor.Tile.Direction == DoorDirection.Down) {
                        connector = _verticalConnector;
                    }

                    // Find positions relating to the connector
                    Vector3Int connectorRoomOrigin = connector.GetDoorMarkerTile(randomDoor.Tile.OppositeDirection).Pos;
                    Vector3Int connectorLevelPos = randomDoor.Pos + randomDoor.Tile.DirectionVector;

                    // Find positions relating to the random room
                    Vector3Int randomRoomRoomOrigin = randomRoom.GetDoorMarkerTile(randomDoor.Tile.OppositeDirection).Pos;
                    Vector3Int randomRoomLevelPos = connector.GetDoorMarkerTile(randomDoor.Tile.Direction).Pos + connectorLevelPos - connectorRoomOrigin + randomDoor.Tile.DirectionVector;

                    // Check if the connector and room can be loaded without overlapping other rooms
                    if (CanLoadRoom(connector, connectorRoomOrigin, connectorLevelPos) && CanLoadRoom(randomRoom, randomRoomRoomOrigin, randomRoomLevelPos)) {

                        // Removes the picked random door from the global list because a room is generated there
                        _pickableDoors.Remove(randomDoor);

                        // Load the connector and random room to the level
                        LoadRoom(connector, connectorRoomOrigin, connectorLevelPos);
                        LoadRoom(randomRoom, randomRoomRoomOrigin, randomRoomLevelPos);

                        // Add the doors from the random room to the global list of pickable doors, making sure to exclude the entrance
                        AddDoorsToUnused(randomRoom, randomRoomRoomOrigin, randomRoomLevelPos, randomDoor.Tile.OppositeDirection);

                        // Load the bar if it is a bar room
                        if (isBarRoom) LoadBar(randomRoom, randomRoomRoomOrigin, randomRoomLevelPos);

                        // Return out of the method to prevent more rooms spawning
                        return;
                    }

                    // If there are no doors to pick from either loop back and try new room or if there are no rooms to try, drop metal pipe
                }
                // If the list of valid rooms is empty, there are no rooms that can be loaded => drop metal pipe
            }
            Debug.LogError("https://www.youtube.com/watch?v=f8mL0_4GeV0");
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

        private void LoadBar(Room room, Vector3Int roomOrigin, Vector3Int levelPos) {
            TileInfo<MarkerTile> barMarkerTile = room.GetBarMarkerTile();
            _tilemapController.SetTile(barMarkerTile.Pos + levelPos - roomOrigin, _barTile, (int)LevelLayers.Extras);
        }

        private void AddDoorsToUnused(Room room, Vector3Int roomOrigin, Vector3Int levelPos, DoorDirection directionException = DoorDirection.None) {
            foreach (TileInfo<DoorMarkerTile> doorMarkerTile in room.GetDoorMarkerTiles()) {
                if (doorMarkerTile.Tile.Direction != directionException) {
                    _pickableDoors.Add(new TileInfo<DoorMarkerTile>(doorMarkerTile.Pos + levelPos - roomOrigin, doorMarkerTile.Tile));
                }
            }
        }

        private void CoverUnusedDoors() {
            foreach (TileInfo<DoorMarkerTile> tileInfo in _pickableDoors) {
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

        private void ReplaceBlankDanceFloor() {
            foreach (TileInfo<BlankDanceFloorTile> tileInfo in _tilemapController.GetTiles<BlankDanceFloorTile>()) {
                DanceFloorTile randomTile = Colors.RandomColor() switch {
                    Colors.Color.Red => _redTile,
                    Colors.Color.Green => _greenTile,
                    Colors.Color.Blue => _blueTile,
                    Colors.Color.Purple => _purpleTile,
                    _ => null,
                };
                _tilemapController.SetTile(tileInfo.Pos, randomTile);
            }
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