using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference PlayerFootsteps { get; private set; }
    
    [field: Header("Tinger SFX")]
    [field: SerializeField] public EventReference TingerCollected { get; private set; }
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