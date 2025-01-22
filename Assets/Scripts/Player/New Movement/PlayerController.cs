using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader input;

    private Vector2 movementDir;

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
        Vector3 velocity = new Vector3(movementDir.x, 0, movementDir.y);
        Vector3 displacement = velocity * Time.deltaTime;
        transform.localPosition += displacement;
    }

    private void GetInputDirection(Vector2 dir)
    {
        movementDir = dir;
        movementDir = Vector2.ClampMagnitude(movementDir, 1f); // same as normalize function
    }
}
