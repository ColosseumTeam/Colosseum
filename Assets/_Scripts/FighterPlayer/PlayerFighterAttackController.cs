using Cinemachine;
using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;

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

    [Header("References")]
    [SerializeField] private AimController aimController;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private CrossHairLookAt crossHairLookAt;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TimelineClip ultTimeline;
    [SerializeField] private Transform ultEffectCameraGroup;
    [SerializeField] private Camera ultCamera;

    [Header("Skill Collider")]
    [SerializeField] private CapsuleCollider leftClickSkillCollider;
    [SerializeField] private CapsuleCollider rightClickSkillCollider;

    [Header("Skill Object")]
    [SerializeField] private Transform qSkillGroup;
    [SerializeField] private NetworkObject qSkillPrefab;
    [SerializeField] private GameObject shiftClickSkillPrefab;
    [SerializeField] private GameObject eSkillPrefab;
    [SerializeField] private GameObject eSkillRockFront;
    [SerializeField] private GameObject eSkillRockBack;

    [Header("Skill Sound")]
    [SerializeField] private AudioClip leftClickSkillClip;
    [SerializeField] private AudioClip rightClickSkillClip;
    [SerializeField] private AudioClip qSkillClip;

    private PlayerController playerController;
    private Animator animator;
    private NetworkMecanimAnimator mecanimAnimator;
    private PlayableDirector playableDirector;
    private float eState;
    [SerializeField] private bool isAttacking;
    private bool isPressedShift;
    private Vector3 skillPos;
    private int qCount;
    private Transform enemyTr;
    // for test
    [SerializeField] private GameObject mainCam;

    private bool isReadyToShootQ;

    public bool IsQSkillOn { get; set; }
    public PlayableDirector PlayableDirector { get { return playableDirector; } }


    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        mecanimAnimator = GetComponent<NetworkMecanimAnimator>();
        playableDirector = GetComponent<PlayableDirector>();
        //crossHairLookAt = Camera.main.GetComponent<CrossHairLookAt>();
    }

    public override void Spawned()
    {
        base.Spawned();

        gameManager = FindObjectOfType<GameManager>();
        aimController = gameManager.AimController;

        if (aimController == null)
        {
            Debug.Log("Couldn't find AimController");
        }

        if (HasStateAuthority)
        {
            mainCam.SetActive(true);
        }
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

            GetComponent<AudioSource>().clip = rightClickSkillClip;
            GetComponent<AudioSource>().volume = 0.3f;
            GetComponent<AudioSource>().Play();
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

                GetComponent<AudioSource>().clip = qSkillClip;
                GetComponent<AudioSource>().volume = 0.3f;
                GetComponent<AudioSource>().Play();
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

    [Rpc]
    public void RPC_StartESkillEffect()
    {
        playableDirector.Play();
    }

    public void OnInstantiateESkillPrefab()
    {
        GameObject eSkillObj = Instantiate(eSkillPrefab, skillPos, transform.rotation);
        eSkillObj.GetComponent<FighterESkill>().SetAttackController(this);
    }

    [Rpc]
    public void RPC_SetEnemyOnUltCamera()
    {
        //enemyTr = enemy;
        List<NetworkObject> networkObjects = Runner.GetAllNetworkObjects();
        foreach (var obj in networkObjects)
        {
            if (obj.TryGetComponent(out PlayerController component))
            {
                if (obj.tag == "Enemy")
                {
                    enemyTr = obj.transform;
                    Debug.Log("Found enemy");
                }
            }
        }

        for (int i = 0; i < ultEffectCameraGroup.childCount; i++)
        {
            CinemachineVirtualCamera vircam = ultEffectCameraGroup.GetChild(i).GetComponent<CinemachineVirtualCamera>();
            if (enemyTr != transform)
            {
                vircam.Follow = transform;
                vircam.LookAt = enemyTr;
            }
            else
            {
                vircam.Follow = enemyTr;
                vircam.LookAt = transform;
            }
        }
    }

    public void OnUltEffectStart()
    {
        //Camera.main.enabled = false;
        ultCamera.gameObject.SetActive(true);
        GameObject rock1 = Instantiate(eSkillRockFront, enemyTr.position - (enemyTr.position - transform.position).normalized * 2, Quaternion.identity);
        rock1.transform.LookAt(enemyTr);
        GameObject rock2 = Instantiate(eSkillRockBack, enemyTr.position + (enemyTr.position - transform.position).normalized * 2, Quaternion.identity);
        rock2.transform.LookAt(enemyTr);
    }

    public void OnUltEffectEnd()
    {
        //Camera.main.enabled = true;
        ultCamera.gameObject.SetActive(false);
    }

    private void OnEmotion1(InputValue value)
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
