using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub.Assets.Scripts.Map
{
    [CreateAssetMenu(menuName = "2D/Tiles/POI")]
    public class POITile : Tile
    {
        [SerializeField] private POIType _type;
        public POIType Type => _type;

        public enum POIType {
            Bar,
            EnemySpawn,
            DoorLeft, 
            DoorRight,
            DoorDown,
            DoorUp,
        }
    }
}
