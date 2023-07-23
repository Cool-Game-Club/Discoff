using System.Diagnostics;
using System.Numerics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace CoolGameClub.Player
{
    public class PlayerScript : MonoBehaviour
    {
        [Header("Player Speed")]
        [Tooltip("Max move speed of the character in m/s")]
        [SerializeField] private float _maxMoveSpeed = 7f;

        [Header("Player Attacks")] 
        [SerializeField] private float _attackSpeed = 1.0f;

        private PlayerInputManager _input;
        private Rigidbody2D _rigidbody;

        // Initializing the "nextAttack" variable, which is a float to store the time spent between attacks
        private float _nextAttack;

        void Start() {
            _input = FindAnyObjectByType<PlayerInputManager>();
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate() {
            Move();
            Attack(-_input.AttackDirection.x, -_input.AttackDirection.y);
        }

        private void Move() {
            // Convert input direction to speed and make it so if there is no input then there is no speed
            float targetSpeed = (_input.MoveDirection == Vector2.zero) ? 0f : _maxMoveSpeed;

            // Find the change in position
            Vector3 velocity = targetSpeed * Time.fixedDeltaTime * _input.MoveDirection.normalized;

            // Apply the movement changes
            _rigidbody.MovePosition(_rigidbody.transform.position + velocity);
        }

        private void Attack(float x, float y) {

            if ((x == 0 && y == 0) || Time.time < _nextAttack)
                return;

            _nextAttack = Time.time + _attackSpeed;

            Debug.Log("Attacking");
        }
    }
}
