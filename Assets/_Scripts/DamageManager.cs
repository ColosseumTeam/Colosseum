using Fusion;
using UnityEngine;

public class DamageManager : NetworkBehaviour
{
    public void DamageTransmission(GameObject skillObj, GameObject targetObj)
    {
        float damage;
        bool skillType;
        bool downAttack;
        float stiffnessTime;

        skillObj.GetComponent<ISkill>().GetSkillState(out damage, out skillType, out downAttack, out stiffnessTime);

        // 상대방이 Enemy Tag를 보유했을 때
        if (targetObj != null && targetObj.tag == "Enemy" || /* 수정 예정*/ targetObj.tag == "Player")
        {
            // 상대방이 봇일 때
            if (targetObj.GetComponent<BotController>() != null)
            {
                //targetObj.GetComponent<BotController>().TakeDamage(damage, skillType, downAttack, stiffnessTime);
            }

            // 상대방이 플레이어일 때
            else if(targetObj.GetComponent<PlayerDamageController>() != null)
            {
                //targetObj.GetComponent<PlayerDamageController>().TakeDamage(damage, skillType, downAttack, stiffnessTime);
            }
        }
    }
}
