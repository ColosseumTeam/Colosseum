using System.Collections;
using UnityEngine;

public class BotController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    [SerializeField] private bool isDowning;
    [SerializeField] private bool isGrounding;
    [SerializeField] private float downTime = 0f;
    [SerializeField] private float downEndTime = 3f;
    [SerializeField] private float force = 10f;
    [SerializeField] private float attackReadyTime = 0f;
    [SerializeField] private float attackReadyEndTime = 5f;
    [SerializeField] private DamageManager damageManager;

    [SerializeField] private Transform botAttackPosition;
    [SerializeField] private GameObject botSkillObj;
    [SerializeField] private float botSkillSpeed = 15f;

    private bool isAttacking = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!isAttacking)
        {
            attackReadyTime += Time.deltaTime;
            if(attackReadyTime >= attackReadyEndTime)
            {
                animator.SetTrigger("Attack");
                attackReadyTime = 0f;
            }
        }        

        if (isDowning && isGrounding)
        {
            downTime += Time.deltaTime;
            if(downTime >= downEndTime )
            {
                IdleAnimationChanged();
            }
        }
    }

    // skillType : false => 경직 피격
    // skillType : true => 다운 판정
    // downAttack : false => 다운 판정 시 피격X
    // downAttack : true => 다운 판정 시 피격O 
    // stiffnessTime : 경직 시간
    // TakeHitState : 0, 1 => 피격 모션
    // TakeHitState : 2 => 다운 모션
    // TakeHitState : 3 => 다운 상태 유지 모션
    public void TakeDamage(float damage, bool skillType, bool downAttack, float stiffnessTime)
    {
        if (!isDowning || (isDowning && downAttack))
        {
            // 경직 스킬을 맞았을 때
            if (!skillType)
            {
                int rnd = Random.Range(0, 2);

                animator.speed = stiffnessTime;

                animator.SetFloat("TakeHitState", rnd);
            }
            // 다운 스킬을 맞았을 때
            else
            {
                if (!isDowning) 
                {
                    animator.SetFloat("TakeHitState", 2);
                }
                else if (isDowning)
                {
                    animator.SetFloat("TakeHitState", 3);
                }
                
                isDowning = true;

                Vector3 upForce = new Vector3(0, force, 0);
                rb.AddForce(upForce, ForceMode.Impulse);                
            }

            animator.SetTrigger("TakeHit");
        }
    }

    public void IdleAnimationChanged()
    {
        animator.speed = 1f;
        animator.SetTrigger("Idle");        

        isDowning = false;
        downTime = 0f;
    }

    public void BotNormalAttack()
    {
        GameObject normalObj = Instantiate(botSkillObj, botAttackPosition.position, botAttackPosition.rotation);

        Rigidbody normalObjRb = normalObj.GetComponent<Rigidbody>();

        if (normalObjRb != null)
        {
            normalObj.GetComponent<ISkill>().GetDamageManager(damageManager);
            normalObjRb.velocity = botAttackPosition.forward * botSkillSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            isGrounding = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounding = false;
    }

}
