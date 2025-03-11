using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float rotationSpeed = 1f;

    void Update()
    {
        transform.Rotate(Vector3.right * (rotationSpeed * Time.unscaledDeltaTime));
    }
}
