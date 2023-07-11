using CoolGameClub.Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub.Assets.Scripts.Map
{
    [CreateAssetMenu(menuName = "2D/Tiles/Dance Floor")]
    public class DanceFloorTile : Tile {
        [SerializeField] private Colors.Type type;
    }
}