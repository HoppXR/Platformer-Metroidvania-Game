using UnityEngine;

public class AbilityCollectable : MonoBehaviour
{
    [SerializeField] private AbilityManager.Ability abilityToGive;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GiveAbility(abilityToGive);
            
            Destroy(gameObject);
        }
    }

    private void GiveAbility(AbilityManager.Ability ability)
    {
        AbilityManager.Instance.EnableAbility(ability);
    }
}
