using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub
{
    [CreateAssetMenu(menuName = "2D/Tiles/Markers/Door")]
    public class DoorMarkerTile : Tile
    {
        [SerializeField] private DoorDirection _direction;
        public DoorDirection Direction => _direction;

        public DoorDirection OppositeDirection {
            get => _direction switch {
                DoorDirection.Left => DoorDirection.Right,
                DoorDirection.Right => DoorDirection.Left,
                DoorDirection.Up => DoorDirection.Down,
                DoorDirection.Down => DoorDirection.Up,
                _ => DoorDirection.Left
            };
        }
        public Vector3Int DirectionVector {
            get => _direction switch {
                DoorDirection.Left => new Vector3Int(-1, 0, 0),
                DoorDirection.Right => new Vector3Int(1, 0, 0),
                DoorDirection.Up => new Vector3Int(0, 1, 0),
                DoorDirection.Down => new Vector3Int(0, -1, 0),
                _ => Vector3Int.zero
            };
        }

    }

    public enum DoorDirection
    {
        Left,
        Right,
        Up,
        Down,
    }
}
