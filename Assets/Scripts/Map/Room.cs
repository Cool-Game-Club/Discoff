using System.Collections.Generic;
using CoolGameClub.Core;
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

        [SerializeField] private List<Vector3Int> _doorCellPos = new();
        [SerializeField] private List<Vector3Int> _enemySpawnCellPos = new();
        [SerializeField] private Vector3Int _barCellPos;

        // HACK Dont assign the tile in every room, dumbass
        [SerializeField] private Tile _barTile;

        private void Start() {
            InitDanceFloor();
            FindMarkerTiles();
            CreateBar();
        }

        public Dictionary<Vector3Int, TileBase> GetRoomTiles() {
            return _RoomTilemap.GetTileDict();
        }

        private void InitDanceFloor() {
            foreach (Vector3Int blankPos in _RoomTilemap.FindTilePositions<BlankDanceFloorTile>()) {
                _RoomTilemap.SetTile(blankPos, DanceFloorManager.Instance.GetTile(Colors.RandomColor()));
            }
        }

        private void FindMarkerTiles() {
            // Compress the bounds of the tilemap to prevent looping through unassigned tiles
            _MarkerTilemap.CompressBounds();

            // Loop through every tile and populate MarkerTiles fields
            for (int x = _MarkerTilemap.cellBounds.min.x; x < _MarkerTilemap.cellBounds.max.x; x++) {
                for (int y = _MarkerTilemap.cellBounds.min.y; y < _MarkerTilemap.cellBounds.max.y; y++) {
                    for (int z = _MarkerTilemap.cellBounds.min.z; z < _MarkerTilemap.cellBounds.max.z; z++) {

                        Vector3Int cellPos = new Vector3Int(x, y, z);
                        TileBase tile = _MarkerTilemap.GetTile(cellPos);

                        if (tile == null) continue;

                        if (tile is MarkerTile markerTile) {
                            switch (markerTile.Type) {

                                // TODO Fix error if there are multiple Bar tiles 
                                case MarkerTile.MarkerType.Bar:
                                    _barCellPos = cellPos;
                                    break;

                                case MarkerTile.MarkerType.EnemySpawn:
                                    _enemySpawnCellPos.Add(cellPos);
                                    break;

                                // TODO Fix error if there are multiple Door tiles with the same direction
                                case MarkerTile.MarkerType.DoorLeft:
                                case MarkerTile.MarkerType.DoorRight:
                                case MarkerTile.MarkerType.DoorUp:
                                case MarkerTile.MarkerType.DoorDown:
                                    _doorCellPos.Add(cellPos);
                                    break;

                                default: break;

                            }
                        } else {
                            Debug.LogWarning("Non-Marker tile found in Marker Tilemap");
                        }
                    }
                }
            }
        }

        public void SpawnEnemies() {

        }

        public void CreateBar() {
            _ExtrasTilemap.SetTile(_barCellPos, _barTile);
        }

        public void OnEnter() {

        }
    }
}
