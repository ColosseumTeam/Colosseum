using UnityEngine;

public class FighterPlayerShiftClick : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Todo: 데미지 처리 필요.
            Debug.Log("ShiftClick Skill Hit");
        }
    }
}
