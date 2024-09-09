using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public void DamageTransmission(GameObject skillObj, GameObject targetObj)
    {
        float damage;
        bool skillType;
        bool downAttack;

        skillObj.GetComponent<IRangeSkill>().GetSkillState(out damage, out skillType, out downAttack);

        targetObj.GetComponent<BotController>().TakeDamage(damage, skillType, downAttack);
    }
}
