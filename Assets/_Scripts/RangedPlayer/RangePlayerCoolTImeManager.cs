using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RangePlayerCoolTImeManager : MonoBehaviour
{   
    [SerializeField] private List<bool> skillCheckList = new List<bool>() { true, true, true, true };
    public List<bool> SkillCheckLis {  get { return skillCheckList; } }

    [SerializeField] private List<float> skillCoolTimer = new List<float>() { 0, 0, 0, 0 };
    [SerializeField] private List<float> skillCoolTimerEnd = new List<float>();
    [SerializeField] private List<GameObject> skillUI;
    
    private float skillUIStartFill;

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
        skillUI[index].SetActive(true);
        skillUIStartFill = skillUI[index].GetComponent<Image>().fillAmount;
    }

    private void SkillManagement(int index)
    {
        if (!skillCheckList[index])
        { 
            skillCoolTimer[index] += Time.deltaTime;

            skillUI[index].GetComponent<Image>().fillAmount = 
                Mathf.Lerp(skillUIStartFill, 0f, skillCoolTimer[index] / skillCoolTimerEnd[index]);

            if (skillCoolTimer[index] >= skillCoolTimerEnd[index])
            {
                skillCoolTimer[index] = 0;
                skillCheckList[index] = true;

                skillUI[index].GetComponent<Image>().fillAmount = 1f;
                skillUI[index].SetActive(false);               
            }
        }
    }
}
