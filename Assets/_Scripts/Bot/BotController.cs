using Fusion;
using System.Collections;
using UnityEngine;

public class BotController : NetworkBehaviour
{
    public enum BotHitType
    {
        None,
        Down
    }

    private BotHitType botHitType;
    private Animator animator;
    private Rigidbody rb;

    [SerializeField] private bool isDowning;
    [SerializeField] private bool isGrounding;
    [SerializeField] private float downTime = 0f;
    [SerializeField] private float downEndTime = 3f;
    [SerializeField] private float upForce = 10f;
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
            if (attackReadyTime >= attackReadyEndTime)
            {
                animator.SetTrigger("Attack");
                attackReadyTime = 0f;
            }
        }

        if (isDowning && isGrounding)
        {
            downTime += Time.deltaTime;
            if (downTime >= downEndTime)
            {
                IdleAnimationChanged();
            }
        }
    }

    public void TakeDamage(float damage, BotHitType newBotHitType, bool downAttack, float stiffnessTime)
    {
        Debug.Log($"{downAttack} && {newBotHitType}");

        if (!isDowning || !isGrounding || (isDowning && downAttack))
        {
            switch (newBotHitType)
            {
                case BotHitType.None:
                    int rnd = Random.Range(0, 2);
                    animator.speed = stiffnessTime;
                    animator.SetFloat("TakeHitState", rnd);
                    animator.SetTrigger("TakeHit");

                    if (!isGrounding)
                    {
                        rb.velocity = Vector3.zero;
                    }

                    break;

                case BotHitType.Down:

                    animator.SetFloat("TakeHitState", 2);

                    if (isDowning)
                    {
                        animator.SetFloat("TakeHitState", 3);
                    }

                    isDowning = true;
                    Vector3 upDamageForce = new Vector3(0, upForce, 0);
                    rb.AddForce(upDamageForce, ForceMode.Impulse);
                    animator.SetTrigger("TakeHit");

                    break;
            }
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
            normalObjRb.velocity = botAttackPosition.forward * botSkillSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounding = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounding = false;
    }

}
