using FMOD.Studio;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private EventInstance _backgroundMusic;

    private void Start()
    {
        _backgroundMusic = AudioManager.instance.CreateInstance(FMODEvents.Instance.BGM);
        
        _backgroundMusic.start();
        _backgroundMusic.release();
    }

    public void AddTrack(int index)
    {
        if (index == 0)
        {
            _backgroundMusic.setParameterByName("powerups 1", 1);
        }
        else if (index == 1)
        {
            _backgroundMusic.setParameterByName("powerups 2", 1);
        }
        else if (index == 2)
        {
            _backgroundMusic.setParameterByName("powerups 3", 1);
        }
    }

    public void ToggleMuffle(bool enable)
    {
        _backgroundMusic.setParameterByName("Underground muffle", enable ? 1 : 0);
    }
}
