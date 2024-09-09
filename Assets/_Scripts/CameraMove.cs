using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 60f;
    [SerializeField] private float minXRotation = -60f;
    [SerializeField] private float maxXRotation = 30f;

    private float xRotation = 0f;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnLook(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        xRotation -= input.y * rotationSpeed * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, minXRotation, maxXRotation);
        transform.localRotation = Quaternion.Euler(xRotation, transform.localRotation.y, transform.localRotation.z);
    }
}
