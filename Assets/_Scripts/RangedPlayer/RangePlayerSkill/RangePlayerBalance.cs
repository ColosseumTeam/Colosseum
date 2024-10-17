
using Fusion;
using UnityEngine;

public class RangePlayerBalance : NetworkBehaviour
{
    private AudioSource audioSource;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        Destroy(gameObject, 3f);
    }

    [Rpc]
    public void RPC_SetVolume()
    {
        audioSource.volume = FindObjectOfType<VolumeManager>().skillVolume;
    }
}
