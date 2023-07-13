using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub.Map
{
    [CreateAssetMenu(menuName = "2D/Tiles/Marker")]
    public class MarkerTile : Tile
    {
        [SerializeField] private MarkerType _type;
        public MarkerType Type => _type;

        public enum MarkerType
        {
            Bar,
            EnemySpawn,
            DoorLeft,
            DoorRight,
            DoorDown,
            DoorUp,
        }
    }
}
