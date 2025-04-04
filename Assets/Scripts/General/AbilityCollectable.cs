using FMODUnity;
using Managers;
using TMPro;
using UnityEngine;

namespace General
{
    public class AbilityCollectable : MonoBehaviour
    {
        [SerializeField] private AbilityManager.Abilities abilityToGive;
        [SerializeField] private EventReference collectedSound;

        [SerializeField] private GameObject TextWindow;
        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField] private string abilityDescription;
        
        [SerializeField] private ParticleSystem collectParticle;

        private float _initialY;

        private void Start()
        {
            _initialY = transform.position.y;
        }

        private void Update()
        {
            HandleMovement();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                AudioManager.Instance.PlayOneShot(collectedSound, this.transform.position);
            
                Quaternion particleRotation = Quaternion.Euler(-90f, 0f, 0f);
                ParticleSystem particle = Instantiate(collectParticle, transform.position, particleRotation);
                Destroy(particle.gameObject, 2f);
                
                GiveAbility(abilityToGive);
                if (tutorialText != null)
                {
                    tutorialText.text = abilityDescription;
                    TextWindow.SetActive(true);
                }
                
                Destroy(gameObject);
                
            }
        }

        private void GiveAbility(AbilityManager.Abilities ability)
        {
            AbilityManager.Instance?.EnableAbility(ability);
        }

        private void HandleMovement()
        {
            transform.Rotate(0f, 1f, 0f);
            transform.position = new Vector3(transform.position.x, 0.35f * Mathf.Sin(Time.time * 1f) + _initialY, transform.position.z);
        }
    }
}