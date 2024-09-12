using System.Security.Cryptography.X509Certificates;
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
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private DamageManager damageManager;
    [SerializeField] private CrossHairLookAt crossHairLookAt;

    [Header("Skill Collider")]
    [SerializeField] private CapsuleCollider rightClickSkillCollider;

    [Header("Skill Object")]
    [SerializeField] private Transform qSkillGroup;
    [SerializeField] private GameObject qSkillPrefab;
    [SerializeField] private GameObject shiftClickSkillPrefab;
    [SerializeField] private GameObject eSkillPrefab;

    private PlayerController playerController;
    private Animator animator;
    private float eState;
    private bool isAttacking;
    private bool isPressedShift;
    private Vector3 skillPos;
    private int qCount;

    private bool isReadyToShootQ;

    public DamageManager DamageManager { get; private set; }
    public bool IsQSkillOn { get; set; }


    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        crossHairLookAt = Camera.main.GetComponent<CrossHairLookAt>();
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
                // Todo: 공중에 떠있는 돌 애니메이션 이벤트에 적용
            }
            else
            {
                if (!isReadyToShootQ)
                {
                    return;
                }

                GameObject stone = Instantiate(qSkillPrefab, transform.position + new Vector3(0, 1.5f, 0), transform.rotation);
                stone.GetComponent<FighterQSkill>().Look(aimController.transform.position);
                qCount--;

                qSkillGroup.GetChild(qCount).gameObject.SetActive(false);

                if (qCount <= 0)
                {
                    IsQSkillOn = false;
                    isReadyToShootQ = false;
                }
            }
        }
    }

    public void QSkillInit(int count)
    {
        qCount = count;

        for (int i = 0; i < qSkillGroup.childCount; i++)
        {
            qSkillGroup.GetChild(i).gameObject.SetActive(true);
        }

        isReadyToShootQ = true;
    }

    // Shift + Click
    private void OnRangeThreeSkill(InputValue value)
    {
        // Action type을 Value로 변경.
        float input = value.Get<float>();

        if (input == 1f)
        {
            isPressedShift = true;
            aimController.SkillReadyAcitve(0f);
        }
        if (input == 0f)
        {
            Debug.Log("Canceled");
            isPressedShift = false;
            aimController.SkillReadyNonActive();
        }
    }

    private void OnAttack(InputValue value)
    {
        if (value.isPressed && isPressedShift)
        {
            isPressedShift = false;
            aimController.SkillReadyNonActive();

            skillPos = crossHairLookAt.GroundHitPositionTransmission();
            /*
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
            }
            */

            animator.SetTrigger("Skill");
            animator.SetInteger("SkillState", 2);
        }
    }

    public void OnInstantiateShiftClickSkillPrefab()
    {
        Instantiate(shiftClickSkillPrefab, skillPos, transform.rotation);
    }

    // E Skill
    private void OnRangeFourSkill(InputValue value)
    {
        if (value.isPressed)
        {
            skillPos = crossHairLookAt.GroundHitPositionTransmission();

            animator.SetTrigger("Skill");
            animator.SetInteger("SkillState", 3);
        }
    }

    public void OnInstantiateESkillPrefab()
    {
        Instantiate(eSkillPrefab, skillPos, transform.rotation);
    }

    private void OnEmotion1(InputValue value)
    {
        if (value.isPressed)
        {
            animator.SetTrigger("Emotion1");
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
