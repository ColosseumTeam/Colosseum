using UnityEngine;

public class PlayerDamagedBehaviour : BehaviourBase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<PlayerController>().SetState(State.Damaged);
    }
}
