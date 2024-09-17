using System;
using UnityEngine;

namespace Platformer
{
    public class Grappling : MonoBehaviour
    {
        [Header("References")] 
        private PlayerMovement _pm;
        [SerializeField] private Transform cam;
        [SerializeField] private Transform gunTip;
        [SerializeField] private LayerMask whatIsGrappleable;
        [SerializeField] private LineRenderer lr;

        [Header("Grappling")] 
        [SerializeField] private float maxGrappleDistance;
        [SerializeField] private float grappleDelayTime;
        [SerializeField] private float overshootYAxis;

        private Vector3 _grapplePoint;

        [Header("Cooldown")] 
        [SerializeField] private float grapplingCd;
        private float _grapplingCdTimer;
        
        [Header("Prediction")]
        private RaycastHit _predictionHit;
        [SerializeField] private float predictionSphereCastRadius;
        [SerializeField] private Transform predictionPoint;

        [Header("Input")] 
        [SerializeField] private KeyCode grappleKey;

        private bool _grappling;

        private void Start()
        {
            _pm = GetComponent<PlayerMovement>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(grappleKey)) StartGrapple();

            CheckForSwingPoints();
            
            if (_grapplingCdTimer > 0)
                _grapplingCdTimer -= Time.deltaTime;
        }

        private void LateUpdate()
        {
            if (_grappling)
                lr.SetPosition(0, gunTip.position);
        }

        private void StartGrapple()
        {
            if (_grapplingCdTimer > 0) return;

            if (_predictionHit.point == Vector3.zero) return;
            
            // deactivate active swinging
            GetComponent<Swinging>().StopSwing();
            
            _grappling = true;

            _pm.freeze = true;
                
            _grapplePoint = _predictionHit.point;
            
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);

            lr.SetPosition(1, _grapplePoint);
        }

        private void ExecuteGrapple()
        {
            _pm.freeze = false;
            
            Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
            
            float grapplePointRelativeYPos = _grapplePoint.y - lowestPoint.y;
            float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;
            
            if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;
            
            _pm.JumpToPosition(_grapplePoint, highestPointOnArc);
            
            Invoke(nameof(StopGrapple), 1f);
        }

        public void StopGrapple()
        {
            _pm.freeze = false;
            
            _grappling = false;

            _grapplingCdTimer = grapplingCd;
        }

        private void CheckForSwingPoints()
        {
            if (_grappling) return;
            
            RaycastHit sphereCastHit;
            Physics.SphereCast(cam.position, predictionSphereCastRadius, cam.forward, out sphereCastHit, maxGrappleDistance, whatIsGrappleable);
            
            RaycastHit raycastHit;
            Physics.Raycast(cam.position, cam.forward, out raycastHit, maxGrappleDistance, whatIsGrappleable);

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
