using UnityEngine;

public class PlayerJumpBehaviour : BehaviourBase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<PlayerController>().SetState(State.Jump);
    }
}
