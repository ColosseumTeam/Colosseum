using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RangePlayerCoolTImeManager : MonoBehaviour
{
    [SerializeField] private List<bool> skillCheckList = new List<bool>() { true, true, true, true };    
    [SerializeField] private List<float> skillCoolTimer = new List<float>() { 0, 0, 0, 0 };
    [SerializeField] private List<float> skillCoolTimerEnd = new List<float>();
    [SerializeField] private List<Image> skillUI;

    public List<bool> SkillCheckLis { get { return skillCheckList; } }

    private void Awake()
    {
        skillUI = FindAnyObjectByType<GameManager>().SkillUI;
    }

    private void Update()
    {
        for (int ix = 0; ix < skillCoolTimer.Count; ix++)
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

            skillUI[index].fillAmount = 1 - skillCoolTimer[index] / skillCoolTimerEnd[index];

            if (skillCoolTimer[index] >= skillCoolTimerEnd[index])
            {
                skillCoolTimer[index] = 0;
                skillCheckList[index] = true;
            }
        }
    }
}
