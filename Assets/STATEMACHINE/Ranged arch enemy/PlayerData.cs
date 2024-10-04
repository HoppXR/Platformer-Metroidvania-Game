using UnityEngine;

public class PlayerMovementData : MonoBehaviour
{
    public Vector3 center { get; private set; }
    public Vector3 velocity { get; private set; }

    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
        center = Vector3.zero;
    }

    void Update()
    {
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
        
        center = new Vector3(0, 1f, 0);
    }
}