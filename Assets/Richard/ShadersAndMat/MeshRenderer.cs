using System.Collections;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    [Header("Dash Trail Settings")]
    public float dashMeshRefreshRate = 0.1f;
    public float dashMeshDestroyDelay = 3f;

    [Header("Shader Settings")]
    public Transform positionToSpawn;
    public Material mat;
    public string shaderVarRef;
    public float shaderFadeSpeed = 0.1f;

    private SkinnedMeshRenderer[] skinnedRenderers;
    private bool isDashActive = false;
    
    public void StartDashTrail()
    {
        if (!isDashActive) StartCoroutine(DashTrail());
    }

    IEnumerator DashTrail()
    {
        isDashActive = true;
        float elapsedTime = 0f;

        while (elapsedTime < 0.5)
        {
            elapsedTime += dashMeshRefreshRate;
            SpawnMeshTrail(dashMeshDestroyDelay);
            yield return new WaitForSeconds(dashMeshRefreshRate);
        }

        isDashActive = false;
    }
    
    private void SpawnMeshTrail(float destroyDelay)
    {
        skinnedRenderers = positionToSpawn.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (var skinnedRenderer in skinnedRenderers)
        {
            GameObject meshObject = new GameObject("MeshTrailInstance");
            meshObject.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);
            meshObject.transform.rotation *= Quaternion.Euler(-90f, 0f, 0f);

            MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
            MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();

            Mesh mesh = new Mesh();
            skinnedRenderer.BakeMesh(mesh);

            meshFilter.mesh = mesh;
            meshRenderer.material = mat;

            //StartCoroutine(AnimateMaterialFloat(meshRenderer.material, 0, shaderFadeSpeed));

            Destroy(meshObject, destroyDelay);
        }
    }
    
    /*
    IEnumerator AnimateMaterialFloat(Material m, float valueGoal, float fadeSpeed)
    {
        float valueToAnimate = m.GetFloat(shaderVarRef);

        while (valueToAnimate > valueGoal)
        {
            valueToAnimate -= fadeSpeed;
            m.SetFloat(shaderVarRef, valueToAnimate);
            yield return null;
        }
    }*/
}
