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
        [SerializeField] private RoomTemplate _horizontalHallwayTemplate;
        [Tooltip("Will be loaded between any vertically-connecting rooms")]
        [SerializeField] private RoomTemplate _verticalHallwayTemplate;
        [SerializeField] private List<RoomTemplate> _loadableRoomTemplates;


        [Header("Wall tile")]
        [SerializeField] private TileBase _wallTile;

        private List<TileInfo<DoorMarkerTile>> _pickableDoors = new();

        private TilemapController _tilemapController;

        private List<Room> _rooms;

        private Dictionary<Room, TileInfo<DoorMarkerTile>> _usedDoors = new();
        private Dictionary<TileInfo<DoorMarkerTile>, Room> _doorRoomDict = new();

        public void Start() {
            _tilemapController = new(_tilemapLayers);
            _rooms = new();

            InitRoomTemplates();

            LoadRoomTemplate(_spawnRoomTemplate, Vector3Int.zero, Vector3Int.zero);
            AddDoorsToPickable(_spawnRoomTemplate, Vector3Int.zero, Vector3Int.zero);

            for (int i = 0; i < _numberOfRooms; i++) AddRandomRoom();

            CreateDoors();
        }

        private void CreateDoors() {
            foreach (KeyValuePair<Vector3Int, (DoorDirection, Room)> pair in _usedDoors) {

                //pair.Value.Item2.AddDoor();
            }
        }

        private void InitRoomTemplates() {
            _spawnRoomTemplate.Init();
            _horizontalHallwayTemplate.Init();
            _verticalHallwayTemplate.Init();
            foreach (RoomTemplate roomTemplate in _loadableRoomTemplates) { roomTemplate.Init(); }
        }

        private void AddRandomRoom() {

            // Initialize the list of valid rooms to pick from
            // Rooms are removed from the list if they are deemed unvalid
            List<RoomTemplate> validRoomTemplates = new(_loadableRoomTemplates);

            // Only execute if there are rooms to pick from
            while (validRoomTemplates.Count > 0) {

                // Choose a random room from the list of valid rooms
                RoomTemplate randomRoomTemplate = validRoomTemplates[Random.Range(0, validRoomTemplates.Count)];

                // Remove the picked room from the list to prevent it being picked again
                validRoomTemplates.Remove(randomRoomTemplate);

                // Find all the valid door directions that can be picked from in the room
                // Example: If room only has a door going up -> only pick doors that are down in the level
                // A HashSet is a list that only has unique elements, so each direction will only be in the list once even if it is added multiple times
                HashSet<DoorDirection> validDoorDirections = new();
                foreach (TileInfo<DoorMarkerTile> doorMakerTile in randomRoomTemplate.GetDoorMarkerTiles()) {
                    validDoorDirections.Add(doorMakerTile.Tile.OppositeDirection);
                }

                // Find all the doors in the level that have a valid direction
                List<TileInfo<DoorMarkerTile>> validDoorMarkerTiles = new();
                foreach (TileInfo<DoorMarkerTile> door in _pickableDoors) {
                    if (validDoorDirections.Contains(door.Tile.Direction)) {
                        validDoorMarkerTiles.Add(door);
                    }
                }

                // Only execute if there are doors to pick from
                while (validDoorMarkerTiles.Count > 0) {

                    // Choose a random door from the list of valid doors
                    // The list of valid doors will only be doors that 
                    TileInfo<DoorMarkerTile> randomLevelDoorMarkerTile = validDoorMarkerTiles[Random.Range(0, validDoorMarkerTiles.Count)];

                    // Remove the picked door from the list to prevent it being picked again
                    validDoorMarkerTiles.Remove(randomLevelDoorMarkerTile);


                    // Find the correct connector for the random door
                    // The connector will be placed between the picked random door and the picked random room
                    RoomTemplate hallwayTemplate = null;
                    if (randomLevelDoorMarkerTile.Tile.Direction == DoorDirection.Left || randomLevelDoorMarkerTile.Tile.Direction == DoorDirection.Right) {
                        hallwayTemplate = _horizontalHallwayTemplate;
                    } else if (randomLevelDoorMarkerTile.Tile.Direction == DoorDirection.Up || randomLevelDoorMarkerTile.Tile.Direction == DoorDirection.Down) {
                        hallwayTemplate = _verticalHallwayTemplate;
                    }

                    // Find positions relating to the connector
                    Vector3Int connectorRoomOrigin = hallwayTemplate.GetDoorMarkerTiles().Find(x => x.Tile.Direction == randomLevelDoorMarkerTile.Tile.OppositeDirection).Pos;
                    Vector3Int connectorLevelPos = randomLevelDoorMarkerTile.Pos + randomLevelDoorMarkerTile.Tile.DirectionVector;

                    // Find positions relating to the random room
                    List<TileInfo<DoorMarkerTile>> validRandomRoomTemplateDoorMarkerTiles = randomRoomTemplate.GetDoorMarkerTiles().FindAll(x => x.Tile.Direction == randomLevelDoorMarkerTile.Tile.OppositeDirection).ToList();
                    TileInfo<DoorMarkerTile> randomRoomTemplateDoorMarkerTile = validRandomRoomTemplateDoorMarkerTiles[Random.Range(0, validRandomRoomTemplateDoorMarkerTiles.Count)];

                    Vector3Int randomRoomTemplateRoomOrigin = randomRoomTemplateDoorMarkerTile.Pos;
                    Vector3Int randomRoomTemplateLevelPos = hallwayTemplate.GetDoorMarkerTiles().Find(x => x.Tile.Direction == randomLevelDoorMarkerTile.Tile.Direction).Pos + connectorLevelPos - connectorRoomOrigin + randomLevelDoorMarkerTile.Tile.DirectionVector;

                    // Check if the connector and room can be loaded without overlapping other rooms
                    if (CanLoadRoomTemplate(hallwayTemplate, connectorRoomOrigin, connectorLevelPos) && CanLoadRoomTemplate(randomRoomTemplate, randomRoomTemplateRoomOrigin, randomRoomTemplateLevelPos)) {

                        // Removes the picked random door from the global list because a room is generated there
                        _pickableDoors.Remove(randomLevelDoorMarkerTile);

                        // Load the connector and random room to the level
                        LoadRoomTemplate(hallwayTemplate, connectorRoomOrigin, connectorLevelPos);
                        LoadRoomTemplate(randomRoomTemplate, randomRoomTemplateRoomOrigin, randomRoomTemplateLevelPos);

                        AddDoorsToPickable(randomRoomTemplate, randomRoomTemplateRoomOrigin, randomRoomTemplateLevelPos, randomRoomTemplateRoomOrigin);

                        // TODO SPAWN ROOM BARROOM ETC.
                        Room room = new Room();

                        // Return out of the method to prevent more rooms spawning
                        return;
                    }
                    // If there are no doors to pick from either loop back and try new room or if there are no rooms to try, drop metal pipe
                }
                // If the list of valid rooms is empty, there are no rooms that can be loaded => drop metal pipe
            }
            Debug.LogError("https://www.youtube.com/watch?v=f8mL0_4GeV0");
        }

        private void LoadRoomTemplate(RoomTemplate roomTemplate, Vector3Int roomOrigin, Vector3Int levelPos) {
            foreach (TileInfo<TileBase> tileInfo in roomTemplate.GetEnvironmentTiles()) {
                int layerIndex = (tileInfo.Tile == _wallTile) ? (int)LevelLayer.Walls : (int)LevelLayer.Floor;
                _tilemapController.SetTile(tileInfo.Pos + levelPos - roomOrigin, tileInfo.Tile, layerIndex);
            }
        }

        private bool CanLoadRoomTemplate(RoomTemplate roomTemplate, Vector3Int roomOrigin, Vector3Int levelPos) {
            foreach (TileInfo<TileBase> tileInfo in roomTemplate.GetEnvironmentTiles()) {
                Vector3Int pos = tileInfo.Pos + levelPos - roomOrigin;
                bool containsTileWalls = _tilemapController.ContainsTile(pos, (int)LevelLayer.Walls);
                bool containsTileFloor = _tilemapController.ContainsTile(pos, (int)LevelLayer.Floor);
                if (containsTileWalls || containsTileFloor) {
                    return false;
                }
            }
            return true;
        }

        private void AddDoorsToPickable(RoomTemplate roomTemplate, Vector3Int roomOrigin, Vector3Int levelPos, Vector3Int? exception = null) {
            foreach (TileInfo<DoorMarkerTile> doorMarkerTile in roomTemplate.GetDoorMarkerTiles()) {
                if (exception == null || doorMarkerTile.Pos != exception) {
                    _pickableDoors.Add(new TileInfo<DoorMarkerTile>(doorMarkerTile.Pos + levelPos - roomOrigin, doorMarkerTile.Tile));
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