using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private AimController aimController;
    [SerializeField] private List<Image> skillUI;
    [SerializeField] private Image hpBar;

    public AimController AimController { get { return aimController; } }
    public List<Image> SkillUI { get { return skillUI; } }
    public Image HpBar { get { return hpBar; } }
}
