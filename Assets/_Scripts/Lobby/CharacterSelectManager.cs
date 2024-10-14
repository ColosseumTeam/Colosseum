using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{
    [SerializeField] private int myCharacterNumber;
    [SerializeField] private int enemyCharacterNumber;
    [SerializeField] private bool win;

    public int MyCharacterNumber { get { return myCharacterNumber; } }
    public int EnemyCharacterNumber { get { return enemyCharacterNumber; } }
    public bool Win { get { return win; } }


    private void Awake()
    {
        if (FindObjectOfType<CharacterSelectManager>() != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this);
    }

    public void ChangeMyCharacter(int number)
    {
        myCharacterNumber = number;
    }

    public void ChangeEnemyCharacter(int number)
    {
        enemyCharacterNumber = number;
    }

    // Todo: 데미지 컨트롤러의 씬 넘어가는 함수에 적용하기.
    public void Winner(bool win)
    {
        this.win = win;
    }
}
