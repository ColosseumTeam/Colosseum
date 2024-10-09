using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class PlayerDamageController : NetworkBehaviour
{
    public enum PlayerHitType
    {
        None,
        Down
    }

    [SerializeField] private Animator animator;
    [SerializeField] private NetworkMecanimAnimator mecanimAnimator;
    [SerializeField] private SimpleKCC kcc;
    [SerializeField] private PlayerData playerData;

    [SerializeField] private bool isDowning;
    [SerializeField] private bool isUping;
    [SerializeField] private bool isGrounding;
    [SerializeField] private bool upDamageCheck;
    [SerializeField] private float upForce = 12f;
    [SerializeField] private float downTimer = 0f;
    [SerializeField] private float downEndTimer = 3f;
    [SerializeField] private GameObject hitEffect;

    [SerializeField] private float airTimer = 0f;
    [SerializeField] private float airEndTimer = 0.5f;
    private bool airCheck;
    private Vector3 airPosition;

    private GameObject gameManager;
    private float hp;
    private float MaxHp;
    private Image hpBar;
    private Vector3 playerVector;

    private void Awake()
    {
        playerData = GetComponent<PlayerController>().PlayerData;
        MaxHp = playerData.MaxHp;
        hp = MaxHp;

        mecanimAnimator = GetComponent<NetworkMecanimAnimator>();
        animator = GetComponent<Animator>();
        kcc = GetComponent<SimpleKCC>();

        gameManager = FindAnyObjectByType<GameManager>().gameObject;
        hpBar = gameManager.GetComponent<GameManager>().HpBar;
    }

    public override void FixedUpdateNetwork()
    {
        DownTimeCheck();

        if (kcc.IsGrounded)
        {
            isGrounding = true;
        }
        else
        {
            isGrounding = false;
        }

        if (airCheck)
        {
            airTimer += Runner.DeltaTime;
            if (airTimer < airEndTimer)
            {
                gameObject.transform.position = airPosition;
                return;
            }

            else
            {
                airCheck = false;
                airTimer = 0f;
            }
        }

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

    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = true)]
    public void RPC_TakeDamage(float damage, PlayerHitType playerHitType, bool downAttack, float stiffnessTime, Vector3 skillPosition)
    {
        Debug.Log($"{damage}, {playerHitType}, {downAttack}, {stiffnessTime}");

        if (!isDowning || !isGrounding || (isDowning && downAttack))
        {
            PlayerHPDecrease(damage);

            if (HasStateAuthority)
            {
                Runner.Spawn(hitEffect, skillPosition);
            }

            switch (playerHitType)
            {
                case PlayerHitType.None:
                    int rnd = Random.Range(0, 2);
                    animator.speed = stiffnessTime;

                    if (isDowning && !isGrounding)
                    {
                        airPosition = gameObject.transform.position;
                        airCheck = true;
                        break;
                    }

                    animator.SetFloat("TakeHitState", rnd);
                    mecanimAnimator.SetTrigger("TakeHit");
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
                    mecanimAnimator.SetTrigger("TakeHit");
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
                mecanimAnimator.SetTrigger("Idle");
            }
        }
    }

    // HP 감소 메서드
    private void PlayerHPDecrease(float newDamage)
    {
        if (HasStateAuthority)
        {
            hp -= newDamage;

            hpBar.fillAmount = hp / MaxHp;

            if (hp <= 0)
            {
                // 플레이어가 패배할 경우 playerNumber의 반대되는 수를 메개변수로 전달
                if (playerData.playerNumber == 0)
                {
                    gameManager.GetComponent<ResultSceneConversion>().ResultSceneBringIn(1);
                }

                else if (playerData.playerNumber == 1)
                {
                    gameManager.GetComponent<ResultSceneConversion>().ResultSceneBringIn(0);
                }
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_TakeHitNonAcitve()
    {
        mecanimAnimator.SetTrigger("Idle");
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