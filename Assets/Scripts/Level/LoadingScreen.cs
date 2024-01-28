using Cinemachine;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Level
{
    public class LoadingScreen : MonoBehaviour
    {
        // Singleton instance of LoadingScreen
        public static LoadingScreen Instance { get; private set; }

        private Animator _animator; // Animator component for the loading screen animations

        // Static readonly integers to cache the hash values for animator parameters
        private static readonly int LoadingFadeIn = Animator.StringToHash("LoadingFadeIn");
        private static readonly int LoadingFadeOut = Animator.StringToHash("LoadingFadeOut");

        private int _nextLevelID; // ID of the next level to load

        private bool _shown; // Flag to track if the loading screen is shown
        private CinemachineBrain _camera; // Reference to the main camera's CinemachineBrain component

        private void Awake()
        {
            Instance = this; // Set the singleton instance

            // Subscribe to the sceneLoaded event to perform actions when a new scene is loaded
            SceneManager.sceneLoaded += (_, _) =>
            {
                _camera = Camera.main!.GetComponent<CinemachineBrain>(); // Get the CinemachineBrain component

                // Special actions based on specific scene names
                if (SceneManager.GetActiveScene().name == "Level_3")
                    GameManager.Instance.OnAllCollectablesCollected += ShowLoadingScreen;

                if (SceneManager.GetActiveScene().name == "Level_End") 
                    _nextLevelID = -1; // Set the next level ID for the ending level
                else
                {
                    // Increment the level ID based on the current scene's name
                    _nextLevelID = int.Parse(SceneManager.GetActiveScene().name.Split("_")[1]);
                    _nextLevelID++;
                }

                HideLoadingScreen(); // Hide the loading screen
            };

            _animator = GetComponent<Animator>(); // Initialize the animator component
            DontDestroyOnLoad(gameObject); // Prevent the loading screen from being destroyed on load
            gameObject.SetActive(false); // Initially set the loading screen to inactive
        }

        public void ShowLoadingScreen()
        {
            // Method to show the loading screen without specifying a level ID
            gameObject.SetActive(true);
            _shown = true;
            InputManager.Instance.ToggleInput(false); // Disable player input
            _camera.enabled = false; // Disable the main camera's CinemachineBrain
            _animator.Play(LoadingFadeIn, -1, 0f); // Play the fade-in animation
        }
        
        public void ShowLoadingScreen(int levelID)
        {
            // Method to show the loading screen and specify the next level ID
            _nextLevelID = levelID;
            gameObject.SetActive(true);
            _shown = true;
            InputManager.Instance.ToggleInput(false); // Disable player input
            _camera.enabled = false; // Disable the main camera's CinemachineBrain
            _animator.Play(LoadingFadeIn, -1, 0f); // Play the fade-in animation
        }

        private void HideLoadingScreen()
        {
            // Hide the loading screen if it is shown
            if(!_shown) return;
            _animator.Play(LoadingFadeOut, -1, 0f); // Play the fade-out animation
        }

        public void AnimationFinished(string animationName)
        {
            // Method called at the end of animations
            switch (animationName)
            {
                case "Shown":
                    // Load the next level or the end level scene
                    SceneManager.LoadScene(_nextLevelID != 4 ? $"Level_{_nextLevelID}" : "Level_End");
                    break;
                case "Hidden":
                    // Perform actions when the loading screen is hidden
                    _shown = false;
                    if (SceneManager.GetActiveScene().name == "Level_End")
                    {
                        // Start the end scene animation if the current scene is the end level
                        GameObject.Find("End Scene Manager").GetComponent<EndSceneManager>().StartAnimation();
                    }
                    else
                    {
                        // Re-enable the camera and input, activate the HUD for other levels
                        _camera.enabled = true;
                        InputManager.Instance.ToggleInput(true);
                        GameManager.Instance.ActivateHUD();
                    }
                    gameObject.SetActive(false); // Deactivate the loading screen gameObject
                    break;
            }
        }
    }
}
