using Cinemachine;
using Managers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Utilities;

namespace UI
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer; // Reference to the AudioMixer to control sound levels
        [SerializeField] private GameObject resetButton; // The reset button in the pause menu
        
        public GameObject firstSelected; // The first UI element to be selected when pause menu is shown
        private GameObject _caller; // The GameObject that called the pause menu

        private bool _paused; // Flag to track if the game is paused

        private CinemachineBrain _camera; // Reference to the main camera's CinemachineBrain
        
        private void Awake()
        {
            DontDestroyOnLoad(gameObject); // Prevent the pause menu from being destroyed on loading new scenes
            SceneManager.sceneLoaded += SceneLoad; // Subscribe to the sceneLoaded event
            SceneManager.sceneUnloaded += SceneUnload; // Subscribe to the sceneUnloaded event
        }

        private void Start()
        {
            gameObject.SetActive(false); // Initially set the pause menu to be inactive
        }

        private void ToggleMenu()
        {
            // Check if pausing is allowed and the debug console is not visible
            if (!GameManager.Instance.CanPause || DebugController.Instance.ConsoleVisible()) return;
            _paused = !_paused; // Toggle the paused state
            if (_paused)
            {
                DisplayPauseMenu(); // Display the pause menu
            }
            else
            {
                HidePauseMenu(); // Hide the pause menu
            }
        }

        public void DisplayPauseMenu(GameObject caller)
        {
            _caller = caller; // Store the caller GameObject
            _caller.SetActive(false); // Deactivate the caller
            resetButton.SetActive(false); // Deactivate the reset button
            gameObject.SetActive(true); // Activate the pause menu
            EventSystem.current.SetSelectedGameObject(firstSelected); // Set the first selected UI element
        }

        private void DisplayPauseMenu()
        {
            InputManager.Instance.ToggleInput(false); // Disable player input
            InputManager.Instance.CursorLock(false); // Unlock the cursor
            resetButton.SetActive(true); // Activate the reset button
            gameObject.SetActive(true); // Activate the pause menu
            Time.timeScale = 0f; // Pause the game
            _camera.enabled = false; // Disable the main camera's CinemachineBrain
            GameManager.Instance.ShowHUD(false); // Hide the HUD
        }

        public void HidePauseMenu()
        {
            if (_caller != null)
            {
                _caller.SetActive(true); // Activate the caller
                gameObject.SetActive(false); // Deactivate the pause menu
                EventSystem.current.SetSelectedGameObject(GameObject.Find("Options")); // Set the selected UI element to 'Options'
                _caller = null; // Reset the caller
            }
            else
            {
                InputManager.Instance.ToggleInput(true); // Enable player input
                InputManager.Instance.CursorLock(true); // Lock the cursor
                _paused = false; // Set paused state to false
                gameObject.SetActive(false); // Deactivate the pause menu
                Time.timeScale = 1f; // Resume the game
                _camera.enabled = true; // Enable the main camera's CinemachineBrain
                GameManager.Instance.ShowHUD(true); // Show the HUD
            }
        }
        
        public void SetMasterVol(float value)
        {
            var masterVol = Mathf.Log10(value) * 20; // Convert the volume to decibels
            audioMixer.SetFloat("MasterVol", masterVol); // Set the master volume
        }
        
        public void SetMusicVol(float value)
        {
            var musicVol = Mathf.Log10(value) * 20; // Convert the volume to decibels
            audioMixer.SetFloat("MusicVol", musicVol); // Set the music volume
        }
        
        public void SetSfxVol(float value)
        {
            var sfxVol = Mathf.Log10(value) * 20; // Convert the volume to decibels
            audioMixer.SetFloat("SfxVol", sfxVol); // Set the SFX volume
        }

        public void ResetButton()
        {
            GameManager.Instance.ResetPlayer(); // Reset the player's position
            HidePauseMenu(); // Hide the pause menu
        }

        public void ExitGame()
        {
            Application.Quit(); // Quit the application
        }

        private void SceneLoad(Scene scene, LoadSceneMode loadSceneMode)
        {
            _camera = Camera.main.GetComponent<CinemachineBrain>(); // Initialize the main camera's CinemachineBrain
            // Subscribe to ToggleMenu if the scene is not the end level
            if (scene.name != "Level_End") InputManager.Instance.OnPause += ToggleMenu;
        }
        
        private void SceneUnload(Scene scene)
        {
            InputManager.Instance.OnPause -= ToggleMenu; // Unsubscribe from ToggleMenu
        }
    }
}
