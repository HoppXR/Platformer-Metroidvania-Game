using Platformer;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader input;
    private PlayerMovement _pm;
    private Animator animator;
    
    [Header("Combat")]
    [SerializeField] private float damage;
    [SerializeField] private float atkSpeed;
    [SerializeField] private float range;
    private bool _isMeleeAttack;
    
    private void Start()
    {
        _pm = GetComponent<PlayerMovement>();
        
        input.AttackEvent += Attack;
        input.SwapAttackEvent += SwapAttackType;
    }

    private void SwapAttackType()
    {
        _isMeleeAttack = !_isMeleeAttack;
        
        Debug.Log("Attack Swapped");
        
        // update the ui
    }

    private void Attack()
    {
        if (_isMeleeAttack)
        {
            MeleeAttack();
        }
        else
        {
            RangedAttack();
        }
    }

    private void GroundSlam()
    {
        // Raycast to check how high the player is
        
        // if the player is high enough
    }
    
    private void MeleeAttack()
    {
        Debug.Log("Melee Attack");
        // animations are needed to implement attacks
    }

    private void RangedAttack()
    {
        Debug.Log("Ranged Attack");
    }
}
