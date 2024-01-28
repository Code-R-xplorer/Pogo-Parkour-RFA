using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.Events;
using Utilities;

namespace Level
{
    public class Button : MonoBehaviour
    {
        // Serialized fields to customize the button's behavior in Unity editor
        [SerializeField] private bool delayPress; // Flag to enable or disable delay before the button press is registered
        [SerializeField] private float delayTime; // Time to delay the button press
        
        private Animator _animator; // Animator component reference for button animations
        private static readonly int On = Animator.StringToHash("On"); // Animator hash for the "On" state
        private static readonly int Off = Animator.StringToHash("Off"); // Animator hash for the "Off" state

        private bool _on; // Flag to track the button's state (on/off)

        public UnityEvent primaryAction; // Event triggered for the primary action of the button
        public UnityEvent buttonPressed; // Event triggered when the button is pressed
        public UnityEvent buttonReleased; // Event triggered when the button is released

        private void Start()
        {
            _animator = GetComponent<Animator>(); // Initialize the animator component
        }

        private void OnTriggerEnter(Collider other)
        {
            // Checks if the player enters the trigger and if the button is not already on
            if (!other.CompareTag(Tags.Player) || _on) return;
            Switch(); // Call the Switch method to change the button state
        }

        public void Switch()
        {
            _on = !_on; // Toggle the button's state
            AudioManager.Instance.Play("button"); // Play the button sound
            _animator.Play(_on ? On : Off, -1, 0f); // Play the appropriate animation based on the button's state

            // Perform actions based on the button's current state
            switch (_on)
            {
                case true:
                    primaryAction?.Invoke(); // Invoke the primary action if the button is turned on
                    if (delayPress) StartCoroutine(Delay()); // Start the delay coroutine if delay is enabled
                    else buttonPressed?.Invoke(); // Immediately invoke the button pressed event if no delay
                    break;
                case false:
                    buttonReleased?.Invoke(); // Invoke the button released event if the button is turned off
                    break;
            }
        }

        private IEnumerator Delay()
        {
            yield return new WaitForSeconds(delayTime); // Wait for the specified delay time
            buttonPressed?.Invoke(); // Invoke the button pressed event after the delay
        }
    }
}
