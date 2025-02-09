using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Platformer
{
    public class PlayerMovement : MonoBehaviour
    {
        #region Variables
        [Header("References")]
        [SerializeField] private InputReader input;
        [SerializeField] private Transform orientation;
        private Rigidbody _rb;
        private PlayerSound _ps;
        private List<Timer> _timers;
        private CountdownTimer _jumpTimer;
        private CountdownTimer _jumpCooldownTimer;

        [Header("Movement")] 
        [SerializeField] private float walkSpeed;
        [SerializeField] private float slideSpeed;
        [SerializeField] private float swingSpeed;
        [SerializeField] private float dashSpeed;
        [SerializeField] private float dashSpeedChangeFactor;

        [Header("Movement Settings")] 
        [SerializeField] private float acceleration;
        [SerializeField] private float deceleration;
        
        private MovementState _lastState;
        private Vector3 _moveDirection;
        private Vector2 _inputDirection;
        private float _moveSpeed;
        private float _desiredMoveSpeed;
        private float _lastDesiredMoveSpeed;
        private bool _keepMomentum;

        [Header("Jumping")]
        [SerializeField] private float airMultiplier; // player speed in-air
        [SerializeField] private float jumpForce;
        [SerializeField] private float jumpBoostForce;
        [SerializeField] private float jumpDuration;
        [SerializeField] private float jumpCooldown;
        [SerializeField] private float jumpMaxHeight;
        [SerializeField] private float gravityMultiplier;
        private float _jumpVelocity;
        private bool _doubleJump;
        private bool _isJumping;
        
        [SerializeField] private float coyoteTime = 0.2f;
        [SerializeField] private float jumpBufferTime = 0.2f;
        private float jumpBufferTimer;
        private float _coyoteTimer;
        
        [Header("Ground Check")] 
        [SerializeField] private float playerHeight;
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private float groundCheckDistance;
        public static bool Grounded;
        private RaycastHit _groundHit;

        [Header("Slope Handling")] 
        [SerializeField] private float maxSlopeAngle;
        private RaycastHit _slopeHit;
        private bool _exitingSlope;
        
        public enum MovementState
        {
            Freeze,
            Swinging,
            Walking,
            Crouching,
            Sliding,
            Dashing,
            Air
        }
        
        [HideInInspector] public MovementState state;

        [HideInInspector] public bool freeze;
        [HideInInspector] public bool restricted;

        [HideInInspector] public bool swinging;
        [HideInInspector] public bool dashing;
        [HideInInspector] public bool sliding;
        #endregion
        
        #region Unity Built-in Methods
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
            
            _jumpTimer = new CountdownTimer(jumpDuration);
            _jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            _timers = new List<Timer>(2) { _jumpTimer, _jumpCooldownTimer };
            _jumpTimer.OnTimerStart += () => _jumpCooldownTimer.Start();

            input.MoveEvent += HandleMove;
            input.JumpEvent += HandleJump;
            input.JumpCancelledEvent += HandleCancelJump;
        }

        private void Update()
        {
            HandleCoyoteTime();
            HandleTimers(); 
            HandleJumpInput();
            SpeedControl();
            StateHandler();
            CheckIfGrounded();

            // handle landing
            if (!Grounded)
                StartCoroutine(WaitForLanding());
        }
        
        private void FixedUpdate()
        {
            Jump();
            MovePlayer();
        }
        #endregion

        #region Movement Handling
        private void HandleMove(Vector2 dir)
        {
            _inputDirection = dir;
        }
        
        private void MovePlayer()
        {
            if (state == MovementState.Dashing || swinging || restricted) return;
            
            // calculate movement direction
            _moveDirection = orientation.forward * _inputDirection.y + orientation.right * _inputDirection.x;

            if (_inputDirection != Vector2.zero)
            {
                var targetVelocity = _moveDirection.normalized * (_moveSpeed * 10f);
                
                // on slope
                if (OnSlope() && !_exitingSlope)
                {
                    // adds opposing force when changing direction on slope
                    if (Vector3.Dot(_rb.velocity.normalized, GetSlopeMoveDirection(_moveDirection)) < 0)
                    {
                        _rb.AddForce(-GetSlopeMoveDirection(_moveDirection) * (_moveSpeed * 9f * acceleration), ForceMode.Acceleration);
                    }
                    
                    _rb.AddForce(GetSlopeMoveDirection(_moveDirection) * (_moveSpeed * 20f * acceleration) - _rb.velocity, ForceMode.Acceleration);
                
                    if (_rb.velocity.y > 0)
                        _rb.AddForce(Vector3.down * 80f, ForceMode.Acceleration);
                }
                else switch (Grounded)
                {
                    // on ground
                    case true:
                        // adds opposing force when changing direction
                        if (Vector3.Dot(_rb.velocity.normalized, _moveDirection) < 0)
                        {
                            _rb.AddForce(-targetVelocity * (acceleration * 0.45f), ForceMode.Acceleration);
                        }
                        
                        _rb.AddForce(targetVelocity * acceleration, ForceMode.Acceleration);
                        break;
                    // in air
                    case false:
                        _rb.AddForce(_moveDirection.normalized * (_moveSpeed * 10f * airMultiplier), ForceMode.Acceleration);
                        break;
                }
            }
            else
            {
                // slows the player down when they release input
                _rb.AddForce(-_rb.velocity * (deceleration * 10f), ForceMode.Acceleration);
            }
            
            // turn gravity off while on slope
            _rb.useGravity = !OnSlope();
        }
        #endregion
        
        #region Jump Handling
        private void HandleJump()
        {
            jumpBufferTimer = jumpBufferTime;
            
            _isJumping = true;
        }

        private void HandleCancelJump()
        {
            _isJumping = false;
        }
        
        private void HandleJumpInput()
        {
            // Double Jump
            if (Grounded && !_isJumping)
            {
                _doubleJump = false;
            }

            jumpBufferTimer -= Time.deltaTime;
            
            // Start Jump
            if (jumpBufferTimer > 0 && !_jumpCooldownTimer.IsRunning && !swinging)
            {
                if (!(_coyoteTimer > 0) && !_doubleJump) return;
                
                _jumpTimer.Start();
                    
                _ps.PlayJumpSound();
                    
                if (AbilityManager.DoubleJumpEnabled)
                    _doubleJump = !_doubleJump;

                jumpBufferTimer = 0;
            }
            // Stop Jump
            else if (!_isJumping && _jumpTimer.IsRunning)
            {
                _jumpTimer.Stop();
                
                _coyoteTimer = 0;
            }
        }

        private void HandleCoyoteTime()
        {
            if (Grounded)
            {
                _coyoteTimer = coyoteTime;
            }
            else
            {
                _coyoteTimer -= Time.deltaTime;
            }
        }
        
        private void Jump()
        {
            if (state is MovementState.Dashing or MovementState.Swinging)
            {
                _jumpVelocity = _rb.velocity.y;
                return;
            }
            
            if (!_jumpTimer.IsRunning && Grounded)
            {
                _jumpTimer.Stop();
                return;
            }
            
            if (_jumpTimer.IsRunning)
            {
                _exitingSlope = true;

                if (_jumpTimer.Progress == 0)
                {
                    _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
                }
                
                float launchPoint = 0.9f;
                
                if (_jumpTimer.Progress > launchPoint)
                    _jumpVelocity = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(Physics.gravity.y));
                else
                    _jumpVelocity += (1 - _jumpTimer.Progress) * jumpForce * Time.fixedDeltaTime;
            }
            // gravity
            else if (!Grounded && state != MovementState.Swinging || state != MovementState.Freeze || !OnSlope())
            {
                // limit fall speed
                if (_jumpVelocity < -99) return;
                
                _jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }
            
            // applies the force
            _rb.velocity = new Vector3(_rb.velocity.x, _jumpVelocity, _rb.velocity.z);
        }
        
        private IEnumerator WaitForLanding()
        {
            yield return new WaitUntil(() => !Grounded);
            yield return new WaitUntil(() => Grounded);

            // prevents canceled double jump
            if (_jumpTimer.IsRunning) yield break;
            
            _jumpTimer.Stop();
            _jumpVelocity = 0;
            _exitingSlope = false;
        }
        #endregion

        #region General Handlers
        private void CheckIfGrounded()
        {
            Vector3 center = transform.position - new Vector3(0, (playerHeight - 1.2f) * 0.5f, 0);
            
            Grounded = Physics.SphereCast(center, 0.5f,Vector3.down, out _groundHit, groundCheckDistance, whatIsGround);

            if (Grounded && !OnSlope() && Mathf.Abs(_rb.velocity.y) > 0.1f)
            {
                Grounded = false;
            }
            
            Debug.DrawRay(center, Vector3.down * groundCheckDistance, Grounded ? Color.green : Color.red);
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
                    _desiredMoveSpeed = walkSpeed;
            }
            // Mode - Walking
            else if (Grounded)
            {
                state = MovementState.Walking;
                _desiredMoveSpeed = walkSpeed;
            }
            // Mode - Air
            else
            {
                state = MovementState.Air;
                
                if (_inputDirection != Vector2.zero)
                    _desiredMoveSpeed = walkSpeed;
                else
                    _desiredMoveSpeed = walkSpeed * 0.6f;
            }
            
            bool desiredMoveSpeedHasChanged = !Mathf.Approximately(_desiredMoveSpeed, _lastDesiredMoveSpeed);
            if (_lastState is MovementState.Dashing) _keepMomentum = true;

            if (desiredMoveSpeedHasChanged)
            {
                if (_keepMomentum)
                    StartCoroutine(SmoothlyLerpMoveSpeed());
                else
                    _moveSpeed = _desiredMoveSpeed;
            }
            
            _lastDesiredMoveSpeed = _desiredMoveSpeed;
            _lastState = state;
        }

        private void SpeedControl()
        {
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
            _keepMomentum = true;
        }
        
        public bool OnSlope()
        {
            Vector3 center = transform.position - new Vector3(0, (playerHeight - 1.2f) * 0.5f, 0);

            if (Physics.SphereCast(center, 0.5f, Vector3.down, out _slopeHit, groundCheckDistance + 0.25f))
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
        
        private void HandleTimers()
        {
            foreach (var timer in _timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }
        #endregion
    }
}
