using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class RangePlayerOneAttack : NetworkBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private PlayerDamageController.PlayerHitType playerHitType;
    [SerializeField] private BotController.BotHitType botHitType;
    [SerializeField] private bool downAttack = true;
    [SerializeField] private GameObject attecktEffect;
    [SerializeField] private GameObject balanceObject;

    private GameObject player;
    private float maxHeight = 0f;
    private float upSpeed = 20f;

    private void Awake()
    {
        Destroy(gameObject, 1.5f);
    }


    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (transform.position.y < maxHeight)
        {
            // 기존 position을 받아와서 y 값을 수정한 후 다시 할당
            Vector3 newPosition = transform.position;
            newPosition.y += upSpeed * Time.deltaTime;
            transform.position = newPosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (other.gameObject.GetComponentInParent<PlayerDamageController>() != null
                && other.gameObject != player && HasStateAuthority)
            {
                other.gameObject.GetComponentInParent<PlayerDamageController>().RPC_TakeDamage(damage, playerHitType, downAttack, 1f, transform.position);
                //Runner.Spawn(attecktEffect, gameObject.transform.position, gameObject.transform.rotation);
            }
            else
            {
                if (other.gameObject.TryGetComponent(out BotController component))
                {
                    component.TakeDamage(damage, botHitType, downAttack, 1f, transform.position);
                    //Runner.Spawn(attecktEffect, gameObject.transform.position, gameObject.transform.rotation);
                }
            }

            Vector3 instanceBalanceRotation = new Vector3(
                                gameObject.transform.eulerAngles.x + 90f,
                                gameObject.transform.eulerAngles.y,
                                gameObject.transform.eulerAngles.z
                            );

            Runner.Spawn(balanceObject, gameObject.transform.position, Quaternion.Euler(instanceBalanceRotation));

            GetComponent<BoxCollider>().enabled = false;
        }
    }


    public void GetRangePlayer(GameObject newPlayer)
    {
        player = newPlayer;
    }
}
