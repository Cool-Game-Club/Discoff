using CoolGameClub.Core;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub.Map
{
    [CreateAssetMenu(menuName = "2D/Tiles/Dance Floor")]
    public class DanceFloorTile : Tile
    {
        [SerializeField] private Colors.Color _color;
    }
}