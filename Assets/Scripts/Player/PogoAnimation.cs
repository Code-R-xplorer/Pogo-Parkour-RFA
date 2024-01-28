using UnityEngine;

namespace Player
{
    public class PogoAnimation : MonoBehaviour
    {
        private Animator _animator; // Animator component to control animations
        
        // Static readonly integers to cache the hash values for animator parameters
        private static readonly int PullIn = Animator.StringToHash("Pull In");
        private static readonly int PushOut = Animator.StringToHash("Push Out");

        private void Start()
        {
            // Initialize the animator component by getting the Animator of the first child
            _animator = transform.GetChild(1).GetComponent<Animator>();
        }

        // Method to play either the PushOut or PullIn animation based on the release parameter
        public void PlayAnimation(bool release)
        {
            // Play the appropriate animation based on the release parameter
            _animator.Play(release ? PushOut : PullIn, -1, 0f);
        }
    }
}