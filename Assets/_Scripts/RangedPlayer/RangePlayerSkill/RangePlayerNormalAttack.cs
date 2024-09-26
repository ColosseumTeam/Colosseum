using Fusion;
using UnityEngine;

public class RangePlayerNormalAttack : NetworkBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private PlayerDamageController.PlayerHitType playerHitType;
    [SerializeField] private BotController.BotHitType botHitType;
    [SerializeField] private bool downAttack = false;
    [SerializeField] private float stiffnessTime = 1f;    

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

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponent<PlayerDamageController>() != null)
            {               
                collision.gameObject.GetComponent<PlayerDamageController>().TakeDamage(damage, playerHitType, downAttack, stiffnessTime);
            }
            else
            {             
                collision.gameObject.GetComponent<BotController>().TakeDamage(damage, botHitType, downAttack, stiffnessTime);
            }

            Destroy(gameObject); 
        }
    }
}
