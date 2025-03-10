using System.Collections;
using System.Collections.Generic;
using Managers;
using Platformer;
using Player.Animation;
using Player.Input;
using Sound;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        #region Variables
        [Header("References")]
        [SerializeField] private InputReader input;
        [SerializeField] private Transform orientation;
        private PlayerAnimation _playerAnimation;
        private Rigidbody _rb;
        private PlayerSound _ps;
        private List<Timer> _timers;
        private CountdownTimer _jumpTimer;
        private CountdownTimer _jumpCooldownTimer;

        [Header("Movement Speed")] 
        [SerializeField] private float walkSpeed;
        [SerializeField] private float rollSpeed;
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
        [SerializeField] private float jumpDuration;
        [SerializeField] private float jumpCooldown;
        [SerializeField] private float jumpMaxHeight;
        [SerializeField] private float gravityMultiplier;
        private float _jumpVelocity;
        private bool _doubleJump;
        private bool _isJumping;
        [SerializeField] private float coyoteTime = 0.2f;
        [SerializeField] private float jumpBufferTime = 0.2f;
        private float _jumpBufferTimer;
        private float _coyoteTimer;

        [Header("Ground Check")] 
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private float groundCheckDistance;
        public static bool Grounded;
        private RaycastHit _groundHit;
        private Vector3 _groundCheckOffset;

        [Header("Slope Handling")] 
        [SerializeField] private float maxSlopeAngle;
        [SerializeField] private float slopeCheckDistance;
        private RaycastHit _slopeHit;
        private bool _exitingSlope;
        
        public enum MovementState
        {
            Freeze,
            Swinging,
            Walking,
            Crouching,
            Rolling,
            Dashing,
            Air,
            Falling
        }
        
        [HideInInspector] public MovementState state;

        [HideInInspector] public bool freeze;
        [HideInInspector] public bool restricted;

        [HideInInspector] public bool swinging;
        [HideInInspector] public bool dashing;
        [HideInInspector] public bool crouching;
        #endregion
        
        #region Unity Built-in Methods
        private void Start()
        {
            _playerAnimation = GetComponent<PlayerAnimation>();
            _ps = GetComponent<PlayerSound>();
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
            _groundCheckOffset = groundCheckPoint.position + new Vector3(0f, groundCheckDistance, 0f);
            
            // turn gravity off while on slope
            _rb.useGravity = !OnSlope();
            HandleTimers(); 
            SpeedControl();
            CheckIfGrounded();
            
            HandleCoyoteTime();
            HandleJumpInput();

            StateHandler();

            // handle landing
            if (!Grounded)
                StartCoroutine(WaitForLanding());
            
            HandleAnimation();
        }
        
        private void FixedUpdate()
        {
            Jump();
            MovePlayer();
        }
        
        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (groundCheckPoint != null)
            {
                Gizmos.color = Grounded ? Color.green : Color.red;
                Gizmos.DrawWireSphere(_groundCheckOffset, 0.45f);
            }
        }
        #endif
        
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
        }
        #endregion
        
        #region Jump Handling
        private void HandleJump()
        {
            _jumpBufferTimer = jumpBufferTime;
            
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

            _jumpBufferTimer -= Time.deltaTime;
            
            // Start Jump
            if (_jumpBufferTimer > 0 && !_jumpCooldownTimer.IsRunning && !swinging)
            {
                if (!(_coyoteTimer > 0) && !_doubleJump) return;
                
                _jumpTimer.Start();
                
                if (PlayerAnimation.CurrentAnimation != "Attack")
                    _playerAnimation.ChangeAnimation("Jump");
                
                _ps?.PlayJumpSound();
                    
                if (AbilityManager.DoubleJumpEnabled)
                    _doubleJump = !_doubleJump;

                _jumpBufferTimer = 0;
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
            if (state is MovementState.Swinging)
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
                    _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
                }
                
                float launchPoint = 0.45f;
                
                if (_jumpTimer.Progress > launchPoint)
                {
                    _jumpVelocity = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(Physics.gravity.y));
                }
                else
                {
                    _jumpVelocity += (1.5f - _jumpTimer.Progress) * jumpForce * Time.fixedDeltaTime;
                }
            }
            // gravity
            else if (!Grounded && state != MovementState.Swinging || state != MovementState.Freeze || !OnSlope())
            {
                // limit fall speed
                if (_jumpVelocity > -25)
                    _jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }
            
            // applies the force
            _rb.velocity = new Vector3(_rb.velocity.x, _jumpVelocity, _rb.velocity.z);
        }
        
        private IEnumerator WaitForLanding()
        {
            yield return new WaitUntil(() => !Grounded);
            yield return new WaitUntil(() => Grounded);

            if (PlayerAnimation.CurrentAnimation != "Attack")
                _playerAnimation.ChangeAnimation("Landing");
            
            // prevents canceled double jump
            if (_jumpTimer.IsRunning) yield break;
            
            _jumpTimer.Stop();
            _jumpVelocity = 0;
            _exitingSlope = false;
        }
        #endregion

        #region General Handlers

        public void HandleAnimation()
        {
            if (PlayerAnimation.CurrentAnimation == "Attack" || PlayerAnimation.CurrentAnimation == "Jump" || PlayerAnimation.CurrentAnimation == "Fall" && !Grounded)
                return;
            
            if (Grounded && _inputDirection.x >= 0.1f || _inputDirection.y >= 0.1f || _inputDirection.x <= -0.1f || _inputDirection.y <= -0.1f)
            {
                if (!crouching)
                    _playerAnimation.ChangeAnimation("Run", 0.15f);
                else
                    _playerAnimation.ChangeAnimation("Roll");
            }
            // Idle will be at the bottom as default animation
            else if (Grounded && PlayerAnimation.CurrentAnimation != "Landing")
            {
                if (!crouching)
                    _playerAnimation.ChangeAnimation("Idle");
                else
                    _playerAnimation.ChangeAnimation("Crouch", 0.1f);
            }
        }
        
        private void CheckIfGrounded()
        {
            Grounded = Physics.OverlapSphere(_groundCheckOffset, 0.45f, whatIsGround).Length > 0;
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
            // Mode - Falling
            else if (_jumpVelocity < 0 && !swinging && !Grounded)
            {
                state = MovementState.Falling;
                
                if (_inputDirection != Vector2.zero)
                    _desiredMoveSpeed = walkSpeed * 1.1f;
                else
                    _desiredMoveSpeed = walkSpeed * 0.75f;
                
                if (PlayerAnimation.CurrentAnimation != "Attack")
                    _playerAnimation.ChangeAnimation("Fall");
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
            // Mode - Crouching
            else if (crouching && _inputDirection == Vector2.zero)
            {
                state = MovementState.Crouching;
            }
            // Mode - Rolling
            else if (crouching)
            {
                state = MovementState.Rolling;

                if (OnSlope() && _rb.velocity.y < 0.1f)
                    _desiredMoveSpeed = rollSpeed * 1.25f;
                else
                    _desiredMoveSpeed = rollSpeed;
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
                    _desiredMoveSpeed = walkSpeed * 1.1f;
                else
                    _desiredMoveSpeed = walkSpeed * 0.75f;
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
            Debug.DrawRay(_groundCheckOffset, Vector3.down * slopeCheckDistance, Color.cyan);
            
            if (Physics.Raycast(_groundCheckOffset, Vector3.down, out _slopeHit, slopeCheckDistance))
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
