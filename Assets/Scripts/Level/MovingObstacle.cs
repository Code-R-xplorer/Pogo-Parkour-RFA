using System.Collections;
using UnityEngine;

namespace Level
{
    public class MovingObstacle : MonoBehaviour
    {
        // Serialized fields for customizing the obstacle's movement in the Unity editor
        [SerializeField] private float movementLength; // The length of the obstacle's movement
        [Range(0.001f, 1f), SerializeField] private float speed = 0.03f; // Speed of the obstacle's movement, within a specified range
        [SerializeField] private float waitingTime = 1f; // Time the obstacle waits before moving again
        [SerializeField] private bool moveOnStart; // Determines if the obstacle starts moving when the level starts

        private Vector3 _nextPosition; // Target position for the next movement
        private float _journeyLength; // Total length of the current movement
        private float _startTime; // Time when the current movement started
        private bool _waiting; // Flag to indicate if the obstacle is currently waiting
        private bool _waitingEnabled; // Flag to enable or disable waiting
        private bool _retract = true; // Flag to indicate the direction of movement (extend or retract)
        private Transform _wallTransform; // Transform of the wall that is part of the obstacle

        private bool _canMove; // Flag to control if the obstacle can move

        private void Start()
        {
            _waitingEnabled = waitingTime != 0; // Initialize waiting behavior based on the waitingTime setting
            _wallTransform = transform.GetChild(1); // Get the transform of the wall child
            if (!moveOnStart) return; // Check if the obstacle should move on start and exit early if not
            StartMoving(); // Start moving the obstacle
        }

        public void StartMoving()
        {
            _canMove = true; // Set the flag to allow obstacle movement
            GoToNextPos(); // Initialize the next position for the obstacle to move towards
        }

        private void GoToNextPos()
        {
            // Set the next position based on the retract flag and movement length
            _nextPosition = _retract ? new Vector3(0, 0, movementLength) : new Vector3(0, 0, 0);
            // Calculate the distance between the current position and the next position
            _journeyLength = Vector3.Distance(_nextPosition, _wallTransform.localPosition);
            // Record the start time of the movement
            _startTime = Time.time;
        }

        // FixedUpdate is used for physics-based updates
        void FixedUpdate()
        {
            if (!_canMove) return; // Exit early if the obstacle is not allowed to move

            // Check if the obstacle has reached its target position
            if (Vector3.Distance(_nextPosition, _wallTransform.localPosition) < 0.05f)
            {
                if (!_waiting && _waitingEnabled) // Check if the obstacle should wait before moving again
                {
                    StartCoroutine(WaitToGoToNextPos(waitingTime)); // Start the waiting coroutine
                }
                else
                {
                    _retract = !_retract; // Toggle the retract flag to change the direction of movement
                    GoToNextPos(); // Initialize the next position for the obstacle to move towards
                }
            }
            else
            {
                // Calculate the distance covered since the start of the movement
                float distCovered = (Time.time - _startTime) * speed;
                // Calculate the fraction of the journey completed
                float fractionOfJourney = distCovered / _journeyLength;
                // Move the obstacle towards the next position
                _wallTransform.localPosition = Vector3.Lerp(_wallTransform.localPosition, _nextPosition, fractionOfJourney);
            }
        }

        private IEnumerator WaitToGoToNextPos(float waitTime)
        {  
            _waiting = true; // Set the waiting flag to true
            yield return new WaitForSeconds(waitTime); // Wait for the specified amount of time
            GoToNextPos(); // Initialize the next position for the obstacle to move towards
            _waiting = false; // Set the waiting flag to false
        }
    }
}
