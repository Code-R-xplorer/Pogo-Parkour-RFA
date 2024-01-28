using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using UI;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        // Singleton instance of GameManager
        public static GameManager Instance { get; private set; }
        
        // Enum for different types of collectables
        public enum CollectableTypes
        {
            Type1,
            Type2
        }

        private int _collectableCountT1; // Count of Type1 collectables
        private int _collectableCountT2; // Count of Type2 collectables

        // Event triggered when all collectables are collected
        public event Action OnAllCollectablesCollected;

        private readonly List<int> collectableIDs = new(); // List to store collectable IDs

        public Vector3 RespawnPoint { get; private set; } // Player's respawn point

        public PogoController Player { get; private set; } // Reference to the player controller

        private HUD _hud; // Reference to the HUD
        
        public bool CanPause { get; private set; } // Flag to control if the game can be paused

        // Method to invoke the OnAllCollectablesCollected event
        private void AllCollectablesCollected()
        {
            OnAllCollectablesCollected?.Invoke();
        }

        private void Awake()
        {
            Instance = this; // Initialize the singleton instance
            Player = GameObject.Find("Player").GetComponent<PogoController>(); // Find and assign the player controller
            _hud = GameObject.Find("HUD").GetComponent<HUD>(); // Find and assign the HUD
            _hud.gameObject.SetActive(false); // Initially set the HUD to inactive
        }

        private void Start()
        {
            SetRespawnPoint(Player.transform.position); // Set initial respawn point to player's position
            AudioManager.Instance.PlayRandomSound(new []{"ambient1","ambient2", "ambient3"}); // Play a random ambient sound
            // if(SceneManager.GetActiveScene().name != "Level_1") ActivateHUD();
        }

        // Method called when a collectable is spawned
        public int CollectableSpawned(CollectableTypes collectableType)
        {
            switch (collectableType)
            {
                case CollectableTypes.Type1:
                    _collectableCountT1++; // Increase Type1 collectable count
                    _hud.SetTotalText(collectableType, _collectableCountT1); // Update HUD
                    break;
                case CollectableTypes.Type2:
                    _collectableCountT2++; // Increase Type2 collectable count
                    _hud.SetTotalText(collectableType, _collectableCountT2); // Update HUD
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(collectableType), collectableType, null);
            }
            
            var id = collectableIDs.Count; // Generate a new ID for the collectable
            collectableIDs.Add(id); // Add the ID to the list
            return id; // Return the ID
        }

        // Method called when a collectable is collected
        public void CollectableCollected(CollectableTypes collectableType, int id)
        {
            if (collectableIDs.All(i => id != i)) return; // Return if the ID is not in the list
            collectableIDs.Remove(id); // Remove the ID from the list
            switch (collectableType)
            {
                case CollectableTypes.Type1:
                    _collectableCountT1--; // Decrease Type1 collectable count
                    _hud.SetCurrentText(collectableType, _collectableCountT1); // Update HUD
                    break;
                case CollectableTypes.Type2:
                    _collectableCountT2--; // Decrease Type2 collectable count
                    _hud.SetCurrentText(collectableType, _collectableCountT2); // Update HUD
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(collectableType), collectableType, null);
            }
            // Check if all collectables are collected
            if(_collectableCountT1 == 0 && _collectableCountT2 == 0) AllCollectablesCollected();
        }

        // Method to complete the level
        public void CompleteLevel()
        {
            _collectableCountT1 = 0; // Reset Type1 collectable count
            _collectableCountT2 = 0; // Reset Type2 collectable count
            _hud.SetCurrentText(CollectableTypes.Type1, 0); // Update HUD
            _hud.SetCurrentText(CollectableTypes.Type2, 0); // Update HUD
            AllCollectablesCollected(); // Trigger the AllCollectablesCollected event
        }

        // Method to set the respawn point
        public void SetRespawnPoint(Vector3 respawnPoint)
        {
            RespawnPoint = respawnPoint; // Update the respawn point
        }

        // Method to activate the HUD
        public void ActivateHUD()
        {
            _hud.ResetMessage(); // Reset the message on the HUD
            _hud.gameObject.SetActive(true); // Activate the HUD
            CanPause = true; // Allow the game to be paused
        }

        // Method to show or hide the HUD
        public void ShowHUD(bool show)
        {
            _hud.gameObject.SetActive(show); // Show or hide the HUD based on the parameter
        }

        // Method to reset the player
        public void ResetPlayer()
        {
            Player.ResetPlayer(); // Call the ResetPlayer method on the player controller
        }
    }
}
