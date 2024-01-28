using UnityEngine;

namespace UI
{
    public class CreditsMenu : MonoBehaviour
    {
        [SerializeField] private GameObject[] tabs; // Array of GameObjects representing different tabs in the credits menu
        
        private int _currentTab; // Index of the currently active tab

        // Method to change the active tab in the credits menu
        public void ChangeTab(int newTab)
        {
            // Check if the new tab index is the same as the current or out of bounds, and return if so
            if (_currentTab == newTab || newTab >= tabs.Length) return;

            tabs[_currentTab].SetActive(false); // Deactivate the currently active tab
            _currentTab = newTab; // Update the current tab index
            tabs[_currentTab].SetActive(true); // Activate the new tab
        }
    }
}