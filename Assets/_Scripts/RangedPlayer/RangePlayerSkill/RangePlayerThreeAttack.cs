using Fusion;
using UnityEngine;

public class RangePlayerThreeAttack : NetworkBehaviour, ISkill
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private bool skillType = false;
    [SerializeField] private bool downAttack = false;
    [SerializeField] private float stiffnessTime = 1f;
    [SerializeField] private float dealingPeriodTime = 0f;
    [SerializeField] private float dealingPeriodEndTIme = 0.5f;

    private DamageManager damageManager;
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
            damageManager.DamageTransmission(gameObject, otherGameObject);
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

    public void GetSkillState(out float getDamage, out bool getSkillType, out bool getDownAttack, out float getStiffnessTime)
    {
        getDamage = damage;
        getSkillType = skillType;
        getDownAttack = downAttack;
        getStiffnessTime = stiffnessTime;
    }

    public void GetDamageManager(DamageManager newDamageManager)
    {
        damageManager = newDamageManager;
    }
}
