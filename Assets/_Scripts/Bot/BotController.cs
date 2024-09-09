using UnityEngine;

public class BotController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private bool isDowning;
    [SerializeField] private bool isGrounding;
    [SerializeField] private float downTime = 0f;
    private float downEndTime = 3f;
    private Rigidbody rb;
    private float force = 10f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isDowning && isGrounding)
        {
            downTime += Time.deltaTime;
            if(downTime >= downEndTime )
            {
                IdleAnimationChanged();
            }
        }
    }

    // damageType : 0 => ���� �ǰ� Ÿ��
    // damageType : 2 => �ٿ� ���� Ÿ��.
    // TakeHitState : 0, 1 => ���� �ǰ�
    // TakeHitState : 2 => �ٿ� ����
    // downAttack : false => �ٿ� ���� �� �ǰ�X
    // downAttack : true => �ٿ� ���� �� �ǰ�O 
    public void TakeDamage(float damage, float damageType, bool downAttack)
    {
        if (!isDowning || (isDowning && downAttack))
        {
            if (damageType == 0)
            {
                int rnd = Random.Range(0, 2);
                animator.SetFloat("TakeHitState", rnd);
            }
            else if (damageType == 2)
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

                // ĳ���͸� ������ ���� �о��ִ� ���� �߰�
                Vector3 upForce = new Vector3(0, force, 0); // y������ 5�� ���� ���� (���� �ʿ信 �°� ���� ����)
                rb.AddForce(upForce, ForceMode.Impulse);

                isGrounding = false;
            }

            animator.SetTrigger("TakeHit");
        }
    }

    public void IdleAnimationChanged()
    {
        animator.SetTrigger("Idle");
        isDowning = false;
        downTime = 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounding = true;
        }
    }
}
