using System.Collections;
using UnityEngine;

public class PodobooBehavior : MonoBehaviour
{
    [Header("Podoboo Settings")]
    public float moveSpeed = 5f;     // Speed multiplier for movement
    public float height = 5f;       // Height the Podoboo reaches
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
        yield return StartCoroutine(MoveWithEase(startPosition, startPosition + Vector3.up * height, EaseOutQuint));
        Quaternion rotation = Quaternion.Euler(90, 0, 0);
        yield return StartCoroutine(MoveWithEase(startPosition + Vector3.up * height, startPosition, EaseInCubic));
        rotation = Quaternion.Euler(-90, 0, 0);
        transform.rotation = rotation;
        yield return new WaitForSeconds(cooldownTime);
        isCooldown = false;
    }

    private IEnumerator MoveWithEase(Vector3 start, Vector3 end, System.Func<float, float> easingFunction)
    {
        float elapsedTime = 0f;
        float duration = Vector3.Distance(start, end) / moveSpeed;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            float easedT = easingFunction(t);
            transform.position = Vector3.Lerp(start, end, easedT);
            yield return null;
        }
        transform.position = end;
    }

    private float EaseOutQuint(float x)
    {
        return 1 - Mathf.Pow(1 - x, 5);
    }

    private float EaseInCubic(float x)
    {
        return x * x * x;
    }
}
