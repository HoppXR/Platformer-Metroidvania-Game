using Enemy;
using Managers;
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
        [SerializeField] private float atkSpeed;
        [SerializeField] private float atkRange;
        private bool _canAttack = true;

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
            if (!_canAttack) return;

            _canAttack = false;

            if (player != null)
                _attackOffset = player.position + new Vector3(0f, attackPositionOffset, 0f);
            //Debug.Log("Attacked");
            _playerAnimation?.ChangeAnimation("Attack");

            Collider[] enemies = Physics.OverlapSphere(_attackOffset, atkRange);
            foreach (var enemy in enemies)
            {
                if (enemy.TryGetComponent(out EnemyHealth enemyHealth))
                {
                    enemyHealth.TakeDamage(GameManager.PlayerDamage);
                }

                if (enemy.TryGetComponent(out BossHealth bossHealth))
                {
                    bossHealth.TakeDamage(GameManager.PlayerDamage);
                }
            }

            Invoke(nameof(ResetAttack), atkSpeed);
        }

        private void ResetAttack()
        {
            _canAttack = true;
        }
    }
}
