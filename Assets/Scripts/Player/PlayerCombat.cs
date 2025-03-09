using Enemy;
using Player.Animation;
using Player.Input;
using UnityEngine;

namespace Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader input;
        [SerializeField] private Transform player;
        [SerializeField] private float attackPositionOffset;
        private Animator _animator;
        private PlayerAnimation _playerAnimation;
        private Vector3 _attackOffset;
    
        [Header("Combat")]
        [SerializeField] private int damage;
        [SerializeField] private float atkSpeed;
        [SerializeField] private float atkRange;
        private bool _isRangedAttack;
    
        private void Start()
        {
            _playerAnimation = GetComponent<PlayerAnimation>();
            
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
            _playerAnimation.ChangeAnimation("Attack", 0.1f);

            Collider[] enemies = Physics.OverlapSphere(_attackOffset, atkRange);
            foreach (var enemy in enemies)
            {
                if (enemy.TryGetComponent(out EnemyHealth enemyHealth))
                {
                    enemyHealth.TakeDamage(damage);
                }

                if (enemy.TryGetComponent(out BossHealth bossHealth))
                {
                    bossHealth.TakeDamage(damage);
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
}
