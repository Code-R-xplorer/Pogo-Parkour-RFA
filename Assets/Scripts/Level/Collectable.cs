using System;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace Level
{
    public class Collectable : MonoBehaviour
    {
        
        public float rotationSpeed = 100.0f; // Adjust the speed as needed
        public float bounceHeight = 0.1f;    // Adjust the bounce height as needed
        public float bounceSpeed = 1.0f;     // Adjust the bounce speed as needed

        [SerializeField] private GameManager.CollectableTypes collectableType;

        private Vector3 _initialPosition;

        private Transform _model;

        private int id;
        
        private void Start()
        {
            id = GameManager.Instance.CollectableSpawned(collectableType);
            
            // Store the initial position of the item
            _model = transform.GetChild(0);
            _initialPosition = _model.position;
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tags.Player))
            {
                GameManager.Instance.CollectableCollected(collectableType, id);
                AudioManager.Instance.PlaySoundWithRandomPitch("collectablePickup",0.9f, 1.25f);
                Destroy(gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Rotate the item around its local up (Y) axis
            _model.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));

            // Calculate vertical position offset for bouncing
            float bounceOffset = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;

            // Update the item's position to create a bouncing effect
            Vector3 newPosition = _initialPosition + new Vector3(0f, bounceOffset, 0f);
            _model.position = newPosition;
        }
    }
}
