using System;
using UnityEngine;
using FMOD.Studio;

public class Footstep : MonoBehaviour
{
    private EventInstance playerFootsteps;
    private Rigidbody _rb;

    private void Start()
    {
        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.instance.playerFootsteps);
    }

    private void FixedUpdate()
    {
        UpdateSound();
    }
    
    private void UpdateSound()
    {
        if (_rb.velocity.x != 0)
        {
            Debug.Log("asd");
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerFootsteps.start();
                Debug.Log("asdf");
            }
        }
        else
        {
            playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
            Debug.Log("wtf");
        }
    }
}
