using Fusion;
using UnityEngine;

public class RangePlayerFourAttackMiddle : NetworkBehaviour
{
    private RangePlayerFourAttack rangePlayerFourAttack;

    [SerializeField] private bool timerCheck = false;
    [SerializeField] private float timer = 0f;
    [SerializeField] private float endTimer = 1f;
    [SerializeField] private float rayDistance = 0.5f;

    private void Awake()
    {
        rangePlayerFourAttack = GetComponentInParent<RangePlayerFourAttack>();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        GroundCheckWithRay();

        if (timerCheck)
        {
            timer += Runner.DeltaTime;
            if(timer >= endTimer)
            {
                rangePlayerFourAttack.IsAttackingActive();
                Destroy(gameObject);
            }          
        }
    }
    
    private void GroundCheckWithRay()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                rangePlayerFourAttack.OnGroundCheck();
                GetComponent<Collider>().enabled = false;
                timerCheck = true;
            }
        }
    }
}
