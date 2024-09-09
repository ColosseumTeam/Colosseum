using UnityEngine;

public class PlayerIdleBehaviour : BehaviourBase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<PlayerController>().SetState(State.Idle);
    }
}
