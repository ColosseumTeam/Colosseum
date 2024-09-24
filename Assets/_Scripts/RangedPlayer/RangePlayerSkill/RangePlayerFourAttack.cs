using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class RangePlayerFourAttack : NetworkBehaviour, ISkill
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private bool skillType = true;
    [SerializeField] private bool downAttack = true;
    [SerializeField] private float attackTime = 0f;
    [SerializeField] private float attackTimeEnd = 0.1f;
    [SerializeField] private GameObject afterAttackObject;

    private bool isAttacking;
    private GameObject enemyObj;
    private DamageManager damageManager;

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

                damageManager.DamageTransmission(gameObject, enemyObj);
                Destroy(gameObject);
            }            
        }   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemyObj = other.gameObject;
            damageManager.DamageTransmission(gameObject, other.gameObject);
        }

        if (other.gameObject.CompareTag("Ground"))
        {
            isAttacking = true;
            attackTime = 0f;
        }
    }


    public void GetSkillState(out float getDamage, out bool getSkillType, out bool getDownAttack, out float getStiffnessTime)
    {
        getDamage = damage;
        getSkillType = skillType;
        getDownAttack = downAttack;
        
        // 다운 스킬이기에 경직 시간 0으로 고정
        getStiffnessTime = 0f;

    }

    public void GetDamageManager(DamageManager newDamageManager)
    {
        damageManager = newDamageManager;
    }
}
