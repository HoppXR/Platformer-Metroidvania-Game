using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("References")]
        private Rigidbody _rb;
        private List<Timer> _timers;
        private CountdownTimer _jumpTimer;
        private CountdownTimer _jumpCooldownTimer;
        
        [Header("Movement")] 
        private float _moveSpeed;
        [SerializeField] private float walkSpeed;
        [SerializeField] private float sprintSpeed;
        [SerializeField] private float slideSpeed;
        [SerializeField] private float swingSpeed;

        [SerializeField] private float dashSpeed;
        [SerializeField] private float dashSpeedChangeFactor;

        private float _desiredMoveSpeed;
        private float _lastDesiredMoveSpeed;
        private MovementState _lastState;
        private bool _keepMomentum;
        
        [SerializeField] private float groundDrag;

        [Header("Jumping")]
        [SerializeField] private float jumpForce;
        [SerializeField] private float jumpBoostForce;
        [SerializeField] private float jumpDuration;
        [SerializeField] private float jumpCooldown;
        [SerializeField] private float jumpMaxHeight;
        [SerializeField] private float airMultiplier; // player speed in-air
        [SerializeField] private float gravityMultiplier;
        [SerializeField] private int maxNumberOfJumps;
        private int _numberOfJumps;
        private float _jumpVelocity;

        [Header("Crouching")]
        [SerializeField] private float crouchSpeed;
        [SerializeField] private float crouchYScale;
        private float _startYScale;
        private bool _crouching;
        
        [Header("Keybinds")] 
        [SerializeField] private KeyCode jumpKey;
        [SerializeField] private KeyCode sprintKey;
        [SerializeField] private KeyCode crouchKey;
        
        [Header("Ground Check")] 
        [SerializeField] private float playerHeight;
        [SerializeField] private LayerMask whatIsGround;
        private bool _grounded;

        [Header("Slope Handling")] 
        [SerializeField] private float maxSlopeAngle;
        private RaycastHit _slopeHit;
        private bool _exitingSlope;
        
        [SerializeField] private Transform orientation;

        private float _horizontalInput;
        private float _verticalInput;

        private Vector3 _moveDirection;

        public MovementState state;
        
        public enum MovementState
        {
            Freeze,
            Unlimited,
            Swinging,
            Walking,
            Sprinting,
            Crouching,
            Sliding,
            Dashing,
            Air
        }

        [HideInInspector] public bool freeze;
        [HideInInspector] public bool unlimited;
        [HideInInspector] public bool restricted;
        
        [HideInInspector] public bool activeGrapple;
        [HideInInspector] public bool swinging;
        [HideInInspector] public bool dashing;
        [HideInInspector] public bool sliding;
        
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
            
            _jumpTimer = new CountdownTimer(jumpDuration);
            _jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            _timers = new List<Timer>(2) { _jumpTimer, _jumpCooldownTimer };

            _jumpTimer.OnTimerStart += () => _jumpCooldownTimer.Start();;
            
            _startYScale = transform.localScale.y;
        }

        private void Update()
        {
            // double jump check
            if (_numberOfJumps != 0) StartCoroutine(WaitForLanding());
            
            // ground check
            _grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
            
            MyInput();
            SpeedControl();
            StateHandler();
            HandleTimers();
            
            // handle drag
            if (state is MovementState.Walking or MovementState.Sprinting or MovementState.Crouching && !activeGrapple)
                _rb.drag = groundDrag;
            else
                _rb.drag = 0;
        }

        private void FixedUpdate()
        {
            MovePlayer();
            HandleJump();
        }

        private void MyInput()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");
            
            // when to jump
            if (Input.GetKeyDown(jumpKey) && !_jumpTimer.IsRunning && !_jumpCooldownTimer.IsRunning)
            {
                _jumpTimer.Start();
                _numberOfJumps++;
            }
            else if (Input.GetKeyUp(jumpKey) && _jumpTimer.IsRunning)
            {
                _jumpTimer.Stop();
            }
            
            // start crouch
            if (Input.GetKeyDown(crouchKey))
            {
                _crouching = true;
                
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            }
            
            // stop crouch
            if (Input.GetKeyUp(crouchKey))
            {
                transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);

                _crouching = false;
            }
        }

        private void StateHandler()
        {
            // Mode - Freeze
            if (freeze)
            {
                state = MovementState.Freeze;
                _desiredMoveSpeed = 0;
                _rb.velocity = Vector3.zero;
            }
            
            // Mode - Unlimited
            else if (unlimited)
            {
                state = MovementState.Unlimited;
                _desiredMoveSpeed = 999f;
                return;
            }
            
            // Mode - Swinging
            else if (swinging)
            {
                state = MovementState.Swinging;
                _desiredMoveSpeed = swingSpeed;
            }
            
            // Mode - Dashing
            else if (dashing)
            {
                state = MovementState.Dashing;
                _desiredMoveSpeed = dashSpeed;
                _speedChangeFactor = dashSpeedChangeFactor;
            }
            
            // Mode - Sliding
            else if (sliding)
            {
                state = MovementState.Sliding;

                if (OnSlope() && _rb.velocity.y < 0.1f)
                    _desiredMoveSpeed = slideSpeed;
                
                else
                    _desiredMoveSpeed = sprintSpeed;
            }
            
            // Mode - Crouching
            else if (Input.GetKey(crouchKey))
            {
                state = MovementState.Crouching;
                _desiredMoveSpeed = crouchSpeed;
            }
            
            // Mode - Sprinting
            else if (_grounded && Input.GetKey(sprintKey))
            {
                state = MovementState.Sprinting;
                _desiredMoveSpeed = sprintSpeed;
            }
            
            // Mode - Walking
            else if (_grounded)
            {
                state = MovementState.Walking;
                _desiredMoveSpeed = walkSpeed;
            }
            
            // Mode - Air
            else
            {
                state = MovementState.Air;

                if (_desiredMoveSpeed < sprintSpeed)
                    _desiredMoveSpeed = walkSpeed;
                else
                    _desiredMoveSpeed = sprintSpeed;
            }

            bool desiredMoveSpeedHasChanged = !Mathf.Approximately(_desiredMoveSpeed, _lastDesiredMoveSpeed);
            if (_lastState == MovementState.Dashing) _keepMomentum = true;

            if (desiredMoveSpeedHasChanged)
            {
                if (_keepMomentum)
                {
                    StartCoroutine(SmoothlyLerpMoveSpeed());
                }
                else
                {
                    _moveSpeed = _desiredMoveSpeed;
                }
            }
            
            _lastDesiredMoveSpeed = _desiredMoveSpeed;
            _lastState = state;
        }

        private float _speedChangeFactor;
        private IEnumerator SmoothlyLerpMoveSpeed()
        {
            // smoothly lerp movementSpeed to desired value
            float time = 0;
            float difference = Mathf.Abs(_desiredMoveSpeed - _moveSpeed);
            float startValue = _moveSpeed;

            float boostFactor = _speedChangeFactor;

            while (time < difference)
            {
                _moveSpeed = Mathf.Lerp(startValue, _desiredMoveSpeed, time / difference);
                
                time += Time.deltaTime * boostFactor;
                
                yield return null;
            }

            _moveSpeed = _desiredMoveSpeed;
            _speedChangeFactor = 1f;
            _keepMomentum = false;
        }
        
        private void MovePlayer()
        {
            if (state == MovementState.Dashing || activeGrapple || swinging || restricted) return;
            
            // calculate movement direction
            _moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;
            
            // on slope
            if (OnSlope() && !_exitingSlope)
            {
                _rb.AddForce(GetSlopeMoveDirection(_moveDirection) * (_moveSpeed * 20f), ForceMode.Force);
                
                if (_rb.velocity.y > 0)
                    _rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }    
            
            // on ground
            else if (_grounded)
                _rb.AddForce(_moveDirection.normalized * (_moveSpeed * 10f), ForceMode.Force);
            
            // in air
            else if (!_grounded)
                _rb.AddForce(_moveDirection.normalized * (_moveSpeed * 10f * airMultiplier), ForceMode.Force);
            
            // turn gravity off while on slope
            _rb.useGravity = !OnSlope();
        }

        private void HandleJump()
        {
            if (state == MovementState.Dashing || state == MovementState.Swinging || activeGrapple || _numberOfJumps >= maxNumberOfJumps)
            {
                _jumpVelocity = _rb.velocity.y;
                return;
            }
            
            if (!_jumpTimer.IsRunning && _grounded)
            {
                _jumpTimer.Stop();
                return;
            }
            
            _exitingSlope = true;
            
            if (_jumpTimer.IsRunning)
            {
                float launchPoint = 0.9f;
                
                if (_jumpTimer.Progress > launchPoint)
                    _jumpVelocity = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(Physics.gravity.y));
                else
                    _jumpVelocity += (1 - _jumpTimer.Progress) * jumpForce * Time.fixedDeltaTime;
            }
            else if (!_grounded && state != MovementState.Swinging || state != MovementState.Freeze)
            {
                _jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }
            else
                _jumpVelocity = 0;
            
            // applies the force
            _rb.velocity = new Vector3(_rb.velocity.x, _jumpVelocity, _rb.velocity.z);
        }

        private void SpeedControl()
        {
            if (activeGrapple) return;
            
            // limiting speed on slope
            if (OnSlope() && !_exitingSlope)
            {
                if (_rb.velocity.magnitude > _desiredMoveSpeed)
                    _rb.velocity = _rb.velocity.normalized * _desiredMoveSpeed;
            }

            // limiting speed on ground or in air
            else
            {
                Vector3 flatVel = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            
                // limit velocity if needed
                if (flatVel.magnitude > _desiredMoveSpeed)
                {
                    Vector3 limitedVel = flatVel.normalized * _desiredMoveSpeed;
                    _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
                }
            }
        }

        private IEnumerator WaitForLanding()
        {
            yield return new WaitUntil(() => !_grounded);
            yield return new WaitUntil(() => _grounded);

            _numberOfJumps = 0;
        }

        private bool _enableMovementOnNextTouch;
        public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
        {
            activeGrapple = true;
            
            _velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
            Invoke(nameof(SetVelocity), 0.1f);
            
            Invoke(nameof(ResetRestrictions), 3f);
        }

        private Vector3 _velocityToSet;
        private void SetVelocity()
        {
            _enableMovementOnNextTouch = true;
            
            _rb.velocity = _velocityToSet;
        }

        public void ResetRestrictions()
        {
            activeGrapple = false;
        }
        
        private void OnCollisionEnter(Collision collision)
        {
            if (_enableMovementOnNextTouch)
            {
                _enableMovementOnNextTouch = false;
                ResetRestrictions();
            }
        }

        public bool OnSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, playerHeight * 0.5f + 0.3f))
            {
                float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }

            return false;
        }

        public Vector3 GetSlopeMoveDirection(Vector3 direction)
        {
            return Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
        }

        public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
        {
            float gravity = Physics.gravity.y;
            float displacementY = endPoint.y - startPoint.y;
            Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);
            
            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
            Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));
            
            return velocityXZ + velocityY;
        }

        private void HandleTimers()
        {
            foreach (var timer in _timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }
    }
}
