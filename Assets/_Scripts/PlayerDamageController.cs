using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using static BotController;


public class PlayerDamageController : NetworkBehaviour
{
    public enum PlayerHitType
    {
        None,
        Down
    }

    [SerializeField] private Animator animator;
    [SerializeField] private SimpleKCC kcc;

    [SerializeField] private bool isDowning;
    [SerializeField] private bool isUping;
    [SerializeField] private bool isGrounding;
    [SerializeField] private float upForce = 12f;
    [SerializeField] private float downTimer = 0f;
    [SerializeField] private float downEndTimer = 3f;

    private Vector3 playerVector;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        kcc = GetComponent<SimpleKCC>();
    }

    public override void FixedUpdateNetwork()
    {
        DownTimeCheck();

        if (isUping && gameObject.transform.position.y <= playerVector.y + upForce)
        {
            kcc.Move(jumpImpulse: upForce);
            isUping = false;
        }
        else
        {
            kcc.Move();
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

    public void TakeDamage(float damage, PlayerHitType playerHitType, bool downAttack, float stiffnessTime)
    {
        if (!isDowning || (isDowning || downAttack))
        {
            switch (playerHitType)
            {
                case PlayerHitType.None:
                    int rnd = Random.Range(0, 2);
                    animator.speed = stiffnessTime;
                    animator.SetFloat("TakeHitState", rnd);
                    animator.SetTrigger("TakeHit");

                    break;

                case PlayerHitType.Down:
                    animator.SetFloat("TakeHitState", 2);
                    if (isDowning)
                    {
                        animator.SetFloat("TakeHitState", 3);
                    }

                    isDowning = true;
                    isUping = true;
                    playerVector = gameObject.transform.position;
                    animator.SetTrigger("TakeHit");

                    break;
            }
        }
    }

    private void DownTimeCheck()
    {
        if (isDowning && isGrounding)
        {
            downTimer += Time.deltaTime;
            if (downTimer >= downEndTimer)
            {
                isDowning = false;
                downTimer = 0;
                animator.SetTrigger("Idle");
            }
        }
    }

    // 애니메이션 피격 애니메이션 프레임에 설정되어 있는 메서드
    public void TakeHitNonAcitve()
    {
        animator.SetTrigger("Idle");
        GetComponent<PlayerController>().PlayerTakeHitStopAction();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Ground")
        {
            isGrounding = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Ground")
        {
            isGrounding = false;
        }
    }
}