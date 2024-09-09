using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 100f;  // ���콺 ����
    [SerializeField] private Transform playerBody;  // ī�޶� ����ٴ� �÷��̾� Ʈ������

    private float maxXRotation = -10f;
    private float minXRotation = 25f;

    private float xRotation = 0f;  // X�� ȸ�� ����

    private void Start()
    {
        // ���콺 Ŀ�� ����
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // ���콺 �Է� �ޱ�
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ���콺 Y�࿡ ���� ī�޶� ���� ȸ�� (��ġ)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, maxXRotation, minXRotation);  // ���� ȸ�� ����

        // ī�޶��� ���� X�� ȸ�� ����
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // ���콺 X�࿡ ���� �÷��̾� �¿� ȸ�� (���)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
