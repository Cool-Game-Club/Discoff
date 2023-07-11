using UnityEngine;

namespace CoolGameClub
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        protected virtual void Awake() {
            if (Instance == null) {
                Instance = GetComponent<T>();
            } else {
                Debug.LogError("Multiple Singletons, ruh roh Raggy");
            }
        }
    }
}
