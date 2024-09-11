using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using static PlayerFighterAttackController;

public class PlayerFighterAttackController : MonoBehaviour
{
    public enum Skill
    {
        LeftClick,
        RightClick,
        Q,
        ShiftClick,
        E,
    }

    [Header("Primary Setting")]
    [SerializeField] private AimController aimController;
    [SerializeField] private GameObject shiftClickSkillPrefab;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private DamageManager damageManager;

    [Header("Skill Collider")]
    [SerializeField] private CapsuleCollider rightClickSkillCollider;

    private Animator animator;
    private float eState;
    private bool isAttacking;
    private bool isPressedShift;
    private Vector3 skillPos;
    private int qCount;

    public DamageManager DamageManager { get; private set; }
    public bool IsQSkillOn { get; set; }


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Right Click Skill
    private void OnRangeOneSkill(InputValue value)
    {
        if (value.isPressed && !isAttacking)
        {
            isAttacking = true;
            if (eState > 1f)
            {
                eState = 0f;
            }
            animator.SetTrigger("Skill");
            animator.SetInteger("SkillState", 0);
        }
    }

    // Q Skill
    private void OnRangeTwoSkill(InputValue value)
    {
        if (value.isPressed)
        {
            if (!IsQSkillOn)
            {
                IsQSkillOn = true;
                animator.SetTrigger("Skill");
                animator.SetInteger("SkillState", 1);
                // Todo: 공중에 떠있는 돌 생성하는 함수 만들어서 애니메이션 이벤트에 적용
            }
            else
            {
                // Todo: Q 스킬 프리팹 생성하고 날아가는 효과
                // Todo: Q 공중에 떠있는 오브젝트 하나 숨기기
                qCount--;

                if (qCount == 0)
                {
                    IsQSkillOn = false;
                }
            }
        }
    }

    public void QSkillInit(int count)
    {
        qCount = count;
    }

    // Shift + Click
    private void OnAttack(InputValue value)
    {
        if (value.isPressed && isPressedShift)
        {
            isPressedShift = false;
            aimController.SkillReadyNonActive();

            Ray ray = Camera.main.ScreenPointToRay(aimController.transform.position);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
            {
                skillPos = hit.point;
            }
            else
            {
                float skillDistance = 3f;
                Vector3 dir = Camera.main.ScreenToWorldPoint(aimController.transform.position);
                Debug.Log($"dir: {dir}");
                float dirZ = Mathf.Sqrt(Mathf.Abs(skillDistance * skillDistance - (dir.x * dir.x) - (dir.y * dir.y)));
                dir = Camera.main.ScreenToWorldPoint(new Vector3(aimController.transform.position.x, aimController.transform.position.y, dirZ));
                //dirZ = dirZ >= 0 ? dirZ : -dirZ;
                //skillPos = transform.forward.z >= 0f ? transform.position + new Vector3(-dir.x, dir.y, dirZ) : transform.position + new Vector3(-dir.x, dir.y, -dirZ);
                skillPos = transform.position + new Vector3(dir.x, dir.y, -dir.z);
                Debug.Log($"skillPos: {skillPos}");
            }

            animator.SetTrigger("Skill");
            animator.SetInteger("SkillState", 2);
        }
    }

    public void OnInstantiateShiftClickSkillPrefab()
    {
        Instantiate(shiftClickSkillPrefab, skillPos, Quaternion.identity);
    }

    private void OnE(InputValue value)
    {
        if (value.isPressed)
        {
            animator.SetTrigger("Skill");
            animator.SetInteger("SkillState", 3);
        }
    }

    private void OnEmotion1(InputValue value)
    {
        if (value.isPressed)
        {
            animator.SetTrigger("Emotion1");
        }
    }

    private void OnShift(InputValue value)
    {
        //if (value.isPressed)
        //{
        //    isPressedShift = true;
        //}
        if (!isPressedShift)
        {
            isPressedShift = true;
            aimController.SkillReadyAcitve(0f);
        }
        else if (isPressedShift)
        {
            isPressedShift = false;
            aimController.SkillReadyNonActive();
        }
    }

    private void OnAttackEnd()
    {
        // 공격이 끝났을 때 호출되어 공격 플래그를 해제
        isAttacking = false;
    }

    public void AttackColliderOn(Skill skill)
    {
        switch (skill)
        {
            case Skill.RightClick:
                rightClickSkillCollider.enabled = true;
                break;

            case Skill.Q:

                break;

            case Skill.ShiftClick:

                break;

            case Skill.E:

                break;
        }
    }

    public void AttackColliderOff(Skill skill)
    {
        switch (skill)
        {
            case Skill.RightClick:
                rightClickSkillCollider.enabled = false;
                break;

            case Skill.Q:

                break;

            case Skill.ShiftClick:

                break;

            case Skill.E:

                break;
        }
    }
}
