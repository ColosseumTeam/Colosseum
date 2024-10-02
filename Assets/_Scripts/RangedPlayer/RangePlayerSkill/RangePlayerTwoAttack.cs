using Fusion;
using UnityEditor.PackageManager;
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
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float fieldTimer = 0f;
    [SerializeField] private float fieldEndTimer = 1f;

    private GameObject targetObj;

    private void Awake()
    {
        GroundPositionGrasp();

        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        GroundPositionGrasp();
    }

    private void GroundPositionGrasp()
    {
        fieldTimer += Time.deltaTime;

        if (fieldTimer >= fieldEndTimer)
        {
            GameObject instaceFireField = Instantiate(fireFieldMaker, gameObject.transform.position, Quaternion.identity);
            instaceFireField.GetComponent<RangePlayerTwoAfterAttack>().GetTargetObject(targetObj);
            fieldTimer = 0f;
        }
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
                collision.gameObject.GetComponent<PlayerDamageController>().RPC_TakeDamage(damage, playerHitType, downAttack, 1f);
                targetObj = collision.gameObject;
            }
            else
            {
                collision.gameObject.GetComponent<BotController>().TakeDamage(damage, botHitType, downAttack, 1f);
                targetObj = collision.gameObject;
            }

            Destroy(gameObject);
        }
    }
}
