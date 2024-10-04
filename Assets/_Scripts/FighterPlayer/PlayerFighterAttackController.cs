using Fusion;
using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFighterAttackController : NetworkBehaviour
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
    [SerializeField] private CrossHairLookAt crossHairLookAt;
    [SerializeField] private GameManager gameManager;

    [Header("Skill Collider")]
    [SerializeField] private CapsuleCollider leftClickSkillCollider;
    [SerializeField] private CapsuleCollider rightClickSkillCollider;

    [Header("Skill Object")]
    [SerializeField] private Transform qSkillGroup;
    [SerializeField] private NetworkObject qSkillPrefab;
    [SerializeField] private NetworkObject shiftClickSkillPrefab;
    [SerializeField] private NetworkObject eSkillPrefab;

    private PlayerController playerController;
    private Animator animator;
    private NetworkMecanimAnimator mecanimAnimator;
    private float eState;
    [SerializeField] private bool isAttacking;
    private bool isPressedShift;
    private Vector3 skillPos;
    private int qCount;

    private bool isReadyToShootQ;

    public bool IsQSkillOn { get; set; }


    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        mecanimAnimator = GetComponent<NetworkMecanimAnimator>();
        //crossHairLookAt = Camera.main.GetComponent<CrossHairLookAt>();
        gameManager = FindObjectOfType<GameManager>();
        aimController = gameManager.AimController;
    }

    // Shift + Click
    private void OnRangeOneSkill(InputValue value)
    {
        if (!HasStateAuthority) return;

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
            aimController.AimSkillReadyNonActive();
        }
    }

    private void OnAttack(InputValue value)
    {
        if (!HasStateAuthority) return;

        if (value.isPressed && isPressedShift)
        {
            isPressedShift = false;
            aimController.AimSkillReadyNonActive();

            skillPos = crossHairLookAt.GroundHitPositionTransmission();

            mecanimAnimator.SetTrigger("Skill");
            animator.SetInteger("SkillState", 2);
        }

        else if (value.isPressed)
        {
            isAttacking = true; 
        }
    }

    public void OnInstantiateShiftClickSkillPrefab()
    {
        NetworkObject shifhtClickObj = Runner.Spawn(shiftClickSkillPrefab, skillPos, transform.rotation);
    }

    // Right Click Skill
    private void OnRangeTwoSkill(InputValue value)
    {
        if (!HasStateAuthority) return;

        if (value.isPressed && !isAttacking)
        {
            isAttacking = true;
            if (eState > 1f)
            {
                eState = 0f;
            }
            mecanimAnimator.SetTrigger("Skill");
            animator.SetInteger("SkillState", 0);
        }
    }

    // Q Skill
    private void OnRangeThreeSkill(InputValue value)
    {
        if (!HasStateAuthority) return;

        if (value.isPressed)
        {
            if (!IsQSkillOn)
            {
                IsQSkillOn = true;
                mecanimAnimator.SetTrigger("Skill");
                animator.SetInteger("SkillState", 1);
                // Todo: 공중에 떠있는 돌 애니메이션 이벤트에 적용
            }
            else
            {
                if (!isReadyToShootQ)
                {
                    return;
                }

                NetworkObject stone = Runner.Spawn(qSkillPrefab, transform.position + new Vector3(0, 1.5f, 0), transform.rotation);
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

    // E Skill
    private void OnRangeFourSkill(InputValue value)
    {
        if (!HasStateAuthority) return;

        if (value.isPressed)
        {
            skillPos = crossHairLookAt.GroundHitPositionTransmission();

            mecanimAnimator.SetTrigger("Skill");
            animator.SetInteger("SkillState", 3);
        }
    }

    public void OnInstantiateESkillPrefab()
    {
        NetworkObject eSkillObj = Runner.Spawn(eSkillPrefab, skillPos, transform.rotation);
    }

    private void OnDance(InputValue value)
    {
        if (value.isPressed)
        {
            mecanimAnimator.SetTrigger("Emotion1");
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
            case Skill.LeftClick:
                leftClickSkillCollider.enabled = true;
                break;

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
            case Skill.LeftClick:
                leftClickSkillCollider.enabled = false;
                isAttacking = false;
                break;

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
