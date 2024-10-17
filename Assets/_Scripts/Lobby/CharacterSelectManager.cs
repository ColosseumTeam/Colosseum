using Fusion;
using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{
    [SerializeField] private int myCharacterNumber;
    [SerializeField] private int enemyCharacterNumber;
    [SerializeField] private bool win = true;
    [SerializeField] private int winner;
    [SerializeField] private int loser;

    public int MyCharacterNumber { get { return myCharacterNumber; } }
    public int EnemyCharacterNumber { get { return enemyCharacterNumber; } }
    public bool Win { get { return win; } }
    public int Winner { get { return winner; } }
    public int Loser { get { return loser; } }


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

    public bool Local_Winner(int winner, int loser)
    {
        this.winner = winner;
        this.loser = loser;

        return true;
    }

    [Rpc]
    public void RPC_Winner(int winner, int loser)
    {
        this.winner = winner;
        this.loser = loser;
        Debug.Log("Winner Changed");
    }
}
