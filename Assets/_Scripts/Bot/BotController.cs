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

    // damageType : 0 => 경직 피격 타입
    // damageType : 2 => 다운 판정 타입.
    // TakeHitState : 0, 1 => 경직 피격
    // TakeHitState : 2 => 다운 판정
    // downAttack : false => 다운 판정 시 피격X
    // downAttack : true => 다운 판정 시 피격O 
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

                // 캐릭터를 일정량 위로 밀어주는 힘을 추가
                Vector3 upForce = new Vector3(0, force, 0); // y축으로 5의 힘을 가함 (값은 필요에 맞게 조절 가능)
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
