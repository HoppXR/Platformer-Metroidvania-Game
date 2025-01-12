using System.Collections;
using UnityEngine;

public class ObjectTracker : MonoBehaviour
{
    public GameObject goTrackingObject; // Object being tracked
    public GameObject goIndicator;     // Indicator for the projected position
    public Vector3 v3AverageVelocity;  // Current velocity
    public Vector3 v3AverageAcceleration; // Current acceleration

    private Rigidbody rb;             // Rigidbody of the tracking object
    private Vector3 v3PrevVel;        // Previous velocity
    private Vector3 v3PrevPos;        // Previous position

    private void Start()
    {
        if (goTrackingObject != null)
        {
            rb = goTrackingObject.GetComponent<Rigidbody>();
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

        // Calculate current velocity and acceleration
        Vector3 currentVelocity = rb.velocity;
        Vector3 acceleration = (currentVelocity - v3PrevVel) / Time.deltaTime;

        v3AverageVelocity = currentVelocity;
        v3AverageAcceleration = acceleration;

        // Project and update the indicator
        GetProjectedPosition(1f); // Project 1 second into the future

        // Update previous values
        v3PrevVel = currentVelocity;
        v3PrevPos = goTrackingObject.transform.position;
    }

    public Vector3 GetProjectedPosition(float fTime)
    {
        if (rb == null)
            return Vector3.zero;

        // Projected position: X(t) = X0 + v0 * t + 0.5 * a * t^2
        Vector3 projectedPosition = goTrackingObject.transform.position
                                    + (v3AverageVelocity * fTime)
                                    + (0.5f * v3AverageAcceleration * fTime * fTime);

        // Update the indicator's position
        if (goIndicator != null)
        {
            goIndicator.transform.position = projectedPosition;
        }

        return projectedPosition;
    }
}
