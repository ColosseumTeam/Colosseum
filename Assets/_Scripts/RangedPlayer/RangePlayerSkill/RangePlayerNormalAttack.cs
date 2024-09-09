using UnityEngine;

public class RangePlayerNormalAttack : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ground 태그와 충돌한 경우
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); // 현재 오브젝트 파괴
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<BotController>().TakeDamage(10f, 0f, false);

            Destroy(gameObject); // 현재 오브젝트 파괴
        }
    }
}
