using UnityEngine;

public class FighterRightClickSkillBehaviour : FighterBehaviourBase
{
    private float rightClickState;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (rightClickState > 1f)
        {
            InitSkillState();
        }

        animator.SetFloat("RightClickState", rightClickState);

        if (rightClickState >= 1)
        {
            animator.GetComponent<RangePlayerCoolTImeManager>().SkillChecking(1);
        }

        rightClickState += 0.5f;
    }

    public void InitSkillState()
    {
        rightClickState = 0f;
    }
}
