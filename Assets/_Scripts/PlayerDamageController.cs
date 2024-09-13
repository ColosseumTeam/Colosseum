using UnityEngine;

public class PlayerDamageController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    private bool isDowning;
    private bool isGrounding;
    private float force = 5f;
    private float downTimer = 0f;
    private float downEndTimer = 3f;

    private void Awake()
    {        
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>(); 
    }

    private void Update()
    {
        DownTimeCheck();
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
            if (!skillType)
            {
                int rnd = Random.Range(0, 2);

                animator.speed = stiffnessTime;

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
            }

            animator.SetBool("TakeHit", true);
        }
    }


    private void DownTimeCheck()
    {
        if (isDowning && isGrounding)
        {
            downTimer += Time.deltaTime;
            if (downTimer >= downEndTimer)
            {
                animator.SetBool("TakeHit", false);
            }
        }
    }

    // 애니메이션 피격 애니메이션 프레임에 설정되어 있는 메서드
    public void TakeHitNonAcitve()
    {        
        isDowning = false;
        animator.SetBool("TakeHit", false);
        GetComponent<PlayerController>().PlayerTakeHitStopAction();
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
