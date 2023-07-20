using System.Diagnostics;
using System.Numerics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace CoolGameClub.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Speed")]
        [Tooltip("Max move speed of the character in m/s")]
        [SerializeField] private float _maxMoveSpeed = 7f;

        [Header("Attacks Prefabs")] 
        [SerializeField] private GameObject _testObject;
        [SerializeField] private GameObject _ringAttack;
        [SerializeField] private GameObject _beamAttack, _lightningAttack;

        [SerializeField] private Vector3 _attackDirection;

        //private PlayerInput _playerInput;
        private PlayerInputManager _input;
        private Rigidbody2D _rigidbody;
        private Rigidbody2D _testRigidbody;

        void Start() {
            _input = FindAnyObjectByType<PlayerInputManager>();
            _rigidbody = GetComponent<Rigidbody2D>();
            _testRigidbody = _testObject.GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate() {
            Move();
            Attacks();
        }

        private void Move() {
            // Convert input direction to speed and make it so if there is no input then there is no speed
            float targetSpeed = (_input.MoveDirection == Vector2.zero) ? 0f : _maxMoveSpeed;

            // Find the change in position
            Vector3 velocity = targetSpeed * Time.fixedDeltaTime * _input.MoveDirection.normalized;

            _rigidbody.MovePosition(_rigidbody.transform.position + velocity);
        }

        private void Attacks() {
            // Trying method 1: didn't work. I'm gonna do a different approach anyway as it'll be more balanced between controller and keyboard. The one i tried here made it so joystick would be way better at aiming.
            //_attackDirection = _input.AttackDirection.y + _input.AttackDirection.x;
            //_testObject.transform.Rotate(0,0,_attackDirection);

            // Didn't work either.
            //Vector3 _attackDirection = _input.AttackDirection.normalized;
            //_testRigidbody.MoveRotation(_testRigidbody.transform.rotation + _attackDirection);

            // Much more simple. I think i'm going to rename the current method (Attacks) to "Attack(direction float)" so I can generalize all the attacks to the same method and just use a different sprite depending on floor color. Will see later. (2 hours later: Idk how im going to do but i want to figure it out without googling)
            // I did this but it turns the player too, it looks pretty funny lmao
            _attackDirection = _input.AttackDirection;

            switch (_attackDirection.y) {
                case 1:
                    // Up
                    _testRigidbody.SetRotation(-90);
                    break;
                case -1:
                    // Down
                    _testRigidbody.SetRotation(90);
                    break;
            }
            switch (_attackDirection.x) {
                case 1:
                    // Right
                    _testRigidbody.SetRotation(180);
                    break;
                case -1:
                    // Left
                    _testRigidbody.SetRotation(0);
                    break;
            }
        }
    }
}
