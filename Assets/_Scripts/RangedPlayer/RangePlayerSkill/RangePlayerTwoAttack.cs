using Fusion;
using UnityEngine;

public class RangePlayerTwoAttack : NetworkBehaviour, ISkill
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private bool skillType = true;
    [SerializeField] private bool downAttack = false;
    [SerializeField] private float knockBackFoce = 10f;
    [SerializeField] private float knockBackEndForce = 500f;
    
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

        if (collision.gameObject.CompareTag("Enemy"))
        {            
            Collider[] colls = Physics.OverlapSphere(this.transform.position, 1f, 1 << LayerMask.NameToLayer("Enemy"));

            foreach (Collider coll in colls)
            {
                coll.GetComponent<Rigidbody>().AddExplosionForce(knockBackEndForce, this.transform.position, knockBackFoce);
            }

            damageManager.DamageTransmission(gameObject, collision.gameObject);

            Destroy(gameObject);
        }
    }

    public void GetSkillState(out float getDamage, out bool getSkillType, out bool getDownAttack, out float getStiffnessTime)
    {
        getDamage = damage;
        getSkillType = skillType;
        getDownAttack = downAttack;

        // 다운 스킬이기에 경직 시간이 필요로 하지 않음
        getStiffnessTime = 0f;
    }

    public void GetDamageManager(DamageManager newDamageManager)
    {
        damageManager = newDamageManager;
    }
}
