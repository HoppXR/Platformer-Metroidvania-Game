using System;
using Platformer;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    private Rigidbody _rb;
    private PlayerMovement _player;
    [SerializeField] private ParticleSystem movementParticle;

    [Range(0, 10)] [SerializeField] private float velocityToActivate;
    [Range(0, 1)] [SerializeField] private float particleOffset;
    
    private float _timer;
    
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        
        HandleMovementParticle();
    }

    private void HandleMovementParticle()
    {
        if (!(Math.Abs(_rb.velocity.x) > velocityToActivate)) return;
        
        if (_timer >= particleOffset)
        {
            movementParticle.Play();
            _timer = 0;
        }
    }
}
