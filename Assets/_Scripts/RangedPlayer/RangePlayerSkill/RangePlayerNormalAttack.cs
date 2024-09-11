using UnityEngine;

public class RangePlayerNormalAttack : MonoBehaviour, IRangeSkill
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private bool skillType = false;
    [SerializeField] private bool downAttack = false;

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
            damageManager.DamageTransmission(gameObject, collision.gameObject);

            Destroy(gameObject); 
        }
    }

    public void GetSkillState(out float getDamage, out bool getSkillType, out bool getDownAttack)
    {
        getDamage = damage;
        getSkillType = skillType;
        getDownAttack = downAttack;
    }

    public void GetDamageManager(DamageManager newDamageManager)
    {
        damageManager = newDamageManager;
    }
}
