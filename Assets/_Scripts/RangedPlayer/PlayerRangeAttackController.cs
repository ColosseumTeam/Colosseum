using Fusion;
using Fusion.Addons.SimpleKCC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
//using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerRangeAttackController : NetworkBehaviour
{
    [SerializeField] private PlayerRangeAttackBehaviour.State state = PlayerRangeAttackBehaviour.State.None;

    [SerializeField] private DamageManager damageManager;
    [SerializeField] private AimController aimObject;
    [SerializeField] private CrossHairLookAt corssHairLookAt;
    [SerializeField] private Transform rangeTransform;
    [SerializeField] private Transform rangeThreeSkillTransform;

    [SerializeField] private GameObject rangeNormalSkillPrefab;
    [SerializeField] private GameObject rangeOneSkillPrefab;
    [SerializeField] private GameObject rangeTwoSkillPrefab;
    [SerializeField] private GameObject rangeThreeSkillPrefab;
    [SerializeField] private GameObject rangeFourSkillPrefab;

    [SerializeField] private float rangeNormalPrefabSpeed = 15f;
    [SerializeField] private float rangeTwoSkillPrefabSpeed = 10f;
    [SerializeField] private float rangeFourSkillPrefabSpeed = 15f;
    [SerializeField] private float rangeFourSkillSpawnHeight = 20f;

    private PlayerController playerController;
    private RangePlayerCoolTImeManager rangePlayerCoolTImeManager;
    private Animator animator;
    private NetworkMecanimAnimator mecanimAnimator;
    private Vector3 rangeHitPosition;
    private int isCoolTimeSkill;
    private bool isSkillReady;
    private bool isOneSkillReady;
    [SerializeField] private GameObject cameraRig;
    private GameElementsSynchronizer gameElementsSynchronizer;

    private void Awake()
    {
        aimObject = FindAnyObjectByType<AimController>();
        damageManager = FindAnyObjectByType<DamageManager>();
        gameElementsSynchronizer = FindAnyObjectByType<GameElementsSynchronizer>();

        animator = GetComponent<Animator>();
        mecanimAnimator = GetComponent<NetworkMecanimAnimator>();
        playerController = GetComponent<PlayerController>();
        rangePlayerCoolTImeManager = GetComponent<RangePlayerCoolTImeManager>();       
    }

    public void GetState(PlayerRangeAttackBehaviour.State newState)
    {
        state = newState;
    }

    // 스킬에 따른 애니메이션 선정 메서드 및 스킬 준비를 알리는 메서드
    private void SkillStateChagned(float state)
    {
        if (!isSkillReady)
        {
            isSkillReady = true;
            animator.SetFloat("SkillState", state);

            RangeAttackOn();
        }
    }

    // 스킬 준비 및 시전 상태를 취소하는 메서드
    public void SkillReadyNonActive()
    {
        // 스킬 준비 취소(스킬 범위 조준 취소)
        isSkillReady = false;
        playerController.SkillEnd();
        aimObject.AimSkillReadyNonActive();
    }

    private void HitPositionGrasp(int index)
    {
        aimObject.SkillReadyAcitve(index);

        if (index == 0)
        {
            rangeHitPosition = corssHairLookAt.GroundHitPositionTransmission();
        }

        if (index == 3)
        {
            rangeHitPosition = new Vector3(
                corssHairLookAt.GroundHitPositionTransmission().x,
                corssHairLookAt.GroundHitPositionTransmission().y + rangeFourSkillSpawnHeight,
                corssHairLookAt.GroundHitPositionTransmission().z
                );
        }
    }

    private void RangeAttackOn()
    {
        HitPositionGrasp(isCoolTimeSkill);

        mecanimAnimator.SetTrigger("Skill");

        // 스킬 쿨타임 시작
        rangePlayerCoolTImeManager.SkillChecking(isCoolTimeSkill);

        GetComponent<PlayerController>().SkillReady();
    }

    private void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            // 두 개의 버튼을 클릭해야 하는 SkillOne은 예외처리
            if (isOneSkillReady)
            {
                SkillStateChagned(0f);                

                animator.SetFloat("SkillState", 0f);

                RangeAttackOn();

            }
        }
    }

    // 첫번째 스킬 발동 메서드
    private void OnRangeOneSkill(InputValue value)
    {
        float input = value.Get<float>();

        if (input == 1f && !isSkillReady && rangePlayerCoolTImeManager.SkillCheckLis[0] && HasStateAuthority)
        {
            isCoolTimeSkill = 0;

            isOneSkillReady = true;
        }
        else if (input == 0f)
        {
            isOneSkillReady = false;
        }
    }

    // 두번째 스킬 발동 메서드
    private void OnRangeTwoSkill(InputValue value)
    {
        if (value.isPressed && !isSkillReady && rangePlayerCoolTImeManager.SkillCheckLis[1] && HasStateAuthority)
        {
            isCoolTimeSkill = 1;

            SkillStateChagned(1f);
        }
    }

    // 세번째 스킬 발동 메서드
    private void OnRangeThreeSkill(InputValue value)
    {
        if (value.isPressed && !isSkillReady && rangePlayerCoolTImeManager.SkillCheckLis[2] && HasStateAuthority)
        {
            isCoolTimeSkill = 2;
            SkillStateChagned(2f);
        }
    }

    // 네번째 스킬 발동 메서드
    private void OnRangeFourSkill(InputValue value)
    {
        if (value.isPressed && !isSkillReady && rangePlayerCoolTImeManager.SkillCheckLis[3] && HasStateAuthority)
        {
            isCoolTimeSkill = 3;
            
            cameraRig = GetComponentInChildren<RangePlayerFourAttackCameraChanged>().gameObject;
            cameraRig.GetComponent<RangePlayerFourAttackCameraChanged>().ActivateCloseUp();

            playerController.RangerESkillActiveCheck();
            SkillStateChagned(3f);
        }
    }

    // 기본 공격 시 특정 프레임에서 실행되는 기본 공격 이벤트
    public void NormalRangeAttackEvent()
    {
        if (!HasStateAuthority) return;

        // rangeTransform의 위치와 회전을 사용하여 오브젝트 생성
        NetworkObject normalObj = Runner.Spawn(rangeNormalSkillPrefab, rangeTransform.position, rangeTransform.rotation);

        Rigidbody normalObjRb = normalObj.GetComponent<Rigidbody>();
        normalObj.GetComponent<RangePlayerNormalAttack>().Look(aimObject.transform.position);
        normalObj.GetComponent<RangePlayerNormalAttack>().GetRangePlayer(gameObject, gameElementsSynchronizer);

    }

    // 첫 번째 스킬 사용 시 특정 프레임에서 실행되는 스킬 공격 이벤트
    public void OneRangeAttackEvent()
    {
        if (!HasStateAuthority) return;

        // aimObject.GetGroundIndicatorCenter()로부터 일정 거리 아래에서 오브젝트 생성
        Vector3 oneSkillObjPosition = rangeHitPosition;

        // 오브젝트 생성
        NetworkObject oneSkillObj = Runner.Spawn(rangeOneSkillPrefab, oneSkillObjPosition, Quaternion.identity);
        oneSkillObj.GetComponent<RangePlayerOneAttack>().RPC_SetVolume();
        oneSkillObj.GetComponent<RangePlayerOneAttack>().GetRangePlayer(gameObject);
        corssHairLookAt.EndPointDistanceChanged(3f);


    }

    // 두 번째 스킬 사용 시 특정 프레임에서 실행되는 스킬 공격 이벤트
    public void TwoRangeSkillAttackEvent()
    {
        if (!HasStateAuthority) return;

        // 프리팹 생성
        NetworkObject twoSkillObj = Runner.Spawn(rangeTwoSkillPrefab, rangeTransform.position, rangeTransform.rotation);
        twoSkillObj.GetComponent<RangePlayerTwoAttack>().RPC_SetVolume();

        // 생성된 프리팹 RigidBody 참조
        Rigidbody twoSkillObjRb = twoSkillObj.GetComponent<Rigidbody>();

        cameraRig = GetComponentInChildren<RangePlayerFourAttackCameraChanged>().gameObject;
        CameraRotation cr = cameraRig.GetComponentInChildren<CameraRotation>();
        cr.CameraShake();

        // 생성된 프리팹의 RigidBody를 정면으로 날아가도록
        if (twoSkillObjRb != null)
        {
            //twoSkillObj.GetComponent<ISkill>().GetDamageManager(damageManager);
            twoSkillObjRb.velocity = transform.forward * rangeTwoSkillPrefabSpeed;
        }

        twoSkillObj.GetComponent<RangePlayerTwoAttack>().GetRangePlayer(gameObject);
    }

    // 세 번째 스킬 사용 시 특정 프레임에서 실행되는 스킬 공격 이벤트
    public void ThreeRangeSkillAttackEvent()
    {
        if (!HasStateAuthority) return;

        Transform centerObject = rangeThreeSkillTransform;
        float orbitRadius = 1.5f;
        float orbitSpeed = 300f;
        float angleBetweenOrbits = 180f;
        int numberOfOrbits = 2;

        if (rangeThreeSkillPrefab == null || centerObject == null)
        {
            return;
        }

        Vector3 centerPosition = centerObject.position;

        for (int i = 0; i < numberOfOrbits; i++)
        {            
            NetworkObject orbitInstance = Runner.Spawn(rangeThreeSkillPrefab, centerPosition, Quaternion.identity, null);
            orbitInstance.GetComponent<RangePlayerThreeAttack>().RPC_SetVolume();

            RangePlayerThreeAttack orbitScript = orbitInstance.GetComponent<RangePlayerThreeAttack>();
            if (orbitScript != null)
            {
                float initialAngle = i * angleBetweenOrbits;

                orbitScript.ThreeSkillOptionChanged(centerObject, orbitRadius, orbitSpeed, initialAngle);

                orbitInstance.transform.position = centerObject.position + new Vector3(
                    Mathf.Cos(initialAngle * Mathf.Deg2Rad),
                    0,
                    Mathf.Sin(initialAngle * Mathf.Deg2Rad)
                ) * orbitRadius;
            }
        }

        gameObject.GetComponentInChildren<RangePlayerThreeAttackDamage>().GetPlayer(gameObject.transform);

    }


    // 네 번째 스킬 사용 시 특정 프레임에서 실행되는 스킬 공격 이벤트
    public void FourRangeSkillAttackEvent()
    {
        if (!HasStateAuthority) { return; }

        Vector3 fourSkillObjPosition = rangeHitPosition;

        NetworkObject fourSkillObj = Runner.Spawn(rangeFourSkillPrefab, fourSkillObjPosition, Quaternion.identity);
        fourSkillObj.GetComponent<RangePlayerFourAttack>().RPC_SetVolume();

        Rigidbody fourSkillObjRb = fourSkillObj.GetComponent<Rigidbody>();

        fourSkillObj.GetComponent<RangePlayerFourAttack>().GetRangePlayer(gameObject, 
            cameraRig.GetComponentInChildren<CameraRotation>().GetComponent<Camera>());

        if (fourSkillObjRb != null)
        {
            //fourSkillObj.GetComponent<ISkill>().GetDamageManager(damageManager);
            //fourSkillObjRb.velocity = Vector3.down * rangeFourSkillPrefabSpeed;
        }
    }


    public void EndESkill()
    {
        cameraRig.GetComponent<RangePlayerFourAttackCameraChanged>().DeactivateCloseUp();
        playerController.RangerESkillNonActiveCheck();
    }

    // 1번 키를 누를 경우 춤 추도록 하는 메서드
    public void OnDance(InputValue value)
    {
        if (value.isPressed)
        {
            animator.SetBool("Dance", true);
            AudioSource audio = GetComponent<AudioSource>();
            audio.Play();
        }
    }

    public void EndDance()
    {
        animator.SetBool("Dance", false);
    }
}
