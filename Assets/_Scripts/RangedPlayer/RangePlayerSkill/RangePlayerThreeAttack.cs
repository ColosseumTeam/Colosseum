using Fusion;
using UnityEngine;

public class RangePlayerThreeAttack : NetworkBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private PlayerDamageController.PlayerHitType playerHitType;
    [SerializeField] private BotController.BotHitType botHitType;
    [SerializeField] private bool downAttack = false;
    [SerializeField] private float stiffnessTime = 1f;
    [SerializeField] private float dealingPeriodTime = 0f;
    [SerializeField] private float dealingPeriodEndTIme = 0.5f;

    private Transform player;
    private GameObject otherGameObject;
    private float timer = 0f;
    private float timerArrive = 10f;
    private bool timerCheck;

    private void Update()
    {
        if (timerCheck)
        {
            transform.position = player.transform.position;
            timer += Time.deltaTime;
            if (timer >= timerArrive)
            {
                timerCheck = false;
                Destroy(gameObject);
            }
        }

        if (otherGameObject != null)
        {
            DealingPeriodEvent();
        }
    }

    public void GetPlayer(Transform newPlayer)
    {
        player = newPlayer;
        timerCheck = true;
        timer = 0f;
    }

    private void DealingPeriodEvent()
    {
        dealingPeriodTime += Time.deltaTime;

        if (dealingPeriodTime >= dealingPeriodEndTIme)
        {
            if (otherGameObject.GetComponent<PlayerDamageController>() != null)
            {
                otherGameObject.GetComponent<PlayerDamageController>().TakeDamage(damage, playerHitType, downAttack, stiffnessTime);
            }
            else
            {
                otherGameObject.GetComponent<BotController>().TakeDamage(
                    damage, botHitType, downAttack, stiffnessTime);
            }
            dealingPeriodTime = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            otherGameObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        otherGameObject = null;
    }
}
