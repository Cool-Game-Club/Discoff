using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "2D/Tiles/POI")]
public class POITile : Tile
{
    [SerializeField] private POIType _type;
    public POIType Type { get { return _type; } }

    public enum POIType {
        Bar,
        EnemySpawn,
        DoorLeft, 
        DoorRight,
        DoorDown,
        DoorUp,
    }
}
