using Fusion;
using UnityEngine;

public class RangePlayerFourAttackMiddle : NetworkBehaviour
{
    private RangePlayerFourAttack rangePlayerFourAttack;

    private void Awake()
    {
        rangePlayerFourAttack = GetComponentInParent<RangePlayerFourAttack>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Ground")
        {
            rangePlayerFourAttack.OnGroundCheck();

            GetComponent<Collider>().enabled = false;
        }
    }
}
