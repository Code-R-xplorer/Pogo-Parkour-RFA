using System;
using Managers;
using UnityEngine;
using Utilities;

namespace Player
{
    public class PogoController : MonoBehaviour
    {
        private PogoAnimation _animation; // Holds a reference to the PogoAnimation component.

        private Rigidbody _rigidbody; // Holds a reference to the Rigidbody component.

        [Tooltip("The collider for the spring of the pogo stick model"), SerializeField] 
        private Collider springCollider; // Serialized field for the spring collider of the pogo stick.

        [Header("Jump Parameters")]
        [Tooltip("Minimum amount of force to apply when jumping"), SerializeField]
        private float minJumpForce; // Minimum force applied when jumping.
        [Tooltip("Maximum amount of force to apply when jumping"), SerializeField]
        private float maxJumpForce; // Maximum force applied when jumping.
        [Tooltip("In seconds the amount of time jump needs to be held to apply the max jump force"), SerializeField]
        private float maxHoldTime; // The duration for which a jump must be held to reach maximum force.
        
        [Range(0.001f, 2f)]
        [Tooltip("How fast the pogo stick leans when on the ground"), SerializeField]
        private float torqueForce = 0.1f; // The force applied for leaning the pogo stick on the ground.
        [Tooltip("Speed used to keep the pogo stick upright when on the ground"), SerializeField]
        private float uprightRotationSpeed = -4f; // The rotation speed to maintain upright position on ground.
        [Range(0.001f, 2f)]
        [Tooltip("How fast the pogo stick leans when in the air"), SerializeField]
        private float airTorqueForce = 1; // The force applied for leaning the pogo stick in the air.

        private bool _grounded; // Flag to check if the pogo stick is on the ground.
        private Camera _camera; // Reference to the main camera.

        private bool _land; // Flag to check if the pogo stick has landed.

        private void Start()
        {
            _animation = GetComponent<PogoAnimation>(); // Getting the PogoAnimation component.
            _rigidbody = GetComponent<Rigidbody>(); // Getting the Rigidbody component.

            InputManager.Instance.OnJump += Jump; // Subscribing to the jump event.

            _camera = Camera.main; // Getting the reference to the main camera.
        }

        private void FixedUpdate()
        {
            if (_grounded)
            {
                // Applies a force to keep the pogo stick upright when it's on the ground.
                var torque = -_rigidbody.angularVelocity * (uprightRotationSpeed * Time.fixedDeltaTime);
                _rigidbody.AddTorque(torque, ForceMode.Force);
                
                if(!InputManager.Instance.AllowInput) return; // If input is not allowed, exit the function.

                // Processes leaning inputs on the ground.
                switch (InputManager.Instance.LeanInput.y)
                {
                    case > 0:
                        // Forward leaning.
                        _rigidbody.AddTorque(_camera.transform.right * torqueForce);
                        break;
                    case < 0:
                        // Backward leaning.
                        _rigidbody.AddTorque(-_camera.transform.right * torqueForce);
                        break;
                }
                
                switch (InputManager.Instance.LeanInput.x)
                {
                    case > 0:
                        // Right leaning.
                        _rigidbody.AddTorque(-_camera.transform.forward * torqueForce);
                        break;
                    case < 0:
                        // Left leaning.
                        _rigidbody.AddTorque(_camera.transform.forward * torqueForce);
                        break;
                }
            }
            else
            {
                // Processes leaning inputs in the air.
                switch (InputManager.Instance.LeanInput.y)
                {
                    case > 0:
                        // Forward leaning in the air.
                        _rigidbody.AddTorque(_camera.transform.right * airTorqueForce);
                        break;
                    case < 0:
                        // Backward leaning in the air.
                        _rigidbody.AddTorque(-_camera.transform.right * airTorqueForce);
                        break;
                }

                switch (InputManager.Instance.LeanInput.x)
                {
                    case > 0:
                        // Right leaning in the air.
                        _rigidbody.AddTorque(-_camera.transform.forward * airTorqueForce);
                        break;
                    case < 0:
                        // Left leaning in the air.
                        _rigidbody.AddTorque(_camera.transform.forward * airTorqueForce);
                        break;
                }

                // Applying forces based on horizontal and vertical inputs for leaning in the air.
                float forceHorizontal = InputManager.Instance.LeanInput.x * airTorqueForce * Time.fixedDeltaTime;
                float forceVertical = InputManager.Instance.LeanInput.y * airTorqueForce * Time.fixedDeltaTime;
                _rigidbody.AddForce(_camera.transform.right * forceHorizontal, ForceMode.VelocityChange);
                _rigidbody.AddForce(_camera.transform.forward * forceVertical, ForceMode.VelocityChange);
            }

            // Additional torque forces applied based on player inputs.
            float v = InputManager.Instance.LeanInput.y * torqueForce * Time.fixedDeltaTime;
            float h = InputManager.Instance.LeanInput.x * torqueForce * Time.fixedDeltaTime;
            
            _rigidbody.AddTorque(Vector3.forward * h, ForceMode.VelocityChange);
            _rigidbody.AddTorque(Vector3.up * v, ForceMode.VelocityChange);
        }

        private void Jump(bool canceled, double duration)
        {
            if(!InputManager.Instance.AllowInput) return; // Exits if input is not allowed.

            if (canceled)
            {
                _animation.PlayAnimation(true); // Plays the jump animation.
                if (!_grounded) return; // If not grounded, exits the function.

                // Calculates and applies the jump force.
                var jumpForce = (float)duration;
                jumpForce = Mathf.Clamp(jumpForce, 0f, maxHoldTime);
                jumpForce = jumpForce.Map(0f, maxHoldTime, minJumpForce, maxJumpForce);
                _rigidbody.AddForce(jumpForce * transform.up * Time.fixedDeltaTime, ForceMode.Impulse);
                AudioManager.Instance.PlayRandomSound(new []{"pogoBounce1","pogoBounce2", "pogoBounce3", "pogoBounce4"}, 0.9f, 1.25f);
            }
            else
            {
                // Adjusts the center of mass when not jumping.
                _rigidbody.centerOfMass = new Vector3(0, -0.02f, 0);
                _animation.PlayAnimation(false);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            // Handles collision with the ground.
            if (other.collider.CompareTag("Ground"))
            {
                if (other.GetContact(0).thisCollider != springCollider) return; // Ensures the collision is with the spring collider.
                if(!_land) _rigidbody.centerOfMass = new Vector3(0, -0.07f, 0); // Adjusts center of mass on landing.
                _land = true;
                _grounded = true; // Sets the grounded flag to true.
            }
        }

        private void OnCollisionStay(Collision other)
        {
            // Continuously checks for ground collision.
            if (other.collider.CompareTag("Ground"))
            {
                if (other.GetContact(0).thisCollider != springCollider) return; // Ensures the collision is with the spring collider.
                _grounded = true; // Keeps the grounded flag set to true.
            }
        }

        private void OnCollisionExit(Collision other)             
        {
            // Handles exiting collision with the ground.
            if (other.collider.CompareTag("Ground") && _grounded)
            {
                _land = false;
                _grounded = false; // Resets the grounded flag.
                _rigidbody.centerOfMass = Vector3.zero; // Resets the center of mass.
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // Handles collision with water.
            if (!other.CompareTag(Tags.Water)) return; // Exits if not colliding with water.
            Debug.Log("Hit Water"); // Logs the water collision.
            _rigidbody.Move(GameManager.Instance.RespawnPoint, transform.rotation); // Moves the player to the respawn point.
            _rigidbody.velocity = Vector3.zero; // Resets the velocity.
        }

        public void ParentPlayer(Transform newParent)
        {
            transform.parent = newParent; // Sets the parent of the player transform.
        }

        public void UnParentPlayer()
        {
            transform.parent = null; // Removes the parent of the player transform.
        }

        public void ResetPlayer()
        {
            var transform1 = transform;
            var position = transform1.position;
            position.y += 5; // Adjusts the position slightly above the current position.
            _rigidbody.Move(position, transform1.rotation); // Moves the player to the adjusted position.
        }
    }
}
