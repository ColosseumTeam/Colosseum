using UnityEngine;

public class BehaviourBase : StateMachineBehaviour
{
    public enum State
    {
        None,
        Idle,
        Move,
        Run,
        Boost,
        Attack,
        Jump,
        Damaged,
        Stun,
        OnAir,
        Down,
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
