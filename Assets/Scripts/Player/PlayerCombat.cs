using System;
using Platformer;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader input;
    [SerializeField] private Transform player;
    [SerializeField] private float attackPositionOffset;
    private PlayerMovement _pm;
    private Animator _animator;
    private Vector3 _attackOffset;
    
    [Header("Combat")]
    [SerializeField] private int damage;
    [SerializeField] private float atkSpeed;
    [SerializeField] private float atkRange;
    private bool _isRangedAttack;
    
    private void Start()
    {
        _pm = GetComponent<PlayerMovement>();
        
        input.AttackEvent += Attack;
        input.SwapAttackEvent += SwapAttackType;
    }

    private void Update()
    {
        _attackOffset = player.position + new Vector3(0f, attackPositionOffset, 0f);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(_attackOffset, atkRange);
        }
    }
    #endif

    private void SwapAttackType()
    {
        _isRangedAttack = !_isRangedAttack;
        
        Debug.Log("Attack Swapped");
        
        // update the ui
    }

    private void Attack()
    {
        if (!_isRangedAttack)
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

        Collider[] enemies = Physics.OverlapSphere(_attackOffset, atkRange);
        foreach (var enemy in enemies)
        {
            if (enemy.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.TakeDamage(damage);
            }
            
            // remove when animations are added
            Debug.Log("Enemies hit: " + enemies.Length);
        }
    }

    private void RangedAttack()
    {
        Debug.Log("Ranged Attack");
    }
}
