using System;
using System.Collections;
using Player.Movement;
using UnityEngine;

namespace Player
{
    public class ParticleController : MonoBehaviour
    {
        private Rigidbody _rb;
        private PlayerMovement _pm;
    
        [Header("Movement Particle")]
        [SerializeField] private ParticleSystem movementParticle;
        [Range(0, 10)] [SerializeField] private float velocityToActivate;
        [Range(0, 1)] [SerializeField] private float particleOffset;

        [Header("Movement Trail")] 
        [SerializeField] private TrailRenderer trailRenderer;
        [Range(0, 100)] [SerializeField] private float velocityToActivateTrail;
    
        [Header("Landing Particle")]
        [SerializeField] private ParticleSystem landingParticle;
        
        [Header("Jump Particles")]
        [SerializeField] private ParticleSystem jumpParticle;
        [SerializeField] private ParticleSystem doubleJumpParticle;
    
        private float _timer;
    
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _pm = GetComponent<PlayerMovement>();
        }

        private void Update()
        {
            _timer += Time.deltaTime;
        
            HandleMovementParticle();
            HandleTrailRenderer();

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

        private void HandleTrailRenderer()
        {
            if (_pm.swinging || _pm.crouching || Math.Abs(_rb.velocity.x) >= 14.5)
            {
                trailRenderer.emitting = true;
            }
            else
            {
                trailRenderer.emitting = false;
            }
        }

        private IEnumerator WaitForLanding()
        {
            yield return new WaitUntil(() => !PlayerMovement.Grounded);
            yield return new WaitUntil(() => PlayerMovement.Grounded);
        
            landingParticle.Play();
        }
        
        public void PlayJumpParticle()
        {
            jumpParticle.Play();
        }

        public void PlayDoubleJumpParticle()
        {
            doubleJumpParticle.Play();
        }
    }
}
