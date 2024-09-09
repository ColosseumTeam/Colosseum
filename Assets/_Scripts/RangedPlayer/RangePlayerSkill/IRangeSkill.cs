using UnityEngine;

public interface IRangeSkill
{
    public void GetSkillState(out float getDamage, out bool getSkillType, out bool getDownAttack);

    public void GetDamageManager(DamageManager damageManager);
}
