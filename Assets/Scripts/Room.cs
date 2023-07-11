using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    [SerializeField] private Tilemap _RoomTilemap;
    [SerializeField] private Tilemap _POITilemap;
    [SerializeField] private Tilemap _ExtrasTilemap;

    public Tilemap RoomTileMap { get { return _RoomTilemap; } }
    public Tilemap POITilemap { get { return _POITilemap; } }
    public Tilemap ExtrasTilemap { get { return _ExtrasTilemap; } }

    [SerializeField] private List<Vector3Int> _doorCellPos = new();
    [SerializeField] private List<Vector3Int> _enemySpawnCellPos = new();
    [SerializeField] private Vector3Int _barCellPos;

    // HACK Dont assign the tile in every room, dumbass
    [SerializeField] private Tile _barTile;

    private void Start() {
        InitDanceFloor();
        FindPOITiles();
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

    private void FindPOITiles() {
        // Compress the bounds of the tilemap to prevent looping through unassigned tiles
        _POITilemap.CompressBounds();

        // Loop through every tile and populate POITiles fields
        for (int x = _POITilemap.cellBounds.min.x; x < _POITilemap.cellBounds.max.x; x++) {
            for (int y = _POITilemap.cellBounds.min.y; y < _POITilemap.cellBounds.max.y; y++) {
                for (int z = _POITilemap.cellBounds.min.z; z < _POITilemap.cellBounds.max.z; z++) {

                    Vector3Int cellPos = new Vector3Int(x, y, z);
                    TileBase tile = _POITilemap.GetTile(cellPos);

                    if (tile == null) continue;

                    if (tile is POITile POITile) {
                        switch (POITile.Type) {

                            // TODO Fix error if there are multiple Bar tiles 
                            case POITile.POIType.Bar:
                                _barCellPos = cellPos;
                                break;

                            case POITile.POIType.EnemySpawn:
                                _enemySpawnCellPos.Add(cellPos);
                                break;

                            // TODO Fix error if there are multiple Door tiles with the same direction
                            case POITile.POIType.DoorLeft:
                            case POITile.POIType.DoorRight:
                            case POITile.POIType.DoorUp:
                            case POITile.POIType.DoorDown:
                                _doorCellPos.Add(cellPos);
                                break;

                            default: break;

                        }
                    } else {
                        Debug.LogWarning("Non-POI tile found in POI Tilemap");
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
