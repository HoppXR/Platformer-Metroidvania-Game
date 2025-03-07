using FMOD.Studio;
using FMODUnity;
using Platformer;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class PlayerSound : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader input;
    [SerializeField] private float groundCheckDistance;
    private Vector2 _inputDir;
    private Vector3 _groundCheckOffset;
    
    [Header("Audio")]
    [SerializeField] private CurrentTerrain currentTerrain;
    private enum CurrentTerrain { Grass, Stone, Water, Pipe }
    private EventInstance playerFootsteps;

    [SerializeField] private EventReference jumpSound;

    private void Start()
    {
        input.MoveEvent += HandleInput;
        
        playerFootsteps = AudioManager.instance.CreateInstance(FMODEvents.Instance.PlayerFootsteps);
    }

    private void Update()
    {
        _groundCheckOffset = transform.position + new Vector3(0, groundCheckDistance, 0);
        
        DetermineTerrain();
        SelectAndPlayFootstep();
    }

    private void HandleInput(Vector2 dir)
    {
        _inputDir = dir;
    }

    private void DetermineTerrain()
        {
            RaycastHit[] hit;

            hit = Physics.RaycastAll(_groundCheckOffset, Vector3.down, groundCheckDistance);
            Debug.DrawRay(_groundCheckOffset, Vector3.down * groundCheckDistance, Color.magenta);
            
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
                
                default:
                    PlayFootstep(0);
                    break;
            }
        }

        private void PlayFootstep(int terrain)
        {
            playerFootsteps.setParameterByName("Terrain", terrain);
            playerFootsteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
            
            if (_inputDir.y != 0 && PlayerMovement.Grounded || _inputDir.x != 0 && PlayerMovement.Grounded)
            {
                PLAYBACK_STATE playbackState;
                playerFootsteps.getPlaybackState(out playbackState);
                
                if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
                {
                    playerFootsteps.start();
                }
            }
            else
            {
                playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
            }
        }

        public void PlayJumpSound()
        {
            AudioManager.instance.PlayOneShot(jumpSound, transform.position);
        }
}
