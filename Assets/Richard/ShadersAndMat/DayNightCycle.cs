using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private GameObject sunLight; 
    [SerializeField] private GameObject moonLight; 
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField, Range(0, 24)] private float timeOfDay = 0f;

    private void Update()
    {
       
        timeOfDay += Time.unscaledDeltaTime * (rotationSpeed / 24f);
        if (timeOfDay >= 24f) timeOfDay = 0f;
        
        float xRotation = Mathf.Lerp(0f, 360f, timeOfDay / 24f);  
        transform.rotation = Quaternion.Euler(xRotation, 0f, 0f);
        
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