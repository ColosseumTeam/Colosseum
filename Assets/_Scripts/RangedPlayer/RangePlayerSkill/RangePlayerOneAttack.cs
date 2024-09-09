using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangePlayerOneAttack : MonoBehaviour
{
    private float maxHeight = 0f;
    private float upSpeed = 20f;

    private void Awake()
    {
        Destroy(gameObject, 1.5f);
    }

    private void Update()
    {
        if (transform.position.y < maxHeight)
        {
            // 기존 position을 받아와서 y 값을 수정한 후 다시 할당
            Vector3 newPosition = transform.position;
            newPosition.y += upSpeed * Time.deltaTime;
            transform.position = newPosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            other.GetComponent<BotController>().TakeDamage(10f, 2f, true);
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
