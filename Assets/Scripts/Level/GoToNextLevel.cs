using UnityEngine;
using Utilities;

namespace Level
{
    public class GoToNextLevel : MonoBehaviour
    {
        // OnTriggerEnter is called when another collider enters the trigger collider attached to this game object
        private void OnTriggerEnter(Collider other)
        {
            // Check if the collider that entered the trigger is tagged as 'Player'
            if (other.CompareTag(Tags.Player))
            {
                // If it's the player, show the loading screen using the LoadingScreen singleton instance
                LoadingScreen.Instance.ShowLoadingScreen();
            }
        }
    }
}