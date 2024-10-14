using Fusion;
using UnityEngine;

public class RangePlayerNormalAttack : NetworkBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private PlayerDamageController.PlayerHitType playerHitType;
    [SerializeField] private BotController.BotHitType botHitType;
    [SerializeField] private bool downAttack = false;
    [SerializeField] private float stiffnessTime = 1f;
    
    private GameObject player;
    private float speed = 10f;
    private Vector3 dir;

    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    //private void Update()
    //{
    //    transform.position += dir * Time.deltaTime * speed;
    //}

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        transform.position += dir * Runner.DeltaTime * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {        
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); 
        }

        if (collision.gameObject.CompareTag("Enemy"))
        //if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (collision.gameObject.GetComponent<PlayerDamageController>() != null 
                && collision.gameObject != player && HasStateAuthority) 
            {                
                collision.gameObject.GetComponent<PlayerDamageController>().RPC_TakeDamage(damage, playerHitType, downAttack, stiffnessTime, transform.position);
                //Runner.Spawn(attecktEffect, gameObject.transform.position, gameObject.transform.rotation);
            }
            else
            {             
                if (collision.gameObject.TryGetComponent(out BotController component))
                {
                    Debug.Log("Bot Hit");
                    component.TakeDamage(damage, botHitType, downAttack, stiffnessTime, transform.position);
                    //Runner.Spawn(attecktEffect, gameObject.transform.position, gameObject.transform.rotation);
                }
                //collision.gameObject.GetComponent<BotController>().TakeDamage(damage, botHitType, downAttack, stiffnessTime);
            }
            
            Destroy(gameObject); 
        }
    
    }

    public void Look(Vector3 aimPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(aimPos);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.CompareTag("Enemy"))
        {
            dir = (hit.transform.position - transform.position + new Vector3(0, 0.9f)).normalized;
        }
        else
        {
            dir = (Camera.main.ScreenToWorldPoint(new Vector3(aimPos.x, aimPos.y, 10f)) - transform.position).normalized;
        }
    }

    public void GetRangePlayer(GameObject newPlayer)
    {
        player = newPlayer;
    }
}
