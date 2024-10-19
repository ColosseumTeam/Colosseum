using Fusion;
using UnityEngine;

public class RangePlayerFourAfterAttack : NetworkBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private PlayerDamageController.PlayerHitType playerHitType;
    [SerializeField] private BotController.BotHitType botHitType;
    [SerializeField] private bool downAttack = true;

    private Camera rotationCamera;
    private GameObject ranger;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        Destroy(gameObject, 1f);
    }

    private void Start()
    {
        rotationCamera.GetComponent<CameraRotation>().CameraShake();
    }

    [Rpc]
    public void RPC_SetVolume()
    {
        audioSource.volume = FindObjectOfType<VolumeManager>().skillVolume;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);

        if (other.gameObject.tag == "Enemy")
        {
            if (other.gameObject.GetComponentInParent<PlayerDamageController>() != null && other.gameObject != ranger && HasStateAuthority)
            {
                other.gameObject.GetComponentInParent<PlayerDamageController>().TakeDamage(damage, playerHitType, downAttack, 1f, transform.position);
                other.gameObject.GetComponentInParent<PlayerDamageController>().DownTimeChanged(3f);
            }

            if (other.gameObject.TryGetComponent(out BotController component))
            {
                component.TakeDamage(damage, botHitType, downAttack, 1f, transform.position);
            }
        }
    }

    public void GetRanger(GameObject newRanger, Camera newCamera)
    {
        ranger = newRanger;
        rotationCamera = newCamera;
    }
}
