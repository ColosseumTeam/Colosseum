using Fusion;
using UnityEngine;

public class RangePlayerThreeAttack : NetworkBehaviour
{    
    private Transform centerObject;
    private float orbitRadius;
    [SerializeField] private float orbitSpeed;
    private float currentAngle;
    private AudioSource audioSource;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (centerObject == null)
        {            
            enabled = false;
            return; 
        }

        Vector3 offset = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), 0, Mathf.Sin(currentAngle * Mathf.Deg2Rad)) * orbitRadius;
        transform.position = centerObject.position + offset;

        Destroy(gameObject, 10f);
    }

    [Rpc]
    public void RPC_SetVolume()
    {
        audioSource.volume = FindObjectOfType<VolumeManager>().skillVolume;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (centerObject == null)
            return;

        currentAngle += orbitSpeed * Runner.DeltaTime;
        if (currentAngle >= 360f)
        {
            currentAngle -= 360f;
        }

        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * orbitRadius;
        transform.position = centerObject.position + offset;
    }
    

    public void ThreeSkillOptionChanged(Transform newCenterObject, float newOrbitRadius, float newOrbitSpeed, float newCurrentAngle)
    {        
        centerObject = newCenterObject;
        orbitRadius = newOrbitRadius;
        orbitSpeed = newOrbitSpeed;
        currentAngle = newCurrentAngle;
    }
}
