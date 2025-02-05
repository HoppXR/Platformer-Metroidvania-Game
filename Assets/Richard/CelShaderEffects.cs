using UnityEngine;

[ExecuteInEditMode]
public class CelShaderEffect : MonoBehaviour
{
    public Material celMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (celMaterial != null)
        {
            Graphics.Blit(src, dest, celMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}