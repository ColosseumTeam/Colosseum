using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRangeAttackController : MonoBehaviour
{
    [SerializeField] private PlayerRangeAttackBehaviour.State state = PlayerRangeAttackBehaviour.State.None;

    [SerializeField] private AimController aimObject;
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
    private Animator animator;
    private bool isSkillReady;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    public void GetState(PlayerRangeAttackBehaviour.State newState)
    {
        state = newState;
    }

    public void SkillReadyNonActive()
    {
        // 스킬 준비 취소(스킬 범위 조준 취소)
        isSkillReady = false;
        aimObject.SkillReadyNonActive();
    }

    private void SkillReady(float skillState)
    {
        // 스킬 준비 중이 아닐 때
        if (!isSkillReady)
        {
            // 스킬 실행 중 체크
            isSkillReady = true;
            // 스킬 준비 실행(스킬 범위 조준 실행)
            aimObject.SkillReadyAcitve(skillState);
            GetComponent<PlayerController>().SkillReady();
        }
    }

    // 첫번째 스킬 발동 메서드
    private void OnRangeOneSkill(InputValue value)
    {
        if (value.isPressed && !isSkillReady)
        {
            SkillStateChagned(0f);
        }
    }

    // 두번째 스킬 발동 메서드
    private void OnRangeTwoSkill(InputValue value)
    {
        if (value.isPressed && !isSkillReady)
        {
            SkillStateChagned(1f);
        }
    }

    // 세번째 스킬 발동 메서드
    private void OnRangeThreeSkill(InputValue value)
    {
        if (value.isPressed && !isSkillReady)
        {
            SkillStateChagned(2f);
        }
    }

    // 네번째 스킬 발동 메서드
    private void OnRangeFourSkill(InputValue value)
    {
        if (value.isPressed && !isSkillReady)
        {
            SkillStateChagned(3f);
        }
    }

    private void SkillStateChagned(float state)
    {
        animator.SetFloat("SkillState", state);
        SkillReady(state);
    }

    // 기본 공격 시 특정 프레임에서 실행되는 기본 공격 이벤트
    public void NormalRangeAttackEvent()
    {
        // rangeTransform의 위치와 회전을 사용하여 오브젝트 생성
        GameObject normalObj = Instantiate(rangeNormalSkillPrefab, rangeTransform.position, rangeTransform.rotation);

        Rigidbody normalObjRb = normalObj.GetComponent<Rigidbody>();

        if (normalObjRb != null)
        {
            normalObjRb.velocity = rangeTransform.forward * rangeNormalPrefabSpeed;
        }
    }

    // 첫 번째 스킬 사용 시 특정 프레임에서 실행되는 스킬 공격 이벤트
    public void OneRangeAttackEvent()
    {
        // aimObject.GetGroundIndicatorCenter()로부터 일정 거리 아래에서 오브젝트 생성
        Vector3 oneSkillObjPosition = new Vector3(
            aimObject.GetGroundIndicatorCenter().x,
            aimObject.GetGroundIndicatorCenter().y - 0.9f, 
            aimObject.GetGroundIndicatorCenter().z);

        // 오브젝트 생성
        GameObject oneSkillObj = Instantiate(rangeOneSkillPrefab, oneSkillObjPosition, Quaternion.identity);

        playerController.SkillEnd();
    }

    // 두 번째 스킬 사용 시 특정 프레임에서 실행되는 스킬 공격 이벤트
    public void TwoRangeSkillAttackEvent()
    {
        // 프리팹 생성
        GameObject twoSkillObj = Instantiate(rangeTwoSkillPrefab, rangeTransform.position, rangeTransform.rotation);

        // 생성된 프리팹 RigidBody 참조
        Rigidbody twoSkillObjRb = twoSkillObj.GetComponent<Rigidbody>();

        // 생성된 프리팹의 RigidBody를 정면으로 날아가도록
        if (twoSkillObjRb != null)
        {
            twoSkillObjRb.velocity = transform.forward * rangeTwoSkillPrefabSpeed;
        }

        playerController.SkillEnd();
    }

    // 세 번째 스킬 사용 시 특정 프레임에서 실행되는 스킬 공격 이벤트
    public void ThreeRangeSkillAttackEvent()
    {
        float offsetRotation = 15f;
        GameObject[] prefabs = new GameObject[2];

        for (int ix = 0; ix < 2; ix++)
        {
            GameObject threeSkillObj = Instantiate(rangeThreeSkillPrefab, rangeThreeSkillTransform.position, rangeThreeSkillTransform.rotation);
            prefabs[ix] = threeSkillObj;

            Vector3 rotation = threeSkillObj.transform.eulerAngles;
            rotation.z = offsetRotation;
            threeSkillObj.transform.eulerAngles = rotation;

            offsetRotation = -15f;
        }

        foreach(var obj in prefabs)
        {
            obj.GetComponent<RangePlayerThreeAttack>().GetPlayer(rangeThreeSkillTransform);
        }

        playerController.SkillEnd();
    }

    // 네 번째 스킬 사용 시 특정 프레임에서 실행되는 스킬 공격 이벤트
    public void FourRangeSkillAttackEvent()
    {
        Vector3 fourSkillObjPosition = new Vector3(
            aimObject.GetGroundIndicatorCenter().x,
            aimObject.GetGroundIndicatorCenter().y + rangeFourSkillSpawnHeight,
            aimObject.GetGroundIndicatorCenter().z);

        GameObject fourSkillObj = Instantiate(rangeFourSkillPrefab, fourSkillObjPosition, Quaternion.identity);

        Rigidbody fourSkillObjRb = fourSkillObj.GetComponent<Rigidbody>();

        if (fourSkillObjRb != null)
        {
            fourSkillObjRb.velocity = Vector3.down * rangeFourSkillPrefabSpeed;
        }

        playerController.SkillEnd();
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
