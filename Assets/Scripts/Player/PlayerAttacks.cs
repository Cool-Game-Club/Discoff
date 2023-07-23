using CoolGameClub.Core;
using CoolGameClub.Map;
using UnityEngine;

namespace CoolGameClub.Player
{
    public class PlayerAttacks : MonoBehaviour
    {
        [Header("Attacks Prefabs")]
        [SerializeField] private GameObject _ringAttack; // green
        [SerializeField] private float _ringLifeTime;

        [SerializeField] private GameObject _beamAttack; // red
        [SerializeField] private float _beamLifeTime;

        [SerializeField] private GameObject _lightningAttack; // blue
        [SerializeField] private float _lightningLifeTime;

        /*[SerializeField] private GameObject _Attack; // purple
        [SerializeField] private float _Lifetime;
        public float **LifeTime => _**LifeTime;*/

        [Header("Attacks stats")]
        [SerializeField] private float _attackDelay = 1.0f;

        // Initializing the "nextAttack" variable, which is a float to store the time spent between attacks
        private float _nextAttackTime;

        public float AttackLifeTime { get; set; }

        void Update() {
            Attack();
        }


        private void Attack() {

            Vector2 attackDirection = -PlayerInputManager.Instance.AttackDirection;

            if (attackDirection == Vector2.zero || Time.time < _nextAttackTime)
                return;

            _nextAttackTime = Time.time + _attackDelay;

            float angle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;

            switch (LevelManager.Instance.GetColorOfTile(this.transform.position - new Vector3(0, 3.5f, 0))) {
                case Colors.Color.Green:
                    Debug.Log("Attacking with green");
                    AttackLifeTime = _ringLifeTime;
                    GreenAttack(angle);
                    break;
                case Colors.Color.Red:
                    Debug.Log("Attacking with red");
                    AttackLifeTime = _beamLifeTime;
                    RedAttack(angle);
                    break;
                case Colors.Color.Blue:
                    Debug.Log("Attacking with blue");
                    AttackLifeTime = _lightningLifeTime;
                    //BlueAttack(angle);
                    break;
                case Colors.Color.Purple:
                    Debug.Log("Attacking with purple");
                    //AttackType = _**LifeTime;
                    //PurpleAttack(angle);
                    break;
                default:
                    Debug.Log("No colors, dashing");
                    //Dashing(angle);
                    break;
            }
        }

        private void GreenAttack(float angle) {
            GameObject attack = Instantiate(_ringAttack, transform.position, Quaternion.Euler(0f, 0f, angle));
        }

        private void RedAttack(float angle) {
            GameObject attack = Instantiate(_beamAttack, transform.position - new Vector3(0,1,0), Quaternion.Euler(0f, 0f, angle));
        }

        private void BlueAttack(Vector2 inputDirection) {
            // Make it so it targets the closest enemy.
        }

        /*private void PurpleAttack(Vector2 inputDirection) {

        }*/

        /*private void Dashing(Vector2 inputDirection) {
            // ZOOMIES
        }*/

    }
}
