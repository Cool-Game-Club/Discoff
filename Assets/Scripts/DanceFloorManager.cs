using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceFloorManager : Singleton<DanceFloorManager>
{
    [SerializeField] private DanceFloorTile _redTile;
    [SerializeField] private DanceFloorTile _greenTile;
    [SerializeField] private DanceFloorTile _blueTile;
    [SerializeField] private DanceFloorTile _purpleTile;

    public DanceFloorTile GetTile(Colors.Type type) {
        return type switch {
            Colors.Type.Red => _redTile,
            Colors.Type.Green => _greenTile,
            Colors.Type.Blue => _blueTile,
            Colors.Type.Purple => _purpleTile,
            _ => _redTile,
        };
    }
}
