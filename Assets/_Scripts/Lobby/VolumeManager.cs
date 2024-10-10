using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    public float bgVolume = 1f;
    public float skillVolume = 1f;
    public float uiVolume = 1f;
    public float voiceVolume = 1f;


    private void Awake()
    {
        if (FindObjectOfType<VolumeManager>() != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }
}
