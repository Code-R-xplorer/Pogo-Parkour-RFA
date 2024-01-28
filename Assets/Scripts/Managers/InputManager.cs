using UnityEngine;
using Player;
using UnityEngine.SceneManagement;

namespace Managers
{
    [DefaultExecutionOrder(-1)] // Sets the execution order of this script to be before the default
    public class InputManager : MonoBehaviour
    {
        // Singleton instance of InputManager
        public static InputManager Instance { get; private set; }
        private PlayerControls _playerControls; // PlayerControls reference for input handling

        // Delegates for different input actions
        public delegate void BaseAction();
        public delegate void BoolDoubleBaseAction(bool canceled, double duration);
        
        public Vector2 LeanInput { get; private set; } // Property to store lean input values

        // Events that are triggered by specific input actions
        public event BoolDoubleBaseAction OnJump;
        public event BaseAction OnPause;
        public event BaseAction OnToggleDebug;
        public event BaseAction OnExecuteCommand;

        public bool AllowInput { get; private set; } // Flag to enable or disable input processing
        
        private void Awake()
        {
            Instance = this; // Initialize the singleton instance
            
            _playerControls = new PlayerControls(); // Create a new instance of PlayerControls
            if (SceneManager.GetActiveScene().name != "Level_1")
            {
                ToggleInput(true); // Enable input
                CursorLock(true); // Lock the cursor
            }

            // Register input actions with their corresponding events
            _playerControls.Movement.Jump.started += context => { OnJump?.Invoke(context.canceled, context.duration); };
            _playerControls.Movement.Jump.canceled += context => { OnJump?.Invoke(context.canceled, context.duration); };
            _playerControls.Menu.Pause.performed += _ => { OnPause?.Invoke(); };
            _playerControls.Menu.ToggleDebug.performed += _ => { OnToggleDebug?.Invoke(); };
            _playerControls.Menu.ExecuteCommand.performed += _ => { OnExecuteCommand?.Invoke(); };
        }

        // Method to lock or unlock the cursor
        public void CursorLock(bool locked)
        {
            if (locked)
            {
                Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
                Cursor.visible = false; // Make the cursor invisible
            }
            else
            {
                Cursor.lockState = CursorLockMode.None; // Free the cursor
                Cursor.visible = true; // Make the cursor visible
            }
        }

        private void Update()
        {
            LeanInput = _playerControls.Movement.Lean.ReadValue<Vector2>(); // Read lean input values
        }

        // Method to enable or disable input processing
        public void ToggleInput(bool allow)
        {
            AllowInput = allow; // Set the flag based on the parameter
            if(AllowInput) _playerControls.Movement.Enable(); // Enable movement controls
            else _playerControls.Movement.Disable(); // Disable movement controls
        }
        
        private void OnEnable()
        {
            _playerControls.Enable(); // Enable the player controls
        }
    
        private void OnDisable()
        {
            _playerControls.Disable(); // Disable the player controls
        }
    }
}
