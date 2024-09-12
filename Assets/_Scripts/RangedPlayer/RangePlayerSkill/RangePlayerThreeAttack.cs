using UnityEngine;

public class RangePlayerThreeAttack : MonoBehaviour, ISkill
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private bool skillType = false;
    [SerializeField] private bool downAttack = false;
    [SerializeField] private float stiffnessTime = 1f;

    private DamageManager damageManager;

    private Transform player;
    private float timer = 0f;
    private float timerArrive = 10f;

    private bool timerCheck;

    private void Update()
    {
        if (timerCheck)
        {
            transform.position = player.transform.position;
            timer += Time.deltaTime;
            if(timer >= timerArrive)
            {
                timerCheck = false;
                Destroy(gameObject);
            }
        }

    }

    public void GetPlayer(Transform newPlayer)
    {
        player = newPlayer;
        timerCheck = true;
        timer = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            damageManager.DamageTransmission(gameObject, other.gameObject);
        }
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
