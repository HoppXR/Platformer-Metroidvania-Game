using FMOD.Studio;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    private EventInstance backgroundMusic;

    private void Start()
    {
        backgroundMusic = AudioManager.instance.CreateInstance(FMODEvents.Instance.BGM);
        
        backgroundMusic.start();
        backgroundMusic.release();
    }
}
