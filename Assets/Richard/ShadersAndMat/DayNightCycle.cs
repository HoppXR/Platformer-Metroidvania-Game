using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private GameObject sunLight;  // Sunlight Directional Light
    [SerializeField] private GameObject moonLight; // Moonlight Directional Light
    [SerializeField] private float rotationSpeed = 10f; // Speed of time progression
    [SerializeField, Range(0, 24)] private float timeOfDay = 0f; // Start time at 0 AM

    private void Update()
    {
        // Progress time
        timeOfDay += Time.deltaTime * (rotationSpeed / 24f);
        if (timeOfDay >= 24f) timeOfDay = 0f; // Reset after 24 hours

        // Calculate X rotation (0° to 360°)
        float xRotation = Mathf.Lerp(0f, 360f, timeOfDay / 24f);  
        transform.rotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Light activation based on rotation
        if ((xRotation >= 0f && xRotation < 90f) || (xRotation >= 270f && xRotation < 360f)) 
        {
            sunLight.SetActive(true);
            moonLight.SetActive(false);
        }
        else
        {
            sunLight.SetActive(false);
            moonLight.SetActive(true);
        }
    }
}