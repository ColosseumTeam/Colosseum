using System.Collections;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private float maxXRotation = -10f;
    [SerializeField] private float minXRotation = 25f;
    [SerializeField] private float xRotation = 0f;

    [SerializeField] private float shakeDuration = 0.5f; // 흔들림 지속 시간
    [SerializeField] private float shakeMagnitude = 0.1f; // 흔들림 강도
    [SerializeField] private float shakeSpeed = 10f;     // 흔들림 속도

    private bool shaking = false;
    private Quaternion originalRotation;

    private void Start()
    {
        // 마우스 커서 고정
        Cursor.lockState = CursorLockMode.Locked;
        originalRotation = transform.localRotation;
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

    public void CameraShake()
    {
        if (!shaking)
        {
            StartCoroutine(Shake());
        }
    }

    private IEnumerator Shake()
    {
        shaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float angle = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.localRotation = Quaternion.Euler(xRotation, 0f, angle);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        shaking = false;
    }
}
