using System.Collections;
using UnityEngine;

public class RangePlayerQFourAttackCameraShake : MonoBehaviour
{
    public float shakeDuration = 0.5f;   
    public float shakeMagnitude = 0.02f; 
    public float shakeMagnitudeY = 0.02f; 
    public float shakeMagnitudeZ = 0.02f; 
    public float frequency = 2f;
    public float xRotation = 0f;
    public bool shaking = false;

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition; 
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
