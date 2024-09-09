using UnityEngine;

public class PlayerBoostBehaviour : BehaviourBase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<PlayerController>().SetState(State.Boost);
    }
}
