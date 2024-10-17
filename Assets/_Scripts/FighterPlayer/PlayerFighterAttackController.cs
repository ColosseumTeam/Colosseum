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
    [SerializeField] private RangePlayerCoolTImeManager coolTimeManager;

    [Header("Skill Collider")]
    [SerializeField] private CapsuleCollider leftClickSkillCollider;
    [SerializeField] private CapsuleCollider rightClickSkillCollider;

    [Header("Skill Object")]
    [SerializeField] private Transform attackPositionObj;
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
    private AudioSource audioSource;
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
        coolTimeManager = GetComponent<RangePlayerCoolTImeManager>();
        audioSource = GetComponent<AudioSource>();
        //crossHairLookAt = Camera.main.GetComponent<CrossHairLookAt>();
    }

    public override void Spawned()
    {
        base.Spawned();

        gameManager = FindObjectOfType<GameManager>();
        aimController = gameManager.AimController;

        audioSource.volume = playerController.VolumeManager.skillVolume;

        if (aimController == null)
        {
            Debug.Log("Couldn't find AimController");
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

        if (value.isPressed && isPressedShift && coolTimeManager.SkillCheckLis[0] && !playerController.IsSkilling)
        {
            isPressedShift = false;
            aimController.AimSkillReadyNonActive();

            skillPos = crossHairLookAt.GroundHitPositionTransmission();

            mecanimAnimator.SetTrigger("Skill");
            animator.SetInteger("SkillState", 2);

            coolTimeManager.SkillChecking(0);
        }

        else if (value.isPressed)
        {
            isAttacking = true; 
        }
    }

    public void OnInstantiateShiftClickSkillPrefab()
    {
        if (!HasStateAuthority) return;

        NetworkObject shiftClickObj = Runner.Spawn(shiftClickSkillPrefab, skillPos, transform.rotation);
        shiftClickObj.GetComponent<FighterPlayerShiftClick>().RPC_SetVolume();
    }

    // Right Click Skill
    private void OnRangeTwoSkill(InputValue value)
    {
        if (!HasStateAuthority) return;

        if (value.isPressed && !isAttacking && coolTimeManager.SkillCheckLis[1] && !playerController.IsSkilling)
        {
            isAttacking = true;
            if (eState > 1f)
            {
                eState = 0f;
            }

            mecanimAnimator.SetTrigger("Skill");
            animator.SetInteger("SkillState", 0);

            audioSource.PlayOneShot(rightClickSkillClip);
        }
    }

    // Q Skill
    private void OnRangeThreeSkill(InputValue value)
    {
        if (!HasStateAuthority) return;

        if (value.isPressed && coolTimeManager.SkillCheckLis[2] && !playerController.IsSkilling)
        {
            if (!IsQSkillOn)
            {
                IsQSkillOn = true;
                mecanimAnimator.SetTrigger("Skill");
                animator.SetInteger("SkillState", 1);

                playerController.SetMoveSpeed(100f);

                audioSource.PlayOneShot(qSkillClip, playerController.VolumeManager.skillVolume);
                // Todo: 공중에 떠있는 돌 애니메이션 이벤트에 적용
            }
            else
            {
                if (!isReadyToShootQ)
                {
                    return;
                }

                NetworkObject stone = Runner.Spawn(qSkillPrefab, attackPositionObj.position, transform.rotation);
                stone.GetComponent<FighterQSkill>().Look(aimController.transform.position);
                stone.GetComponent<FighterQSkill>().RPC_SetVolume();

                RPC_QShoot();

                if (qCount <= 0)
                {
                    IsQSkillOn = false;
                    isReadyToShootQ = false;

                    playerController.SetMoveSpeed(200f);

                    coolTimeManager.SkillChecking(2);
                }
            }
        }
    }

    [Rpc]
    public void RPC_QSkillInit(int count)
    {
        qCount = count;

        for (int i = 0; i < qSkillGroup.childCount; i++)
        {
            qSkillGroup.GetChild(i).gameObject.SetActive(true);
        }

        isReadyToShootQ = true;
    }

    [Rpc]
    private void RPC_QShoot()
    {
        qCount--;

        qSkillGroup.GetChild(qCount).gameObject.SetActive(false);
    }

    // E Skill
    private void OnRangeFourSkill(InputValue value)
    {
        if (!HasStateAuthority) return;

        if (value.isPressed && !playerController.IsSkilling)
        {
            skillPos = crossHairLookAt.GroundHitPositionTransmission();

            mecanimAnimator.SetTrigger("Skill");
            animator.SetInteger("SkillState", 3);

            coolTimeManager.SkillChecking(3);
        }
    }

    [Rpc]
    public void RPC_StartESkillEffect()
    {
        playableDirector.Play();
    }

    public void OnInstantiateESkillPrefab()
    {
        if (!HasStateAuthority) return;

        NetworkObject eSkillObj = Runner.Spawn(eSkillPrefab, skillPos, transform.rotation);
        eSkillObj.GetComponent<FighterESkill>().SetAttackController(this);
    }

    [Rpc]
    public void RPC_SetEnemyTr()
    {
        List<NetworkObject> networkObjects = Runner.GetAllNetworkObjects();
        foreach (var obj in networkObjects)
        {
            if (obj.TryGetComponent(out PlayerController component))
            {
                if (obj.tag == "Enemy")
                {
                    enemyTr = obj.transform;
                    RPC_SetRockPosition(enemyTr.position);
                }
            }
        }
    }

    public void SetBotTr()
    {
        List<NetworkObject> networkObjects = Runner.GetAllNetworkObjects();
        foreach (var obj in networkObjects)
        {
            if (obj.TryGetComponent(out BotController botComponent))
            {
                if (obj.tag == "Enemy")
                {
                    enemyTr = obj.transform;
                    skillPos = obj.transform.position;
                }
            }
        }
    }

    [Rpc]
    private void RPC_SetRockPosition(Vector3 pos)
    {
        skillPos = pos;
    }

    public void OnUltEffectStart()
    {
        ultCamera.gameObject.SetActive(true);
        //GameObject rock1 = Instantiate(eSkillRockFront, enemyTr.position - (enemyTr.position - transform.position).normalized * 2, Quaternion.identity);
        GameObject rock1 = Instantiate(eSkillRockFront, skillPos - (skillPos - transform.position).normalized * 2, Quaternion.identity);
        //GameObject rock2 = Instantiate(eSkillRockBack, enemyTr.position + (enemyTr.position - transform.position).normalized * 2, Quaternion.identity);
        GameObject rock2 = Instantiate(eSkillRockBack, skillPos + (skillPos - transform.position).normalized * 2, Quaternion.identity);
        rock1.transform.LookAt(rock2.transform);
        rock2.transform.LookAt(rock1.transform);
    }

    public void OnUltEffectEnd()
    {
        ultCamera.gameObject.SetActive(false);
    }

    public void OnUltEnemyHit()
    {
        if (enemyTr.TryGetComponent(out PlayerDamageController enemyComponent))
        {
            enemyComponent.RPC_TakeDamage(10f, PlayerDamageController.PlayerHitType.Down, true, 1f, enemyTr.position);
        }
        else if (enemyTr.TryGetComponent(out BotController botComponent))
        {
            botComponent.TakeDamage(10f, BotController.BotHitType.Down, true, 1f, enemyTr.position);
        }
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
