using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFighterAttackController : MonoBehaviour
{
    [SerializeField] private BehaviourBase.State state = BehaviourBase.State.None;

    [SerializeField] private AimController aimObject;
    [SerializeField] private Transform rangeTransform;
    //[SerializeField] private GameObject rangeNormalSkillPrefab;
    //[SerializeField] private GameObject rangeTwoSkillPrefab;
    //[SerializeField] private GameObject rangeFourSkillPrefab;
    //[SerializeField] private float rangeNormalPrefabSpeed = 15f;
    //[SerializeField] private float rangeTwoSkillPrefabSpeed = 10f;
    //[SerializeField] private float rangeFourSkillPrefabSpeed = 15f;
    //[SerializeField] private float rangeFourSkillSpawnHeight = 20f;

    private Animator animator;
    private bool isSkillReady;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void GetState(BehaviourBase.State newState)
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
    private void OnRightClick(InputValue value)
    {
        if (value.isPressed && !isSkillReady)
        {
            SkillStateChagned(0f);
        }
    }

    // 두번째 스킬 발동 메서드
    private void OnQ(InputValue value)
    {
        if (value.isPressed && !isSkillReady)
        {
            SkillStateChagned(1f);
        }
    }

    // 세번째 스킬 발동 메서드
    private void OnShiftClick(InputValue value)
    {
        if (value.isPressed && !isSkillReady)
        {
            SkillStateChagned(2f);
        }
    }

    // 네번째 스킬 발동 메서드 (E)
    private void OnE(InputValue value)
    {
        if (value.isPressed && !isSkillReady)
        {
            SkillStateChagned(3f);
        }
    }

    private void SkillStateChagned(float state)
    {
        animator.SetFloat("SkillState", state);
        //SkillReady(state);
    }

    // 기본 공격 시 특정 프레임에서 실행되는 기본 공격 이벤트
    //public void NormalRangeAttackEvent()
    //{
    //    // 프리팹 생성
    //    GameObject normalObj = Instantiate(rangeNormalSkillPrefab, rangeTransform.position, rangeTransform.rotation);

    //    // 생성된 프리팹 RigidBody 참조
    //    Rigidbody normalObjRb = normalObj.GetComponent<Rigidbody>();

    //    // 생성된 프리팹의 RigidBody를 정면으로 날아가도록
    //    if (normalObjRb != null)
    //    {
    //        normalObjRb.velocity = transform.forward * rangeNormalPrefabSpeed;
    //    }
    //}

    //// 두 번째 스킬 사용 시 특정 프레임에서 실행되는 스킬 공격 이벤트
    //public void TwoRangeSkillAttackEvent()
    //{
    //    // 프리팹 생성
    //    GameObject twoSkillObj = Instantiate(rangeTwoSkillPrefab, rangeTransform.position, rangeTransform.rotation);

    //    // 생성된 프리팹 RigidBody 참조
    //    Rigidbody twoSkillObjRb = twoSkillObj.GetComponent<Rigidbody>();

    //    // 생성된 프리팹의 RigidBody를 정면으로 날아가도록
    //    if (twoSkillObjRb != null)
    //    {
    //        twoSkillObjRb.velocity = transform.forward * rangeTwoSkillPrefabSpeed;
    //    }
    //}

    //// 네 번째 스킬 사용 시 특정 프레임에서 실행되는 스킬 공격 이벤트
    //public void FourRangeSkillAttackEvent()
    //{
    //    Vector3 fourSkillObjPosition = new Vector3(
    //        aimObject.GetGroundIndicatorCenter().x,
    //        aimObject.GetGroundIndicatorCenter().y + rangeFourSkillSpawnHeight,
    //        aimObject.GetGroundIndicatorCenter().z);

    //    GameObject fourSkillObj = Instantiate(rangeFourSkillPrefab, fourSkillObjPosition, Quaternion.identity);

    //    Rigidbody fourSkillObjRb = fourSkillObj.GetComponent<Rigidbody>();

    //    if (fourSkillObjRb != null)
    //    {
    //        fourSkillObjRb.velocity = Vector3.down * rangeFourSkillPrefabSpeed;
    //    }
    //}
}
