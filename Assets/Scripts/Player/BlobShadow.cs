using UnityEngine;

public class BlobShadow : MonoBehaviour
{
    [SerializeField] private GameObject shadow;
    [SerializeField] private float offset;
    private RaycastHit hit;

    private void Update()
    {
        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y - offset, transform.position.z), -Vector3.up);

        Vector3 hitPosition = hit.point;

        shadow.transform.position = hitPosition;

        if (Physics.Raycast(ray, out hit))
        {
            
        }
    }
}
