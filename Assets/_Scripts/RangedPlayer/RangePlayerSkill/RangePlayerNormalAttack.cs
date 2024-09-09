using UnityEngine;

public class RangePlayerNormalAttack : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ground �±׿� �浹�� ���
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject); // ���� ������Ʈ �ı�
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<BotController>().TakeDamage(10f, 0f, false);

            Destroy(gameObject); // ���� ������Ʈ �ı�
        }
    }
}
