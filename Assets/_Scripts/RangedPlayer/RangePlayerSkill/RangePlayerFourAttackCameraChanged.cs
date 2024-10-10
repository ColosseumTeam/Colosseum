using UnityEngine;

public class RangePlayerFourAttackCameraChanged : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera closeUpCamera;

    private Vector3 fixedPosition;
    private Quaternion fixedRoataion;

    private void Awake()
    {
        mainCamera.enabled = true;
        closeUpCamera.enabled = false;
    }    

    public void ActivateCloseUp()
    {       
        mainCamera.enabled = false;
        closeUpCamera.enabled = true;
    }

    public void DeactivateCloseUp()
    {     
        mainCamera.enabled = true;
        closeUpCamera.enabled = false;
    }
}
