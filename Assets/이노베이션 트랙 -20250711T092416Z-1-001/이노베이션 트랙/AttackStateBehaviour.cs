using UnityEngine;

public class AttackStateBehaviour : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsAttacking", true);
        int currentCount = animator.GetInteger("AttackCount");
        animator.SetInteger("AttackCount", currentCount + 1);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("IsAttacking", false);
        if (!animator.GetCurrentAnimatorStateInfo(layerIndex).IsTag("Attack"))
        {
            animator.SetInteger("AttackCount", 0);
        }
    }
}