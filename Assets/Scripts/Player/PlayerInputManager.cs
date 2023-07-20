using UnityEngine;
using UnityEngine.InputSystem;

namespace CoolGameClub.Player
{
    public class PlayerInputManager : MonoBehaviour
    {
        [Header("Player inputs directions:")]
        [SerializeField] private Vector2 _moveDirection;
        [SerializeField] private Vector2 _attackDirection;

        public Vector2 MoveDirection => _moveDirection;
        public Vector2 AttackDirection => _attackDirection;

        private bool _pressingPause;

        public void OnMove(InputAction.CallbackContext value) => _moveDirection = value.ReadValue<Vector2>();
        public void OnAttack(InputAction.CallbackContext value) => _attackDirection = value.ReadValue<Vector2>();
        public void OnPause(InputAction.CallbackContext value) => _pressingPause = value.action.triggered;
    }
}
