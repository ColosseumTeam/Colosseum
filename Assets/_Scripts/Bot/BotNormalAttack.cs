using Fusion;
using UnityEngine;

public class BotNormalAttack : NetworkBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private PlayerDamageController.PlayerHitType playerHitType; // 다운 가능 여부
    [SerializeField] private bool downAttack = false;
    [SerializeField] private float stiffnessTime = 1f;

    private DamageManager damageManager;

    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {        
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); 
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerDamageController>().RPC_TakeDamage(damage, playerHitType, downAttack, stiffnessTime, transform.position);

            Destroy(gameObject); 
        }
    }
}
