using UnityEditor.Rendering;
using UnityEngine;

public class RangePlayerFourAttackCameraChanged : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera closeUpCamera;


    private void Awake()
    {
        mainCamera.enabled = true;
        closeUpCamera.enabled = false;
    }    

    public void ActivateCloseUp()
    {       
        mainCamera.enabled = false;
        closeUpCamera.enabled = true;

        //closeUpCamera.GetComponent<RangePlayerQFourAttackCameraShake>().StartShake();
        closeUpCamera.GetComponent<RangePlayerQFourAttackCameraShake>().CameraShake();
    }

    public void DeactivateCloseUp()
    {     
        mainCamera.enabled = true;
        closeUpCamera.enabled = false;
    }
}
