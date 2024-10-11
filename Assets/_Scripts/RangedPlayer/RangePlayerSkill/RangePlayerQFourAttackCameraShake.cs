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

    //public void StartShake()
    //{
    //    StartCoroutine(Shake());
    //}

    //IEnumerator Shake()
    //{
    //    float elapsed = 0.0f;

    //    while (elapsed < shakeDuration)
    //    {
    //        float xOffset = Mathf.Sin((elapsed * frequency * Mathf.PI * 2) + Mathf.PI / 3) * shakeMagnitudeX;
    //        float yOffset = Mathf.Sin((elapsed * frequency * Mathf.PI * 2) + Mathf.PI / 2) * shakeMagnitudeY;
    //        float zOffset = Mathf.Sin(elapsed * frequency * Mathf.PI * 2) * shakeMagnitudeZ;

    //        transform.localPosition = new Vector3(originalPosition.x + xOffset, originalPosition.y + yOffset, originalPosition.z + zOffset);

    //        elapsed += Time.deltaTime;

    //        yield return null; 
    //    }

    //    transform.localPosition = originalPosition;
    //}

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
