using UnityEngine;

namespace CoolGameClub.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Speed")]
        [Tooltip("Max move speed of the character in m/s")]
        [SerializeField] private float _maxMoveSpeed = 7f;

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

        private void Move() {
            // Convert input direction to speed and make it so if there is no input then there is no speed
            float targetSpeed = (_input.MoveDirection == Vector2.zero) ? 0f : _maxMoveSpeed;

            // Find the change in position
            Vector3 velocity = targetSpeed * Time.fixedDeltaTime * _input.MoveDirection.normalized;

            _rigidbody.MovePosition(_rigidbody.transform.position + velocity);
        }
    }
}
