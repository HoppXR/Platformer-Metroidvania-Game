using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float speed = 3.0f; // Movement speed of the player
    public Vector3 lastSpeed = Vector3.zero; // The last calculated speed of the player

    void Update()
    {
        // Get input for movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Create a movement vector based on input
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        // If there's input, update position and last speed
        if (moveDirection.magnitude > 0.1f)
        {
            lastSpeed = moveDirection * speed; // Update last speed based on input
            transform.position += lastSpeed * Time.deltaTime; // Move the player
            transform.forward = lastSpeed; // Face the direction of movement
        }
        else
        {
            lastSpeed = Vector3.zero; // Stop if there's no input
        }
    }
}