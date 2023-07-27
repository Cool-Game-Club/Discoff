using UnityEngine;

namespace CoolGameClub.Map
{
    public class Door : MonoBehaviour
    {
        private Animator _animator;

        private bool _isOpen = false;

        private void Start() {
            _animator = GetComponent<Animator>();
        }

        public void Open(bool shouldOpen) {
            _isOpen = shouldOpen;
            _animator.SetBool("isOpen", _isOpen);
        }
    }
}
