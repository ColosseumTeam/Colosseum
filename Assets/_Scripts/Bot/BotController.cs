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

    // skillType : false => 경직 피격
    // skillType : true => 다운 판정
    // downAttack : false => 다운 판정 시 피격X
    // downAttack : true => 다운 판정 시 피격O 
    // TakeHitState : 0, 1 => 피격 모션
    // TakeHitState : 2 => 다운 모션
    // TakeHitState : 3 => 다운 상태 유지 모션
    public void TakeDamage(float damage, bool skillType, bool downAttack)
    {
        if (!isDowning || (isDowning && downAttack))
        {
            if (!skillType)
            {
                int rnd = Random.Range(0, 2);
                animator.SetFloat("TakeHitState", rnd);
            }
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
