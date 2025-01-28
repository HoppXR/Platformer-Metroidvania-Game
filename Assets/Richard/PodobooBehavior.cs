using System.Collections;
using UnityEngine;

public class PodobooBehavior : MonoBehaviour
{
    [Header("Podoboo Settings")]
    public float moveSpeed = 5f; // Speed of the upward and downward movement
    public float height = 5f;   // Height the Podoboo reaches
    public float cooldownTime = 3f; // Cooldown duration

    private Vector3 startPosition;
    private bool isCooldown = false;

    void Start()
    {

        startPosition = transform.position;
    }

    void Update()
    {
        if (!isCooldown)
        {
            StartCoroutine(PodobooCycle());
        }
    }

    private IEnumerator PodobooCycle()
    {
        isCooldown = true;
        Vector3 targetPosition = startPosition + Vector3.up * height;
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(90, 0, 0);
        targetPosition = startPosition;
        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(-90, 0, 0);
        yield return new WaitForSeconds(cooldownTime);

        isCooldown = false;
    }
}