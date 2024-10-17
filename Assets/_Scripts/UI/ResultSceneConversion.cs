using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultSceneConversion : NetworkBehaviour
{
    private CharacterSelectManager characterSelectManager;


    private void Awake()
    {
        characterSelectManager = FindObjectOfType<CharacterSelectManager>();
    }

    [Rpc]
    public void RPC_ResultSceneBringIn(int winner, int loser)
    {
        characterSelectManager.Local_Winner(winner, loser);
        Time.timeScale = 0.2f;

        Invoke("LoadResultScene", 1f);
    }

    private void LoadResultScene()
    {
        if (Runner.IsSceneAuthority)
        {
            Runner.LoadScene("ResultScene");
        }
    }
}
