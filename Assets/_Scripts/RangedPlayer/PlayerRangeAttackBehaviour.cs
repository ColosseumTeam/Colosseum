using UnityEngine;

public class PlayerRangeAttackBehaviour : RangePlayerBehaviourBase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<PlayerRangeAttackController>().GetState(State.RangeSkill);
    }
}
