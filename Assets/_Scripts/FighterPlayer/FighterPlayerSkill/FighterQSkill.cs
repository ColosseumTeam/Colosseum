using Fusion;
using UnityEngine;

public class FighterQSkill : NetworkBehaviour
{
    [SerializeField] protected float damage = 10f;        // 대미지
    [SerializeField] protected PlayerDamageController.PlayerHitType playerHitType; // 다운 가능 여부
    [SerializeField] protected BotController.BotHitType botHitType;
    [SerializeField] protected bool downAttack = false;    // 다운 상태 적 공격 가능 여부
    [SerializeField] protected float stiffnessTime = 1f;  // 경직 시간
    [SerializeField] private float speed = 5f;
    [SerializeField] protected GameObject hitEffect;

    private AudioSource audioSource;
    private Vector3 dir;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        int ran = Random.Range(0, 3);
        transform.GetChild(ran).gameObject.SetActive(true);

        // Todo: Destory 사용한 모든 오브젝트 풀링 필요.
        Destroy(gameObject, 5f);
    }

    [Rpc]
    public void RPC_SetVolume()
    {
        audioSource.volume = FindObjectOfType<VolumeManager>().skillVolume;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        transform.position += dir * Runner.DeltaTime * speed;
    }

    //private void Update()
    //{
    //    transform.position += dir * Time.deltaTime * speed;
    //}

    public void Look(Vector3 aimPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(aimPos);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.CompareTag("Enemy"))
        {
            dir = (hit.transform.position - transform.position + new Vector3(0, 0.9f)).normalized;
        }
        else
        {
            dir = (Camera.main.ScreenToWorldPoint(new Vector3(aimPos.x, aimPos.y, 10f)) - transform.position).normalized;
        }
    }
}
