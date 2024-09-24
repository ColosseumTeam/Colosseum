using Fusion;
using UnityEngine;

public class FighterLeftClickSKill : NetworkBehaviour, ISkill
{
    [SerializeField] private float damage = 10f;        // 대미지
    [SerializeField] private bool skillType = false;     // 다운 가능 여부
    [SerializeField] private bool downAttack = false;    // 다운 상태 적 공격 가능 여부
    [SerializeField] private float stiffnessTime = 1f;  // 경직 시간

    [SerializeField] private DamageManager damageManager;

    private void Awake()
    {
        damageManager = GetComponentInParent<PlayerFighterAttackController>().DamageManager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Todo: 데미지매니저의 DamageTransmission 사용
            damageManager.DamageTransmission(gameObject, other.gameObject);

            Debug.Log("Right Click Hit");
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

    }
}
