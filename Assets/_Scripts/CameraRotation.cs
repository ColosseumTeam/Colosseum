using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;  // 마우스 감도
    [SerializeField] private Transform playerBody;  // 카메라가 따라다닐 플레이어 트랜스폼
    [SerializeField] private float maxXRotation = -10f;
    [SerializeField] private float minXRotation = 25f;

    [SerializeField] private float xRotation = 0f;  // X축 회전 제한

    private void Start()
    {
        // 마우스 커서 고정
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // 마우스 입력 받기
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 마우스 Y축에 따른 카메라 상하 회전 
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, maxXRotation, minXRotation);  // 상하 회전 제한

        // 카메라의 로컬 X축 회전 적용
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);       

        // 마우스 X축에 따른 플레이어 좌우 회전 
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
