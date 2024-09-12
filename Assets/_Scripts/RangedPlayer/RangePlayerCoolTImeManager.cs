using System.Collections.Generic;
using UnityEngine;

public class RangePlayerCoolTImeManager : MonoBehaviour
{
    [SerializeField] private List<bool> skillCheckList = new List<bool>() { true, true, true, true };
    public List<bool> SkillCheckLis {  get { return skillCheckList; } }

    [SerializeField] private List<float> skillCoolTimer = new List<float>() { 0, 0, 0, 0 };
    [SerializeField] private List<float> skillCoolTimerEnd = new List<float>();
    
    private void Update()
    {
        for(int ix = 0; ix < skillCoolTimer.Count; ix++)
        {
            SkillManagement(ix);
        }
    }

    public void SkillChecking(int index)
    {
        skillCheckList[index] = false;
    }

    private void SkillManagement(int index)
    {
        if (!skillCheckList[index])
        {
            skillCoolTimer[index] += Time.deltaTime;
            if (skillCoolTimer[index] >= skillCoolTimerEnd[index])
            {
                skillCoolTimer[index] = 0;
                skillCheckList[index] = true;
            }
        }
    }    
}
