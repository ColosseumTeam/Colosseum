using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Fusion.Addons.SimpleKCC;


// 변수
// 1. isSkilling 스킬 준비 나타내는 변수
// 메서드
// 1. SkillReady
// 2. OnAttack의 RangePlayer용 코드
public class PlayerController : NetworkBehaviour
{
    // 캐릭터의 현재 상태를 관리하는 변수
    [SerializeField] private BehaviourBase.State state = BehaviourBase.State.None;
    [SerializeField] private AimController aimController;
    [SerializeField] private PlayerData playerData;

    // 캐릭터 이동 속도, 회전 속도, 공격 관련 타이머 설정
    [SerializeField] private float hp = 100f;
    [SerializeField] private float maxHp = 100f;
    [SerializeField] private Image hpImage;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 60f;
    [SerializeField] private float attackTimer = 0f;
    [SerializeField] private float attackElapsedTime = 0.5f;
    [SerializeField] private float jumpForce = 7f; // 점프 힘
    [SerializeField] private float maxJumpHeight = 2f; // 최대 점프 높이
    [SerializeField] private float startYPosition; // 점프 시작 시 Y축 위치를 저장
    [SerializeField] private float boostSpeed = 15f; // Boost 시 속도

    // 캐릭터 물리적 제어를 위한 Rigidbody 참조
    [SerializeField] private Rigidbody rb;

    [SerializeField] private GameObject cameraRig;
    [SerializeField] private SimpleKCC kcc;

    // 캐릭터 애니메이션 제어를 위한 Animator 참조
    private Animator animator;
    private NetworkMecanimAnimator mecanimAnimator;
    private AudioSource audioSource;

    // 공격 상태 및 이동 방향을 저장하는 변수
    private float attackState;
    private Vector2 moveVec;
    private Vector2 rotationVec;
    private float jumpInput;
    private bool isAttacking;
    private bool isBoosting; // Boost 상태를 나타내는 변수
    private bool isJumping; // jump 상태를 나타내는 변수
    private bool isGrounding = true;
    private bool isSkilling;
    private bool isDowning;
    [SerializeField] private bool isLeftRightAttack;

    public PlayerData PlayerData => playerData;

    private void Awake()
    {
        // Rigidbody와 Animator 컴포넌트를 가져옴
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        mecanimAnimator = GetComponent<NetworkMecanimAnimator>();
        audioSource = GetComponent<AudioSource>();

        //GameObject cameraObject = GetComponentInChildren<Camera>().gameObject;
        //if(cameraObject != null)
        //{
        //    GameObject aimObject = FindAnyObjectByType<AimController>().gameObject;
        //    cameraObject.GetComponent<CrossHairLookAt>().CameraReceive(aimObject);
        //}
    }

    private void Start()
    {
        if (HasStateAuthority)
        {
            cameraRig.SetActive(true);
            transform.tag = "Player";
            kcc.Collider.tag = "Player";
            
            aimController = FindAnyObjectByType<AimController>();

            if (aimController != null)
            {
                aimController.PlayerObjectTransmission(GetComponent<NetworkObject>());
            }
            else
            {
                Debug.Log("Do not have AimController");
            }
        }
        else
        {
            transform.tag = "Enemy";
            kcc.Collider.tag = "Enemy";
        }
    }    

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        // 대미지를 받은 상태라면 활동 중지
        if (state == BehaviourBase.State.Damaged || isDowning == true) { return; }

        // 공격 상태가 아니고 공격 상태값이 0이 아닐 때 (즉, 공격이 진행 중일 때)
        // 점프 중일 때도 공격은 못하도록 설정

        if (state != BehaviourBase.State.Attack && attackState != 0)
        {
            // 공격 타이머가 설정된 시간을 초과했는지 확인
            if (attackTimer >= attackElapsedTime)
            {
                // 공격 상태 초기화 및 애니메이션 상태 초기화
                attackTimer = 0f;
                attackState = 0f;
                animator.SetFloat("AttackState", attackState);
                return;
            }
            // 공격 타이머 증가 (프레임마다)
            attackTimer += Time.deltaTime;
        }
        else if (state == BehaviourBase.State.Attack)
        {
            // 공격 상태일 때는 별도의 처리가 있을 수 있음 (현재 빈 코드 블록)
        }

        // 점프 상태 체크
        UpdateJumpState();

        // 캐릭터 이동 처리 (현재 이동 벡터를 기반으로)
        //Move(moveVec);
        Look(rotationVec);
        //Jump(jumpState);
        InputAction(moveVec, jumpInput);

        if (kcc.IsGrounded)
        {
            animator.SetFloat("JumpHeight", 1f);
        }
    }

    public void SetState(BehaviourBase.State newState)
    {
        // 외부에서 호출 가능한 함수: 캐릭터 상태 변경
        state = newState;
    }

    private void InputAction(Vector2 moveVec, float jumpInput)
    {
        if(state == BehaviourBase.State.Damaged || isDowning == true) { return; }

        // Boost 중일 때의 이동 처리
        if (isBoosting)
        {
            // 입력이 없거나 뒤로 가는 입력일 경우 강제로 앞으로 이동
            if (moveVec == Vector2.zero || moveVec.y < 0)
            {
                moveVec = new Vector2(0, 1); // 이동 벡터를 앞으로 고정
            }
        }

        float jumpState = jumpInput * jumpForce;

        Vector3 newPosition = Vector3.zero;

        // 입력값이 존재할 때만 이동 처리
        if (moveVec != Vector2.zero)
        {
            // Boost 상태일 때는 Boost 속도로 이동
            float speed = isBoosting ? boostSpeed : moveSpeed;

            Vector3 moveDir = new Vector3(moveVec.x, 0, moveVec.y);
            newPosition = transform.TransformDirection(moveDir) * speed * Runner.DeltaTime;

            // 현재 Y 위치 유지
            newPosition.y = 0;

            // 애니메이션 파라미터를 업데이트하여 캐릭터의 움직임을 반영
            animator.SetFloat("Horizontal", moveVec.x);
            animator.SetFloat("Vertical", moveVec.y);

            animator.SetBool("Move", true);
        }
        else
        {
            // 이동 벡터가 0이 아니면 이동 중 애니메이션, 그렇지 않으면 정지 애니메이션 설정
            animator.SetBool("Move", false);
        }

        if (state != BehaviourBase.State.Damaged && !isDowning && !isSkilling && !isAttacking)
        {
            kcc.Move(newPosition, jumpState);
        }
    }

    private void OnMove(InputValue value)
    {
        // 대미지를 받은 상태라면 정지
        //if (state == BehaviourBase.State.Damaged || isDowning || isSkilling) { return; }
        if (isSkilling) { animator.SetBool("Move", false); }

        // 입력 시스템을 통해 이동 입력을 받아서 이동 벡터를 설정
        moveVec = value.Get<Vector2>();

        // 이동 입력이 없으면 달리기 상태를 해제
        if (moveVec == Vector2.zero)
        {
            animator.SetBool("Run", false);
        }
    }
    private void OnLeftStep()
    {
        audioSource.PlayOneShot(playerData.LeftStep);
    }

    private void OnRightStep()
    {
        audioSource.PlayOneShot(playerData.RightStep);
    }


    private void OnLook(InputValue value)
    {
        // 대미지를 받은 상태라면 정지
        if (state == BehaviourBase.State.Damaged || isDowning == true) { return; }

        // 입력 시스템을 통해 회전 입력을 받아서 캐릭터 회전 처리
        Vector2 input = value.Get<Vector2>();

        // 입력값에 따라 캐릭터의 회전 각도를 업데이트
        rotationVec = input;
    }

    private void Look(Vector2 input)
    {
        kcc.AddLookRotation(0, input.x * rotationSpeed * Runner.DeltaTime);
    }

    public void SkillReady()
    {
        isLeftRightAttack = false;
        isSkilling = true;
    }

    public void SkillEnd()
    {
        isSkilling = false;
        isAttacking = false;
    }

    private void OnAttack(InputValue value)
    {
        // 대미지를 받은 상태라면 정지
        if (state == BehaviourBase.State.Damaged || isDowning) { return; }

        // 공격 입력이 감지되고 캐릭터가 공격 중이 아닐 때
        if (value.isPressed && !isAttacking && isGrounding && !isSkilling && HasStateAuthority)
        {
            // 이동 중이라면 이동을 멈춤
            if (rb.velocity != Vector3.zero)
            {
                rb.velocity = Vector3.zero;
            }

            // 공격 플래그 설정
            isAttacking = true;

            // 공격 타이머 초기화
            attackTimer = 0f;

            // 공격 상태값 증가 (콤보 공격 가능)
            attackState += 0.25f;

            // 애니메이션 파라미터 업데이트 및 공격 트리거 설정
            animator.SetFloat("AttackState", attackState);
            mecanimAnimator.SetTrigger("Attack");

            // 상태를 공격 상태로 변경
            state = BehaviourBase.State.Attack;

            // 최대 공격 상태값을 넘으면 초기화 (콤보 리셋)
            if (attackState >= 1)
            {
                attackState = 0;
            }
        }
    }

    private void OnAttackCollider()
    {
        // 공격이 끝났을 때 호출되어 공격 플래그를 해제
        isAttacking = false;
    }

    // 바닥 부스트 정지 메서드
    public void BoostStop()
    {
        isBoosting = false;
        animator.SetBool("Boost", false);
    }

    private void OnBoost(InputValue value)
    {
        // Boost 입력이 감지되었고, 현재 Boost 상태가 아닐 때
        if (value.isPressed && !isBoosting)
        {
            // 현재 이동 벡터가 뒤로 향하는지 확인
            if (moveVec.y < 0)
            {
                // 뒤로 이동 중이라면 Boost를 실행하지 않음
                return;
            }

            // Boost 상태 시작
            isBoosting = true;
            animator.SetBool("Boost", true);

            // Boost 상태에서 애니메이션 트리거
            animator.SetTrigger("Boost");
        }
    }

    private void OnJump(InputValue value)
    {
        // 대미지를 받은 상태라면 정지
        if (state == BehaviourBase.State.Damaged || isDowning == true || !HasStateAuthority) { return; }

        if (value.isPressed && !isJumping && isGrounding && !isAttacking)
        {
            jumpInput = value.Get<float>();

            animator.SetTrigger("Jump");

            isGrounding = false;
        }
        else
        {
            jumpInput = 0;
        }
    }

    // Start 애니메이션 끝 프레임 애니메이션 이벤트로 실행
    public void OnJumpKeepChangeEvent()
    {
        isJumping = true;
        animator.SetFloat("JumpHeight", 0.33f);
    }

    // Keep 애니메이션 끝 프레임 애니메이션 이벤트로 실행
    public void OnJumpLoopChangeEvent()
    {
        animator.SetFloat("JumpHeight", 0.66f);
    }

    // 땅에 도달했을 때 실행되는 Wait 애니메이션 끝 프레임 이벤트로 실행
    public void OnJumpWaitChangeEvnet()
    {
        animator.SetFloat("JumpHeight", 0f);
        isGrounding = true;
    }

    private void UpdateJumpState()
    {
        // 점프가 시작될 때 한 번만 힘을 가함
        if (isJumping)
        {
            // 캐릭터의 초기 Y 위치를 기록
            if (startYPosition == 0)
            {
                startYPosition = transform.position.y;
            }

            // 캐릭터가 최대 점프 높이에 도달했는지 확인
            if (transform.position.y >= startYPosition + maxJumpHeight)
            {
                // 최대 높이에 도달하면 점프를 멈춤
                isJumping = false;
                startYPosition = 0; // Y 위치 초기화
            }

            // y좌표 상승
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // 애니메이션 피격 애니메이션 프레임에 설정되어 있는 메서드
    public void PlayerTakeHitStopAction()
    {
        isSkilling = false;
        isAttacking = false;
    }

    public void DowningManagement(bool check)
    {
        isDowning = check;
    }

}