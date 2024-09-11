using UnityEngine;

public class RangePlayerFourAttack : MonoBehaviour, IRangeSkill
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private bool skillType = true;
    [SerializeField] private bool downAttack = true;

    private GameObject enemyObj;
    private DamageManager damageManager;

    private void Awake()
    {
        Destroy(gameObject, 3f);
    }


    private void OnTriggerEnter(Collider other)
    {
        // 대미지를 2번 주기 위해 적에 닿았을 때 한 번, 지면에 닿았을 때 한 번 대미지를 주도록 함
        if (other.gameObject.CompareTag("Ground"))
        {            
            downAttack = false;

            damageManager.DamageTransmission(gameObject, enemyObj);
            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            enemyObj = other.gameObject;
            damageManager.DamageTransmission(gameObject, other.gameObject);
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
