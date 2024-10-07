using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    [Header("# UI")]
    [SerializeField] private AimController aimController;
    [SerializeField] private List<Image> skillUI;
    [SerializeField] private Image hpBar;

    [Header("# Audio")]
    [SerializeField] private AudioSource bgAudioSource;

    public AimController AimController { get { return aimController; } }
    public List<Image> SkillUI { get { return skillUI; } }
    public Image HpBar { get { return hpBar; } }
    public AudioSource BgAudioSource { get { return bgAudioSource; } }
}
