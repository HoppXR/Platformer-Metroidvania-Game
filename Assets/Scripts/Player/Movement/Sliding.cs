using UnityEngine;

namespace Platformer
{
    public class Sliding : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private InputReader input;
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform playerObj;
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

            _startYScale = playerObj.localScale.y;

            input.MoveEvent += HandleInput;
            input.CrouchEvent += StartSlide;
            input.CrouchCancelledEvent += StopSlide;
        }

        private void FixedUpdate()
        {
            if (_pm.sliding)
                SlidingMovement();
        }

        private void HandleInput(Vector2 dir)
        {
            _inputDirection = dir;
        }

        private void StartSlide()
        {
            if (_pm.swinging || !AbilityManager.SlideEnabled) return;
            
            _pm.sliding = true;

            if (playerObj != null)
                playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
         
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
            if (!_pm.sliding) return;
            
            _pm.sliding = false;
            
            if (playerObj != null)
                playerObj.localScale = new Vector3(playerObj.localScale.x, _startYScale, playerObj.localScale.z);
        }
    }
}
