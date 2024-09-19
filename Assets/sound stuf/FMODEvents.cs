using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Player SFX")]
    
    [field: SerializeField] public EventReference playerFootsteps { get; private set; }
    
    [field: Header("Coin SFX")]
    [field: SerializeField] public EventReference tingerCollected { get; private set; }
    public static FMODEvents Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("more than one audio manager in the scene rn");
        }

        Instance = this;
    }
}