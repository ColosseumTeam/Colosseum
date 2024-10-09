using Fusion;
using UnityEngine;

public class RangePlayerFourAttack : NetworkBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private PlayerDamageController.PlayerHitType playerHitType;
    [SerializeField] private BotController.BotHitType botHitType;
    [SerializeField] private bool downAttack = true;
    [SerializeField] private GameObject afterAttackObject;
    [SerializeField] private GameObject balanceObject;
    [SerializeField] private GameObject attecktEffect;
    [SerializeField] private AudioClip downClip;

    private AudioSource audioSource;
    private GameObject player;
    private bool isAttacking;
    private GameObject enemyObj;
    private bool isDowning = true;

    private float fallSpeed = 5f; // 떨어지는 속도를 조절할 변수
    private Vector3 velocity; // 이동 벡터

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        // 떨어질 때의 초기 속도 설정
        velocity = Vector3.down * fallSpeed;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        // 스킬 오브젝트가 스스로 떨어지도록 설정
        if (HasStateAuthority && isDowning)
        {
            transform.position += velocity * Runner.DeltaTime; // DeltaTime을 사용해 프레임에 따른 이동
        }

        if (!audioSource.isPlaying)
        {
            audioSource.clip = downClip;
            audioSource.Play();
            audioSource.loop = true;
        }

        if (isAttacking)
        {
            HandleAttack();
        }
    }

    private void HandleAttack()
    {
        if (enemyObj != null)
        {
            Runner.Spawn(afterAttackObject, gameObject.transform.position, Quaternion.identity);

            if (enemyObj.GetComponent<PlayerDamageController>() != null)
            {
                downAttack = true;
                enemyObj.GetComponent<PlayerDamageController>().RPC_TakeDamage(damage, playerHitType, downAttack, 1f, transform.position);
            }
            else
            {
                if (enemyObj.gameObject.TryGetComponent(out BotController component))
                {
                    downAttack = true;

                    Vector3 instanceBalanceRotation = new Vector3(
                        gameObject.transform.eulerAngles.x + 90f,
                        gameObject.transform.eulerAngles.y,
                        gameObject.transform.eulerAngles.z
                    );

                    component.TakeDamage(damage, botHitType, downAttack, 1f, transform.position);

                    Ray ray = new Ray(enemyObj.transform.position, Vector3.down);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                    {
                        if (hit.collider.CompareTag("Ground"))
                        {
                            Runner.Spawn(balanceObject, hit.point, Quaternion.Euler(instanceBalanceRotation));
                        }
                    }
                }
            }

            Destroy(gameObject); // 공격 후 오브젝트 삭제
        }
        else
        {
            Destroy(gameObject); // 적이 없을 때도 삭제
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemyObj = other.gameObject;

            if (enemyObj.GetComponent<PlayerDamageController>() != null
                && enemyObj != player && HasStateAuthority)
            {
                enemyObj.GetComponent<PlayerDamageController>().RPC_TakeDamage(damage, playerHitType, downAttack, 1f, transform.position);
                GetComponent<Collider>().enabled = false;
                GetComponent<Collider>().isTrigger = false;
            }
            else
            {
                if (enemyObj.gameObject.TryGetComponent(out BotController component))
                {
                    component.TakeDamage(damage, botHitType, downAttack, 1f, transform.position);
                    GetComponent<Collider>().enabled = false;
                    GetComponent<Collider>().isTrigger = false;
                }
            }

            downAttack = false;
        }
    }

    public void OnGroundCheck()
    {
        // 물리적 충돌 처리 중지
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        isDowning = false;
    }

    public void IsAttackingActive()
    {
        isAttacking = true;
    }

    public void GetRangePlayer(GameObject newPlayer)
    {
        player = newPlayer;
    }
}
