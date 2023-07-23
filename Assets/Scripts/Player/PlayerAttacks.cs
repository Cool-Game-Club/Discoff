using UnityEngine;

namespace CoolGameClub.Player
{
    public class PlayerAttacks : MonoBehaviour
    {
        [Header("Attacks Prefabs")]
        [SerializeField] private GameObject _ringAttack; // green
        [SerializeField] private GameObject _beamAttack; // red
        [SerializeField] private GameObject _lightningAttack; // blue

        [Header("Attacks stats")]
        [SerializeField] private float _attackSpeed = 1.0f;


        // Initializing the "nextAttack" variable, which is a float to store the time spent between attacks
        private float _nextAttackTime;

        void Update() {
            Attack();
        }


        private void Attack() {

            Vector2 attackDirection = PlayerInputManager.Instance.AttackDirection;

            if (attackDirection == Vector2.zero || Time.time < _nextAttackTime)
                return;

            _nextAttackTime = Time.time + _attackSpeed;

            Debug.Log("Attacking");

            float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

        }

        private void GreenAttack(float angle) {
            GameObject attack = Instantiate(_ringAttack, transform.position, Quaternion.Euler(0f, 0f, angle));
        }

        private void RedAttack(float angle) {
            GameObject attack = Instantiate(_beamAttack, transform.position, Quaternion.Euler(0f, 0f, angle));
        }

        private void BlueAttack(Vector2 inputDirection) {
            // Make it so it targets the closest enemy.
        }

        /*private void PurpleAttack(Vector2 inputDirection) {

        }*/

    }
}
