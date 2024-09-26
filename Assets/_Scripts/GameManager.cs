using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private AimController aimController;
    [SerializeField] private List<Image> skillUI;

    public AimController AimController { get { return aimController; } }
    public List<Image> SkillUI { get { return skillUI; } }
}
