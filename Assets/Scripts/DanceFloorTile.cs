using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "2D/Tiles/Dance Floor")]
public class DanceFloorTile : Tile {
    [SerializeField] private Colors.Type type;
}