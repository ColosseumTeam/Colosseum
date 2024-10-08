using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickButtonSound : MonoBehaviour
{
    [SerializeField] private List<Button> buttonList;
    [SerializeField] private List<Button> beforeButton;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClip;
    [SerializeField] private AudioClip beforeClip;

    private void Awake()
    {        
        foreach (var button in buttonList)
        {
            button.GetComponent<Button>().onClick.AddListener(StartSound);
        }

        foreach(var button in beforeButton)
        {
            button.GetComponent<Button>().onClick.AddListener(BeforeSound);
        }
    }


    private void StartSound()
    {
        audioSource.clip = buttonClip;
        GetComponent<AudioSource>().Play();
    }

    private void BeforeSound()
    {
        audioSource.clip = beforeClip;
        GetComponent<AudioSource>().Play();
    }
}
