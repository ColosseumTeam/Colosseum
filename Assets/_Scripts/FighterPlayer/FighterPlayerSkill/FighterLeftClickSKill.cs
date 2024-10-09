using Fusion;
using UnityEngine;

public class FighterLeftClickSKill : NetworkBehaviour
{
    [SerializeField] private float damage = 10f;        // 대미지
    [SerializeField] private PlayerDamageController.PlayerHitType playerHitType; // 다운 가능 여부
    [SerializeField] private BotController.BotHitType botHitType;
    [SerializeField] private bool downAttack = false;    // 다운 상태 적 공격 가능 여부
    [SerializeField] private float stiffnessTime = 1f;  // 경직 시간
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private GameObject hitPosition;

    private void Awake()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.GetComponentInParent<PlayerDamageController>() != null && HasStateAuthority)
            {
                other.GetComponentInParent<PlayerDamageController>().RPC_TakeDamage(damage, playerHitType, downAttack, stiffnessTime, hitPosition.transform.position);
                //Runner.Spawn(hitEffect, hitPosition.transform.position, hitPosition.transform.rotation);
            }
            else
            {
                if (other.TryGetComponent(out BotController component))
                {
                    component.TakeDamage(damage, botHitType, downAttack, stiffnessTime, hitPosition.transform.position);
                    //Runner.Spawn(hitEffect, hitPosition.transform.position, hitPosition.transform.rotation);
                }
            }            
        }
    }
}
