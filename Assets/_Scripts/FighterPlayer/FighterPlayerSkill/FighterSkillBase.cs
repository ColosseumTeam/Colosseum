using UnityEngine;

public class FighterSkillBase : MonoBehaviour
{
    protected DamageManager damageManager;

    private void Awake()
    {
        //damageManager = GetComponentInParent<PlayerFighterAttackController>().DamageManager;
        damageManager = FindAnyObjectByType<DamageManager>();   
    }
}
