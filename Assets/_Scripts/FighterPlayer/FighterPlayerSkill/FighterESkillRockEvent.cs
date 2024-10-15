using UnityEngine;

public class FighterESkillRockEvent : MonoBehaviour
{
    private enum rock
    {
        front,
        back
    }

    [SerializeField] private rock rockState;
    
    private Rigidbody rigid;


    private void Awake()
    {
        rigid = GetComponentInChildren<Rigidbody>();

        if (rockState == rock.front)
        {
            Invoke("OnRockMove", 1.5f);
        }

        Destroy(gameObject, 5f);
    }

    public void OnRockMove()
    {
        rigid.AddForce(transform.forward * 100, ForceMode.Impulse);
    }

    public void OnFracture()
    {
        for (int i = 0; i < 10; i++)
        {
            //transform.GetChild(1).GetChild(i).GetComponentInChildren<MeshCollider>().isTrigger = true;
            transform.GetChild(1).GetChild(i).GetComponentInChildren<Rigidbody>().useGravity = true;
        }
    }
}
