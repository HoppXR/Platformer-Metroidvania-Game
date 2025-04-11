using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class BossLineToPlayer : MonoBehaviour
{
    private Transform player;
    private LineRenderer lineRenderer;

    private bool isVisible = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform;

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (!isVisible || player == null) return;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, player.position);
    }

    public void ShowLine()
    {
        isVisible = true;
        lineRenderer.enabled = true;
    }

    public void HideLine()
    {
        isVisible = false;
        lineRenderer.enabled = false;
    }
    public IEnumerator ShowLineRenderer()
    {
        ShowLine();

        yield return new WaitForSeconds(1f);

        HideLine();
    }
    
    public IEnumerator ShowLineRendererLazy()
    {
        ShowLine();

        yield return new WaitForSeconds(0.5f);

        HideLine();
    }
}