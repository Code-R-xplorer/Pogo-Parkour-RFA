using Managers;
using UnityEngine;
using Utilities;

namespace Level
{
    public class BreakablePlatform : MonoBehaviour
    {
        [SerializeField] private GameObject[] modelStages;
        [SerializeField] private float cooldown;
        [SerializeField] private BoxCollider boxCollider;

        private int _currentStage;
        
        private bool _inCooldown;
        private float _currentCooldown;

        private bool _onPlatform; // Allows platform to break over time if the player is on the platform

        private bool _destroyedPlatform;

        private void OnCollisionEnter(Collision other)
        {
            if(other.collider.CompareTag(Tags.Player)) ChangeModel();
        }

        private void OnCollisionStay(Collision other)
        {
            if (other.collider.CompareTag(Tags.Player)) _onPlatform = true;
        }

        private void OnCollisionExit(Collision other)
        {
            if (!other.collider.CompareTag(Tags.Player)) return;
            _onPlatform = false;
            _currentCooldown = cooldown;
        }
        
        private void Update()
        {
            if(_destroyedPlatform) return;
            if (_inCooldown)
            {
                _currentCooldown -= Time.deltaTime;
                if (_currentCooldown <= 0)
                {
                    _inCooldown = false;
                }
            }

            if (_onPlatform && !_inCooldown)
            {
                ChangeModel();
            }
        }

        private void ChangeModel()
        {
            // Platform is made up of 5 stages
            // Stage 0 is default
            // Stages 1 - 3 are the broken stages that are cycled through
            if (_currentStage < 3)
            {
                AudioManager.Instance.PlaySoundWithRandomPitch("platformCrack",0.9f, 1.25f);
                modelStages[_currentStage].SetActive(false);
                _currentStage++;
                modelStages[_currentStage].SetActive(true);
                _currentCooldown = cooldown;
                _inCooldown = true;
            }
            else
            {
                AudioManager.Instance.Play("platformBreak");
                boxCollider.enabled = false;
                _inCooldown = true;
                modelStages[_currentStage].SetActive(false);
                modelStages[4].SetActive(true);
                Destroy(gameObject, 1f); // Allows for sound and particle effects to finish
                _destroyedPlatform = true; // Prevents Update while destroying platform
            }
        }
    }
}