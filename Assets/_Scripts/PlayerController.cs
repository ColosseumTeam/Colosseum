using System;
using UnityEngine;
using UnityEngine.InputSystem;


// 변수
// 1. isSkilling 스킬 준비 나타내는 변수
// 메서드
// 1. SkillReady
// 2. OnAttack의 RangePlayer용 코드
public class PlayerController : MonoBehaviour
{
    // 캐릭터의 현재 상태를 관리하는 변수
    [SerializeField] private BehaviourBase.State state = BehaviourBase.State.None;

    // 캐릭터 이동 속도, 회전 속도, 공격 관련 타이머 설정
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 10f; // 달리기 시 속도
    [SerializeField] private float rotationSpeed = 60f;
    [SerializeField] private float attackTimer = 0f;
    [SerializeField] private float attackElapsedTime = 0.5f;
    //[SerializeField] private float attackAfterDelay = 0.2f;
    [SerializeField] private float jumpForce = 7f; // 점프 힘
    [SerializeField] private float maxJumpHeight = 2f; // 최대 점프 높이
    [SerializeField] private float startYPosition; // 점프 시작 시 Y축 위치를 저장
    [SerializeField] private float boostSpeed = 15f; // Boost 시 속도
    [SerializeField] private float downTimer = 0f;
    [SerializeField] private float downEndTimer = 3f;
    [SerializeField] private float airBorneForce = 5f;

    // 캐릭터 물리적 제어를 위한 Rigidbody 참조
    [SerializeField] private Rigidbody rb;

    // 캐릭터 애니메이션 제어를 위한 Animator 참조
    private Animator animator;

    // 공격 상태 및 이동 방향을 저장하는 변수
    private float attackState;
    private Vector2 moveVec;
    private bool isAttacking;
    private bool isRunning; // 달리기 상태를 나타내는 변수
    private bool runInput; // Shift 입력 상태를 추적하는 변수
    private bool isBoosting; // Boost 상태를 나타내는 변수
    private bool isJumping; // jump 상태를 나타내는 변수
    private bool isGrounding = true;
    private bool isSkilling;
    private bool isDowning;

    private void Awake()
    {
        // Rigidbody와 Animator 컴포넌트를 가져옴
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
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

        // 달리기 상태를 갱신
        UpdateRunningState();

        // 점프 상태 체크
        UpdateJumpState();

        // 캐릭터 이동 처리 (현재 이동 벡터를 기반으로)
        Move(moveVec);

        // 다운이 되어있고, 지면에 닿아있을 경우에만 다운 시간이 흐르도록 함
        DownTimeCheck();
    }

    public void SetState(BehaviourBase.State newState)
    {
        // 외부에서 호출 가능한 함수: 캐릭터 상태 변경
        state = newState;
    }

    private void UpdateRunningState()
    {
        // Shift 키가 눌려있고, 이동 입력이 있으며, 달리기가 가능한 상태인지 확인
        if (runInput && CanRun(moveVec))
        {
            isRunning = true;
            animator.SetBool("Run", true);
        }
        else
        {
            isRunning = false;
            animator.SetBool("Run", false);
        }
    }

    private void OnMove(InputValue value)
    {
        // 대미지를 받은 상태라면 정지
        if (state == BehaviourBase.State.Damaged || isDowning == true) { return; }

        // 입력 시스템을 통해 이동 입력을 받아서 이동 벡터를 설정
        moveVec = value.Get<Vector2>();

        // 이동 입력이 없으면 달리기 상태를 해제
        if (moveVec == Vector2.zero)
        {
            isRunning = false;
            animator.SetBool("Run", false);
        }
    }

    private void Move(Vector2 input)
    {
        // 공격 상태일 때는 이동하지 않음
        if (state == BehaviourBase.State.Attack)
        {
            return;
        }

        // Boost 중일 때의 이동 처리
        if (isBoosting)
        {
            // 입력이 없거나 뒤로 가는 입력일 경우 강제로 앞으로 이동
            if (input == Vector2.zero || input.y < 0)
            {
                input = new Vector2(0, 1); // 이동 벡터를 앞으로 고정
            }
            // 뒤가 아닌 다른 방향으로 입력이 있을 경우 해당 방향으로 이동
            else
            {
                // 그대로 input 값을 사용
            }
        }

        // 입력값이 존재할 때만 이동 처리
        if (input != null)
        {
            // Boost 상태일 때는 Boost 속도로 이동
            float speed = isBoosting ? boostSpeed : (isRunning ? runSpeed : moveSpeed);

            Vector3 moveDir = new Vector3(input.x, 0, input.y);
            Vector3 newPosition = rb.position + transform.TransformDirection(moveDir) * speed * Time.deltaTime;

            // 현재 Y 위치 유지
            newPosition.y = rb.position.y;

            rb.MovePosition(newPosition);

            // 애니메이션 파라미터를 업데이트하여 캐릭터의 움직임을 반영
            animator.SetFloat("Horizontal", input.x);
            animator.SetFloat("Vertical", input.y);

            // 이동 벡터가 0이 아니면 이동 중 애니메이션, 그렇지 않으면 정지 애니메이션 설정
            if (input != Vector2.zero)
            {
                animator.SetBool("Move", true);
            }
            else
            {
                animator.SetBool("Move", false);
            }
        }
    }


    private void OnLook(InputValue value)
    {
        // 대미지를 받은 상태라면 정지
        if (state == BehaviourBase.State.Damaged || isDowning == true) { return; }

        // 입력 시스템을 통해 회전 입력을 받아서 캐릭터 회전 처리
        Vector2 input = value.Get<Vector2>();

        // 입력값에 따라 캐릭터의 회전 각도를 업데이트
        transform.localRotation *= Quaternion.Euler(0, input.x * rotationSpeed * Time.deltaTime, 0);
    }

    public void SkillReady()
    {
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
        if (state == BehaviourBase.State.Damaged || isDowning == true) { return; }

        // 공격 입력이 감지되고 캐릭터가 공격 중이 아닐 때
        if (value.isPressed && !isAttacking && isGrounding && !isSkilling)
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
            animator.SetTrigger("Attack");

            // 상태를 공격 상태로 변경
            state = BehaviourBase.State.Attack;

            // 최대 공격 상태값을 넘으면 초기화 (콤보 리셋)
            if (attackState >= 1)
            {
                attackState = 0;
            }
        }

        // 스킬 준비 상태일 땐 스킬 공격 모션 실행
        else if (value.isPressed && !isAttacking && isGrounding && isSkilling)
        {
            animator.SetTrigger("Skill");
            // 스킬 준비 취소 (에임 범위 조준 취소)
            GetComponent<PlayerRangeAttackController>().SkillReadyNonActive();
            isAttacking = true;
        }
    }

    private void OnAttackCollider()
    {
        // 공격이 끝났을 때 호출되어 공격 플래그를 해제
        isAttacking = false;
    }

    private void OnRun(InputValue value)
    {
        // Shift 입력 상태를 추적
        runInput = value.isPressed;
        UpdateRunningState(); // 즉시 달리기 상태를 업데이트
    }

    private bool CanRun(Vector2 input)
    {
        // 후방 이동이 아닌지 확인
        if (input.y < 0)
        {
            return false; // 뒤로 가는 방향일 때는 달리지 않음
        }

        // 달리기 가능한 방향: w, wa, wd, a, d
        return input.y > 0 || input.x != 0;
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
        if (state == BehaviourBase.State.Damaged || isDowning == true) { return; }

        if (value.isPressed && !isJumping && isGrounding && !isAttacking)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);

            animator.SetTrigger("Jump");

            isGrounding = false;
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
                int rnd = UnityEngine.Random.Range(0, 2);
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
                Vector3 upForce = new Vector3(0, airBorneForce, 0); // y축으로 5의 힘을 가함 (값은 필요에 맞게 조절 가능)
                rb.AddForce(upForce, ForceMode.Impulse);

                isGrounding = false;
            }

            animator.SetBool("TakeHit", true);
        }
    }

    // 애니메이션 피격 애니메이션 프레임에 설정되어 있는 메서드
    public void TakeHitNonAcitve()
    {
        isSkilling = false;
        isAttacking = false;
        isDowning = false;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" && isGrounding == false)
        {
            animator.SetFloat("JumpHeight", 1f);
        }
    }
}