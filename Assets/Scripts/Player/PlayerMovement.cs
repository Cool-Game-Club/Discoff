using UnityEngine;

namespace CoolGameClub.Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Player Speed")]
        [Tooltip("Max move speed of the character in m/s")]
        [SerializeField] private float _maxMoveSpeed = 7f;

        private PlayerInputManager _input;
        private Rigidbody2D _rigidbody;

        void Start() {
            _input = FindAnyObjectByType<PlayerInputManager>();
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

            // Apply the movement changes
            _rigidbody.MovePosition(_rigidbody.transform.position + velocity);
        }
    }
}
