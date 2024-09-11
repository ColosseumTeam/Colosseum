using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class ForPresentation : MonoBehaviour
{
    [SerializeField] private AimController aimController;
    [SerializeField] private GameObject shiftClickSkillPrefab;
    [SerializeField] private LayerMask groundLayerMask;

    private Animator animator;
    private float eState;
    private bool isAttacking;
    private bool isPressedShift;
    private Vector3 skillPos;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnRightClick(InputValue value)
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
            animator.SetFloat("eState", eState);
            eState += 0.5f;
        }
    }

    private void OnQ(InputValue value)
    {
        if (value.isPressed)
        {
            animator.SetTrigger("Skill");
            animator.SetInteger("SkillState", 1);
        }
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
}
