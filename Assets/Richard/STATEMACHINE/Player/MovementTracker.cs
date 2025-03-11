using System.Collections;
using Player.Movement;
using UnityEngine;

public class ObjectTracker : MonoBehaviour
{
    public GameObject goTrackingObject;
    public GameObject goIndicator;
    public Vector3 v3AverageVelocity;
    public Vector3 v3AverageAcceleration;

    public float smoothingFactor = 0.1f;
    public float maxAcceleration = 10f;

    private Rigidbody rb;
    private Vector3 v3PrevVel;
    private Vector3 v3PrevPos;

    private PlayerMovement playerMovement;

    private void Start()
    {
        if (goTrackingObject != null)
        {
            rb = goTrackingObject.GetComponent<Rigidbody>();
            playerMovement = GetComponent<PlayerMovement>();
        }
        else
        {
            Debug.LogError("Tracking object is not assigned!");
        }
    }

    private void Update()
    {
        if (rb == null)
            return;

        if (!playerMovement.dashing)
        {
            Vector3 currentVelocity = rb.velocity;
            v3AverageVelocity = Vector3.Lerp(v3AverageVelocity, currentVelocity, smoothingFactor);
            Vector3 rawAcceleration = (currentVelocity - v3PrevVel) / Time.unscaledDeltaTime;
            rawAcceleration = Vector3.ClampMagnitude(rawAcceleration, maxAcceleration);
            v3AverageAcceleration = Vector3.Lerp(v3AverageAcceleration, rawAcceleration, smoothingFactor);
            v3PrevVel = currentVelocity;
        }
        
        GetProjectedPosition(0.5f);
        
        v3PrevPos = goTrackingObject.transform.position;
    }

    public Vector3 GetProjectedPosition(float fTime)
    {
        if (rb == null)
            return Vector3.zero;
        // Projected position: X(t) = X0 + v0 * t + 0.5 * a * t^2
        Vector3 projectedPosition = goTrackingObject.transform.position
                                    + (v3AverageVelocity * fTime)
                                    + (v3AverageAcceleration * (0.5f * fTime * fTime));

        if (goIndicator != null)
        {
            goIndicator.transform.position = projectedPosition;
        }
        return projectedPosition;
    }
}
