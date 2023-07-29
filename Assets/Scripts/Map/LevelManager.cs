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
        private enum LevelLayer { Walls, Floor }
        [SerializeField] private List<TilemapLayer> _tilemapLayers;


        [Header("Room Prefabs")]
        [Tooltip("The first room on each level")]
        [SerializeField] private RoomTemplate _spawnRoomTemplate;
        [Tooltip("Will be loaded between any horizontally-connecting rooms")]
        [SerializeField] private RoomTemplate _horizontalConnectorTemplate;
        [Tooltip("Will be loaded between any vertically-connecting rooms")]
        [SerializeField] private RoomTemplate _verticalConnectorTemplate;
        [SerializeField] private List<RoomTemplate> _loadableRoomTemplates;

        [Header("Environment tiles")]
        [SerializeField] private TileBase _wallTile;
        [SerializeField] private TileBase _floorTile;

        [Header("Doors")]
        [SerializeField] private Door _horizontalDoorPrefab;
        [SerializeField] private Door _verticalDoorPrefab;


        private List<TileInfo<DoorMarkerTile>> _pickableDoors = new();

        private Dictionary<TileInfo<DoorMarkerTile>, Room> _doorsInRoom = new();
        private Dictionary<Vector3Int, (Room, Direction)> _usedDoors = new();

        private TilemapController _tilemapController;

        public void Start() {
            _tilemapController = new(_tilemapLayers);

            InitRoomTemplates();

            LoadRoomTemplate(_spawnRoomTemplate, Vector3Int.zero, Vector3Int.zero);
            AddDoorsToPickable(_spawnRoomTemplate, new Room(isSpawn: true), Vector3Int.zero, Vector3Int.zero);

            for (int i = 0; i < _numberOfRooms; i++) AddRandomRoom();

            CreateDoors();
        }

        private void InitRoomTemplates() {
            _spawnRoomTemplate.Init();
            _horizontalConnectorTemplate.Init();
            _verticalConnectorTemplate.Init();
            foreach (RoomTemplate roomTemplate in _loadableRoomTemplates) { roomTemplate.Init(); }
        }

        private void CreateDoors() {
            List<Vector3Int> verticalCovers = new() { Vector3Int.zero, Vector3Int.up, Vector3Int.down };
            List<Vector3Int> horizontalCovers = new() { Vector3Int.zero, Vector3Int.left, Vector3Int.right };

            foreach (var door in _usedDoors) {
                var doorPrefab = _horizontalDoorPrefab;
                var covers = horizontalCovers;
                if (door.Value.Item2 == Direction.Left || door.Value.Item2 == Direction.Right) {
                    covers = verticalCovers;
                    doorPrefab = _verticalDoorPrefab;
                }

                foreach (var pos in covers) {
                    _tilemapController.SetTile(door.Key + pos, null, (int)LevelLayer.Walls);
                    _tilemapController.SetTile(door.Key + pos, _floorTile, (int)LevelLayer.Floor);
                }

                Instantiate(doorPrefab, door.Key + new Vector3(0.5f, 0.5f), Quaternion.identity);
                door.Value.Item1.AddDoor(doorPrefab);
            }
        }

        private void AddRandomRoom() {

            // Initialize the list of valid rooms to pick from
            // Rooms are removed from the list if they are deemed unvalid
            List<RoomTemplate> validRoomTemplates = new(_loadableRoomTemplates);

            // Only execute if there are rooms to pick from
            while (validRoomTemplates.Count > 0) {

                // Choose a random room from the list of valid rooms
                RoomTemplate roomTemplate = validRoomTemplates[Random.Range(0, validRoomTemplates.Count)];
                validRoomTemplates.Remove(roomTemplate);

                // Find all the valid door directions that can be picked from in the room
                // Example: If room only has a door going up -> only pick doors that are down in the level
                // A HashSet is a list that only has unique elements, so each direction will only be in the list once even if it is added multiple times
                HashSet<Direction> validDoorDirections = new(roomTemplate.DoorMarkerTiles.Select(door => door.Tile.Direction).Distinct());

                // Find all the doors in the level that have a valid direction
                List<TileInfo<DoorMarkerTile>> validLevelDoorMarkers = new(_pickableDoors.FindAll(door => validDoorDirections.Contains(door.Tile.Direction)));

                // Only execute if there are doors in the level to pick from
                while (validLevelDoorMarkers.Count > 0) {

                    // Choose a random door from the list of valid doors
                    // The list of valid doors will only be doors that 
                    TileInfo<DoorMarkerTile> levelDoorMarker = validLevelDoorMarkers[Random.Range(0, validLevelDoorMarkers.Count)];
                    validLevelDoorMarkers.Remove(levelDoorMarker);

                    List<TileInfo<DoorMarkerTile>> validRoomTemplateDoorMarkers = new(roomTemplate.DoorMarkerTiles.FindAll(door => door.Tile.Direction == levelDoorMarker.Tile.OppositeDirection));

                    while (validRoomTemplateDoorMarkers.Count > 0) {

                        TileInfo<DoorMarkerTile> roomTemplateDoorMarker = validRoomTemplateDoorMarkers[Random.Range(0, validRoomTemplateDoorMarkers.Count)];
                        validRoomTemplateDoorMarkers.Remove(roomTemplateDoorMarker);

                        RoomTemplate connectorTemplate = null;
                        if (roomTemplateDoorMarker.Tile.Direction == Direction.Left || roomTemplateDoorMarker.Tile.Direction == Direction.Right) {
                            connectorTemplate = _horizontalConnectorTemplate;
                        } else if (roomTemplateDoorMarker.Tile.Direction == Direction.Up || roomTemplateDoorMarker.Tile.Direction == Direction.Down) {
                            connectorTemplate = _verticalConnectorTemplate;
                        }

                        // Find positions relating to the connector
                        Vector3Int connectorRoomOrigin = connectorTemplate.DoorMarkerTiles.Find(door => door.Tile.Direction == levelDoorMarker.Tile.OppositeDirection).Pos;
                        Vector3Int connectorLevelPos = levelDoorMarker.Pos + levelDoorMarker.Tile.DirectionVector;

                        // Find positions relating to the random room
                        Vector3Int roomTemplateRoomOrigin = roomTemplateDoorMarker.Pos;
                        Vector3Int roomTemplateLevelPos = connectorTemplate.DoorMarkerTiles.Find(door => door.Tile.Direction == levelDoorMarker.Tile.Direction).Pos + connectorLevelPos - connectorRoomOrigin + levelDoorMarker.Tile.DirectionVector;

                        // Check if the connector and room can be loaded without overlapping other rooms
                        if (CanLoadRoomTemplate(connectorTemplate, connectorRoomOrigin, connectorLevelPos) && CanLoadRoomTemplate(roomTemplate, roomTemplateRoomOrigin, roomTemplateLevelPos)) {

                            // TODO SPAWN ROOM BARROOM ETC.
                            Room room = new Room();

                            _usedDoors.Add(levelDoorMarker.Pos, (_doorsInRoom[levelDoorMarker], levelDoorMarker.Tile.Direction));
                            _usedDoors.Add(roomTemplateDoorMarker.Pos + roomTemplateLevelPos - roomTemplateRoomOrigin, (room, roomTemplateDoorMarker.Tile.Direction));

                            // Removes the picked random door from the global list because a room is generated there
                            _pickableDoors.Remove(levelDoorMarker);

                            // Load the connector and random room to the level
                            LoadRoomTemplate(connectorTemplate, connectorRoomOrigin, connectorLevelPos);
                            LoadRoomTemplate(roomTemplate, roomTemplateRoomOrigin, roomTemplateLevelPos);

                            AddDoorsToPickable(roomTemplate, room, roomTemplateRoomOrigin, roomTemplateLevelPos, roomTemplateRoomOrigin);


                            // Return out of the method to prevent more rooms spawning
                            return;
                        }
                    }
                }
            }
            Debug.LogError("https://www.youtube.com/watch?v=f8mL0_4GeV0");
        }

        private void LoadRoomTemplate(RoomTemplate roomTemplate, Vector3Int roomOrigin, Vector3Int levelPos) {
            foreach (TileInfo<TileBase> tileInfo in roomTemplate.EnvironmentTiles) {
                int layerIndex = (tileInfo.Tile == _wallTile) ? (int)LevelLayer.Walls : (int)LevelLayer.Floor;
                _tilemapController.SetTile(tileInfo.Pos + levelPos - roomOrigin, tileInfo.Tile, layerIndex);
            }
        }

        private bool CanLoadRoomTemplate(RoomTemplate roomTemplate, Vector3Int roomOrigin, Vector3Int levelPos) {
            foreach (TileInfo<TileBase> tileInfo in roomTemplate.EnvironmentTiles) {
                Vector3Int pos = tileInfo.Pos + levelPos - roomOrigin;
                bool containsTileWalls = _tilemapController.ContainsTile(pos, (int)LevelLayer.Walls);
                bool containsTileFloor = _tilemapController.ContainsTile(pos, (int)LevelLayer.Floor);
                if (containsTileWalls || containsTileFloor) {
                    return false;
                }
            }
            return true;
        }

        private void AddDoorsToPickable(RoomTemplate roomTemplate, Room room, Vector3Int roomOrigin, Vector3Int levelPos, Vector3Int? exception = null) {
            foreach (TileInfo<DoorMarkerTile> doorMarkerTile in roomTemplate.DoorMarkerTiles) {
                var door = new TileInfo<DoorMarkerTile>(doorMarkerTile.Pos + levelPos - roomOrigin, doorMarkerTile.Tile);
                _doorsInRoom.Add(door, room);
                if (exception == null || doorMarkerTile.Pos != exception) {
                    _pickableDoors.Add(door);
                }
            }
        }

        public Colors.Color? GetColorOfTile(Vector2 worldPos) {
            Vector3Int pos = _tilemapController.WorldToCell(new Vector3(worldPos.x, worldPos.y, 0));
            int layerIndex = (int)LevelLayer.Floor;

            if (!_tilemapController.ContainsTile(pos, layerIndex)) return null;

            TileInfo<DanceFloorTile> danceFloorTile = _tilemapController.GetTile<DanceFloorTile>(pos, layerIndex);
            if (danceFloorTile != null) return danceFloorTile.Tile.Color;

            return null;

        }
    }
}