using UnityEngine;

public class RangePlayerThreeAttack : MonoBehaviour
{
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
            other.gameObject.GetComponent<BotController>().TakeDamage(10f, 0f, false);
        }
    }
}
