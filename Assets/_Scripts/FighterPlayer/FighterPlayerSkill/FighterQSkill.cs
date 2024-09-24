using Fusion;
using UnityEngine;

public class FighterQSkill : NetworkBehaviour, ISkill
{
    [SerializeField] private float damage = 10f;        // 대미지
    [SerializeField] private bool skillType = false;     // 다운 가능 여부
    [SerializeField] private bool downAttack = false;    // 다운 상태 적 공격 가능 여부
    [SerializeField] private float stiffnessTime = 1f;  // 경직 시간
    [SerializeField] private float speed = 5f;

    private DamageManager damageManager;
    private Vector3 dir;


    private void OnEnable()
    {
        // Todo: Destory 사용한 모든 오브젝트 풀링 필요.
        Destroy(gameObject, 2f);
    }

    private void Update()
    {
        transform.position += dir * Time.deltaTime * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            // Todo: Destory 사용한 모든 오브젝트 풀링 필요.
            Destroy(gameObject);
        }
        if (other.CompareTag("Enemy"))
        {
            // Todo: Destory 사용한 모든 오브젝트 풀링 필요.
            // Todo: 데미지 처리 필요.
            damageManager.DamageTransmission(gameObject, other.gameObject);

            Debug.Log("Q Skill Hit");
            Destroy(gameObject);
        }
    }

    public void Look(Vector3 aimPos)
    {
        dir = (Camera.main.ScreenToWorldPoint(new Vector3(aimPos.x, aimPos.y, 10f)) - transform.position).normalized;
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
