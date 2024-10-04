using UnityEngine;

public class FighterQSkillProjectile : FighterQSkill
{
    private void OnEnable()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground"))
        {
            // Todo: Destory 사용한 모든 오브젝트 풀링 필요.
            Destroy(gameObject);
        }

        if (collision.transform.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponent<PlayerDamageController>() != null)
            {
                collision.gameObject.GetComponent<PlayerDamageController>().RPC_TakeDamage(damage, playerHitType, downAttack, stiffnessTime);
            }
            else
            {
                if (collision.gameObject.TryGetComponent(out BotController component))
                {
                    component.TakeDamage(damage, botHitType, downAttack, stiffnessTime);
                }
            }

            Destroy(gameObject, 1f);
        }
    }
}
