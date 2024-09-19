using UnityEngine;
using FMODUnity;

public class Collectable : MonoBehaviour
{
    //[SerializeField] private EventReference tingerCollectedSound;
    private GameManager _gameManager;
    
    private float _initialY;

    private void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        _initialY = transform.position.y;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // add collectable logic
            //AudioManager.instance.PlayOneShot(tingerCollectedSound, this.transform.position);
            _gameManager.IncreaseCount();
            
            Destroy(gameObject);
        }
    }

    private void HandleMovement()
    {
        transform.Rotate(0f,1f,0f);
        transform.position = new Vector3(transform.position.x, 0.35f * Mathf.Sin(Time.time * 1f) + _initialY, transform.position.z);
    }
}
