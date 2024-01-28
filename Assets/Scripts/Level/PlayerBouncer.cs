using UnityEngine;
using Utilities;

namespace Level
{
    public class PlayerBouncer : MonoBehaviour
    {
        // Serialized field to set the bounce force in the Unity editor
        [Range(1f, 10f), SerializeField] private float bounceForce; // The force with which the player will be bounced
        
        // OnCollisionEnter is called when this collider/rigidbody starts touching another rigidbody/collider
        private void OnCollisionEnter(Collision other)
        {
            // Check if the collider that collided is tagged as 'Player'
            if (!other.collider.CompareTag(Tags.Player)) return;
            
            // Apply a force to the player's rigidbody in the direction opposite to the collision normal
            other.transform.root.GetComponent<Rigidbody>().AddForce(other.contacts[0].normal * -bounceForce, ForceMode.Impulse);
        }
    }
}