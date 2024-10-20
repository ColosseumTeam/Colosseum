using Fusion;
using UnityEngine;

public class FighterESkill : NetworkBehaviour
{
    [SerializeField] private float damage = 10f;        // 대미지
    [SerializeField] private PlayerDamageController.PlayerHitType playerHitType; // 다운 가능 여부
    [SerializeField] private BotController.BotHitType botHitType;
    [SerializeField] private bool downAttack = true;    // 다운 상태 적 공격 가능 여부
    [SerializeField] private float stiffnessTime = 1f;  // 경직 시간
    [SerializeField] private PlayerFighterAttackController attackController;


    private void Awake()
    {
        Destroy(gameObject, 2f);
    }

    public void SetAttackController(PlayerFighterAttackController attackController)
    {
        this.attackController = attackController;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.GetComponentInParent<PlayerDamageController>() != null && HasStateAuthority)
            {
                other.GetComponentInParent<PlayerDamageController>().TakeDamage(damage, playerHitType, downAttack, stiffnessTime, other.gameObject.transform.position);
                attackController.RPC_SetEnemyTr();
                attackController.RPC_StartESkillEffect();
            }
            else
            {
                if (other.TryGetComponent(out BotController component))
                {
                    component.TakeDamage(damage, botHitType, downAttack, stiffnessTime, transform.position);
                    attackController.SetBotTr();
                    attackController.PlayableDirector.Play();
                }
            }

            Destroy(gameObject);
        }
    }
}
