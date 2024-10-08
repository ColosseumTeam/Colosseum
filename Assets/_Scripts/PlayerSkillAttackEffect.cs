using Fusion;
using UnityEngine;

public class PlayerSkillAttackEffect : NetworkBehaviour
{
    private void Awake()
    {
        Destroy(gameObject, 1f);
    }
}
