using FMODUnity;
using UnityEngine;

namespace Managers
{
    public class FMODEvents : MonoBehaviour
    {
        [field: Header("Player SFX")]
        [field: SerializeField] public EventReference PlayerFootsteps { get; private set; }
    
        [field: Header("BGM")]
        [field: SerializeField] public EventReference BGM { get; private set; }
    
        public static FMODEvents Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Duplicate FMODEvents instance");
            }

            Instance = this;
        }
    }
}