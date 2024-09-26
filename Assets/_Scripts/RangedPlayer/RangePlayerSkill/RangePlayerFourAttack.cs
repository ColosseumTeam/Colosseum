using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class RangePlayerFourAttack : NetworkBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private PlayerDamageController.PlayerHitType playerHitType;
    [SerializeField] private BotController.BotHitType botHitType;
    [SerializeField] private bool downAttack = true;
    [SerializeField] private float attackTime = 0f;
    [SerializeField] private float attackTimeEnd = 0.1f;
    [SerializeField] private GameObject afterAttackObject;

    private bool isAttacking;
    private GameObject enemyObj;

    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        if (isAttacking)
        {
            downAttack = false;

            attackTime += Time.deltaTime;
            if(attackTime >= attackTimeEnd && enemyObj != null)
            {
                Instantiate(afterAttackObject, enemyObj.transform.position, Quaternion.identity);

                if (enemyObj.GetComponent<PlayerDamageController>() != null)
                {
                    enemyObj.GetComponent<PlayerDamageController>().TakeDamage(damage, playerHitType, downAttack, 1f);
                }
                else
                {
                    enemyObj.GetComponent<BotController>().TakeDamage(
                    damage, botHitType, downAttack, 1f);
                }

                Destroy(gameObject);
            }            
        }   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemyObj = other.gameObject;

            if (enemyObj.GetComponent<PlayerDamageController>() != null)
            {
                enemyObj.GetComponent<PlayerDamageController>().TakeDamage(damage, playerHitType, downAttack, 1f);
            }
            else
            {
                enemyObj.GetComponent<BotController>().TakeDamage(damage, botHitType, downAttack, 1f);
            }
        }

        if (other.gameObject.CompareTag("Ground"))
        {
            isAttacking = true;
            attackTime = 0f;
        }
    }
}
