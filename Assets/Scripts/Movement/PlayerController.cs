using UnityEngine;

namespace CoolGameClub.Movement
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Speed")]
        [Tooltip("Max move speed of the character in m/s")]
        [SerializeField] private float _maxMoveSpeed = 100f;

        //private PlayerInput _playerInput;
        private PlayerInputManager _input;
        private Rigidbody2D _rigidbody;

        void Start() {
            _input = GetComponent<PlayerInputManager>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate() {
            Move();
        }

        public void Move() {
            float targetSpeed = (_input.MoveDirection == Vector2.zero) ? 0f : _maxMoveSpeed;

            // Find the change in position
            Vector3 moveChange = targetSpeed * Time.fixedDeltaTime * _input.MoveDirection.normalized;

            _rigidbody.MovePosition(_rigidbody.transform.position + moveChange);
        }
    }
}
