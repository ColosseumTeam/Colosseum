using Fusion;
using UnityEngine;

public class FighterPlayerShiftClick : NetworkBehaviour, ISkill
{
    [SerializeField] private float damage = 10f;        // 대미지
    [SerializeField] private bool skillType = true;     // 다운 가능 여부
    [SerializeField] private bool downAttack = true;    // 다운 상태 적 공격 가능 여부
    [SerializeField] private float stiffnessTime = 1f;  // 경직 시간

    private DamageManager damageManager;

    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            damageManager.DamageTransmission(gameObject, other.gameObject);

            // Todo: 데미지 처리 필요.
            Debug.Log("ShiftClick Skill Hit");
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
