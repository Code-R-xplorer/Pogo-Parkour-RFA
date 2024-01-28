using System;
using Managers;
using TMPro;
using UnityEngine;

namespace UI
{
    public class HUD : MonoBehaviour
    {
        [SerializeField] private IngredientText ingredientT1; // Reference to the UI element for Type1 collectable
        [SerializeField] private IngredientText ingredientT2; // Reference to the UI element for Type2 collectable

        private int _ingredientT1Total; // Total count of Type1 collectables
        private int _ingredientT2Total; // Total count of Type2 collectables

        private Animator _animator; // Animator component for HUD animations
        private static readonly int DisplayMessage = Animator.StringToHash("DisplayMessage"); // Hash value for the 'DisplayMessage' animation

        private bool _displayedMessage; // Flag to track if a message has been displayed

        private void Awake()
        {
            _animator = GetComponent<Animator>(); // Initialize the Animator component
        }

        // Method to update the current amount of a specific type of collectable on the HUD
        public void SetCurrentText(GameManager.CollectableTypes collectableType, int amount)
        {
            switch (collectableType)
            {
                case GameManager.CollectableTypes.Type1:
                    var collectedT1 = _ingredientT1Total - amount;
                    ingredientT1.SetCurrent(collectedT1);
                    break;
                case GameManager.CollectableTypes.Type2:
                    var collectedT2 = _ingredientT2Total - amount;
                    ingredientT2.SetCurrent(collectedT2);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(collectableType), collectableType, null);
            }
        }
        
        // Method to set the total amount of a specific type of collectable on the HUD
        public void SetTotalText(GameManager.CollectableTypes collectableType, int amount)
        {
            switch (collectableType)
            {
                case GameManager.CollectableTypes.Type1:
                    _ingredientT1Total = amount;
                    ingredientT1.SetTotal(amount);
                    break;
                case GameManager.CollectableTypes.Type2:
                    _ingredientT2Total = amount;
                    ingredientT2.SetTotal(amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(collectableType), collectableType, null);
            }
        }

        private void OnEnable()
        {
            if(!_displayedMessage) return;
            _animator.Play(DisplayMessage, -1, 0f); // Play the 'DisplayMessage' animation
            _displayedMessage = false;
        }

        // Method to reset the displayed message flag
        public void ResetMessage()
        {
            _displayedMessage = true;
        }

        // Inner class representing the UI elements for displaying collectable counts
        [Serializable]
        protected class IngredientText
        {
            [SerializeField] private TextMeshProUGUI current; // Text element for the current count
            [SerializeField] private TextMeshProUGUI total; // Text element for the total count

            // Method to set the current count text
            public void SetCurrent(int amount)
            {
                current.text = amount.ToString();
            }
            
            // Method to set the total count text
            public void SetTotal(int amount)
            {
                total.text = amount.ToString();
            }
        }
    }
}
