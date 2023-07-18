using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CoolGameClub.Movement
{
    public class PlayerInputManager : MonoBehaviour
    {
        [Header("Player move direction:")] 
        [SerializeField] private Vector2 _moving;
        public Vector2 IsMoving => _moving;

        private bool _pressingPause;

        public void OnMove(InputAction.CallbackContext value) => _moving = value.ReadValue<Vector2>();
        public void OnPause(InputAction.CallbackContext value) => _pressingPause = value.action.triggered;
    }
}
