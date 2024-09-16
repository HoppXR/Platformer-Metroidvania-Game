using System;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    private float _initialY;

    private void Start()
    {
        _initialY = transform.position.y;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // add collectable logic
            
            Destroy(gameObject);
        }
    }

    private void HandleMovement()
    {
        transform.Rotate(0f,1f,0f);
        transform.position = new Vector3(transform.position.x, 0.35f * Mathf.Sin(Time.time * 1f) + _initialY, transform.position.z);
    }
}
