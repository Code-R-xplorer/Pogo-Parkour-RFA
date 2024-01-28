using Cinemachine;
using Managers;
using UnityEngine;

namespace Utilities
{
    // Class for swapping cameras when a collider is triggered
    public class CamSwapper : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera; // Reference to the Cinemachine virtual camera to switch to

        // Method called when another collider enters the trigger collider attached to this game object
        private void OnTriggerEnter(Collider other)
        {
            // Check if the root transform of the collider that entered the trigger is tagged as 'Player'
            if (other.transform.root.CompareTag("Player"))
            {
                // Switch the camera to the specified virtual camera using CameraManager
                CameraManager.Instance.SwitchCamera(virtualCamera);
            }
        }
    }
}