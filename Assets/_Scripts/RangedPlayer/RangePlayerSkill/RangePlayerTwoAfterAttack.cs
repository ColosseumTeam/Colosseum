using UnityEngine;
using static BotController;
using static PlayerDamageController;

public class RangePlayerTwoAfterAttack : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private PlayerDamageController.PlayerHitType playerHitType;
    [SerializeField] private BotController.BotHitType botHitType;
    [SerializeField] private bool downAttack = false;

    private GameObject targetObj;

    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy" && other.gameObject == targetObj)
        {
            if (other.gameObject.GetComponent<PlayerDamageController>() != null)
            {
                other.gameObject.GetComponent<PlayerDamageController>().RPC_TakeDamage(damage, playerHitType, downAttack, 1f);
            }
            else
            {
                other.gameObject.GetComponent<BotController>().TakeDamage(damage, botHitType, downAttack, 1f);
            }
        }
    }

    public void GetTargetObject(GameObject newTargetObj)
    {
        targetObj = newTargetObj;
    }
}
