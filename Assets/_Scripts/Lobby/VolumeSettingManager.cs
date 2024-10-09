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

    private float bgVolume = 1f;
    private float skillVolume = 1f;
    private float uiVolume = 1f;
    private float voiceVolume = 1f;


    private void Awake()
    {
        if (FindAnyObjectByType<VolumeSettingManager>() != this)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this);

        InitVolumeOnLobbyScene();
    }

    // Will be used on first line of SceneLoadDone
    public void InitVolumeSettings()
    {
        bgBar.value = bgVolume;
        skillBar.value = skillVolume;
        uiBar.value = uiVolume;
        voiceBar.value = voiceVolume;
    }

    public void InitVolumeOnLobbyScene()
    {
        LobbyManager lobbyManager = FindAnyObjectByType<LobbyManager>();

        bgAudioSource = lobbyManager.BgAudioSource;
        uiAudioSource = lobbyManager.UiAudioSource;

        bgAudioSource.volume = bgVolume;
        uiAudioSource.volume = uiVolume;

        InitVolumeSettings();
    }

    public void InitVolumeOnGameScene()
    {
        GameManager gameManager = FindAnyObjectByType<GameManager>();

        bgAudioSource = gameManager.BgAudioSource;
        // Todo: skillAudioSource를 PlayerData에서 받아와야 함
        // Todo: voiceAudioSource를 PlayerData에서 받아와야 함

        bgAudioSource.volume = bgVolume;
        //skillAudioSource.volume = skillVolume;
        //voiceAudioSource.volume = voiceVolume;
    }

    public void SetBackgroundVolume()
    {
        bgAudioSource.volume = bgBar.value;
        bgVolume = bgBar.value;
    }

    public void SetSkillVolume()
    {
        //skillAudioSource.volume = skillBar.value;
        skillVolume = skillBar.value;
    }

    public void SetUIVolume()
    {
        uiAudioSource.volume = uiBar.value;
        uiVolume = uiBar.value;
    }

    public void SetVoiceVolume()
    {
        //voiceAudioSource.volume = voiceBar.value;
        voiceVolume = voiceBar.value;
    }
}
