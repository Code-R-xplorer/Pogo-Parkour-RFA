using Managers;
using UnityEngine;
using Utilities;

namespace Level
{
    public class Checkpoint : MonoBehaviour
    {
        // Serialized fields to set in the Unity editor
        [SerializeField] private GameObject activatedGameObject; // GameObject to be activated when checkpoint is triggered
        [SerializeField] private Transform respawnPoint; // The point where the player will respawn after activating this checkpoint

        private bool _activated; // Flag to track if the checkpoint has been activated

        private void OnTriggerEnter(Collider other)
        {
            // Check if the colliding object is the player and the checkpoint hasn't been activated yet
            if (!other.CompareTag(Tags.Player) || _activated) return;

            activatedGameObject.SetActive(true); // Activate the specified GameObject
            _activated = true; // Mark the checkpoint as activated

            // Set the respawn point in the GameManager to this checkpoint's position
            GameManager.Instance.SetRespawnPoint(respawnPoint.position);

            // Play the checkpoint activation sound with a random pitch variation
            AudioManager.Instance.PlaySoundWithRandomPitch("checkpointActivated", 0.9f, 1.25f);
        }
    }
}