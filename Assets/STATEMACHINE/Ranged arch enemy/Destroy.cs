using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    // Time in seconds before the object is destroyed
    [SerializeField] private float destroyTime = 3f;

    void Start()
    {
        // Destroy the GameObject after the specified time
        Destroy(gameObject, destroyTime);
    }
}