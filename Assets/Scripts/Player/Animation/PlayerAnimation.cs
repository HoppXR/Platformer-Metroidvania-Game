using System.Collections;
using Player.Movement;
using UnityEngine;

namespace Player.Animation
{
    public class PlayerAnimation : MonoBehaviour
    {
        private PlayerMovement _playerMovement;
        private Animator _animator;
        public static string CurrentAnimation = "";

        private void Start()
        {
            _playerMovement = GetComponent<PlayerMovement>();
            _animator = GetComponentInChildren<Animator>();
        }

        public void ChangeAnimation(string animationName, float crossFadeTime = 0.2f, float duration = 0)
        {
            if (duration > 0) StartCoroutine(Wait());
            else PlayAnimation();
            return;

            IEnumerator Wait()
            {
                yield return new WaitForSeconds(duration - crossFadeTime);
                PlayAnimation();
            }
            
            void PlayAnimation()
            {
                if (CurrentAnimation == animationName) return;
                
                CurrentAnimation = animationName;
                
                if (CurrentAnimation == "")
                    _playerMovement?.HandleAnimation();
                else
                    _animator?.CrossFade(animationName, crossFadeTime);
            }
        }
    }
}
