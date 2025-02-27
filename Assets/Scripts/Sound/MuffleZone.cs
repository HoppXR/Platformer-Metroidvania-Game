using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuffleZone : MonoBehaviour
{
    private MusicManager _MusicManager;

    void Start()
    {
        _MusicManager = FindObjectOfType<MusicManager>();
        if (_MusicManager == null)
        {
            Debug.LogError("AudioManager not found in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            _MusicManager.ToggleMuffle(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _MusicManager.ToggleMuffle(false);
        }
    }
}