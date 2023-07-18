using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace CoolGameClub.Movement
{
    public class PlayerController : MonoBehaviour
    {

        [Header("Player Speed")]
        [Tooltip("Max move speed of the character in m/s")]
        [SerializeField] private float _moveSpeed = 100f;

        //private PlayerInput _playerInput;
        private PlayerInputManager _input;
        private Rigidbody2D _rigidbody;

        void Start() {
            _input = GetComponent<PlayerInputManager>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            Move();
        }

        public void Move()
        {
            float targetSpeed = _moveSpeed;

            // If no input, set target speed to 0.
            if (_input.IsMoving == Vector2.zero) targetSpeed = 0.0f;

            // Normalize the input direction.
            Vector3 moveInput = _input.IsMoving;
            moveInput = moveInput.normalized * targetSpeed * Time.deltaTime;

            _rigidbody.MovePosition(_rigidbody.transform.position + moveInput);
        }
    }
}
