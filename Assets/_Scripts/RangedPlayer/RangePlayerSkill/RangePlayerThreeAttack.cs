using Fusion;
using UnityEngine;

public class RangePlayerThreeAttack : NetworkBehaviour
{    
    private Transform centerObject;
    private float orbitRadius;
    private float orbitSpeed;
    private float currentAngle;    

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

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (centerObject == null)
            return;

        currentAngle += orbitSpeed * Time.deltaTime;
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
