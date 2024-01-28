using Managers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class CreditLinkHandler : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        [SerializeField] private string url; // URL to be opened when the element is clicked
        
        // Method called when the pointer clicks on this UI element
        public void OnPointerClick(PointerEventData eventData)
        {
            AudioManager.Instance.Play("uiClick"); // Play click sound from AudioManager
            Application.OpenURL(url); // Open the specified URL
        }

        // Method called when the pointer enters the area of this UI element
        public void OnPointerEnter(PointerEventData eventData)
        {
            AudioManager.Instance.Play("uiHover"); // Play hover sound from AudioManager
        }
    }
}