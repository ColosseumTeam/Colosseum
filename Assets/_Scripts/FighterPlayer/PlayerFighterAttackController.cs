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
        // ��ų �غ� ���(��ų ���� ���� ���)
        isSkillReady = false;
        aimObject.SkillReadyNonActive();
    }

    private void SkillReady(float skillState)
    {
        // ��ų �غ� ���� �ƴ� ��
        if (!isSkillReady)
        {
            // ��ų ���� �� üũ
            isSkillReady = true;
            // ��ų �غ� ����(��ų ���� ���� ����)
            aimObject.SkillReadyAcitve(skillState);
            GetComponent<PlayerController>().SkillReady();
        }
    }

    // ù��° ��ų �ߵ� �޼���
    private void OnRightClick(InputValue value)
    {
        if (value.isPressed && !isSkillReady)
        {
            SkillStateChagned(0f);
        }
    }

    // �ι�° ��ų �ߵ� �޼���
    private void OnQ(InputValue value)
    {
        if (value.isPressed && !isSkillReady)
        {
            SkillStateChagned(1f);
        }
    }

    // ����° ��ų �ߵ� �޼���
    private void OnShiftClick(InputValue value)
    {
        if (value.isPressed && !isSkillReady)
        {
            SkillStateChagned(2f);
        }
    }

    // �׹�° ��ų �ߵ� �޼��� (E)
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

    // �⺻ ���� �� Ư�� �����ӿ��� ����Ǵ� �⺻ ���� �̺�Ʈ
    //public void NormalRangeAttackEvent()
    //{
    //    // ������ ����
    //    GameObject normalObj = Instantiate(rangeNormalSkillPrefab, rangeTransform.position, rangeTransform.rotation);

    //    // ������ ������ RigidBody ����
    //    Rigidbody normalObjRb = normalObj.GetComponent<Rigidbody>();

    //    // ������ �������� RigidBody�� �������� ���ư�����
    //    if (normalObjRb != null)
    //    {
    //        normalObjRb.velocity = transform.forward * rangeNormalPrefabSpeed;
    //    }
    //}

    //// �� ��° ��ų ��� �� Ư�� �����ӿ��� ����Ǵ� ��ų ���� �̺�Ʈ
    //public void TwoRangeSkillAttackEvent()
    //{
    //    // ������ ����
    //    GameObject twoSkillObj = Instantiate(rangeTwoSkillPrefab, rangeTransform.position, rangeTransform.rotation);

    //    // ������ ������ RigidBody ����
    //    Rigidbody twoSkillObjRb = twoSkillObj.GetComponent<Rigidbody>();

    //    // ������ �������� RigidBody�� �������� ���ư�����
    //    if (twoSkillObjRb != null)
    //    {
    //        twoSkillObjRb.velocity = transform.forward * rangeTwoSkillPrefabSpeed;
    //    }
    //}

    //// �� ��° ��ų ��� �� Ư�� �����ӿ��� ����Ǵ� ��ų ���� �̺�Ʈ
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
