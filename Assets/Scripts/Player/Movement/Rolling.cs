using Managers;
using Player.Input;
using UnityEngine;

namespace Player.Movement
{
    public class Rolling : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private InputReader input;
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform playerCollider;
        private Rigidbody _rb;
        private PlayerMovement _pm;

        [Header("Sliding")] 
        [SerializeField] private float slideForce;
        [SerializeField] private float slideYScale;
        private float _startYScale;

        private Vector2 _inputDirection;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _pm = GetComponent<PlayerMovement>();

            _startYScale = playerCollider.localScale.y;

            input.MoveEvent += HandleInput;
            input.CrouchEvent += StartSlide;
            input.CrouchCancelledEvent += StopSlide;
        }

        private void FixedUpdate()
        {
            if (_pm.crouching)
                SlidingMovement();
        }

        private void OnDisable()
        {
            input.MoveEvent -= HandleInput;
            input.CrouchEvent -= StartSlide;
            input.CrouchCancelledEvent -= StopSlide;
        }

        private void HandleInput(Vector2 dir)
        {
            _inputDirection = dir;
        }

        private void StartSlide()
        {
            if (_pm.swinging || !AbilityManager.SlideEnabled) return;
            
            _pm.crouching = true;

            if (playerCollider != null)
                playerCollider.localScale = new Vector3(playerCollider.localScale.x, slideYScale, playerCollider.localScale.z);
         
            if (_rb != null)
                _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        private void SlidingMovement()
        {
            Vector3 inputDirection = orientation.forward * _inputDirection.y + orientation.right * _inputDirection.x;
            
            // sliding normal
            if (!_pm.OnSlope())
            {
                _rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
            }
            // sliding down a slope
            else
            {
                _rb.AddForce(_pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
            }
        }
        
        private void StopSlide()
        {
            if (!_pm.crouching) return;
            
            _pm.crouching = false;
            
            if (playerCollider != null)
                playerCollider.localScale = new Vector3(playerCollider.localScale.x, _startYScale, playerCollider.localScale.z);
        }
    }
}
