using FMODUnity;
using Managers;
using UnityEngine;

namespace General
{
    public class Collectable : MonoBehaviour
    {
        [SerializeField] private EventReference collectedSound;
        [SerializeField] private ParticleSystem collectParticle;
        [SerializeField] private float collectionTime;
    
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
            if (other.gameObject.CompareTag("Player"))
            {
                AudioManager.Instance.PlayOneShot(collectedSound, this.transform.position);
                
                GameManager.Instance.CoinCollected();
            
                if (collectParticle != null)
                {
                    Quaternion particleRotation = Quaternion.Euler(-90f, 0f, 0f);
                    ParticleSystem particle = Instantiate(collectParticle, transform.position, particleRotation);
                    Destroy(particle.gameObject, 2f);
                }
            
                Destroy(gameObject);
            }
        }

        private void HandleMovement()
        {
            transform.Rotate(0f,1f,0f);
            transform.position = new Vector3(transform.position.x, 0.35f * Mathf.Sin(Time.time * 1f) + _initialY, transform.position.z);
        }
    }
}