using System;
using UnityEngine;
using UnityEngine.InputSystem;


// ����
// 1. isSkilling ��ų �غ� ��Ÿ���� ����
// �޼���
// 1. SkillReady
// 2. OnAttack�� RangePlayer�� �ڵ�
public class PlayerController : MonoBehaviour
{
    // ĳ������ ���� ���¸� �����ϴ� ����
    [SerializeField] private BehaviourBase.State state = BehaviourBase.State.None;

    // ĳ���� �̵� �ӵ�, ȸ�� �ӵ�, ���� ���� Ÿ�̸� ����
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 10f; // �޸��� �� �ӵ�
    [SerializeField] private float rotationSpeed = 60f;
    [SerializeField] private float attackTimer = 0f;
    [SerializeField] private float attackElapsedTime = 0.5f;
    //[SerializeField] private float attackAfterDelay = 0.2f;
    [SerializeField] private float jumpForce = 7f; // ���� ��
    [SerializeField] private float maxJumpHeight = 2f; // �ִ� ���� ����
    [SerializeField] private float startYPosition; // ���� ���� �� Y�� ��ġ�� ����
    [SerializeField] private float boostSpeed = 15f; // Boost �� �ӵ�
    [SerializeField] private float downTimer = 0f;
    [SerializeField] private float downEndTimer = 3f;
    [SerializeField] private float airBorneForce = 5f;

    // ĳ���� ������ ��� ���� Rigidbody ����
    [SerializeField] private Rigidbody rb;

    // ĳ���� �ִϸ��̼� ��� ���� Animator ����
    private Animator animator;

    // ���� ���� �� �̵� ������ �����ϴ� ����
    private float attackState;
    private Vector2 moveVec;
    private bool isAttacking;
    private bool isRunning; // �޸��� ���¸� ��Ÿ���� ����
    private bool runInput; // Shift �Է� ���¸� �����ϴ� ����
    private bool isBoosting; // Boost ���¸� ��Ÿ���� ����
    private bool isJumping; // jump ���¸� ��Ÿ���� ����
    private bool isGrounding = true;
    private bool isSkilling;
    private bool isDowning;

    private void Awake()
    {
        // Rigidbody�� Animator ������Ʈ�� ������
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // ������� ���� ���¶�� Ȱ�� ����
        if (state == BehaviourBase.State.Damaged || isDowning == true) { return; }

        // ���� ���°� �ƴϰ� ���� ���°��� 0�� �ƴ� �� (��, ������ ���� ���� ��)
        // ���� ���� ���� ������ ���ϵ��� ����

        if (state != BehaviourBase.State.Attack && attackState != 0)
        {
            // ���� Ÿ�̸Ӱ� ������ �ð��� �ʰ��ߴ��� Ȯ��
            if (attackTimer >= attackElapsedTime)
            {
                // ���� ���� �ʱ�ȭ �� �ִϸ��̼� ���� �ʱ�ȭ
                attackTimer = 0f;
                attackState = 0f;
                animator.SetFloat("AttackState", attackState);
                return;
            }
            // ���� Ÿ�̸� ���� (�����Ӹ���)
            attackTimer += Time.deltaTime;
        }
        else if (state == BehaviourBase.State.Attack)
        {
            // ���� ������ ���� ������ ó���� ���� �� ���� (���� �� �ڵ� ���)
        }

        // �޸��� ���¸� ����
        UpdateRunningState();

        // ���� ���� üũ
        UpdateJumpState();

        // ĳ���� �̵� ó�� (���� �̵� ���͸� �������)
        Move(moveVec);

        // �ٿ��� �Ǿ��ְ�, ���鿡 ������� ��쿡�� �ٿ� �ð��� �帣���� ��
        DownTimeCheck();
    }

    public void SetState(BehaviourBase.State newState)
    {
        // �ܺο��� ȣ�� ������ �Լ�: ĳ���� ���� ����
        state = newState;
    }

    private void UpdateRunningState()
    {
        // Shift Ű�� �����ְ�, �̵� �Է��� ������, �޸��Ⱑ ������ �������� Ȯ��
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
        // ������� ���� ���¶�� ����
        if (state == BehaviourBase.State.Damaged || isDowning == true) { return; }

        // �Է� �ý����� ���� �̵� �Է��� �޾Ƽ� �̵� ���͸� ����
        moveVec = value.Get<Vector2>();

        // �̵� �Է��� ������ �޸��� ���¸� ����
        if (moveVec == Vector2.zero)
        {
            isRunning = false;
            animator.SetBool("Run", false);
        }
    }

    private void Move(Vector2 input)
    {
        // ���� ������ ���� �̵����� ����
        if (state == BehaviourBase.State.Attack)
        {
            return;
        }

        // Boost ���� ���� �̵� ó��
        if (isBoosting)
        {
            // �Է��� ���ų� �ڷ� ���� �Է��� ��� ������ ������ �̵�
            if (input == Vector2.zero || input.y < 0)
            {
                input = new Vector2(0, 1); // �̵� ���͸� ������ ����
            }
            // �ڰ� �ƴ� �ٸ� �������� �Է��� ���� ��� �ش� �������� �̵�
            else
            {
                // �״�� input ���� ���
            }
        }

        // �Է°��� ������ ���� �̵� ó��
        if (input != null)
        {
            // Boost ������ ���� Boost �ӵ��� �̵�
            float speed = isBoosting ? boostSpeed : (isRunning ? runSpeed : moveSpeed);

            Vector3 moveDir = new Vector3(input.x, 0, input.y);
            Vector3 newPosition = rb.position + transform.TransformDirection(moveDir) * speed * Time.deltaTime;

            // ���� Y ��ġ ����
            newPosition.y = rb.position.y;

            rb.MovePosition(newPosition);

            // �ִϸ��̼� �Ķ���͸� ������Ʈ�Ͽ� ĳ������ �������� �ݿ�
            animator.SetFloat("Horizontal", input.x);
            animator.SetFloat("Vertical", input.y);

            // �̵� ���Ͱ� 0�� �ƴϸ� �̵� �� �ִϸ��̼�, �׷��� ������ ���� �ִϸ��̼� ����
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
        // ������� ���� ���¶�� ����
        if (state == BehaviourBase.State.Damaged || isDowning == true) { return; }

        // �Է� �ý����� ���� ȸ�� �Է��� �޾Ƽ� ĳ���� ȸ�� ó��
        Vector2 input = value.Get<Vector2>();

        // �Է°��� ���� ĳ������ ȸ�� ������ ������Ʈ
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
        // ������� ���� ���¶�� ����
        if (state == BehaviourBase.State.Damaged || isDowning == true) { return; }

        // ���� �Է��� �����ǰ� ĳ���Ͱ� ���� ���� �ƴ� ��
        if (value.isPressed && !isAttacking && isGrounding && !isSkilling)
        {
            // �̵� ���̶�� �̵��� ����
            if (rb.velocity != Vector3.zero)
            {
                rb.velocity = Vector3.zero;
            }

            // ���� �÷��� ����
            isAttacking = true;

            // ���� Ÿ�̸� �ʱ�ȭ
            attackTimer = 0f;

            // ���� ���°� ���� (�޺� ���� ����)
            attackState += 0.25f;

            // �ִϸ��̼� �Ķ���� ������Ʈ �� ���� Ʈ���� ����
            animator.SetFloat("AttackState", attackState);
            animator.SetTrigger("Attack");

            // ���¸� ���� ���·� ����
            state = BehaviourBase.State.Attack;

            // �ִ� ���� ���°��� ������ �ʱ�ȭ (�޺� ����)
            if (attackState >= 1)
            {
                attackState = 0;
            }
        }

        // ��ų �غ� ������ �� ��ų ���� ��� ����
        else if (value.isPressed && !isAttacking && isGrounding && isSkilling)
        {
            animator.SetTrigger("Skill");
            // ��ų �غ� ��� (���� ���� ���� ���)
            GetComponent<PlayerRangeAttackController>().SkillReadyNonActive();
            isAttacking = true;
        }
    }

    private void OnAttackCollider()
    {
        // ������ ������ �� ȣ��Ǿ� ���� �÷��׸� ����
        isAttacking = false;
    }

    private void OnRun(InputValue value)
    {
        // Shift �Է� ���¸� ����
        runInput = value.isPressed;
        UpdateRunningState(); // ��� �޸��� ���¸� ������Ʈ
    }

    private bool CanRun(Vector2 input)
    {
        // �Ĺ� �̵��� �ƴ��� Ȯ��
        if (input.y < 0)
        {
            return false; // �ڷ� ���� ������ ���� �޸��� ����
        }

        // �޸��� ������ ����: w, wa, wd, a, d
        return input.y > 0 || input.x != 0;
    }

    // �ٴ� �ν�Ʈ ���� �޼���
    public void BoostStop()
    {
        isBoosting = false;
        animator.SetBool("Boost", false);
    }

    private void OnBoost(InputValue value)
    {
        // Boost �Է��� �����Ǿ���, ���� Boost ���°� �ƴ� ��
        if (value.isPressed && !isBoosting)
        {
            // ���� �̵� ���Ͱ� �ڷ� ���ϴ��� Ȯ��
            if (moveVec.y < 0)
            {
                // �ڷ� �̵� ���̶�� Boost�� �������� ����
                return;
            }

            // Boost ���� ����
            isBoosting = true;
            animator.SetBool("Boost", true);

            // Boost ���¿��� �ִϸ��̼� Ʈ����
            animator.SetTrigger("Boost");
        }
    }

    private void OnJump(InputValue value)
    {
        // ������� ���� ���¶�� ����
        if (state == BehaviourBase.State.Damaged || isDowning == true) { return; }

        if (value.isPressed && !isJumping && isGrounding && !isAttacking)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);

            animator.SetTrigger("Jump");

            isGrounding = false;
        }
    }

    // Start �ִϸ��̼� �� ������ �ִϸ��̼� �̺�Ʈ�� ����
    public void OnJumpKeepChangeEvent()
    {
        isJumping = true;
        animator.SetFloat("JumpHeight", 0.33f);
    }

    // Keep �ִϸ��̼� �� ������ �ִϸ��̼� �̺�Ʈ�� ����
    public void OnJumpLoopChangeEvent()
    {
        animator.SetFloat("JumpHeight", 0.66f);
    }

    // ���� �������� �� ����Ǵ� Wait �ִϸ��̼� �� ������ �̺�Ʈ�� ����
    public void OnJumpWaitChangeEvnet()
    {
        animator.SetFloat("JumpHeight", 0f);
        isGrounding = true;
    }

    private void UpdateJumpState()
    {
        // ������ ���۵� �� �� ���� ���� ����
        if (isJumping)
        {
            // ĳ������ �ʱ� Y ��ġ�� ���
            if (startYPosition == 0)
            {
                startYPosition = transform.position.y;
            }

            // ĳ���Ͱ� �ִ� ���� ���̿� �����ߴ��� Ȯ��
            if (transform.position.y >= startYPosition + maxJumpHeight)
            {
                // �ִ� ���̿� �����ϸ� ������ ����
                isJumping = false;
                startYPosition = 0; // Y ��ġ �ʱ�ȭ
            }

            // y��ǥ ���
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // damageType : 0 => ���� �ǰ� Ÿ��
    // damageType : 2 => �ٿ� ���� Ÿ��.
    // TakeHitState : 0, 1 => ���� �ǰ�
    // TakeHitState : 2 => �ٿ� ����
    // downAttack : false => �ٿ� ���� �� �ǰ�X
    // downAttack : true => �ٿ� ���� �� �ǰ�O 
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

                // ĳ���͸� ������ ���� �о��ִ� ���� �߰�
                Vector3 upForce = new Vector3(0, airBorneForce, 0); // y������ 5�� ���� ���� (���� �ʿ信 �°� ���� ����)
                rb.AddForce(upForce, ForceMode.Impulse);

                isGrounding = false;
            }

            animator.SetBool("TakeHit", true);
        }
    }

    // �ִϸ��̼� �ǰ� �ִϸ��̼� �����ӿ� �����Ǿ� �ִ� �޼���
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