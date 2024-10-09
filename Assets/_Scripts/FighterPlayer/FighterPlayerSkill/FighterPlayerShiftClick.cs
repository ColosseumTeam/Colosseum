using Fusion;
using UnityEngine;

// Todo: interface(IRangeSkill 윤빈씨가 수정할 예정) 상속 받기.
public class FighterPlayerShiftClick : NetworkBehaviour
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
        Destroy(gameObject, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (other.GetComponentInParent<PlayerDamageController>() != null && HasStateAuthority)
            {
                other.GetComponentInParent<PlayerDamageController>().RPC_TakeDamage(damage, playerHitType, downAttack, stiffnessTime, transform.position);
                //Runner.Spawn(hitEffect, hitPosition.transform.position, hitPosition.transform.rotation);
            }
            else
            {
                if (other.TryGetComponent(out BotController component))
                {
                    component.TakeDamage(damage, botHitType, downAttack, stiffnessTime, transform.position);
                    //Runner.Spawn(hitEffect, hitPosition.transform.position, hitPosition.transform.rotation);
                }
            }
        }
    }
}
