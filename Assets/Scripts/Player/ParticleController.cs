using System;
using System.Collections;
using Player.Movement;
using UnityEngine;

namespace Player
{
    public class ParticleController : MonoBehaviour
    {
        private Rigidbody _rb;
    
        [Header("Movement Particle")]
        [SerializeField] private ParticleSystem movementParticle;
        [Range(0, 10)] [SerializeField] private float velocityToActivate;
        [Range(0, 1)] [SerializeField] private float particleOffset;
    
        [Header("Landing Particle")]
        [SerializeField] private ParticleSystem landingParticle;
    
        private float _timer;
    
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            _timer += Time.deltaTime;
        
            HandleMovementParticle();

            if (!PlayerMovement.Grounded)
                StartCoroutine(WaitForLanding());
        }

        private void HandleMovementParticle()
        {
            if (!(Math.Abs(_rb.velocity.x) > velocityToActivate)) return;
        
            if (_timer >= particleOffset && PlayerMovement.Grounded)
            {
                movementParticle.Play();
                _timer = 0;
            }
        }

        private IEnumerator WaitForLanding()
        {
            yield return new WaitUntil(() => !PlayerMovement.Grounded);
            yield return new WaitUntil(() => PlayerMovement.Grounded);
        
            landingParticle.Play();
        }
    }
}
