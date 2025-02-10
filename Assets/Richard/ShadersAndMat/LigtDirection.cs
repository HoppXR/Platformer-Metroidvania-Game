using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LigtDirection : MonoBehaviour
{

    [SerializeField] private Material skybox;
    void Update()
    {
        skybox.SetVector("_MainLightDirection", transform.forward);
    }
}
