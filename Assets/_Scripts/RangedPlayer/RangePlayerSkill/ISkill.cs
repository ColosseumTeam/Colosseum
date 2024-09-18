using UnityEngine;

public interface ISkill
{
    public void GetSkillState(
        out float getDamage, // 대미지
        out bool getSkillType, // 다운 스킬 여부
        out bool getDownAttack, // 다운 상태에서 공격 가능 여부
        out float stiffnessTime); // 경직 시간

    public void GetDamageManager(DamageManager damageManager);
}
