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
    [SerializeField] private GameObject fireFieldMaker;    
    [SerializeField] private float fieldTimer = 0f;
    [SerializeField] private float fieldEndTimer = 1f;    

    private GameObject player;    

    private void Awake()
    {
        GroundPositionGrasp();

        Destroy(gameObject, 3f);
    }

    //private void Update()
    //{
    //    GroundPositionGrasp();
    //}

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        GroundPositionGrasp();
    }

    private void GroundPositionGrasp()
    {
        fieldTimer += Time.deltaTime;

        if (fieldTimer >= fieldEndTimer)
        {
            NetworkObject instaceFireField = Runner.Spawn(fireFieldMaker, gameObject.transform.position, Quaternion.identity);
            instaceFireField.GetComponent<RangePlayerTwoAfterAttack>().GetTargetObject(player);
            fieldTimer = 0f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!HasStateAuthority)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }

        //Todo: Runner 및 Network 상태에 맞춰 수정할 수 있어야 함
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log(collision.gameObject);

            Collider[] colls = Physics.OverlapSphere(this.transform.position, 1f, 1 << LayerMask.NameToLayer("Enemy"));

            foreach (Collider coll in colls)
            {
                Rigidbody rb = coll.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(knockBackEndForce, this.transform.position, knockBackFoce);
                }
            }

            if (collision.gameObject.GetComponentInParent<PlayerDamageController>() != null
                && collision.gameObject != player && HasStateAuthority)
            {                
                collision.gameObject.GetComponentInParent<PlayerDamageController>().TakeDamage(damage, playerHitType, downAttack, 1f, transform.position);
                //Runner.Spawn(attecktEffect, gameObject.transform.position, gameObject.transform.rotation);                
            }
            else
            {
                if (collision.gameObject.TryGetComponent(out BotController component))
                {
                    component.TakeDamage(damage, botHitType, downAttack, 1f, transform.position);
                    //Runner.Spawn(attecktEffect, gameObject.transform.position, gameObject.transform.rotation);                    
                }
            }

            Destroy(gameObject);
        }
    }

    public void GetRangePlayer(GameObject newPlayer)
    {
        player = newPlayer;
    }
}