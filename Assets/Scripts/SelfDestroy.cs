using System.Collections;
using CoolGameClub.Player;
using UnityEngine;

namespace CoolGameClub
{
    public class SelfDestroy : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start() {
            StartCoroutine(DeathDelay(FindAnyObjectByType<PlayerAttacks>().AttackLifeTime));
        }

        IEnumerator DeathDelay(float time) {
            yield return new WaitForSeconds(time);
            Destroy(gameObject);
        }
    }
}
