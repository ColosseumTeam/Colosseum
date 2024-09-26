using Fusion;
using UnityEngine;

public class FighterPlayerShiftClick : NetworkBehaviour
{
    [SerializeField] private float damage = 10f;        // 대미지
    [SerializeField] private PlayerDamageController.PlayerHitType playerHitType; // 다운 가능 여부
    [SerializeField] private BotController.BotHitType botHitType;
    [SerializeField] private bool downAttack = true;    // 다운 상태 적 공격 가능 여부
    [SerializeField] private float stiffnessTime = 1f;  // 경직 시간

    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.gameObject.GetComponent<PlayerDamageController>() != null)
            {
                other.gameObject.GetComponent<PlayerDamageController>().TakeDamage(damage, playerHitType, downAttack, stiffnessTime);
            }
            else
            {
                other.gameObject.GetComponent<BotController>().TakeDamage(damage, botHitType, downAttack, stiffnessTime);
            }

            Destroy(gameObject);
        }
    }
}
