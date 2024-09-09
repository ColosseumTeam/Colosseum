using UnityEngine;

public class RangePlayerFourAttack : MonoBehaviour, IRangeSkill
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private bool skillType = true;
    [SerializeField] private bool downAttack = true;

    private DamageManager damageManager;

    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ground 태그와 충돌한 경우
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); // 현재 오브젝트 파괴
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            damageManager.DamageTransmission(gameObject, collision.gameObject);

            Destroy(gameObject); // 현재 오브젝트 파괴
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
