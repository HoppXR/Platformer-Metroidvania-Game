using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader input;
    [SerializeField] private Transform orientation;
    private Vector3 velocity;
    private Vector2 _inputDir;
    private Vector2 _movementDir;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed;

    private void Start()
    {
        input.MoveEvent += GetInputDirection;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 acceleration = _movementDir * moveSpeed;
        velocity += acceleration * Time.deltaTime;
        Vector3 displacement = velocity * Time.deltaTime;
        transform.localPosition += displacement;
    }

    private void GetInputDirection(Vector2 dir)
    {
        _inputDir = dir;
        
        // taking camera orientation into account
        _movementDir = orientation.forward * _inputDir.y + orientation.right * _inputDir.x;
        _movementDir = Vector2.ClampMagnitude(_movementDir, 1f); // same as normalize function
    }
}
