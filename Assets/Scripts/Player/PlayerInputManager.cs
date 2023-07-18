using UnityEngine;
using UnityEngine.InputSystem;

namespace CoolGameClub.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        [Header("Player move direction:")]
        [SerializeField] private Vector2 _moveDirection;
        public Vector2 MoveDirection => _moveDirection;

        private bool _pressingPause;

        public void OnMove(InputAction.CallbackContext value) => _moveDirection = value.ReadValue<Vector2>();
        public void OnPause(InputAction.CallbackContext value) => _pressingPause = value.action.triggered;
    }
}
