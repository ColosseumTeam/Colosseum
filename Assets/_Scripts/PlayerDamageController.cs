using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamageController : NetworkBehaviour
{
    public enum PlayerHitType
    {
        None,
        Down
    }

    [SerializeField] private Animator animator;
    [SerializeField] private SimpleKCC kcc;
    [SerializeField] private PlayerData playerData;    

    [SerializeField] private bool isDowning;
    [SerializeField] private bool isUping;
    [SerializeField] private bool isGrounding;
    [SerializeField] private float upForce = 12f;
    [SerializeField] private float downTimer = 0f;
    [SerializeField] private float downEndTimer = 3f;


    private GameObject gameManager;
    private float hp;
    private float MaxHp;
    private Image hpBar;
    private Vector3 playerVector;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        kcc = GetComponent<SimpleKCC>();

        gameManager = FindAnyObjectByType<GameManager>().gameObject;
        hpBar = gameManager.GetComponent<GameManager>().HpBar;        
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

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_TakeDamage(float damage, PlayerHitType playerHitType, bool downAttack, float stiffnessTime)
    {
        if (!isDowning || (isDowning || downAttack))
        {
            PlayerHPDecrease(damage);

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

    // HP 감소 메서드
    private void PlayerHPDecrease(float newDamage)
    {
        hp -= newDamage;

        hpBar.fillAmount = hp / MaxHp;

        if(hp <= 0)
        {
            gameManager.GetComponent<ResultSceneConversion>().ResultSceneBringIn();
            Debug.Log("Player Dead");
        }
    }

    public void PlayerDataReceive(PlayerData newPlayerData)
    {
        playerData = newPlayerData;
        MaxHp = playerData.MaxHp;
        hp = MaxHp;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_TakeHitNonAcitve()
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