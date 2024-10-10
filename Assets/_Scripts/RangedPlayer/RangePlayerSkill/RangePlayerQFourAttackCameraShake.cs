using System.Collections;
using UnityEngine;

public class RangePlayerQFourAttackCameraShake : MonoBehaviour
{
    public float shakeDuration = 0.5f;   
    public float shakeMagnitudeX = 0.02f; 
    public float shakeMagnitudeY = 0.02f; 
    public float shakeMagnitudeZ = 0.02f; 
    public float frequency = 2f;         

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition; 
    }

    public void StartShake()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            float xOffset = Mathf.Sin((elapsed * frequency * Mathf.PI * 2) + Mathf.PI / 3) * shakeMagnitudeX;
            float yOffset = Mathf.Sin((elapsed * frequency * Mathf.PI * 2) + Mathf.PI / 2) * shakeMagnitudeY;
            float zOffset = Mathf.Sin(elapsed * frequency * Mathf.PI * 2) * shakeMagnitudeZ;

            transform.localPosition = new Vector3(originalPosition.x + xOffset, originalPosition.y + yOffset, originalPosition.z + zOffset);

            elapsed += Time.deltaTime;

            yield return null; 
        }

        transform.localPosition = originalPosition;
    }
}
