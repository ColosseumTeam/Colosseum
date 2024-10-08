using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [Header("# Volume Settings Reference")]
    [SerializeField] private AudioSource bgAudioSource;
    [SerializeField] private AudioSource uiAudioSource;

    public AudioSource BgAudioSource { get { return bgAudioSource; } }
    public AudioSource UiAudioSource { get { return uiAudioSource; } }
}
