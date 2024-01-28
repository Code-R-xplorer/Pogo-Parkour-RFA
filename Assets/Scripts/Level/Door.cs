using System;
using System.Collections;
using Cinemachine;
using Managers;
using UnityEngine;

namespace Level
{
    public class Door : MonoBehaviour
    {
        private Animator _animator; // Animator component to control door animations

        // Serialized fields for customizing door behavior in Unity editor
        [SerializeField] private OpenAnim openAnim; // Enum to determine the opening animation direction
        [SerializeField] private bool finalDoor = true; // Flag to determine if this is the final door
        [SerializeField] private CinemachineVirtualCameraBase doorCamera; // Reference to the camera for this door
        
        // Static readonly integers to cache the hash values for animator parameters
        private static readonly int OpenLeft = Animator.StringToHash("Open_Left");
        private static readonly int OpenRight = Animator.StringToHash("Open_Right");
        private static readonly int OpenDown = Animator.StringToHash("Open_Down");
        private static readonly int CloseLeft = Animator.StringToHash("Close_Left");
        private static readonly int CloseRight = Animator.StringToHash("Close_Right");
        private static readonly int CloseDown = Animator.StringToHash("Close_Down");

        private bool _open; // Flag to track whether the door is open

        private void Start()
        {
            _animator = GetComponent<Animator>(); // Initialize the animator component
            if (finalDoor) 
            {
                // Subscribe to the event to open the door when all collectables are collected
                GameManager.Instance.OnAllCollectablesCollected += () => 
                {
                    StartCoroutine(CameraSequence());
                };
            }
        }

        public void OpenDoor()
        {
            if (_open) return; // If the door is already open, return

            // Determine the opening animation based on the openAnim enum
            switch (openAnim)
            {
                case OpenAnim.Left:
                    _animator.Play(OpenLeft, -1, 0f); // Play the animation to open left
                    break;
                case OpenAnim.Right:
                    _animator.Play(OpenRight, -1, 0f); // Play the animation to open right
                    break;
                case OpenAnim.Down:
                    _animator.Play(OpenDown, -1, 0f); // Play the animation to open down
                    break;
                default:
                    throw new ArgumentOutOfRangeException(); // Throw exception if the enum value is not handled
            }
            AudioManager.Instance.Play("door"); // Play the door sound
            _open = true; // Set the door to open
        }

        private IEnumerator CameraSequence()
        {
            CameraManager.Instance.SwitchCamera(doorCamera); // Switch the camera to focus on the door
            yield return new WaitForSeconds(1); // Wait for 1 second
            OpenDoor(); // Open the door after the wait
        }

        public void CloseDoor()
        {
            if (!_open) return; // If the door is already closed, return

            // Determine the closing animation based on the openAnim enum
            switch (openAnim)
            {
                case OpenAnim.Left:
                    _animator.Play(CloseLeft, -1, 0f); // Play the animation to close left
                    break;
                case OpenAnim.Right:
                    _animator.Play(CloseRight, -1, 0f); // Play the animation to close right
                    break;
                case OpenAnim.Down:
                    _animator.Play(CloseDown, -1, 0f); // Play the animation to close down
                    break;
                default:
                    throw new ArgumentOutOfRangeException(); // Throw exception if the enum value is not handled
            }
            AudioManager.Instance.Play("door"); // Play the door sound
            _open = false; // Set the door to closed
        }

        public void AnimationFinished()
        {
            CameraManager.Instance.SwitchCamera(); // Switch the camera back to the default after the animation finishes
        }

        // Enum to define the directions in which the door can open
        private enum OpenAnim
        {
            Left,
            Right,
            Down
        }
    }
}
