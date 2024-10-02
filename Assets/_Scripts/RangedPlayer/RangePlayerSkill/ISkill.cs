using UnityEngine;

public interface ISkill
{    


    public void GetSkillState(
        out float getDamage, // ?€ë¯¸ì?
        out bool getSkillType, // ?¤ìš´ ?¤í‚¬ ?¬ë?
        out bool getDownAttack, // ?¤ìš´ ?íƒœ?ì„œ ê³µê²© ê°€???¬ë?
        out float stiffnessTime); // ê²½ì§ ?œê°„

    public void GetDamageManager(DamageManager damageManager);
}
