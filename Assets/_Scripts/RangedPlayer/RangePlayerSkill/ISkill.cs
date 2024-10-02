using UnityEngine;

public interface ISkill
{    


    public void GetSkillState(
        out float getDamage, // ?�미�?
        out bool getSkillType, // ?�운 ?�킬 ?��?
        out bool getDownAttack, // ?�운 ?�태?�서 공격 가???��?
        out float stiffnessTime); // 경직 ?�간

    public void GetDamageManager(DamageManager damageManager);
}
