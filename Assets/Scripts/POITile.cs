using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class POITile : Tile
{
    [SerializeField] private POIType _type;
    public POIType Type { get { return _type; } private set { _type = value; } }

    public enum POIType {
        Bar,
        EnemySpawn,
        DoorLeft, 
        DoorRight,
        DoorDown,
        DoorUp,
    }
}
