using Fusion;
using UnityEngine;

public class RangePlayerFourAfterAttack : NetworkBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private PlayerDamageController.PlayerHitType playerHitType;
    [SerializeField] private BotController.BotHitType botHitType;
    [SerializeField] private bool downAttack = true;   

    private GameObject ranger;

    private void Awake()
    {
        Destroy(gameObject, 1f);        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (other.GetComponent<PlayerDamageController>() != null && other.gameObject == ranger && HasStateAuthority)
            {
                other.gameObject.GetComponent<PlayerDamageController>().RPC_TakeDamage(damage, playerHitType, downAttack, 1f, transform.position);
            }

            if (other.gameObject.TryGetComponent(out BotController component))
            {
                component.TakeDamage(damage, botHitType, downAttack, 1f, transform.position);
            }
        }
    }

    public void GetRanger(GameObject newRanger)
    {
        ranger = newRanger;
    }
}
