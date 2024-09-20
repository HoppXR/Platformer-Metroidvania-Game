using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("more than one audio manager in the scene rn");
        }

        instance = this;
    }

    public void PlayOneShot(EventReference sound, Collectable collectable, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
}