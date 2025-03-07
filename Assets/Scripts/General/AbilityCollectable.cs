using FMODUnity;
using UnityEngine;

public class AbilityCollectable : MonoBehaviour
{
    [SerializeField] private AbilityManager.Abilities abilityToGive;
    [SerializeField] private EventReference collectedSound;

    private float _initialY;

    private void Start()
    {
        _initialY = transform.position.y;
    }

    private void Update()
    {
        HandleMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            
            GiveAbility(abilityToGive);
        }
    }

    private void GiveAbility(AbilityManager.Abilities ability)
    {
        AbilityManager.Instance?.EnableAbility(ability);
    }

    private void HandleMovement()
    {
        transform.Rotate(0f, 1f, 0f);
        transform.position = new Vector3(transform.position.x, 0.35f * Mathf.Sin(Time.time * 1f) + _initialY, transform.position.z);
    }
}