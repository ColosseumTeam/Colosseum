using UnityEngine;

// Todo: interface(IRangeSkill 윤빈씨가 수정할 예정) 상속 받기.
public class FighterRightClickSkill : FighterSkillBase
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Todo: 데미지매니저의 DamageTransmission 사용
            Debug.Log("Right Click Hit");
        }
    }
}
