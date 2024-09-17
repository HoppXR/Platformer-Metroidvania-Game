using UnityEngine;

namespace Platformer
{
    public class Dashing : MonoBehaviour
    {
        [Header("References")] 
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
        private float _dashCdTimer;

        [Header("Input")]
        [SerializeField] private KeyCode dashKey;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _pm = GetComponent<PlayerMovement>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(dashKey))
                Dash();

            if (_dashCdTimer > 0)
                _dashCdTimer -= Time.deltaTime;
        }

        private void Dash()
        {
            if (_dashCdTimer > 0) return;
            else _dashCdTimer = dashCd;
            
            _pm.dashing = true;

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

        private Vector3 GetDirection(Transform forwardT)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3();

            if (allowAllDirections)
                direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
            else
                direction = forwardT.forward;

            if (verticalInput == 0 && horizontalInput == 0)
                direction = forwardT.forward;

            return direction.normalized;
        }
    }
}
