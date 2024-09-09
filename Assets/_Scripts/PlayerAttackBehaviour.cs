using UnityEngine;

public class PlayerAttackBehaviour : BehaviourBase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<PlayerController>().SetState(State.Attack);
    }
}
