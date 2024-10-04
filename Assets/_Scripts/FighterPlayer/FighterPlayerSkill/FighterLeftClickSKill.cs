using Fusion;
using Fusion.Addons.SimpleKCC;
using UnityEngine;

public class FighterLeftClickSKill : NetworkBehaviour
{
    [SerializeField] private float damage = 10f;        // 대미지
    [SerializeField] private PlayerDamageController.PlayerHitType playerHitType; // 다운 가능 여부
    [SerializeField] private BotController.BotHitType botHitType;
    [SerializeField] private bool downAttack = false;    // 다운 상태 적 공격 가능 여부
    [SerializeField] private float stiffnessTime = 1f;  // 경직 시간


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"tag: {other.gameObject.GetComponentInParent<NetworkObject>().tag}");
        Debug.Log($"layer: {other.gameObject.layer}");
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("LC");
            if (other.gameObject.GetComponent<PlayerDamageController>() != null)
            {
                other.gameObject.GetComponent<PlayerDamageController>().RPC_TakeDamage(damage, playerHitType, downAttack, stiffnessTime);
            }
            else
            {
                //other.gameObject.GetComponent<BotController>().TakeDamage(damage, botHitType, downAttack, stiffnessTime);
                if (other.gameObject.TryGetComponent(out BotController component))
                {
                    component.TakeDamage(damage, botHitType, downAttack, stiffnessTime);
                }
            }
        }
    }
}
