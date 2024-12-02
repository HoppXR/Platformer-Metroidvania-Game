using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;

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
        [SerializeField] private float airMultiplier; // player speed in-air
        [SerializeField] private float jumpForce;
        [SerializeField] private float jumpBoostForce;
        [SerializeField] private float jumpDuration;
        [SerializeField] private float jumpCooldown;
        [SerializeField] private float jumpMaxHeight;
        [SerializeField] private float gravityMultiplier;
        [SerializeField] private int maxNumberOfJumps;
        private float _jumpVelocity;
        private bool _doubleJump;
        
        [SerializeField] private float coyoteTime = 0.2f;
        private float _coyoteTimer;
        
        [SerializeField] private float jumpBufferTime = 0.2f;
        private float jumpBufferTimer;

        [Header("Crouching")]
        [SerializeField] private float crouchSpeed;
        [SerializeField] private float crouchYScale;
        private float _startYScale;
        
        [Header("Keybinds")] 
        [SerializeField] private KeyCode jumpKey;
        [SerializeField] private KeyCode crouchKey;
        
        [Header("Ground Check")] 
        [SerializeField] private float playerHeight;
        [SerializeField] private LayerMask whatIsGround;
        private float _groundCheckDistance = 0.8f;
        public static bool Grounded;
        private RaycastHit _groundHit;

        [Header("Slope Handling")] 
        [SerializeField] private float maxSlopeAngle;
        private RaycastHit _slopeHit;
        private bool _exitingSlope;
        
        [Header("Audio")]
        [SerializeField] private CurrentTerrain currentTerrain;
        private enum CurrentTerrain { Grass, Stone, Water, Pipe }
        private EventInstance playerFootsteps;

        [SerializeField] private EventReference jumpSound;
        
        [Header("")]
        [SerializeField] private Transform orientation;

        private float _horizontalInput;
        private float _verticalInput;

        private Vector3 _moveDirection;

        public MovementState state;
        
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

        [HideInInspector] public bool freeze;
        [HideInInspector] public bool restricted;

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
            _jumpTimer.OnTimerStart += () => _jumpCooldownTimer.Start();

            playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.Instance.PlayerFootsteps);
            
            _startYScale = transform.localScale.y;
        }

        private void Update()
        {
            HandleCoyoteTime();
            HandleTimers();
            MyInput();
            SpeedControl();
            StateHandler();
            CheckIfGrounded();
            DetermineTerrain();
            SelectAndPlayFootstep();
            
            // handle drag
            if (state is MovementState.Walking or MovementState.Crouching)
                _rb.drag = groundDrag;
            else
                _rb.drag = 0;

            // handle landing
            if (!Grounded)
                StartCoroutine(WaitForLanding());
        }

        private void CheckIfGrounded()
        {
            Vector3 center = transform.position - new Vector3(0, (playerHeight - 1.2f) * 0.5f, 0);
            
            Grounded = Physics.SphereCast(center, 0.5f,Vector3.down, out _groundHit, _groundCheckDistance, whatIsGround);
            
            Debug.DrawRay(center, Vector3.down * _groundCheckDistance, Grounded ? Color.green : Color.red);
        }

        private void FixedUpdate()
        {
            HandleJump();
            MovePlayer();
        }

        private void DetermineTerrain()
        {
            RaycastHit[] hit;

            hit = Physics.RaycastAll(transform.position, Vector3.down, 10f);
            
            foreach (RaycastHit rayhit in hit)
            {
                if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Grass"))
                {
                    currentTerrain = CurrentTerrain.Grass;
                    break;
                }
                else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Stone"))
                {
                    currentTerrain = CurrentTerrain.Stone;
                    break;
                }
                else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Water"))
                {
                    currentTerrain = CurrentTerrain.Water;
                }
                else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Pipe"))
                {
                    currentTerrain = CurrentTerrain.Pipe;
                }
            }
        }

        public void SelectAndPlayFootstep()
        {
            switch (currentTerrain)
            {
                case CurrentTerrain.Grass:
                    PlayFootstep(0);
                    break;
                
                case CurrentTerrain.Stone:
                    PlayFootstep(1);
                    break;
                
                case CurrentTerrain.Water:
                    PlayFootstep(2);
                    break;
                
                case CurrentTerrain.Pipe:
                    PlayFootstep(3);
                    break;
                
                default:
                    PlayFootstep(0);
                    break;
            }
        }

        private void PlayFootstep(int terrain)
        {
            playerFootsteps.setParameterByName("Terrain", terrain);
            playerFootsteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            
            if (_verticalInput != 0 && Grounded || _horizontalInput != 0 && Grounded)
            {
                PLAYBACK_STATE playbackState;
                playerFootsteps.getPlaybackState(out playbackState);
                
                if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
                {
                    playerFootsteps.start();
                }
            }
            else
            {
                playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }

        private void MyInput()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");

            // Double Jump
            if (Grounded && !Input.GetKey(jumpKey))
            {
                _doubleJump = false;
            }
            
            // Jump buffer
            if (Input.GetKeyDown(jumpKey))
            {
                jumpBufferTimer = jumpBufferTime;
            }
            else
            {
                jumpBufferTimer -= Time.deltaTime;
            }
            
            // Start Jump
            if (jumpBufferTimer > 0 && !_jumpCooldownTimer.IsRunning)
            {
                if (_coyoteTimer > 0 || _doubleJump)
                {
                    _jumpTimer.Start();
                    
                    AudioManager.instance.PlayOneShot(jumpSound, transform.position);
                    
                    _doubleJump = !_doubleJump;

                    jumpBufferTimer = 0;
                }
            }
            // Stop Jump
            else if (Input.GetKeyUp(jumpKey) && _jumpTimer.IsRunning)
            {
                _jumpTimer.Stop();
                
                _coyoteTimer = 0;
            }
            // start crouch
            if (Input.GetKeyDown(crouchKey))
            {
                transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
                _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            }
            // stop crouch
            if (Input.GetKeyUp(crouchKey))
            {
                transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
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
        
        private void HandleJump()
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
                
                float launchPoint = 0.9f;
                
                if (_jumpTimer.Progress > launchPoint)
                    _jumpVelocity = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(Physics.gravity.y));
                else
                    _jumpVelocity += (1 - _jumpTimer.Progress) * jumpForce * Time.fixedDeltaTime;
            }
            else if (!Grounded && state != MovementState.Swinging || state != MovementState.Freeze || !OnSlope())
            {
                _jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }
            else
                _jumpVelocity = 0;
            
            // applies the force
            _rb.velocity = new Vector3(_rb.velocity.x, _jumpVelocity, _rb.velocity.z);
        }
        
        private IEnumerator WaitForLanding()
        {
            yield return new WaitUntil(() => !Grounded);
            yield return new WaitUntil(() => Grounded);
            
            _exitingSlope = false;
            _jumpVelocity = 0;
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
            // Mode - Crouching
            else if (Input.GetKey(crouchKey))
            {
                state = MovementState.Crouching;
                _desiredMoveSpeed = crouchSpeed;
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
                _desiredMoveSpeed = walkSpeed;
            }
            
            bool desiredMoveSpeedHasChanged = !Mathf.Approximately(_desiredMoveSpeed, _lastDesiredMoveSpeed);
            if (_lastState == MovementState.Dashing) _keepMomentum = true;

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
        
        private void MovePlayer()
        {
            if (state == MovementState.Dashing || swinging || restricted) return;
            
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
            else if (Grounded)
                _rb.AddForce(_moveDirection.normalized * (_moveSpeed * 10f), ForceMode.Force);
            
            // in air
            else if (!Grounded)
                _rb.AddForce(_moveDirection.normalized * (_moveSpeed * 10f * airMultiplier), ForceMode.Force);
            
            // turn gravity off while on slope
            _rb.useGravity = !OnSlope();
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

            if (Physics.SphereCast(center, 0.5f, Vector3.down, out _slopeHit, _groundCheckDistance))
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
    }
}
