using FMODUnity;
using UnityEngine;

namespace Platformer
{
    public class Swinging : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private InputReader input;
        [SerializeField] private LineRenderer lr;
        [SerializeField] private Transform gunTip, cam, player;
        [SerializeField] private LayerMask whatIsGrappleable;
        private PlayerMovement _pm;
        private Rigidbody _rb;
        
        [Header("Swinging")] 
        private float _maxSwingDistance = 25f;
        private Vector3 _swingPoint;
        private SpringJoint _joint;
        private Vector2 _inputDirection;

        [Header("OdmGear")] 
        [SerializeField] private Transform orientation;
        [SerializeField] private float horizontalThrustForce;
        [SerializeField] private float forwardThrustForce;
        [SerializeField] private float extendCableSpeed;
        
        [Header("Prediction")]
        private RaycastHit _predictionHit;
        [SerializeField] private float predictionSphereCastRadius;
        [SerializeField] private Transform predictionPoint;
        
        [Header("Sounds")]
        [SerializeField] private EventReference swingSound;

        private void Start()
        {
            _pm = GetComponent<PlayerMovement>();
            _rb = GetComponent<Rigidbody>();
            
            input.SwingEvent += StartSwing;
            input.SwingCancelledEvent += StopSwing;
            
            input.SwingShortenEvent += ShortenCable;
            input.SwingExtendEvent += ExtendCable;
            
            input.MoveEvent += HandleMove;
        }

        private void Update()
        {
            CheckForSwingPoints();
            
            if (_joint != null) OdmGearMovement();
        }

        private void LateUpdate()
        {
            DrawRope();
        }

        private void StartSwing()
        {
            // return if predictionHit not found or if ability is not enabled
            if (_predictionHit.point == Vector3.zero || !AbilityManager.SwingEnabled) return;
            
            _pm.swinging = true;
            
            AudioManager.instance.PlayOneShot(swingSound, transform.position);

            _swingPoint = _predictionHit.point;
            _joint = player.gameObject.AddComponent<SpringJoint>();
            _joint.autoConfigureConnectedAnchor = false;
            _joint.connectedAnchor = _swingPoint;
                
            float distanceFromPoint = Vector3.Distance(player.position, _swingPoint);
                
            // the distance grapple will try to keep from grapple point
            _joint.maxDistance = distanceFromPoint * 0.8f;
            _joint.minDistance = distanceFromPoint * 0.25f;
                
            // customize values as you like
            _joint.spring = 4.5f;
            _joint.damper = 7f;
            _joint.massScale = 4.5f;

            lr.positionCount = 2;
            _currentGrapplePosition = gunTip.position;
        }
        
        public void StopSwing()
        {
            _pm.swinging = false;
            
            lr.positionCount = 0;
            Destroy(_joint);
        }

        private Vector3 _currentGrapplePosition;
        private void DrawRope()
        {
            // if not swinging, don't draw rope
            if (!_joint) return;
            
            _currentGrapplePosition = Vector3.Lerp(_currentGrapplePosition, _swingPoint, Time.deltaTime * 8f);

            lr.SetPosition(0, gunTip.position);
            lr.SetPosition(1, _currentGrapplePosition);
        }

        private void HandleMove(Vector2 dir)
        {
            _inputDirection = dir;
        }

        private void OdmGearMovement()
        {
            // right
            if (_inputDirection.x > 0) _rb.AddForce(orientation.right * (horizontalThrustForce * Time.deltaTime));
            // left
            if (_inputDirection.x < 0) _rb.AddForce(-orientation.right * (horizontalThrustForce * Time.deltaTime));
            // forward
            if (_inputDirection.y > 0) _rb.AddForce(orientation.forward * (forwardThrustForce * Time.deltaTime));
        }

        private void ShortenCable()
        {
            if (!_pm.swinging) return;
            
            Vector3 directionToPoint = _swingPoint - transform.position;
            _rb.AddForce(directionToPoint.normalized * (forwardThrustForce * Time.deltaTime));
                
            float distancefromPoint = Vector3.Distance(transform.position, _swingPoint);
                
            _joint.maxDistance = distancefromPoint * 0.8f;
            _joint.minDistance = distancefromPoint * 0.25f;
        }

        private void ExtendCable()
        {
            if (!_pm.swinging) return;
            
            float extendedDistanceFromPoint = Vector3.Distance(transform.position, _swingPoint) + extendCableSpeed;
                
            _joint.maxDistance = extendedDistanceFromPoint * 0.8f;
            _joint.minDistance = extendedDistanceFromPoint * 0.25f;
        }

        private void CheckForSwingPoints()
        {
            if (_joint != null || !AbilityManager.SwingEnabled) return;
            
            RaycastHit sphereCastHit;
            Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out sphereCastHit, _maxSwingDistance, whatIsGrappleable);
            
            RaycastHit raycastHit;
            Physics.Raycast(cam.position, cam.forward, out raycastHit, _maxSwingDistance, whatIsGrappleable);

            Vector3 realHitPoint;
            
            // Option 1 - Direct Hit
            if (raycastHit.point != Vector3.zero)
                realHitPoint = raycastHit.point;
            
            // Option 2 - Indirect (predicted) Hit
            else if (sphereCastHit.point != Vector3.zero)
                realHitPoint = sphereCastHit.point;
            
            // Option 3 - Miss
            else
                realHitPoint = Vector3.zero;
            
            // realHitPoint found
            if (realHitPoint != Vector3.zero)
            {
                predictionPoint.gameObject.SetActive(true);
                predictionPoint.position = realHitPoint;
            }
            // realHitPoint not found
            else
            {
                predictionPoint.gameObject.SetActive(false);
            }
            
            _predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
        }
    }
}
