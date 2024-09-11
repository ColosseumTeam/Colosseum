using UnityEngine;

public class FighterQSkill : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Vector3 dir;


    private void OnEnable()
    {
        // Todo: Destory 사용한 모든 오브젝트 풀링 필요.
        Destroy(gameObject, 2f);
    }

    private void Update()
    {
        transform.position += dir * Time.deltaTime * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            // Todo: Destory 사용한 모든 오브젝트 풀링 필요.
            Destroy(gameObject);
        }
        if (other.CompareTag("Enemy"))
        {
            // Todo: Destory 사용한 모든 오브젝트 풀링 필요.
            // Todo: 데미지 처리 필요.
            Debug.Log("Q Skill Hit");
            Destroy(gameObject);
        }
    }

    public void Look(Vector3 aimPos)
    {
        dir = (Camera.main.ScreenToWorldPoint(new Vector3(aimPos.x, aimPos.y, 10f)) - transform.position).normalized;
    }
}
