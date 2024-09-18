using UnityEngine;

public class FighterESkill : MonoBehaviour
{
    private void Awake()
    {
        // Todo: Destory 사용한 모든 오브젝트 풀링 필요.
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Todo: 데미지 및 컷씬 처리 필요.
            Debug.Log("E Skill Hit");
        }
    }
}
