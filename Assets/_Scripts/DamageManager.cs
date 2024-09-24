using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public void DamageTransmission(GameObject skillObj, GameObject targetObj)
    {
        float damage;
        bool skillType;
        bool downAttack;
        float stiffnessTime;

        skillObj.GetComponent<ISkill>().GetSkillState(out damage, out skillType, out downAttack, out stiffnessTime);

        if (targetObj != null && targetObj.tag == "Enemy")
        {
            if (targetObj.GetComponent<BotController>() != null)
            {
                targetObj.GetComponent<BotController>().TakeDamage(damage, skillType, downAttack, stiffnessTime);
            }
            else if(targetObj.GetComponent<PlayerController>() != null)
            {
                targetObj.GetComponent<PlayerController>().TakeDamage(damage, skillType, downAttack, stiffnessTime);
            }
        }
    }
}
