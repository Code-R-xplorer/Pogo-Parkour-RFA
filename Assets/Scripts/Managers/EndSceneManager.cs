using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class EndSceneManager : MonoBehaviour
    {
        // Serialized fields for setting references in the Unity editor
        [SerializeField] private Animator bear; // Animator for the bear character
        [FormerlySerializedAs("camera")] [SerializeField] private Animator cameraAnimator; // Animator for the camera
        [SerializeField] private GameObject endCanvas; // GameObject representing the end canvas

        // Static readonly integers to cache the hash values for animator parameters
        private static readonly int Bear = Animator.StringToHash("Bear");
        private static readonly int CameraIn = Animator.StringToHash("CameraIn");
        private static readonly int CameraOut = Animator.StringToHash("CameraOut");

        // Method to start the ending animation sequence
        public void StartAnimation()
        {
            cameraAnimator.Play(CameraIn, -1, 0f); // Play the camera-in animation
        }

        // Method called at the end of animations
        public void AnimationFinished(string animationName)
        {
            switch (animationName)
            {
                case "CameraIn":
                    bear.Play(Bear, -1, 0f); // Play the bear animation after the camera-in animation finishes
                    break;
                case "Bear":
                    cameraAnimator.Play(CameraOut, -1, 0f); // Play the camera-out animation after the bear animation finishes
                    break;
                case "CameraOut":
                    endCanvas.SetActive(true); // Activate the end canvas after the camera-out animation finishes
                    InputManager.Instance.CursorLock(false); // Unlock the cursor
                    break;
            }
        }
        
        // UI Button Methods
        
        // Exit Button Method
        public void ExitGame()
        {
            Application.Quit(); // Quit the application
        }

        // Method to play sound when a UI button is hovered over
        public void ButtonHover()
        {
            AudioManager.Instance.Play("uiHover"); // Play the UI hover sound
        }

        // Method to play sound when a UI button is clicked
        public void ButtonClick()
        {
            AudioManager.Instance.Play("uiClick"); // Play the UI click sound
        }      
    }
}
