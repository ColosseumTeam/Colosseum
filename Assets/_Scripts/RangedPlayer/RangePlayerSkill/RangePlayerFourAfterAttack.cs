using Fusion;
using UnityEngine;

public class RangePlayerFourAfterAttack : NetworkBehaviour
{
    [SerializeField] private float timer = 0f;
    [SerializeField] private float timerEnd = 2f;

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= timerEnd)
        {
            Destroy(gameObject);
        }
    }

}
