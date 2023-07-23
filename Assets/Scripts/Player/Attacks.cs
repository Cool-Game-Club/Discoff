using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoolGameClub.Player
{
    public class Attacks : MonoBehaviour
    {
        [Header("Attacks Prefabs")]
        [SerializeField] private GameObject _ringAttack; // green
        [SerializeField] private GameObject _beamAttack; // red
        [SerializeField] private GameObject _lightningAttack; // blue
        
        [SerializeField] private Vector3 _attackDirection;

        private void GreenAttack(float x, float y) {

            Vector3 normalizedAttack = new Vector3(x, y).normalized;
            float angle = Mathf.Atan2(normalizedAttack.y, normalizedAttack.x) * Mathf.Rad2Deg;
            
            GameObject attack = Instantiate(_ringAttack, transform.position, UnityEngine.Quaternion.Euler(0f, 0f, angle));
        }

        private void RedAttack(float x, float y) {

            Vector3 normalizedAttack = new Vector3(x, y).normalized;
            float angle = Mathf.Atan2(normalizedAttack.y, normalizedAttack.x) * Mathf.Rad2Deg;
            
            GameObject attack = Instantiate(_beamAttack, transform.position, UnityEngine.Quaternion.Euler(0f, 0f, angle));
        }

        private void BlueAttack(float x, float y) {
            // Make it so it targets the closest enemy.
        }
        /*private void PurpleAttack(float x, float y) {

        }*/

    }
}
