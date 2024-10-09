using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [Header("# Sound bar Settings")]
    [SerializeField] private Slider bgBar;
    [SerializeField] private Slider skillBar;
    [SerializeField] private Slider uiBar;
    [SerializeField] private Slider voiceBar;

    [Header("# Volume Settings Reference")]
    [SerializeField] private AudioSource bgAudioSource;
    [SerializeField] private AudioSource uiAudioSource;

    public AudioSource BgAudioSource { get { return bgAudioSource; } }
    public AudioSource UiAudioSource { get { return uiAudioSource; } }
    public Slider BgBar { get { return bgBar; } }
    public Slider SkillBar { get { return skillBar; } }
    public Slider UiBar { get { return uiBar; } }
    public Slider VoiceBar { get { return voiceBar; } }
}
