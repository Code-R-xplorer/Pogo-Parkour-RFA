using Cinemachine;
using UnityEngine;

namespace Managers
{
    public class CameraManager : MonoBehaviour
    {
        // Singleton instance of CameraManager
        public static CameraManager Instance { get; private set; }

        private CinemachineVirtualCameraBase _currentVCam; // Reference to the currently active virtual camera

        private CinemachineVirtualCameraBase _playerCamera; // Reference to the player's camera

        private void Awake()
        {
            // Initialize the singleton instance
            Instance = this;
            // Find and assign the player camera by searching for the "PlayerCamera" GameObject
            _playerCamera = GameObject.Find("PlayerCamera").GetComponent<CinemachineVirtualCameraBase>();
            // Set the current virtual camera to the player camera
            _currentVCam = _playerCamera;
        }

        // Switches to the player camera and shows the HUD
        public void SwitchCamera()
        {
            // Lower the priority of the current camera to make it inactive
            _currentVCam.Priority = 0;
            // Set the current camera to the player camera
            _currentVCam = _playerCamera;
            // Increase the priority of the player camera to make it active
            _currentVCam.Priority = 1;
            // Show the HUD through the GameManager
            GameManager.Instance.ShowHUD(true);
        }
        
        // Switches to a new specified camera and optionally hides the HUD
        public void SwitchCamera(CinemachineVirtualCameraBase newCam)
        {
            // If _currentVCam is null, initialize it with newCam
            if (_currentVCam == null) _currentVCam = newCam;
            // Return early if the current camera is already the new camera
            if (_currentVCam == newCam) return;
            // Lower the priority of the current camera to make it inactive
            _currentVCam.Priority = 0;
            // Set the current camera to the new camera
            _currentVCam = newCam;
            // Increase the priority of the new camera to make it active
            _currentVCam.Priority = 1;
            // Hide the HUD through the GameManager
            GameManager.Instance.ShowHUD(false);
        }
    }
}