using Fusion;
using UnityEngine;
using static BotController;
using static PlayerDamageController;

public class RangePlayerTwoAfterAttack : NetworkBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private PlayerDamageController.PlayerHitType playerHitType;
    [SerializeField] private BotController.BotHitType botHitType;
    [SerializeField] private bool downAttack = false;
    [SerializeField] private GameObject attecktEffect;    
    [SerializeField] private GameObject player;

    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter(Collider other)
    {        

        if (!HasStateAuthority)
            return;

        var obj = other.transform.root;

        if (obj.TryGetComponent(out PlayerDamageController controller) &&
            controller.gameObject.tag == "Enemy")
        {
            if (other.gameObject != player)
            {
                Debug.Log("Main Two Skill After Attack");
                other.gameObject.GetComponentInParent<PlayerDamageController>().TakeDamage(damage, playerHitType, downAttack, 1f, transform.position);
                //Runner.Spawn(attecktEffect, gameObject.transform.position, gameObject.transform.rotation);
            }

            //else
            //{
            //    if (other.gameObject.TryGetComponent(out BotController component))
            //    {
            //        component.TakeDamage(damage, botHitType, downAttack, 1f, transform.position);
            //        //Runner.Spawn(attecktEffect, gameObject.transform.position, gameObject.transform.rotation);
            //    }
            //}

            Debug.Log(other.gameObject);

            Destroy(gameObject);
        }
    }

    public void GetTargetObject(GameObject newPlayer)
    { 
        player = newPlayer;
    }
}
