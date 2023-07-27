using UnityEngine;
using UnityEngine.Tilemaps;

namespace CoolGameClub.Map
{
    [CreateAssetMenu(menuName = "2D/Tiles/Door")]
    public class DoorTile : Tile
    {
        [SerializeField] private DoorDirection doorDirection;

        [SerializeField] private Sprite open;
        [SerializeField] private Sprite closed;

        private bool isOpen;
        public bool IsOpen {
            get { return isOpen; }
            set {
                isOpen = value;
                gameObject.GetComponent<SpriteRenderer>().sprite = (isOpen) ? open : closed;
            }
        }
    }
}
