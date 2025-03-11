using FMOD.Studio;
using FMODUnity;
using Managers;
using Player.Input;
using Player.Movement;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Sound
{
    public class PlayerSound : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader input;
        [SerializeField] private Transform groundCheckPoint;
        [SerializeField] private float groundCheckDistance;
        [SerializeField] private float terrainCheckDistance;
        private Vector2 _inputDir;
        private Vector3 _groundCheckOffset;
    
        [Header("Audio")]
        [SerializeField] private CurrentTerrain currentTerrain;
        private enum CurrentTerrain { Grass, Stone, Water, Pipe }
        private EventInstance _playerFootsteps;

        [SerializeField] private EventReference jumpSound;

        private void Start()
        {
            input.MoveEvent += HandleInput;
        
            _playerFootsteps = AudioManager.Instance.CreateInstance(FMODEvents.Instance.PlayerFootsteps);
        }

        private void Update()
        {
            _groundCheckOffset = groundCheckPoint.position + new Vector3(0, groundCheckDistance, 0);
        
            DetermineTerrain();
            SelectAndPlayFootstep();
        }
        
        private void OnDisable()
        {
            input.MoveEvent -= HandleInput;
        }

        private void HandleInput(Vector2 dir)
        {
            _inputDir = dir;
        }

        private void DetermineTerrain()
        {
            var hit = Physics.RaycastAll(_groundCheckOffset, Vector3.down, terrainCheckDistance);
            Debug.DrawRay(_groundCheckOffset, Vector3.down * terrainCheckDistance, Color.magenta);
            
            foreach (RaycastHit rayhit in hit)
            {
                if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Grass"))
                {
                    currentTerrain = CurrentTerrain.Grass;
                    break;
                }
                else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Stone"))
                {
                    currentTerrain = CurrentTerrain.Stone;
                    break;
                }
                else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Water"))
                {
                    currentTerrain = CurrentTerrain.Water;
                }
                else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Pipe"))
                {
                    currentTerrain = CurrentTerrain.Pipe;
                }
            }
        }

        public void SelectAndPlayFootstep()
        {
            switch (currentTerrain)
            {
                case CurrentTerrain.Grass:
                    PlayFootstep(0);
                    break;
                
                case CurrentTerrain.Stone:
                    PlayFootstep(1);
                    break;
                
                case CurrentTerrain.Water:
                    PlayFootstep(2);
                    break;
                
                case CurrentTerrain.Pipe:
                    PlayFootstep(3);
                    break;
            }
        }

        private void PlayFootstep(int terrain)
        {
            _playerFootsteps.setParameterByName("footsteps", terrain);
            _playerFootsteps.set3DAttributes(gameObject.To3DAttributes());
            
            if (PlayerMovement.Grounded && _inputDir.x >= 0.1f || _inputDir.y >= 0.1f || _inputDir.x <= -0.1f || _inputDir.y <= -0.1f)
            {
                PLAYBACK_STATE playbackState;
                _playerFootsteps.getPlaybackState(out playbackState);
                
                if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
                {
                    _playerFootsteps.start();
                }
            }
            else
            {
                _playerFootsteps.stop(STOP_MODE.IMMEDIATE);
            }
        }

        public void PlayJumpSound()
        {
            AudioManager.Instance.PlayOneShot(jumpSound, transform.position);
        }
    }
}
