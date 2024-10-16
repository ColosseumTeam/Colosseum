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
    [SerializeField] private NetworkMecanimAnimator mecanimAnimator;
    [SerializeField] private SimpleKCC kcc;
    [SerializeField] private PlayerData playerData;

    [SerializeField] private bool isDowning;
    [SerializeField] private bool isUping;
    [SerializeField] private bool isGrounding;
    [SerializeField] private float upForce = 12f;
    [SerializeField] private float downTimer = 0f;
    [SerializeField] private float downEndTimer = 3f;
    [SerializeField] private GameObject hitEffectPrefab;

    [SerializeField] private float airTimer = 0f;
    [SerializeField] private float airEndTimer = 0.5f;
    private bool airCheck;
    private Vector3 airPosition;

    private GameObject gameManager;
    private float hp;
    private float MaxHp;
    private Image hpBar;
    private Vector3 playerVector;

    private void Start()
    {
        playerData = GetComponent<PlayerController>().PlayerData;
        MaxHp = playerData.MaxHp;
        hp = MaxHp;

        mecanimAnimator = GetComponent<NetworkMecanimAnimator>();
        animator = GetComponent<Animator>();
        kcc = GetComponent<SimpleKCC>();

        gameManager = FindObjectOfType<GameManager>()?.gameObject;
        if (gameManager != null)
        {
            hpBar = gameManager.GetComponent<GameManager>().HpBar;
        }
    }

    public override void FixedUpdateNetwork()
    {
        DownTimeCheck();

        // KCC 초기화 확인
        if (kcc == null)
        {
            return;
        }

        isGrounding = kcc.IsGrounded;

        if (airCheck)
        {
            airTimer += Runner.DeltaTime;
            if (airTimer < airEndTimer)
            {
                transform.position = airPosition;
                return;
            }
            else
            {
                airCheck = false;
                airTimer = 0f;
            }
        }

        if (isUping && transform.position.y <= playerVector.y + upForce)
        {
            kcc.Move(jumpImpulse: upForce);
            isUping = false;
        }
        else
        {
            kcc.Move();
        }
    }

    // 피해를 입었을 때 호출되는 메서드입니다.    
    /// <param name="damage">입는 피해량</param>
    /// <param name="playerHitType">타격 유형</param>
    /// <param name="downAttack">다운 공격 여부</param>
    /// <param name="stiffnessTime">애니메이션의 강직 시간</param>
    /// <param name="skillPosition">스킬 위치</param>
    public void TakeDamage(float damage, PlayerHitType playerHitType, bool downAttack, float stiffnessTime, Vector3 skillPosition)
    {
        Debug.Log($"TakeDamage 호출됨: damage={damage}, HitType={playerHitType}, DownAttack={downAttack}, StiffnessTime={stiffnessTime}");

        // 소유 클라이언트에서만 HandleLocalDamageVisuals 호출
        HandleLocalDamageVisuals(damage, playerHitType, downAttack, stiffnessTime, skillPosition);

        // 다른 클라이언트에 피해 이벤트 알림
        RPC_TakeDamage(damage, playerHitType, downAttack, stiffnessTime, skillPosition);
    }

    // 모든 다른 클라이언트에 피해 시각적 효과와 애니메이션을 동기화
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_TakeDamage(float damage, PlayerHitType playerHitType, bool downAttack, float stiffnessTime, Vector3 skillPosition)
    {
        Debug.Log("RPC_TakeDamage 호출됨");

        if (!HasStateAuthority)
        {
            return;
        }

        RPC_HandleRemoteDamageVisuals(damage, playerHitType, downAttack, stiffnessTime, skillPosition);

        // 권한 있는 클라이언트에서만 HP 감소 처리
        if (HasStateAuthority)
        {
            PlayerHPDecrease(damage);
        }
    }

    // 소유 클라이언트에서 즉각적인 시각적 효과와 애니메이션을 처리
    private void HandleLocalDamageVisuals(float damage, PlayerHitType playerHitType, bool downAttack, float stiffnessTime, Vector3 skillPosition)
    {         
        Debug.Log("HandleLocalDamageVisuals 호출됨");

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, skillPosition, Quaternion.identity);
            Debug.Log("hiteffect On");
        }

        // 애니메이션 트리거
        switch (playerHitType)
        {
            case PlayerHitType.None:
                int rnd = Random.Range(0, 2);
                animator.speed = stiffnessTime;
                animator.SetFloat("TakeHitState", rnd);
                animator.SetTrigger("TakeHit");
                //mecanimAnimator.SetTrigger("TakeHit");
                break;

            case PlayerHitType.Down:
                animator.SetFloat("TakeHitState", 2);
                animator.SetTrigger("TakeHit");
                //mecanimAnimator.SetTrigger("TakeHit");
                break;
        }

        if (playerHitType == PlayerHitType.Down)
        {
            isDowning = true;
            isUping = true;
            playerVector = transform.position;
            Debug.Log("isDowning 및 isUping 설정됨");
        }
        else if (playerHitType == PlayerHitType.None && isDowning && !isGrounding)
        {
            airPosition = transform.position;
            airCheck = true;
            Debug.Log("airCheck 설정됨");
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    // 원격 클라이언트에서 시각적 효과와 애니메이션을 처리
    private void RPC_HandleRemoteDamageVisuals(float damage, PlayerHitType playerHitType, bool downAttack, float stiffnessTime, Vector3 skillPosition)
    {
        if (!HasStateAuthority)
        {
            return;
        }

        Debug.Log("HandleRemoteDamageVisuals 호출됨");

        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, skillPosition, Quaternion.identity);
        }

        Debug.Log(playerHitType);

        switch (playerHitType)
        {
            case PlayerHitType.None:
                Debug.Log("None Active");
                int rnd = Random.Range(0, 2);
                animator.speed = stiffnessTime;
                animator.SetFloat("TakeHitState", rnd);
                animator.SetTrigger("TakeHit");
                //mecanimAnimator.SetTrigger("TakeHit");
                break;

            case PlayerHitType.Down:
                Debug.Log("Down Active");
                animator.SetFloat("TakeHitState", 2);
                animator.SetTrigger("TakeHit");
                //mecanimAnimator.SetTrigger("TakeHit");
                break;
        }

        // 추가 상태 변경
        if (playerHitType == PlayerHitType.Down)
        {
            isDowning = true;
            isUping = true;
            playerVector = transform.position;
        }
        else if (playerHitType == PlayerHitType.None && isDowning && !isGrounding)
        {
            airPosition = transform.position;
            airCheck = true;
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

    // 플레이어의 HP를 감소시키고 UI를 업데이트    
    private void PlayerHPDecrease(float newDamage)
    {
        if (HasStateAuthority)
        {
            var cameraRotation = GetComponentInChildren<CameraRotation>();
            if (cameraRotation != null)
            {
                cameraRotation.CameraShake();
            }

            hp -= newDamage;
            hpBar.fillAmount = hp / MaxHp;

            if (hp <= 0)
            {
                if (playerData.playerNumber == 0)
                {
                    // gameManager.GetComponent<ResultSceneConversion>().ResultSceneBringIn(1);                    
                }
                else if (playerData.playerNumber == 1)
                {
                    // gameManager.GetComponent<ResultSceneConversion>().ResultSceneBringIn(0);
                }
            }
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_TakeHitNonActive()
    {
        mecanimAnimator.SetTrigger("Idle");
        GetComponent<PlayerController>().PlayerTakeHitStopAction();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounding = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounding = false;
        }
    }

    // 애니메이션 이벤트를 통해 호출되는 메서드 (RPC 호출 대신 사용)
    public void OnTakeHitAnimationEnd()
    {
        animator.ResetTrigger("TakeHit");
        // NetworkMecanimAnimator에는 ResetTrigger가 없으므로 제거
        // animator.SetFloat("TakeHitState", 0); // 필요 시 파라미터 리셋
    }
}