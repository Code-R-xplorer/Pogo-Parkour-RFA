using System.Collections;
using Managers;
using Player;
using UnityEngine;
using Utilities;

namespace Level
{
    public class Bridge : MonoBehaviour
    {
        // Serialized fields for customizing the bridge movement and behavior in the Unity editor
        [SerializeField] private float movementLength; // Length of the bridge's movement
        [Range(0.001f, 1f), SerializeField] private float speed = 0.03f; // Speed of the bridge's movement, with a specified range
        [SerializeField] private float waitingTime = 1f; // Time the bridge waits before moving again
        [SerializeField] private bool moveOnStart; // Determines if the bridge starts moving when the level starts

        // Private variables for controlling the movement and state of the bridge
        private Vector3 _nextPosition; // Target position for the next movement
        private float _journeyLength; // Total length of the current movement
        private float _startTime; // Time when the current movement started
        private bool _waiting; // Flag to indicate if the bridge is currently waiting
        private bool _waitingEnabled; // Flag to enable or disable waiting
        private bool _retract; // Flag to indicate the direction of movement (extend or retract)

        private bool _canMove; // Flag to control if the bridge can move

        private PogoController _player; // Reference to the player controller

        private void Start()
        {
            _waitingEnabled = waitingTime != 0; // Initialize waiting behavior based on the waitingTime setting
            _player = GameManager.Instance.Player; // Get a reference to the player from the GameManager
            if (!moveOnStart) return; // Check if the bridge should move on start and exit early if not
            StartMoving(); // Start moving the bridge
        }

        public void StartMoving()
        {
            _canMove = true; // Set the flag to allow bridge movement
            GoToNextPos(); // Initialize the next position for the bridge to move towards
        }

        private void GoToNextPos()
        {
            // Set the next position based on the retract flag and movement length
            _nextPosition = new Vector3(0, 0, _retract ? movementLength : 0);
            // Calculate the distance between the current position and the next position
            _journeyLength = Vector3.Distance(_nextPosition, transform.localPosition);
            // Record the start time of the movement
            _startTime = Time.time;
        }
        
        void FixedUpdate()
        {
            if (!_canMove) return; // Exit early if the bridge is not allowed to move

            // Check if the bridge has reached its target position
            if (Vector3.Distance(_nextPosition, transform.localPosition) < 0.05f)
            {
                if (!_waiting && _waitingEnabled) // Check if the bridge should wait before moving again
                {
                    StartCoroutine(WaitToGoToNextPos(waitingTime)); // Start the waiting coroutine
                }
                else
                {
                    _retract = !_retract; // Toggle the retract flag to change the direction of movement
                    GoToNextPos(); // Initialize the next position for the bridge to move towards
                }
            }
            else
            {
                // Calculate the distance covered since the start of the movement
                float distCovered = (Time.time - _startTime) * speed;
                // Calculate the fraction of the journey completed
                float fractionOfJourney = distCovered / _journeyLength;
                // Move the bridge towards the next position
                transform.localPosition = Vector3.Lerp(transform.localPosition, _nextPosition, fractionOfJourney);
            }
        }
        
        private IEnumerator WaitToGoToNextPos(float waitTime)
        {  
            _waiting = true; // Set the waiting flag to true
            yield return new WaitForSeconds(waitTime); // Wait for the specified amount of time
            GoToNextPos(); // Initialize the next position for the bridge to move towards
            _waiting = false; // Set the waiting flag to false
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check if the player enters the trigger area of the bridge
            if (other.CompareTag(Tags.Player)) _player.ParentPlayer(transform); // Parent the player to the bridge
        }

        private void OnTriggerExit(Collider other)
        {
            // Check if the player exits the trigger area of the bridge
            if (other.CompareTag(Tags.Player))
            {
                _player.UnParentPlayer(); // Un-parent the player from the bridge
            }
        }
    }
}
