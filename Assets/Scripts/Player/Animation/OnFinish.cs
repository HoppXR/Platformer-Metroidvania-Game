using UnityEngine;

namespace Player.Animation
{
    public class OnFinish : StateMachineBehaviour
    {
        [SerializeField] private string nextAnimation;
        [SerializeField] private float crossFadeTime;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
           animator.GetComponentInParent<PlayerAnimation>().ChangeAnimation(nextAnimation, crossFadeTime, stateInfo.length);
        }
    }
}
