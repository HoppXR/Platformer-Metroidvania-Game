using UnityEngine;

namespace Platformer
{
    public class Sliding : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform playerObj;
        private Rigidbody _rb;
        private PlayerMovement _pm;

        [Header("Sliding")] 
        [SerializeField] private float maxSlideTime;
        [SerializeField] private float slideForce;
        private float _sliderTimer;

        [SerializeField] private float slideYScale;
        private float _startYScale;

        [Header("Input")]
        [SerializeField] private KeyCode slideKey;
        private float _horizontalInput;
        private float _verticalInput;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _pm = GetComponent<PlayerMovement>();

            _startYScale = playerObj.localScale.y;
        }

        private void Update()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(slideKey) && (_horizontalInput != 0 || _verticalInput != 0))
                StartSlide();
            
            if (Input.GetKeyUp(slideKey) && _pm.sliding)
                StopSlide();
        }

        private void FixedUpdate()
        {
            if (_pm.sliding)
                SlidingMovement();
        }

        private void StartSlide()
        {
            _pm.sliding = true;

            playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
            _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

            _sliderTimer = maxSlideTime;
        }

        private void SlidingMovement()
        {
            Vector3 inputDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;
            
            // sliding normal
            if (!_pm.OnSlope() || _rb.velocity.y > -0.1f)
            {
                _rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);
                
                _sliderTimer -= Time.deltaTime;
            }
            
            // sliding down a slope
            else
            {
                _rb.AddForce(_pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
            }
            
            if (_sliderTimer <= 0)
                StopSlide();
        }
        
        private void StopSlide()
        {
            _pm.sliding = false;
            
            playerObj.localScale = new Vector3(playerObj.localScale.x, _startYScale, playerObj.localScale.z);
        }
    }
}
