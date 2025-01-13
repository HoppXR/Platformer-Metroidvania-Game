using UnityEngine;

public class BlobShadow : MonoBehaviour
{
    [SerializeField] private Transform shadow;
    [SerializeField] private float offset;

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            shadow.position = hit.point + Vector3.up * offset;
        }
    }
}
