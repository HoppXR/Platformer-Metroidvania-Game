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
    
        private void Start()
        {
            _playerAnimation = GetComponent<PlayerAnimation>();

            input.AttackEvent += MeleeAttack;
        }

        private void OnDisable()
        {
            input.AttackEvent -= MeleeAttack;
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

        private void MeleeAttack()
        {
            if (player != null)
                _attackOffset = player.position + new Vector3(0f, attackPositionOffset, 0f);
            
            _playerAnimation?.ChangeAnimation("Attack");

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
            }
        }
    }
}
