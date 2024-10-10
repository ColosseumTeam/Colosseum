using Fusion;
using System.ComponentModel;
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
            NetworkObject afterObj = Runner.Spawn(afterAttackObject, gameObject.transform.position, Quaternion.identity);
            afterObj.GetComponent<RangePlayerFourAfterAttack>().GetRanger(player);

            Vector3 instanceBalanceRotation = new Vector3(
                        gameObject.transform.eulerAngles.x + 90f,
                        gameObject.transform.eulerAngles.y,
                        gameObject.transform.eulerAngles.z
            );

            Ray ray = new Ray(gameObject.transform.position, Vector3.down);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Ground"))
                {
                    Runner.Spawn(balanceObject, hit.point, Quaternion.Euler(instanceBalanceRotation));
                }
            }

            Destroy(gameObject);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemyObj = other.gameObject;

            if (enemyObj.GetComponent<PlayerDamageController>() != null && enemyObj != player && HasStateAuthority)
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

    // RangePlayerFourAttackMiddle이 오브젝트를 멈추도록 신호를 보내줌
    public void OnGroundCheck()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        isDowning = false;
    }

    // RangePlayerFourAttackMiddle이 일정 시간 지나면 폭발할 수 있도록 신호를 보내줌
    public void IsAttackingActive()
    {
        isAttacking = true;
    }

    public void GetRangePlayer(GameObject newPlayer)
    {
        player = newPlayer;
    }
}
