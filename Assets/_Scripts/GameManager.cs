using Fusion;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    [Header("# UI")]
    [SerializeField] private AimController aimController;
    [SerializeField] private List<Image> skillUI;
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI gameTime;

    [Header("# Audio")]
    [SerializeField] private AudioSource bgAudioSource;

    [Header("# Game Options")]
    [SerializeField] private float gameTimer = 300;
    [SerializeField] private Transform spawnPosition1;
    [SerializeField] private Transform spawnPosition2;

    private VolumeManager volumeManager;

    public AimController AimController { get { return aimController; } }
    public List<Image> SkillUI { get { return skillUI; } }
    public Image HpBar { get { return hpBar; } }
    public AudioSource BgAudioSource { get { return bgAudioSource; } }
    public Transform SpawnPosition1 { get { return spawnPosition1; } }
    public Transform SpawnPosition2 { get { return spawnPosition2; } }


    private void Awake()
    {
        volumeManager = FindObjectOfType<VolumeManager>();
    }

    private void Start()
    {
        bgAudioSource.volume = volumeManager.bgVolume;
    }

    private void Update()
    {
        gameTimer -= Time.deltaTime;

        gameTime.text = $"{(int)(gameTimer / 60)}:{(int)(gameTimer % 60)}";

        if (gameTimer <= 0)
        {
            TimeOver();
        }
    }

    private void TimeOver()
    {
        PlayerDamageController[] playerDamageControllers = FindObjectsOfType<PlayerDamageController>();
        float p1hp = playerDamageControllers[0].Hp / playerDamageControllers[0].MaxHp;
        float p2hp = playerDamageControllers[1].Hp / playerDamageControllers[1].MaxHp;

        if (p1hp >= p2hp)
        {
            GetComponent<ResultSceneConversion>().RPC_ResultSceneBringIn(playerDamageControllers[0].PlayerData.playerNumber, playerDamageControllers[1].PlayerData.playerNumber);
        }
        else
        {
            GetComponent<ResultSceneConversion>().RPC_ResultSceneBringIn(playerDamageControllers[1].PlayerData.playerNumber, playerDamageControllers[0].PlayerData.playerNumber);
        }
    }
}
