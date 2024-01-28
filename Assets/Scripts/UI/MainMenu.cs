using Cinemachine;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCameraBase menuCam; // Reference to the menu camera

        private void Start()
        {
            // On start, switch to the menu camera
            CameraManager.Instance.SwitchCamera(menuCam);
        }

        // Method to start the game from the main menu
        public void StartGame()
        {
            // Switch back to the default camera
            CameraManager.Instance.SwitchCamera();
            // Hide the HUD initially when the game starts
            GameManager.Instance.ShowHUD(false);
            // Lock the cursor when the game starts
            InputManager.Instance.CursorLock(true);
            // Enable input when the game starts
            InputManager.Instance.ToggleInput(true);
            // Activate the HUD
            GameManager.Instance.ActivateHUD();
            // Deactivate the main menu
            gameObject.SetActive(false);
        }

        // Method to set the currently selected UI item, useful for controller or keyboard navigation
        public void SetSelectedItem(GameObject selected)
        {
            // Set the selected GameObject in the EventSystem
            EventSystem.current.SetSelectedGameObject(selected);
        }
    }
}