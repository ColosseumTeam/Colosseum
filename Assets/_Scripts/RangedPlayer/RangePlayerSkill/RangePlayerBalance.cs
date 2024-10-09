
using Fusion;
using UnityEngine;

public class RangePlayerBalance : NetworkBehaviour
{
    private void Awake()
    {
        Destroy(gameObject, 3f);
    }
}
