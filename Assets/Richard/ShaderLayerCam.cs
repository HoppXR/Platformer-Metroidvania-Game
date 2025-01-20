using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ShaderLayerCam : MonoBehaviour
{
    public Material m_renderMaterial;
    public LayerMask layerMask;  
    private Camera _camera;  
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int originalMask = _camera.cullingMask;
        _camera.cullingMask = layerMask;
        Graphics.Blit(source, destination, m_renderMaterial);
        _camera.cullingMask = originalMask;
    }
}