using Managers;
using UnityEngine;

namespace Utilities
{
    // Class for handling animation events
    public class AnimationEvent : MonoBehaviour
    {
        [SerializeField] private EndSceneManager endSceneManager; // Reference to the EndSceneManager

        // Method to be called by animation events
        public void Event(string input)
        {
            endSceneManager.AnimationFinished(input); // Call the AnimationFinished method in the EndSceneManager with the provided input
        }
    }
}