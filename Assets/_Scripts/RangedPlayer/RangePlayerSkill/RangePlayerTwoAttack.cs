using Fusion;
using UnityEngine;

public class RangePlayerTwoAttack : NetworkBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private PlayerDamageController.PlayerHitType playerHitType;
    [SerializeField] private BotController.BotHitType botHitType;
    [SerializeField] private bool downAttack = false;
    [SerializeField] private float knockBackFoce = 10f;
    [SerializeField] private float knockBackEndForce = 500f;
  
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

        //Todo: Runner 및 Network 상태에 맞춰 수정할 수 있어야 함
        if (collision.gameObject.CompareTag("Enemy"))
        {            
            Collider[] colls = Physics.OverlapSphere(this.transform.position, 1f, 1 << LayerMask.NameToLayer("Enemy"));

            foreach (Collider coll in colls)
            {
                Rigidbody rb = coll.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(knockBackEndForce, this.transform.position, knockBackFoce);
                }
            }

            if (collision.gameObject.GetComponent<PlayerDamageController>() != null)
            {
                collision.gameObject.GetComponent<PlayerDamageController>().TakeDamage(damage, playerHitType, downAttack, 1f);
            }
            else
            {
                collision.gameObject.GetComponent<BotController>().TakeDamage(
                    damage, botHitType, downAttack, 1f);
            }

            Destroy(gameObject);
        }
    }
}
