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
    [SerializeField] private bool isUp;
    [SerializeField] private bool isGrounding;
    [SerializeField] private float upForce = 12f;
    [SerializeField] private float downTimer = 0f;
    [SerializeField] private float downEndTimer = 3f;
    [SerializeField] private GameObject hitEffectPrefab;

    [SerializeField] private float airTimer = 0f;
    [SerializeField] private float airEndTimer = 0.5f;
    private bool airCheck;
    private Vector3 airPosition;

    private GameManager gameManager;
    private PlayerController playerController;
    private float hp;
    private float maxHp;
    private Image hpBar;
    private Vector3 playerVector;

    public float Hp { get { return hp; } }
    public float MaxHp { get { return maxHp; } }
    public PlayerData PlayerData { get { return playerData; } }

    
    private void Start()
    {
        playerData = GetComponent<PlayerController>().PlayerData;
        maxHp = playerData.MaxHp;
        hp = maxHp;

        playerController = GetComponent<PlayerController>();
        mecanimAnimator = GetComponent<NetworkMecanimAnimator>();
        animator = GetComponent<Animator>();
        kcc = GetComponent<SimpleKCC>();

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            hpBar = gameManager.HpBar;
            hpBar.fillAmount = hp / maxHp;
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
        RPC_GroundCheck(kcc.IsGrounded);

        // 위로 올라가는 상태라면 플레이어를 점프시킴
        if (isUping && transform.position.y <= playerVector.y + upForce)
        {
            isUp = true;
            kcc.Move(jumpImpulse: upForce);
        }
        else
        {
            isUping = false;
            if (airCheck)
            {
                airTimer += Runner.DeltaTime;

                if (airTimer < airEndTimer)
                {
                    return;
                }
            }

            airCheck = false;
            airTimer = 0;
            kcc.Move(); // 일반 이동 처리
        }

        if (kcc.IsGrounded && isUp)
        {
            EndAirTime();
            RPC_EndAirTime();
            isUp = false;
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

        playerController.BoostStop();

        // 소유 클라이언트에서만 HandleLocalDamageVisuals 호출
        HandleLocalDamageVisuals(damage, playerHitType, downAttack, stiffnessTime, skillPosition);

        // 다른 클라이언트에 피해 이벤트 알림
        RPC_TakeDamage(damage, playerHitType, downAttack, stiffnessTime, skillPosition);
    }

    // 모든 다른 클라이언트에 피해 시각적 효과와 애니메이션을 동기화
    [Rpc(RpcSources.All, RpcTargets.All, InvokeLocal = false)]
    public void RPC_TakeDamage(float damage, PlayerHitType playerHitType, bool downAttack, float stiffnessTime, Vector3 skillPosition)
    {
        if (!HasStateAuthority)
        {
            playerController.BoostStop();
            return;
        }

        RPC_HandleRemoteDamageVisuals(damage, playerHitType, downAttack, stiffnessTime, skillPosition);
    }

    // 소유 클라이언트에서 즉각적인 시각적 효과와 애니메이션을 처리
    private void HandleLocalDamageVisuals(float damage, PlayerHitType playerHitType, bool downAttack, float stiffnessTime, Vector3 skillPosition)
    {
        // 애니메이션 트리거
        switch (playerHitType)
        {
            case PlayerHitType.None:
                if (isDowning && isGrounding) { break; }

                int rnd = Random.Range(0, 2);
                animator.speed = stiffnessTime;
                animator.SetFloat("TakeHitState", rnd);
                animator.SetTrigger("TakeHit");

                Instantiate(hitEffectPrefab, skillPosition, Quaternion.identity);
                //mecanimAnimator.SetTrigger("TakeHit");
                break;

            case PlayerHitType.Down:

                if (!downAttack && isDowning && isGrounding)
                {
                    break;
                }

                animator.SetFloat("TakeHitState", 2);
                animator.SetTrigger("TakeHit");

                Instantiate(hitEffectPrefab, skillPosition, Quaternion.identity);
                //mecanimAnimator.SetTrigger("TakeHit");
                break;
        }

        if (playerHitType == PlayerHitType.Down)
        {
            // 다운 상태에서 downAttack이 부정형일 때는 적용이 되지 않도록 해야 함.
            if (!downAttack && isDowning && isGrounding)
            {

            }

            else
            {
                isDowning = true;
                isUping = true;
                playerVector = transform.position;
            }

        }

        else if (playerHitType == PlayerHitType.None && isDowning && !isGrounding)
        {
            airPosition = transform.position;
            airCheck = true;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    // 원격 클라이언트에서 시각적 효과와 애니메이션을 처리
    private void RPC_HandleRemoteDamageVisuals(float damage, PlayerHitType playerHitType, bool downAttack, float stiffnessTime, Vector3 skillPosition)
    {
        if (!HasStateAuthority)
        {
            playerController.BoostStop();
            return;
        }

        Debug.Log(playerHitType);

        switch (playerHitType)
        {
            case PlayerHitType.None:
                if (isDowning && isGrounding) { break; }

                int rnd = Random.Range(0, 2);
                animator.speed = stiffnessTime;
                animator.SetFloat("TakeHitState", rnd);
                animator.SetTrigger("TakeHit");

                // 권한 있는 클라이언트에서만 HP 감소 처리
                if (HasStateAuthority)
                {
                    Instantiate(hitEffectPrefab, skillPosition, Quaternion.identity);
                    PlayerHPDecrease(damage);
                }

                //mecanimAnimator.SetTrigger("TakeHit");
                break;

            case PlayerHitType.Down:

                if (!downAttack && isDowning && isGrounding)
                {
                    break;
                }

                // 권한 있는 클라이언트에서만 HP 감소 처리
                if (HasStateAuthority)
                {
                    Instantiate(hitEffectPrefab, skillPosition, Quaternion.identity);
                    PlayerHPDecrease(damage);
                }

                animator.SetFloat("TakeHitState", 2);
                animator.SetTrigger("TakeHit");
                //mecanimAnimator.SetTrigger("TakeHit");
                break;
        }

        // 추가 상태 변경
        if (playerHitType == PlayerHitType.Down)
        {
            if (!downAttack && isDowning && isGrounding)
            {

            }
            else
            {
                isDowning = true;
                isUping = true;
                playerVector = transform.position;
            }
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
            hpBar.fillAmount = hp / maxHp;

            if (hp <= 0)
            {
                CharacterSelectManager characterSelectManager = FindObjectOfType<CharacterSelectManager>();

                gameManager.GetComponent<ResultSceneConversion>().RPC_ResultSceneBringIn(characterSelectManager.EnemyCharacterNumber, characterSelectManager.MyCharacterNumber);

                MotionTrailGenerator[] motionTrailGenerators = GetComponentsInChildren<MotionTrailGenerator>();
                for (int i = 0; i < motionTrailGenerators.Length; i++)
                {
                    motionTrailGenerators[i].enabled = false;
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

    private void EndAirTime()
    {
        Debug.Log("EndAirTimeOn");
        animator.SetFloat("TakeHitState", 2);
        animator.ResetTrigger("TakeHit");
        animator.SetTrigger("TakeHit");
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_EndAirTime()
    {
        //if (HasStateAuthority)
        //{
            Debug.Log("RPCEndAirTimeOn");
            animator.SetFloat("TakeHitState", 2);
            animator.ResetTrigger("TakeHit");
            animator.SetTrigger("TakeHit");
        //}
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_GroundCheck(bool newState)
    {
        isGrounding = newState;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void RPC_UpCheck(bool newState)
    {
        isUping = newState;
    }

   
    public void DownTimeChanged(float newDownTime)
    {
        Debug.Log(newDownTime);
        downEndTimer = newDownTime;
        RPC_DownTimeChanged(downEndTimer);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_DownTimeChanged(float newDownTime)
    {
        downEndTimer = newDownTime;
    }
}