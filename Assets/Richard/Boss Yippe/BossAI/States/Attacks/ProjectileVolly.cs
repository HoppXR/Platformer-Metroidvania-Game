using System.Collections;
using UnityEngine;

public class ProjectileVolley : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject skyIndicatorPrefab;
    public Transform projectileSpawnPoint;
    public Transform player;
    public Transform playerPrediction;

    private BossAIManager boss;

    public int totalSkyProjectiles = 15;
    public int totalTargetedProjectiles = 7;
    public float skySpawnRadius = 10f;
    public float skyHeight = 20f;
    public float skyFireRate = 0.5f;
    public float targetedFireRate = 1f;
    public float projectileSpeed = 15f;
    public float predictionChance = 0.3f;
    public float arcVerticalAngle = 45f;

    private bool isShooting = false;
    private Rigidbody rb;

    void Start()
    {
        boss = GetComponent<BossAIManager>();
        rb = GetComponent<Rigidbody>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        if (playerPrediction == null)
        {
            GameObject predictionObj = GameObject.Find("EnemyPredictionPoint");
            if (predictionObj != null) playerPrediction = predictionObj.transform;
        }
    }

    IEnumerator FireVolley()
    {
        isShooting = true;

        StartCoroutine(SpawnSkyProjectiles());
        yield return StartCoroutine(FireTargetedProjectiles());

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isShooting = false;
    }

    IEnumerator SpawnSkyProjectiles()
    {
        for (int i = 0; i < totalSkyProjectiles; i++)
        {
            Vector3 spawnPosition = SpawnFallingProjectile();
            SpawnSkyIndicator(spawnPosition);

            yield return new WaitForSeconds(skyFireRate);
        }
    }

    IEnumerator FireTargetedProjectiles()
    {
        for (int i = 0; i < totalTargetedProjectiles; i++)
        {
            Transform target = (Random.value < predictionChance) ? playerPrediction : player;
            FireArchedAtTarget(target);

            yield return new WaitForSeconds(targetedFireRate);
        }
    }

    Vector3 SpawnFallingProjectile()
    {
        if (projectilePrefab == null) return Vector3.zero;

        Vector2 randomOffset = Random.insideUnitCircle * skySpawnRadius;
        Vector3 spawnPosition = new Vector3(transform.position.x + randomOffset.x, transform.position.y + skyHeight, transform.position.z + randomOffset.y);

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.down * projectileSpeed;
        }

        return spawnPosition;
    }

    void SpawnSkyIndicator(Vector3 spawnPosition)
    {
        if (skyIndicatorPrefab == null) return;

        Vector3 indicatorPosition = new Vector3(spawnPosition.x, 51f, spawnPosition.z);
        Quaternion fixedRotation = Quaternion.Euler(90f, 0f, 0f);

        GameObject indicator = Instantiate(skyIndicatorPrefab, indicatorPosition, fixedRotation);
        Destroy(indicator, 2f);
    }

    void FireArchedAtTarget(Transform target)
    {
        if (projectilePrefab == null || projectileSpawnPoint == null || target == null) return;
        Vector3 targetPosition = target.position;
        float heightOffset = Random.Range(arcVerticalAngle * 0.5f, arcVerticalAngle * 1.5f);
        Vector3 direction = (targetPosition - projectileSpawnPoint.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(-heightOffset, 0, 0);
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = rotation * Vector3.forward * projectileSpeed;
        }
    }

    public IEnumerator FireVolleyRoutine()
    {
        yield return StartCoroutine(FireVolley());
    }

    public void StopVolley()
    {
        isShooting = false;
        StopAllCoroutines();
    }
}
