using Managers;
using UnityEngine;

namespace Sound
{
    public class BossArea : MonoBehaviour
    {
        private MusicManager _musicManager;

        private void Start()
        {
            _musicManager = FindFirstObjectByType<MusicManager>();
            if (_musicManager == null)
            {
                Debug.LogError("Music Manager not found");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) 
            {
                _musicManager.ToggleBoss(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _musicManager.ToggleBoss(false);
            }
        }
    }
}