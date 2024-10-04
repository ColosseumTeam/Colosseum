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

    private GameObject player;
    private bool isAttacking;
    private GameObject enemyObj;

    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (isAttacking)
        {
            attackTime += Time.deltaTime;
            if (attackTime >= attackTimeEnd && enemyObj != null)
            {
                Runner.Spawn(afterAttackObject, gameObject.transform.position, Quaternion.identity);

                if (enemyObj.GetComponent<PlayerDamageController>() != null)
                {
                    downAttack = true;
                    enemyObj.GetComponent<PlayerDamageController>().RPC_TakeDamage(damage, playerHitType, downAttack, 1f);
                }
                else
                {
                    if (enemyObj.gameObject.TryGetComponent(out BotController component))
                    {
                        downAttack = true;
                        component.TakeDamage(damage, botHitType, downAttack, 1f);
                    }
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

            if (enemyObj.GetComponent<PlayerDamageController>() != null
                && enemyObj != player && HasStateAuthority)
            {
                enemyObj.GetComponent<PlayerDamageController>().RPC_TakeDamage(damage, playerHitType, downAttack, 1f);
            }
            else
            {
                if (enemyObj.gameObject.TryGetComponent(out BotController component))
                {
                    component.TakeDamage(damage, botHitType, downAttack, 1f);
                }
            }            

            GetComponent<Collider>().enabled = false;
            downAttack = false;
            attackTime = 0f;
            isAttacking = true;
        }
    }

    public void OnGroundCheck()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;

        //attackTime = 0f;
        //isAttacking = true;        
    }

    public void GetRangePlayer(GameObject newPlayer)
    {
        player = newPlayer;
    }
}
