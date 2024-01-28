using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Level;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

// Defines a namespace for organization and avoiding naming conflicts.
namespace Utilities
{
    // DebugController class, inheriting from MonoBehaviour, to manage debug functionalities in a Unity game.
    public class DebugController : MonoBehaviour
    {
        // Singleton instance for global access.
        public static DebugController Instance { get; private set; }
        
        // Boolean flags to control the visibility of the debug console and help menu.
        private bool showConsole;
        private bool showHelp;

        // Variables for handling user input and command output in the debug console.
        private string input;
        private string _commandOutput;
        private bool showCommandOutput;

        // Commands available in the debug console.
        private static DebugCommand _help;
        private static DebugCommand<int> _changeLevel;
        private static DebugCommand _restart;
        private static DebugCommand _completeLevel;

        // A list to store all the debug commands.
        public List<object> commandList;

        // For handling scrolling within the console window.
        private Vector2 scroll;

        // Reference to the main camera's CinemachineBrain component.
        private CinemachineBrain _camera;

        // Method to toggle the debug console's visibility and adjust game input and camera accordingly.
        private void ToggleDebug()
        {
            // Toggles console visibility and adjusts game input and camera behavior.
            showConsole = !showConsole;
            InputManager.Instance.ToggleInput(!showConsole);
            InputManager.Instance.CursorLock(!showConsole);
            _camera.enabled = !showConsole;
            if (!showConsole) showHelp = false;
        }

        // Executes a command entered into the debug console.
        private void ExecuteCommand()
        {
            // Exits if the console is not visible.
            if(!showConsole) return;

            // Processes the entered input.
            HandleInput();
            input = "";

            // Hides command output after a delay, if needed.
            if (showCommandOutput)
            {
                StartCoroutine(HideCommandOutput());
            }

            // Exits if the help menu is shown.
            if (showHelp) return;

            // Toggles debug visibility.
            ToggleDebug();
        }

        // Initialization method for setting up debug commands and event listeners.
        private void Awake()
        {
            // Sets up the singleton instance.
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize debug commands with their descriptions and actions.
            _help = new DebugCommand("help", "Shows all commands", "help", () =>
            {
                Debug.Log("Help Command Called");
                showHelp = !showHelp;
            });

            _changeLevel = new DebugCommand<int>("change_level", "Changes the current level", "change_level <level_id>",
                x =>
            {
                LoadingScreen.Instance.ShowLoadingScreen(x);
            });
            
            _restart = new DebugCommand("restart", "Restarts the current level", "restart", () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });

            _completeLevel = new DebugCommand("complete_level", "Completes the objectives for the current level",
                "complete_level", () =>
            {
                GameManager.Instance.CompleteLevel();
            });

            // Adds the commands to the command list.
            commandList = new List<object>
            {
                _help,
                _changeLevel,
                _restart,
                _completeLevel
            };

            // Event subscriptions for scene loading and unloading.
            SceneManager.sceneLoaded += (_, _) =>
            {
                InputManager.Instance.OnToggleDebug += ToggleDebug;
                InputManager.Instance.OnExecuteCommand += ExecuteCommand;
                _camera = Camera.main!.GetComponent<CinemachineBrain>();
            };

            SceneManager.sceneUnloaded += _ =>
            {
                InputManager.Instance.OnToggleDebug -= ToggleDebug;
                InputManager.Instance.OnExecuteCommand -= ExecuteCommand;
            };
        }

        // Coroutine to hide command output after a set delay.
        private IEnumerator HideCommandOutput()
        {
            yield return new WaitForSeconds(5);
            showCommandOutput = false;
            _commandOutput = "";
        }

        // Method to render the debug console GUI.
        private void OnGUI()
        {
            // Exits if the console is not visible.
            if(!showConsole) return;

            float y = 0f; // Y position for rendering GUI elements.

            // Renders the help menu, if enabled.
            if (showHelp)
            {
                GUI.Box(new Rect(0,y,Screen.width, 100), "");

                Rect viewport = new Rect(0, 0, Screen.width - 30, 20 * commandList.Count);
                scroll = GUI.BeginScrollView(new Rect(0, y + 5f, Screen.width, 90), scroll, viewport);

                for (int i = 0; i < commandList.Count; ++i)
                {
                    DebugCommandBase command = commandList[i] as DebugCommandBase;
                    string label = $"{command!.CommandFormat} - {command.CommandDescription}";
                    Rect labelRect = new Rect(5, 20 * i, viewport.width - 100, 20);
                    GUI.Label(labelRect, label);
                }
            
                GUI.EndScrollView();
                y += 100;
            }

            // Renders command output, if any.
            if (showCommandOutput)
            {
                Rect viewport = new Rect(0, 0, Screen.width - 30, 20);
                Rect labelRect = new Rect(0, 0, viewport.width - 100, 20);
                GUI.Label(labelRect, _commandOutput);
                y += 30;
            }

            // Renders the input text field for the console.
            GUI.Box(new Rect(0,y,Screen.width, 30), "");
            GUI.backgroundColor = new Color(0, 0, 0, 0);
            input = GUI.TextField(new Rect(10f, y + 5f, Screen.width - 20f, 20f), input);
        }
    
        // Parses and executes the input command from the debug console.
        private void HandleInput()
        {
            string[] properties = input.Split(new[] {' '}, 2);
            bool commandSuccess = false;
            for (int i = 0; i < commandList.Count; i++)
            {
                DebugCommandBase commandBase = commandList[i] as DebugCommandBase;
                if (!input.Contains(commandBase!.CommandId)) continue;
                commandSuccess = true;
                   
                // Executes the command without arguments or with a single argument.
                if (properties.Length == 1)
                {
                    (commandList[i] as DebugCommand)?.Invoke();
                    return;
                }
                if (properties[1] == "")
                {
                    (commandList[i] as DebugCommand)?.Invoke();
                    return;
                }

                // Handles numeric arguments for commands.
                bool isNumber = int.TryParse(properties[1], out int n);
                if(isNumber) (commandList[i] as DebugCommand<int>)?.Invoke(n);
                else (commandList[i] as DebugCommand<string>)?.Invoke(properties[1]);
            }
            if(!commandSuccess) DisplayCommandOutput("Command: " + properties[0] + " not found!");
        }

        // Displays a given command output in the console.
        private void DisplayCommandOutput(string commandOutput)
        {
            _commandOutput = commandOutput;
            showCommandOutput = true;
        }

        // Method to check if the console is visible.
        public bool ConsoleVisible()
        {
            return showConsole;
        }
    }
}