using UnityEngine;

public class RangePlayerTwoAttack : MonoBehaviour, ISkill
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private bool skillType = false;
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

        if (collision.gameObject.CompareTag("Enemy"))
        {
            damageManager.DamageTransmission(gameObject, collision.gameObject);

            Destroy(gameObject); 
        }
    }

    public void GetSkillState(out float getDamage, out bool getSkillType, out bool getDownAttack, out float getStiffnessTime)
    {
        getDamage = damage;
        getSkillType = skillType;
        getDownAttack = downAttack;
        getStiffnessTime = stiffnessTime;
    }

    public void GetDamageManager(DamageManager newDamageManager)
    {
        damageManager = newDamageManager;
    }
}
