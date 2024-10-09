using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSettingManager : MonoBehaviour
{
    [Header("# Sound bar Settings")]
    [SerializeField] private Slider bgBar;
    [SerializeField] private Slider skillBar;
    [SerializeField] private Slider uiBar;
    [SerializeField] private Slider voiceBar;

    [Header("# Audio Sources")]
    [SerializeField] private AudioSource bgAudioSource;
    [SerializeField] private AudioSource skillAudioSource;
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioSource voiceAudioSource;

    private VolumeManager volumeManager;

    //private float bgVolume = 1f;
    //private float skillVolume = 1f;
    //private float uiVolume = 1f;
    //private float voiceVolume = 1f;


    private void Awake()
    {
        volumeManager = FindObjectOfType<VolumeManager>();
    }

    private void Start()
    {
        InitVolumeOnLobbyScene();
    }

    public void InitReference()
    {
        LobbyManager lobbyManager = FindObjectOfType<LobbyManager>();

        bgBar = lobbyManager.BgBar;
        skillBar = lobbyManager.SkillBar;
        uiBar = lobbyManager.UiBar;
        voiceBar = lobbyManager.VoiceBar;

        bgAudioSource = lobbyManager.BgAudioSource;
        uiAudioSource = lobbyManager.UiAudioSource;
    }

    // Will be used on first line of SceneLoadDone
    public void InitVolumeSettings()
    {
        bgBar.value = volumeManager.bgVolume;
        skillBar.value = volumeManager.skillVolume;
        uiBar.value = volumeManager.uiVolume;
        voiceBar.value = volumeManager.voiceVolume;
    }

    public void InitVolumeOnLobbyScene()
    {
        LobbyManager lobbyManager = FindAnyObjectByType<LobbyManager>();

        bgAudioSource = lobbyManager.BgAudioSource;
        uiAudioSource = lobbyManager.UiAudioSource;

        bgAudioSource.volume = volumeManager.bgVolume;
        uiAudioSource.volume = volumeManager.uiVolume;

        InitVolumeSettings();
    }

    public void InitVolumeOnGameScene()
    {
        GameManager gameManager = FindAnyObjectByType<GameManager>();

        bgAudioSource = gameManager.BgAudioSource;
        // Todo: skillAudioSource를 PlayerData에서 받아와야 함
        // Todo: voiceAudioSource를 PlayerData에서 받아와야 함

        bgAudioSource.volume = volumeManager.bgVolume;
        //skillAudioSource.volume = volumeManager.skillVolume;
        //voiceAudioSource.volume = volumeManager.voiceVolume;
    }

    public void SetBackgroundVolume()
    {
        bgAudioSource.volume = bgBar.value;
        volumeManager.bgVolume = bgBar.value;
    }

    public void SetSkillVolume()
    {
        //skillAudioSource.volume = skillBar.value;
        volumeManager.skillVolume = skillBar.value;
    }

    public void SetUIVolume()
    {
        uiAudioSource.volume = uiBar.value;
        volumeManager.uiVolume = uiBar.value;
    }

    public void SetVoiceVolume()
    {
        //voiceAudioSource.volume = voiceBar.value;
        volumeManager.voiceVolume = voiceBar.value;
    }
}
