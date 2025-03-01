using UnityEngine;

public class AbilityCollectable : MonoBehaviour
{
    [SerializeField] private AbilityManager.Abilities abilityToGive;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GiveAbility(abilityToGive);
            
            Destroy(gameObject);
        }
    }

    private void GiveAbility(AbilityManager.Abilities ability)
    {
        AbilityManager.Instance?.EnableAbility(ability);
    }
}