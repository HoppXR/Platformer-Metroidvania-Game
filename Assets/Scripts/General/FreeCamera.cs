using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float lookSpeed = 2f;
    public float scrollSensitivity = 2f; // How fast the scroll affects speed
    public float minSpeed = 1f;
    public float maxSpeed = 50f;

    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Adjust movement speed with scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            moveSpeed += scroll * scrollSensitivity;
            moveSpeed = Mathf.Clamp(moveSpeed, minSpeed, maxSpeed);
        }

        // Mouse look
        yaw += lookSpeed * Input.GetAxis("Mouse X");
        pitch -= lookSpeed * Input.GetAxis("Mouse Y");
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0f);

        // Movement
        float x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float y = 0;

        if (Input.GetKey(KeyCode.E)) y = moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q)) y = -moveSpeed * Time.deltaTime;

        transform.Translate(new Vector3(x, y, z));
    }
}