using UnityEngine;

public class KebordSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] KeyCode[] ignoredKeys;

    private bool kebordSoundCheck;

    void Update()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {            
            if (Input.GetKeyDown(key))
            {
                if (IsIgnoredKey(key))
                    continue;

                PlayKeySound();
            }
        }
    }

    void PlayKeySound()
    {
        if (audioSource != null && kebordSoundCheck)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
    }

    bool IsIgnoredKey(KeyCode key)
    {
        foreach (KeyCode ignoredKey in ignoredKeys)
        {
            if (key == ignoredKey)
                return true;
        }
        return false;
    }

    public void KeyBordSoundCheck(bool check)
    {
        kebordSoundCheck = check;
    }
}
