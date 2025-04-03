using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMotion : MonoBehaviour
{
    private float _initialY;

    void Start()
    {
        _initialY = transform.position.y;
    }
    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        transform.Rotate(0f,0.1f,0f);
        transform.position = new Vector3(transform.position.x, 0.35f * Mathf.Sin(Time.time * 1f) + _initialY, transform.position.z);
    }
}
