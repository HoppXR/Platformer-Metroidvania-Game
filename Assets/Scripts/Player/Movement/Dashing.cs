using System;
using FMODUnity;
using Managers;
using Player.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Player.Movement
{
    public class Dashing : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private InputReader input;
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform playerCam;
        private Rigidbody _rb;
        private PlayerMovement _pm;

        [Header("Dashing")] 
        [SerializeField] private float dashForce;
        [SerializeField] private float dashUpwardForce;
        [SerializeField] private float dashDuration;
        
        [Header("Settings")]
        [SerializeField] private bool allowAllDirections = true;
        [SerializeField] private bool disableGravity = true;
        [SerializeField] private bool resetVel = true;
        
        [Header("Cooldown")] 
        [SerializeField] private float dashCd;
        [SerializeField] private Image imageCd;
        private float _dashCdTimer;
        
        [Header("Sounds")]
        [SerializeField] private EventReference dashSound;
        
        private Vector2 _inputDirection;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _pm = GetComponent<PlayerMovement>();

            imageCd.fillAmount = 0.0f;

            input.MoveEvent += HandleDirection;
            input.DashEvent += Dash;
        }

        private void Update()
        {
            if (_dashCdTimer > 0)
                _dashCdTimer -= Time.deltaTime;

            imageCd.fillAmount = _dashCdTimer / dashCd;
        }

        private void OnDisable()
        {
            input.MoveEvent -= HandleDirection;
            input.DashEvent -= Dash;
        }

        private void Dash()
        {
            if (_dashCdTimer > 0 || !AbilityManager.DashEnabled) return;
            
            _dashCdTimer = dashCd;
            
            _pm.dashing = true;
            
            AudioManager.Instance?.PlayOneShot(dashSound, transform.position);

            Transform forwardT = orientation;

            Vector3 direction = GetDirection(forwardT);
            
            Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

            if (disableGravity)
                _rb.useGravity = false;

            _delayedForceToApply = forceToApply;
            Invoke(nameof(DelayedDashForce), 0.025f);
            
            Invoke(nameof(ResetDash), dashDuration);
        }

        private Vector3 _delayedForceToApply;
        private void DelayedDashForce()
        {
            if (resetVel)
                _rb.velocity = Vector3.zero;
            
            _rb.AddForce(_delayedForceToApply, ForceMode.Impulse);
        }
        
        private void ResetDash()
        {
            _pm.dashing = false;
            
            if (disableGravity)
                _rb.useGravity = true;
        }

        private void HandleDirection( Vector2 dir)
        {
            _inputDirection = dir;
        }
        
        private Vector3 GetDirection(Transform forwardT)
        {
            Vector3 direction = new Vector3();

            if (allowAllDirections)
                direction = forwardT.forward * _inputDirection.y + forwardT.right * _inputDirection.x;
            else
                direction = forwardT.forward;

            if (_inputDirection.y == 0 && _inputDirection.x == 0)
                direction = forwardT.forward;

            return direction.normalized;
        }
    }
}
