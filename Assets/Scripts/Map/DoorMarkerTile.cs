using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub
{
    [CreateAssetMenu(menuName = "2D/Tiles/Markers/Door")]
    public class DoorMarkerTile : Tile
    {
        [SerializeField] private Direction _direction;
        public Direction Direction => _direction;

        public Direction OppositeDirection {
            get => _direction switch {
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                _ => Direction.Left
            };
        }
        public Vector3Int DirectionVector {
            get => _direction switch {
                Direction.Left => new Vector3Int(-1, 0, 0),
                Direction.Right => new Vector3Int(1, 0, 0),
                Direction.Up => new Vector3Int(0, 1, 0),
                Direction.Down => new Vector3Int(0, -1, 0),
                _ => Vector3Int.zero
            };
        }

    }

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down,
    }
}
