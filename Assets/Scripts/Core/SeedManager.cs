using UnityEngine;

namespace CoolGameClub
{
    public class SeedManager : Singleton<SeedManager>
    {
        [SerializeField] private int _seed;

        protected override void Awake() {
            base.Awake();
            Random.InitState(_seed);
        }
    }
}
