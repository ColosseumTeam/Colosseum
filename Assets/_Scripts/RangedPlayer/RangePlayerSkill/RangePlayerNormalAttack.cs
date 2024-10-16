using Fusion;
using System;
using UnityEngine;

public class RangePlayerNormalAttack : NetworkBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private PlayerDamageController.PlayerHitType playerHitType;
    [SerializeField] private BotController.BotHitType botHitType;
    [SerializeField] private bool downAttack = false;
    [SerializeField] private float stiffnessTime = 1f;

    private GameElementsSynchronizer gameElementsSynchronizer;
    private GameObject player;
    private float speed = 20f;
    private Vector3 dir;

    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        // 스킬의 움직임 처리 (필요한 경우 주석 해제)
        transform.position += dir * Runner.DeltaTime * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (collision.gameObject.GetComponent<PlayerDamageController>() != null && collision.gameObject != player && HasStateAuthority)
            {
                collision.gameObject.GetComponent<PlayerDamageController>().TakeDamage(damage, playerHitType, downAttack, stiffnessTime, transform.position);
            }
            else if (collision.gameObject.TryGetComponent(out BotController component))
            {
                Debug.Log("Bot Hit");
                component.TakeDamage(damage, botHitType, downAttack, stiffnessTime, transform.position);
            }

            if (HasStateAuthority)
            {
                Destroy(gameObject);  // 권한이 있는 경우에만 파괴
            }
        }
    }

    public void Look(Vector3 aimPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(aimPos);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform.CompareTag("Enemy"))
        {
            dir = (hit.transform.position - transform.position + new Vector3(0, 0.9f)).normalized;
        }
        else
        {
            dir = (Camera.main.ScreenToWorldPoint(new Vector3(aimPos.x, aimPos.y, 10f)) - transform.position).normalized;
        }
    }

    public void GetRangePlayer(GameObject newPlayer, GameElementsSynchronizer newGameElementsSynchronizer)
    {
        player = newPlayer;
        gameElementsSynchronizer = newGameElementsSynchronizer;

        //int normalId = Guid.NewGuid().GetHashCode();
        //gameElementsSynchronizer.SpawnProjectile(normalId, "RangerLeftClickSkill", Vector3.forward, gameObject.transform.position);
    }
}
